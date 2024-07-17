namespace WebApplicationTRY_TWO.Models;

public class Products
{
    public int id { get; set; }
    public string products_name { get; set; } = string.Empty;
    public string? products_description { get; set; } = string.Empty;
    public decimal? products_price { get; set; }
    
}