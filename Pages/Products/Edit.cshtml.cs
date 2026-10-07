using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrderAndPaymentAPI.Data;
using OrderAndPaymentAPI.DTO;
using OrderAndPaymentAPI.Entities;

using OrderAndPaymentAPI.Services;


namespace OrderAndPaymentAPI.Pages.Products;


public class EditModel : PageModel
{
    private readonly IProductService _productService;
    private readonly AppDbContext _context;

    public EditModel(IProductService productService, AppDbContext context)
    {
        _productService = productService;
        _context = context;
    }

    [BindProperty]
    public Product Product{ get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int Id)
    {
        if (!User.IsInRole("admin"))
            return Forbid();

        var product = await _productService.GetProductByIdAsync(Id);
        if (product == null)
            return NotFound();

        Product = product;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!User.IsInRole("admin"))
            return Forbid();

        if (!ModelState.IsValid)
            return Page();

        _context.Attach(Product).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        await _context.SaveChangesAsync();
        return RedirectToPage("/Products/Index");
    }
}