using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using OrderAndPaymentAPI.Exceptions;
using OrderAndPaymentAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddExceptionHandler<AppExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
await using var app = builder.Build();
app.UseExceptionHandler();

// Exercise the real middleware pipeline in memory, without a database or test endpoints.
var pipeline = new ApplicationBuilder(app.Services);
pipeline.UseExceptionHandler();
pipeline.Run(context =>
{
    if (context.Request.Path == "/unexpected")
        throw new InvalidOperationException("INTERNAL_SECRET");
    throw new AppException("Business error", int.Parse(context.Request.Path.Value!.Trim('/')));
});
var request = pipeline.Build();

foreach (var status in new[] { 400, 404, 409, 500 })
{
    using var scope = app.Services.CreateScope();
    var context = new DefaultHttpContext();
    context.RequestServices = scope.ServiceProvider;
    context.Request.Path = status == 500 ? "/unexpected" : "/" + status;
    context.Response.Body = new MemoryStream();
    await request(context);
    context.Response.Body.Position = 0;
    var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
    using var json = JsonDocument.Parse(body);
    if (context.Response.StatusCode != status ||
        json.RootElement.GetProperty("status").GetInt32() != status ||
        !json.RootElement.TryGetProperty("traceId", out _))
        throw new Exception("Incorrect response: " + body);
    if (status == 500 && body.Contains("INTERNAL_SECRET"))
        throw new Exception("Internal error leaked.");
    if (status != 500 && json.RootElement.GetProperty("detail").GetString() != "Business error")
        throw new Exception("Business message lost.");
    Console.WriteLine("PASS: " + status);
}
