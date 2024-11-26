using System.ComponentModel.DataAnnotations;

namespace Test.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Prodcut name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "calories are requiredd")]
        [Range(0, int.MaxValue, ErrorMessage = "Calories must be a positive number")]
        public int Calories { get; set; }

        [Required(ErrorMessage = "Fat is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Fat must be a positive number")]
        public double Fat { get; set; }

        [Required(ErrorMessage = "Carbohydrates are required")]
        [Range(0, double.MaxValue, ErrorMessage = "Carbohydrates must be a positive number")]
        public double Carbohydrates { get; set; }

        [Required(ErrorMessage = "Proteins are required")]
        [Range(0, double.MaxValue, ErrorMessage = "Proteins must be a positive number")]
        public double Protein { get; set; }

        public DateTime CreatedAt { get; set; }

        // Fremmednøkkel til bruker
        public int UserId { get; set; }
        public User? User { get; set; }

        // for å koble de samme, og lage mange til mange relasjoner
         public ICollection<ProductCategory>? ProductCategories { get; set; }

         public string? ImageUrl { get; set; }

    }
}
