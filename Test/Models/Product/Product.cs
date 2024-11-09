using System.ComponentModel.DataAnnotations;

namespace Test.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Produktnavn er påkrevd")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Beskrivelse er påkrevd")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Kalorier er påkrevd")]
        [Range(0, int.MaxValue, ErrorMessage = "Kalorier må være et positivt tall")]
        public int Calories { get; set; }

        [Required(ErrorMessage = "Fett er påkrevd")]
        [Range(0, double.MaxValue, ErrorMessage = "Fett må være et positivt tall")]
        public double Fat { get; set; }

        [Required(ErrorMessage = "Karbohydrater er påkrevd")]
        [Range(0, double.MaxValue, ErrorMessage = "Karbohydrater må være et positivt tall")]
        public double Carbohydrates { get; set; }

        [Required(ErrorMessage = "Proteiner er påkrevd")]
        [Range(0, double.MaxValue, ErrorMessage = "Proteiner må være et positivt tall")]
        public double Protein { get; set; }

        public DateTime CreatedAt { get; set; }

        // Fremmednøkkel til bruker
        public int UserId { get; set; }
        public User? User { get; set; }


        // for å koble de samme, og lage mange til mange relasjoner
         public ICollection<ProductCategory>? ProductCategories { get; set; }
         
    }
}
