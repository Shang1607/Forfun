using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Test.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Test.ViewModels
{
    public class EditProductViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Produktnavn er påkrevd")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Beskrivelse er påkrevd")]
        public string Description { get; set; }

        
        public int Calories { get; set; }
        public double Fat { get; set; }
        public double Carbohydrates { get; set; }
        public double Protein { get; set; }

        // For å vise eksisterende bilde
        public string? ExistingImageUrl { get; set; }

        // For å laste opp nytt bilde
        public IFormFile? ImageFile  { get; set; }

        // For å håndtere kategorier
        [BindNever]
        public List<Category>? AllCategories { get; set; }
        public int[]? SelectedCategories { get; set; }
    }
}
