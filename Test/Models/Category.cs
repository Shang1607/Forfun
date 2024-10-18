using System;
using System.ComponentModel;
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
    
    [Range(1, int.MaxValue, ErrorMessage = "Display Order for category must be greater than 0")]
    public int DisplayOrder { get; set; }
    [DisplayName("Display Order")]   

public DateTime Created { get; set; } 

}
