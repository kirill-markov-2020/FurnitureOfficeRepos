using FurnitureProject.Models;
using Microsoft.EntityFrameworkCore;


namespace FurnitureProject
{
    public class AppDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=KIRILL-MARKOV;Database=DataBase;Integrated Security=True;");
            }
        }
    }
}
