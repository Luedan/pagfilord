using System.ComponentModel.DataAnnotations.Schema;

namespace PagFilOrd.bd.models;

public class Brand
{

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Name { get; set; }

    public List<Category>? Categories { get; set; }
}