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
        private void TextBoxInputLogin_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBoxInputLogin.Text))
            {
                TextBoxInputLogin.Text = "Введите логин";
                TextBoxInputLogin.Foreground = Brushes.Gray;
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
            if (TextBoxInputLogin.Text == "ManagerLogin" && PasswordBox.Password == "ManagerPassword")
            {
                AuthorizationPanel.Visibility = Visibility.Collapsed;

                ManagerPanel.Visibility = Visibility.Visible;
                AdministratorPanel.Visibility = Visibility.Collapsed;
                ConsultantPanel.Visibility = Visibility.Collapsed;
                LoadManagerCategoriesAndProducts();
            }
            else if (TextBoxInputLogin.Text == "AdministratorLogin" && PasswordBox.Password == "AdministratorPassword")
            {
                AuthorizationPanel.Visibility = Visibility.Collapsed;

                ManagerPanel.Visibility = Visibility.Collapsed;
                AdministratorPanel.Visibility = Visibility.Visible;
                ConsultantPanel.Visibility = Visibility.Collapsed;
                LoadAdminCategoriesAndProducts();
            }
            else if (TextBoxInputLogin.Text == "ConsultantLogin" && PasswordBox.Password == "ConsultantPassword")
            {
                AuthorizationPanel.Visibility = Visibility.Collapsed;

                ManagerPanel.Visibility = Visibility.Collapsed;
                AdministratorPanel.Visibility = Visibility.Collapsed;
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
            connection = new SqlConnection("Server=KIRILL-MARKOV;Database=FurnitureData;Integrated Security=True;");
            return connection;
        }

        private void LoadCategories()
        {
            CategoriesTreeView.Items.Clear();

            try
            {
                using (SqlConnection connection = GetDatabaseConnection())
                {
                    connection.Open();

                    string query = @"SELECT cf.id as CategoryId, cf.name as CategoryName, 
                                        p.id as ProductId, p.name as ProductName, p.price, p.quantity
                                     FROM Category cf
                                     LEFT JOIN Product p ON cf.id = p.categoryFurniture_id
                                     ORDER BY cf.id, p.id";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    int currentCategoryId = -1;
                    TreeViewItem currentCategoryItem = null;

                    while (reader.Read())
                    {
                        int categoryId = reader.GetInt32(reader.GetOrdinal("CategoryId"));
                        string categoryName = reader.GetString(reader.GetOrdinal("CategoryName"));

                        if (categoryId != currentCategoryId)
                        {
                            currentCategoryItem = new TreeViewItem
                            {
                                Header = categoryName,
                                FontSize = 20,
                                IsExpanded = false
                            };
                            CategoriesTreeView.Items.Add(currentCategoryItem);
                            currentCategoryId = categoryId;
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("ProductId")))
                        {
                            string name = reader.GetString(reader.GetOrdinal("ProductName"));
                            decimal price = reader.GetDecimal(reader.GetOrdinal("price"));
                            int quantity = reader.GetInt32(reader.GetOrdinal("quantity"));

                            TreeViewItem productItem = new TreeViewItem
                            {
                                Header = $"Товар: {name}; Цена: {price:C}; Товаров на складе: {quantity}",
                                FontSize = 16,
                                IsExpanded = false
                            };

                            currentCategoryItem?.Items.Add(productItem);
                        }
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
        }
        private void LoadManagerCategoriesAndProducts()
        {
            ManagerCategoriesTreeView.Items.Clear();

            try
            {
                using (SqlConnection connection = GetDatabaseConnection())
                {
                    connection.Open();

                    string query = @"SELECT cf.id as CategoryId, cf.name as CategoryName, 
                                p.id as ProductId, p.name as ProductName, p.price, p.quantity
                             FROM Category cf
                             LEFT JOIN Product p ON cf.id = p.categoryFurniture_id
                             ORDER BY cf.id, p.id";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    int currentCategoryId = -1;
                    TreeViewItem currentCategoryItem = null;

                    while (reader.Read())
                    {
                        int categoryId = reader.GetInt32(reader.GetOrdinal("CategoryId"));
                        string categoryName = reader.GetString(reader.GetOrdinal("CategoryName"));

                        if (categoryId != currentCategoryId)
                        {
                            currentCategoryItem = new TreeViewItem
                            {
                                Header = categoryName,
                                FontSize = 20,
                                IsExpanded = false
                            };
                            ManagerCategoriesTreeView.Items.Add(currentCategoryItem);
                            currentCategoryId = categoryId;
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("ProductId")))
                        {
                            int productId = reader.GetInt32(reader.GetOrdinal("ProductId"));
                            string productName = reader.GetString(reader.GetOrdinal("ProductName"));
                            decimal price = reader.GetDecimal(reader.GetOrdinal("price"));
                            int quantity = reader.GetInt32(reader.GetOrdinal("quantity"));

                            TreeViewItem productItem = new TreeViewItem
                            {
                                Header = $"Товар: {productName}; Цена: {price:C}; Товаров на складе: {quantity}",
                                FontSize = 16,
                                IsExpanded = false,
                                Tag = productId 
                            };
                            currentCategoryItem?.Items.Add(productItem);
                        }
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }

        }
        private int? selectedProductId = null; 

        private void ManagerCategoriesTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (ManagerCategoriesTreeView.SelectedItem is TreeViewItem selectedItem && selectedItem.Tag is int productId)
            {
                selectedProductId = productId; 
                QuantityControlPanel.Visibility = Visibility.Visible; 
                using (SqlConnection connection = GetDatabaseConnection())
                {
                    connection.Open();
                    string query = "SELECT quantity FROM Product WHERE id = @ProductId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ProductId", productId);

                    int quantity = (int)command.ExecuteScalar();
                    SelectedProductQuantityText.Text = $"Количество: {quantity}";
                }
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
            if (selectedProductId.HasValue)
            {
                try
                {
                    using (SqlConnection connection = GetDatabaseConnection())
                    {
                        connection.Open();
                        string query = "UPDATE Product SET quantity = quantity + @Change WHERE id = @ProductId";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@Change", change);
                        command.Parameters.AddWithValue("@ProductId", selectedProductId.Value);
                        command.ExecuteNonQuery();                       
                        query = "SELECT quantity FROM Product WHERE id = @ProductId";
                        command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@ProductId", selectedProductId.Value);
                        int newQuantity = (int)command.ExecuteScalar();
                        UpdateTreeViewItemText(newQuantity);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при изменении количества: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите товар для изменения количества", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
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





        // AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA




        private void LoadAdminCategoriesAndProducts()
        {
            AdminCategoriesTreeView.Items.Clear();

            try
            {
                using (SqlConnection connection = GetDatabaseConnection())
                {
                    connection.Open();

                    string query = @"SELECT cf.id as CategoryId, cf.name as CategoryName, 
                             p.id as ProductId, p.name as ProductName, p.price, p.quantity
                             FROM Category cf
                             LEFT JOIN Product p ON cf.id = p.categoryFurniture_id
                             ORDER BY cf.id, p.id";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    int currentCategoryId = -1;
                    TreeViewItem currentCategoryItem = null;

                    while (reader.Read())
                    {
                        int categoryId = reader.GetInt32(reader.GetOrdinal("CategoryId"));
                        string categoryName = reader.GetString(reader.GetOrdinal("CategoryName"));

                        if (categoryId != currentCategoryId)
                        {
                            currentCategoryItem = new TreeViewItem
                            {
                                Header = categoryName,
                                FontSize = 20,
                                IsExpanded = false
                            };
                            AdminCategoriesTreeView.Items.Add(currentCategoryItem);
                            currentCategoryId = categoryId;
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("ProductId")))
                        {
                            string name = reader.GetString(reader.GetOrdinal("ProductName"));
                            decimal price = reader.GetDecimal(reader.GetOrdinal("price"));
                            int quantity = reader.GetInt32(reader.GetOrdinal("quantity"));

                            TreeViewItem productItem = new TreeViewItem
                            {
                                Header = $"Товар: {name}; Цена: {price:C}; Кол-во: {quantity}",
                                FontSize = 16
                            };

                            currentCategoryItem?.Items.Add(productItem);
                        }
                    }

                    reader.Close();
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

            try
            {
                using (SqlConnection connection = GetDatabaseConnection())
                {
                    connection.Open();
                    string getMaxIdQuery = "SELECT MAX(id) FROM Category";
                    SqlCommand getMaxIdCommand = new SqlCommand(getMaxIdQuery, connection);
                    object maxIdObj = getMaxIdCommand.ExecuteScalar();
                    int newCategoryId = maxIdObj != DBNull.Value ? Convert.ToInt32(maxIdObj) + 1 : 1; 
                    string query = "INSERT INTO Category (id, name) VALUES (@CategoryId, @CategoryName)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@CategoryId", newCategoryId);
                    command.Parameters.AddWithValue("@CategoryName", categoryName);
                    command.ExecuteNonQuery();

                    MessageBox.Show("Категория успешно добавлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }

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
            if (AdminCategoriesTreeView.SelectedItem is TreeViewItem selectedItem &&
                selectedItem.Parent is TreeView treeView)
            {
                string categoryName = selectedItem.Header.ToString();

                MessageBoxResult result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить категорию '{categoryName}'? Все связанные товары также будут удалены.",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (SqlConnection connection = GetDatabaseConnection())
                        {
                            connection.Open();
                            string query = "SELECT id FROM Category WHERE name = @CategoryName";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.Parameters.AddWithValue("@CategoryName", categoryName);
                            object categoryIdObj = command.ExecuteScalar();
                            if (categoryIdObj == null)
                            {
                                MessageBox.Show("Категория не найдена в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                            int categoryId = Convert.ToInt32(categoryIdObj);
                            string deleteProductsQuery = "DELETE FROM Product WHERE categoryFurniture_id = @CategoryId";
                            SqlCommand deleteProductsCommand = new SqlCommand(deleteProductsQuery, connection);
                            deleteProductsCommand.Parameters.AddWithValue("@CategoryId", categoryId);
                            deleteProductsCommand.ExecuteNonQuery();
                            string deleteCategoryQuery = "DELETE FROM Category WHERE id = @CategoryId";
                            SqlCommand deleteCategoryCommand = new SqlCommand(deleteCategoryQuery, connection);
                            deleteCategoryCommand.Parameters.AddWithValue("@CategoryId", categoryId);
                            deleteCategoryCommand.ExecuteNonQuery();
                            MessageBox.Show($"Категория '{categoryName}' успешно удалена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadAdminCategoriesAndProducts();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при удалении категории: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите категорию для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


    }
}
