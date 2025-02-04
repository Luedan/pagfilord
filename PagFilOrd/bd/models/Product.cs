using System.ComponentModel.DataAnnotations.Schema;

namespace PagFilOrd.bd.models;

public class Product
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}