using System.ComponentModel.DataAnnotations;

namespace Test.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        // Valgfritt: Legg til en egenskap for å markere om det er "Nøkkelhullmerket"
        public bool IsKeyhole { get; set; } = false;

        // Navigasjonsproperti for mange-til-mange-relasjon
        public ICollection<ProductCategory> ProductCategories { get; set; }
    }
}
