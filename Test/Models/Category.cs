using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Models;

public class Category
{
    [Key]
    public int Id { get; set; }
    [Required]
    [Display(Name = "Category Name")]
    [Column(TypeName = "TEXT")]

    public string Name { get; set; } = String.Empty;
    public int DisplayOrder { get; set; }   

[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
public DateTime Created { get; set; } 

}
