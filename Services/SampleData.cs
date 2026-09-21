using OrderAndPaymentAPI.Web.Models;

namespace OrderAndPaymentAPI.Web.Services;

// Dữ liệu mẫu để dựng giao diện.
// Khi có backend thật, thay các property này bằng HttpClient gọi sang OrderAndPaymentAPI.
public static class SampleData
{
    public static List<ProductVm> Products => new()
    {
        new ProductVm { Id = 1, Name = "Bàn phím cơ Keychron K2", Price = 1890000, Stock = 12 },
        new ProductVm { Id = 2, Name = "Chuột Logitech MX Master 3", Price = 2290000, Stock = 8 },
        new ProductVm { Id = 3, Name = "Tai nghe Sony WH-1000XM4", Price = 6490000, Stock = 5 },
        new ProductVm { Id = 4, Name = "Màn hình Dell 27\" 2K", Price = 5990000, Stock = 3 },
        new ProductVm { Id = 5, Name = "Ổ cứng SSD Samsung 1TB", Price = 1690000, Stock = 20 },
        new ProductVm { Id = 6, Name = "Webcam Logitech C920", Price = 1290000, Stock = 15 },
    };

    public static CartVm Cart => new()
    {
        Id = 1,
        Items = new List<CartItemVm>
        {
            new() { ProductId = 1, ProductName = "Bàn phím cơ Keychron K2", Price = 1890000, Quantity = 1 },
            new() { ProductId = 3, ProductName = "Tai nghe Sony WH-1000XM4", Price = 6490000, Quantity = 2 },
        }
    };

    public static List<OrderVm> Orders => new()
    {
        new OrderVm
        {
            Id = 101, Status = "Completed", CreatedAt = DateTime.Now.AddDays(-3), TotalAmount = 8380000,
            Items = new List<OrderItemVm>
            {
                new() { ProductId = 1, ProductName = "Bàn phím cơ Keychron K2", Quantity = 1, Price = 1890000 },
                new() { ProductId = 3, ProductName = "Tai nghe Sony WH-1000XM4", Quantity = 1, Price = 6490000 },
            }
        },
        new OrderVm
        {
            Id = 102, Status = "Pending", CreatedAt = DateTime.Now.AddDays(-1), TotalAmount = 2290000,
            Items = new List<OrderItemVm>
            {
                new() { ProductId = 2, ProductName = "Chuột Logitech MX Master 3", Quantity = 1, Price = 2290000 },
            }
        },
        new OrderVm
        {
            Id = 103, Status = "Cancelled", CreatedAt = DateTime.Now.AddDays(-7), TotalAmount = 1690000,
            Items = new List<OrderItemVm>
            {
                new() { ProductId = 5, ProductName = "Ổ cứng SSD Samsung 1TB", Quantity = 1, Price = 1690000 },
            }
        },
    };
}
