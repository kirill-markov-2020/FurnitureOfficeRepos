using FurnitureProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace FurnitureProject
{
    public class AppDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string configFileName = "config.json";
            string connectionString = string.Empty;
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFileName);
            if (!File.Exists(configPath))
            {
                MessageBox.Show($"Конфигурационный файл не найден по пути: {configPath}");

                return;
            }
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile(configFileName, optional: false, reloadOnChange: true)
                .Build();

            connectionString = configuration.GetConnectionString("Connection");
            try
            {
                optionsBuilder.UseSqlServer(connectionString);
                File.AppendAllText("error.log", $"{DateTime.Now}: Да, подключение к БД успешно настроено.\n");

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}");
                File.WriteAllText("error.log", ex.ToString());
            }
        }



    }
}
