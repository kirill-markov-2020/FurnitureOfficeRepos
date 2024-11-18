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
            connection = new SqlConnection("Server=KIRILL-MARKOV;Database=Furniture;Integrated Security=True;");
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
                                     FROM CategoryFurniture cf
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
    }
}
