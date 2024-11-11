using System.Linq;
using Test.Models;

namespace Test.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDBContext context)
        {
            // Sjekk om kategorier allerede finnes
            if (context.Categories.Any())
            {
                return; // DB har blitt seed'et
            }

            var categories = new Category[]
            {
                new Category { Name = "Kjøtt" },
                new Category { Name = "Frukt" },
                new Category { Name = "Grønnsaker" },
                new Category { Name = "Bær og nøtter" },
                new Category { Name = "Bakevarer" },
                new Category { Name = "Grøt" },
                new Category { Name = "Brød" },
                new Category { Name = "Pasta" },
                new Category { Name = "Melkevarer" },
                new Category { Name = "Ost" },
                new Category { Name = "Fiskevarer" },
                new Category { Name = "Dressing" },
                new Category { Name = "Sauser" },
                // Legg til Nøkkelhullmerket med IsKeyhole satt til true
                new Category { Name = "Nøkkelhullmerket", IsKeyhole = true }
            };

            context.Categories.AddRange(categories);
            context.SaveChanges();
        }
    }
}