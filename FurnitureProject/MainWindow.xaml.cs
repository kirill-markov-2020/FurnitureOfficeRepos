using FurnitureProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FurnitureProject
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBoxInputLogin_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TextBoxInputLogin.Text == "Введите логин")
            {
                TextBoxInputLogin.Text = "";
                TextBoxInputLogin.Foreground = Brushes.Black;
            }
        }
        private void ProductNameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ProductNameTextBox.Text == "Название товара:")
            {
                ProductNameTextBox.Text = "";
                ProductNameTextBox.Foreground = Brushes.Black;
            }
        }
        private void ProductPriceTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ProductPriceTextBox.Text == "Стоимость:")
            {
                ProductPriceTextBox.Text = "";
                ProductPriceTextBox.Foreground = Brushes.Black;
            }
        }
        private void ProductQuantityTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ProductQuantityTextBox.Text == "Количество:")
            {
                ProductQuantityTextBox.Text = "";
                ProductQuantityTextBox.Foreground = Brushes.Black;
            }
        }
        private void TextBoxInputLogin_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBoxInputLogin.Text))
            {
                TextBoxInputLogin.Text = "Введите логин";
                TextBoxInputLogin.Foreground = Brushes.Gray;
            }
        }
        private void ProductNameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ProductNameTextBox.Text))
            {
                ProductNameTextBox.Text = "Название товара:";
                ProductNameTextBox.Foreground = Brushes.Gray;
            }
        }
        private void ProductPriceTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ProductPriceTextBox.Text))
            {
                ProductPriceTextBox.Text = "Стоимость:";
                ProductPriceTextBox.Foreground = Brushes.Gray;
            }
        }
        private void ProductQuantityTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ProductQuantityTextBox.Text))
            {
                ProductQuantityTextBox.Text = "Количество:";
                ProductQuantityTextBox.Foreground = Brushes.Gray;
            }
        }
        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordHintText.Visibility = Visibility.Collapsed;
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PasswordBox.Password))
            {
                PasswordHintText.Visibility = Visibility.Visible;
            }
        }
        private void AuthorizationButton_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxInputLogin.Text == "2" && PasswordBox.Password == "2")
            {
                AuthorizationPanel.Visibility = Visibility.Collapsed;
                ManagerPanel.Visibility = Visibility.Visible;
                LoadManagerCategoriesAndProducts();
            }
            else if (TextBoxInputLogin.Text == "3" && PasswordBox.Password == "3")
            {
                AuthorizationPanel.Visibility = Visibility.Collapsed;
                AdministratorPanel.Visibility = Visibility.Visible;
                LoadAdminCategoriesAndProducts();
            }
            else if (TextBoxInputLogin.Text == "1" && PasswordBox.Password == "1")
            {
                AuthorizationPanel.Visibility = Visibility.Collapsed;
                ConsultantPanel.Visibility = Visibility.Visible;
                LoadCategories();
            }
            else if (TextBoxInputLogin.Text == "Введите логин" && PasswordBox.Password == "")
            {
                MessageBox.Show("Введите логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void ShowPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Visibility == Visibility.Visible)
            {
                PasswordBox.Visibility = Visibility.Collapsed;
                PasswordTextBox.Visibility = Visibility.Visible;
                PasswordTextBox.Text = PasswordBox.Password;
            }
            else
            {
                PasswordBox.Visibility = Visibility.Visible;
                PasswordTextBox.Visibility = Visibility.Collapsed;
                PasswordBox.Password = PasswordTextBox.Text;
            }
        }

        private void PasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PasswordBox.Password = PasswordTextBox.Text;
        }

        private void BackToAuthorization_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите выйти из аккаунта?", "Подтверждение выхода", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                TextBoxInputLogin.Text = "Введите логин";
                TextBoxInputLogin.Foreground = Brushes.Gray;
                PasswordBox.Password = string.Empty;
                PasswordHintText.Visibility = Visibility.Visible;
                ManagerPanel.Visibility = Visibility.Collapsed;
                AdministratorPanel.Visibility = Visibility.Collapsed;
                ConsultantPanel.Visibility = Visibility.Collapsed;
                AuthorizationPanel.Visibility = Visibility.Visible;
            }
        }

        private SqlConnection connection;

        private SqlConnection GetDatabaseConnection()
        {
            connection = new SqlConnection("Server=KIRILL-MARKOV;Database=DataBase;Integrated Security=True;");
            return connection;
        }

        private void LoadCategories()
        {
            using (var dbContext = new AppDbContext())
            {
                var categories = dbContext.Categories
                    .Include(c => c.Products)
                    .ToList();

                CategoriesTreeView.Items.Clear();

                foreach (var category in categories)
                {
                    var categoryNode = new TreeViewItem
                    {
                        Header = category.Name,
                        FontSize = 20,
                        IsExpanded = false,
                        Tag = category
                    };

                    foreach (var product in category.Products)
                    {
                        var productNode = new TreeViewItem
                        {
                            Header = $"Товар: {product.Name}; Цена: {product.Price:C}; Товаров на складе: {product.Quantity}",
                            FontSize = 16,
                            Tag = product
                        };

                        categoryNode.Items.Add(productNode);
                    }

                    CategoriesTreeView.Items.Add(categoryNode);
                }
            }
        }
        private void LoadManagerCategoriesAndProducts()
        {
            ManagerCategoriesTreeView.Items.Clear();
            using (var dbContext = new AppDbContext())
            {
                try
                {
                    var categories = dbContext.Categories
                        .Include(c => c.Products)
                        .OrderBy(c => c.Id)
                        .ToList();

                    foreach (var category in categories)
                    {
                        var currentCategoryItem = new TreeViewItem
                        {
                            Header = category.Name,
                            FontSize = 20,
                            IsExpanded = false
                        };
                        ManagerCategoriesTreeView.Items.Add(currentCategoryItem);

                        foreach (var product in category.Products)
                        {
                            var productItem = new TreeViewItem
                            {
                                Header = $"Товар: {product.Name}; Цена: {product.Price:C}; Товаров на складе: {product.Quantity}",
                                FontSize = 16,
                                Tag = product 
                            };
                            currentCategoryItem.Items.Add(productItem);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
                }
            }
        }
        private int? selectedProductId = null;

        private void ManagerCategoriesTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (ManagerCategoriesTreeView.SelectedItem is TreeViewItem selectedItem && selectedItem.Tag is Product product)
            {
                selectedProductId = product.Id;
                QuantityControlPanel.Visibility = Visibility.Visible;
                SelectedProductQuantityText.Text = $"Количество: {product.Quantity}";
            }
            else
            {
                selectedProductId = null;
                SelectedProductQuantityText.Text = "Количество: ";
                QuantityControlPanel.Visibility = Visibility.Collapsed;
            }
        }
        private void IncreaseQuantityButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateProductQuantity(1); 
        }

        private void DecreaseQuantityButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateProductQuantity(-1);
        }

        private void UpdateProductQuantity(int change)
        {
            if (!selectedProductId.HasValue)
            {
                MessageBox.Show("Пожалуйста, выберите товар для изменения количества", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            using (var dbContext = new AppDbContext())
            {
                try
                {
                    var product = dbContext.Products.FirstOrDefault(p => p.Id == selectedProductId.Value);
                    if (product != null)
                    {
                        product.Quantity += change; 
                        dbContext.SaveChanges();    
                        UpdateTreeViewItemText(product.Quantity);
                    }
                    else
                    {
                        MessageBox.Show("Товар не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при изменении количества: " + ex.Message);
                }
            }
        }
        private void UpdateTreeViewItemText(int newQuantity)
        {
            if (ManagerCategoriesTreeView.SelectedItem is TreeViewItem selectedItem)
            {
                string headerText = selectedItem.Header.ToString();
                int index = headerText.LastIndexOf("Товаров на складе: ");
                if (index != -1)
                {
                    headerText = headerText.Substring(0, index) + $"Товаров на складе: {newQuantity}";
                    selectedItem.Header = headerText;
                }

                SelectedProductQuantityText.Text = $"Количество: {newQuantity}";
            }
        }


        private void LoadAdminCategoriesAndProducts()
        {
            AdminCategoriesTreeView.Items.Clear();
            using (var dbContext = new AppDbContext())
                try
                {
                    var categories = dbContext.Categories
                        .Include(c => c.Products)
                        .OrderBy(c => c.Id)
                        .ToList();

                    foreach (var category in categories)
                    {
                        var currentCategoryItem = new TreeViewItem
                        {
                            Header = category.Name,
                            FontSize = 20,
                            IsExpanded = false
                        };
                        AdminCategoriesTreeView.Items.Add(currentCategoryItem);

                        foreach (var product in category.Products)
                        {
                            var productItem = new TreeViewItem
                            {
                                Header = $"Товар: {product.Name}; Цена: {product.Price:C}; Товаров на складе: {product.Quantity}",
                                FontSize = 16
                            };
                            currentCategoryItem.Items.Add(productItem);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
                }
        }

        private void AddCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            AdministratorPanel.Visibility = Visibility.Collapsed;
            AddCategoryPanel.Visibility = Visibility.Visible;
            CategoryNameTextBox.Text = string.Empty;
        }

        private void SaveCategoryButton_Click(object sender, RoutedEventArgs e)
        {

            string categoryName = CategoryNameTextBox.Text.Trim();

            if (string.IsNullOrEmpty(categoryName))
            {
                MessageBox.Show("Название категории не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            using (var dbContext = new AppDbContext())
            try
            {
                var category = new Category
                {
                    Name = categoryName
                };

                dbContext.Categories.Add(category);
                dbContext.SaveChanges();

                MessageBox.Show("Категория успешно добавлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadAdminCategoriesAndProducts();
                AddCategoryPanel.Visibility = Visibility.Collapsed;
                AdministratorPanel.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении категории: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelAddCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            AddCategoryPanel.Visibility = Visibility.Collapsed;
            AdministratorPanel.Visibility = Visibility.Visible;
        }

        private void DeleteCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dbContext = new AppDbContext())
                if (AdminCategoriesTreeView.SelectedItem is TreeViewItem selectedItem)
                {
                    string categoryName = selectedItem.Header.ToString();

                    var category = dbContext.Categories
                        .FirstOrDefault(c => c.Name == categoryName);

                    if (category != null)
                    {
                        var result = MessageBox.Show(
                            $"Вы уверены, что хотите удалить категорию '{categoryName}'? Все связанные товары также будут удалены.", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        if (result == MessageBoxResult.Yes)
                        {

                            try
                            {
                                var products = dbContext.Products.Where(p => p.CategoryFurniture_Id == category.Id).ToList();
                                dbContext.Products.RemoveRange(products);
                                dbContext.Categories.Remove(category);
                                dbContext.SaveChanges();

                                MessageBox.Show($"Категория '{categoryName}' успешно удалена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                                LoadAdminCategoriesAndProducts();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Ошибка при удалении категории: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Категория не найдена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите категорию для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
        }

        private void AdminCategoriesTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (AdminCategoriesTreeView.SelectedItem is TreeViewItem selectedItem && selectedItem.Parent == AdminCategoriesTreeView)
            {
                AddProductButton.IsEnabled = true;
            }
            else
            {
                AddProductButton.IsEnabled = false;
            }
        }



        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (AdminCategoriesTreeView.SelectedItem is TreeViewItem selectedItem)
            {

                ProductCategoryTextBox.Text = selectedItem.Header.ToString();
                AddProductPanel.Visibility = Visibility.Visible;
                AdministratorPanel.Visibility = Visibility.Collapsed;
                ProductQuantityTextBox.Text = "Количество:";
                ProductQuantityTextBox.Foreground = Brushes.Gray;
                ProductPriceTextBox.Text = "Стоимость:";
                ProductPriceTextBox.Foreground = Brushes.Gray;
                ProductNameTextBox.Text = "Название товара:";
                ProductNameTextBox.Foreground = Brushes.Gray;
            }
        }

        private void SaveProductButton_Click(object sender, RoutedEventArgs e)
        {
            string productName = ProductNameTextBox.Text.Trim();
            string categoryName = ProductCategoryTextBox.Text;

            if (!decimal.TryParse(ProductPriceTextBox.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Введите корректную стоимость.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!int.TryParse(ProductQuantityTextBox.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Введите корректное количество.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = GetDatabaseConnection())
                {
                    connection.Open();

                    string insertQuery = @"
                INSERT INTO Product (name, categoryFurniture_id, price, quantity)
                SELECT @ProductName, id, @Price, @Quantity
                FROM Category
                WHERE name = @CategoryName";

                    SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                    insertCommand.Parameters.AddWithValue("@ProductName", productName);
                    insertCommand.Parameters.AddWithValue("@CategoryName", categoryName);
                    insertCommand.Parameters.AddWithValue("@Price", price);
                    insertCommand.Parameters.AddWithValue("@Quantity", quantity);

                    int rowsAffected = insertCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Товар успешно добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Ошибка: Категория не найдена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                AddProductPanel.Visibility = Visibility.Collapsed;
                AdministratorPanel.Visibility = Visibility.Visible;
                LoadAdminCategoriesAndProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении товара: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelAddProductButton_Click(object sender, RoutedEventArgs e)
        {
            AddProductPanel.Visibility = Visibility.Collapsed;
            AdministratorPanel.Visibility = Visibility.Visible;
        }

        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (AdminCategoriesTreeView.SelectedItem is TreeViewItem selectedItem && selectedItem.Parent != AdminCategoriesTreeView)
            {
                string productInfo = selectedItem.Header.ToString();
                string[] productParts = productInfo.Split(new string[] { "; Цена: ", "; Кол-во: " }, StringSplitOptions.None);
                string productName = productParts[0].Replace("Товар: ", "");
                MessageBoxResult result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить товар '{productName}'?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        string productIdQuery = "SELECT id FROM Product WHERE name = @ProductName";
                        using (SqlConnection connection = GetDatabaseConnection())
                        {
                            connection.Open();
                            SqlCommand command = new SqlCommand(productIdQuery, connection);
                            command.Parameters.AddWithValue("@ProductName", productName);
                            object productIdObj = command.ExecuteScalar();

                            if (productIdObj != null)
                            {
                                int productId = Convert.ToInt32(productIdObj);
                                string deleteProductQuery = "DELETE FROM Product WHERE id = @ProductId";
                                SqlCommand deleteCommand = new SqlCommand(deleteProductQuery, connection);
                                deleteCommand.Parameters.AddWithValue("@ProductId", productId);
                                deleteCommand.ExecuteNonQuery();
                                MessageBox.Show($"Товар '{productName}' успешно удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                                LoadAdminCategoriesAndProducts();
                            }
                            else
                            {
                                MessageBox.Show("Товар не найден в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при удалении товара: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите товар для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }



    }
}
