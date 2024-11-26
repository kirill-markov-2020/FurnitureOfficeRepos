using FurnitureProject.Models;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;


namespace FurnitureProject
{
    public class AppDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string configFileName = "config.json";
            if (!File.Exists(configFileName))
            {
                var defaultConfig = new
                {
                    ConnectionStrings = new
                    {
                        Сonnection = "Server=KIRILL-MARKOV;Database=DataBase;Integrated Security=True;"
                    }
                };
                File.WriteAllText(configFileName, System.Text.Json.JsonSerializer.Serialize(defaultConfig, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
            }
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(configFileName, optional: false, reloadOnChange: true)
                .Build();
            string connectionString = configuration.GetConnectionString("Сonnection");
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }
}
