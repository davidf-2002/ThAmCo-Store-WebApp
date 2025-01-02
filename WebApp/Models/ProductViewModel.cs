using System.ComponentModel;

namespace WebApp.Models;
public class ProductViewModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? StockStatus { get; set;}
    public int StockLevel { get; set; }
    public DateTime LastUpdated { get; set; }
    public int CategoryId { get; set; }
}