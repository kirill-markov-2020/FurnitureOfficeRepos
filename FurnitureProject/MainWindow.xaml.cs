using FurnitureProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;


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
            InitializeLoadingTimer();
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

        private DispatcherTimer loadingTimer;
        private void InitializeLoadingTimer()
        {
            loadingTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3.8)
            };
            loadingTimer.Tick += (s, e) =>
            {
                loadingTimer.Stop();
                LoadingPanel.Visibility = Visibility.Collapsed;
                switch (CurrentRole)
                {
                    case "Manager":
                        ManagerPanel.Visibility = Visibility.Visible;
                        LoadManagerCategoriesAndProducts();
                        break;
                    case "Administrator":
                        AdminSelectionPanel.Visibility = Visibility.Visible;
                        LoadAdminCategoriesAndProducts();
                        break;
                    case "Consultant":
                        ConsultantPanel.Visibility = Visibility.Visible;
                        LoadCategories();
                        break;
                }
            };
        }
        private string CurrentRole;
        private void AuthorizationButton_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxInputLogin.Text == "Введите логин" || string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                MessageBox.Show("Введите логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var dbContext = new AppDbContext())
            {
                try
                {
                    var user = dbContext.Users
                        .FirstOrDefault(u => u.Login == TextBoxInputLogin.Text && u.Password == PasswordBox.Password);

                    if (user != null)
                    {
                        CurrentRole = user.Role;
                        AuthorizationPanel.Visibility = Visibility.Collapsed;                      
                        switch (CurrentRole)
                        {
                            case "Manager":
                                ShowLoadingPanel();
                                LoadManagerCategoriesAndProducts();
                                break;
                            case "Administrator":
                                ShowLoadingPanel(); 
                                break;
                            case "Consultant":
                                ShowLoadingPanel();
                                LoadCategories();
                                break;
                            default:
                                MessageBox.Show("Неизвестная роль пользователя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                                AuthorizationPanel.Visibility = Visibility.Visible;
                                break;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при авторизации: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    Debug.WriteLine(ex.Message);
                }
            }
        }



        private void ShowLoadingPanel()
        {
            LoadingPanel.Visibility = Visibility.Visible;
            loadingTimer.Start();
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
                AdminSelectionPanel.Visibility = Visibility.Collapsed;
                ConsultantPanel.Visibility = Visibility.Collapsed;
                AuthorizationPanel.Visibility = Visibility.Visible;
            }
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
            using (var dbContext = new AppDbContext())
            try
            {
                var category = dbContext.Categories.FirstOrDefault(c => c.Name == categoryName);
                if (category != null)
                {
                    var product = new Product
                    {
                        Name = productName,
                        Price = price,
                        Quantity = quantity,
                        CategoryFurniture_Id = category.Id
                    };

                    dbContext.Products.Add(product);
                    dbContext.SaveChanges();
                    MessageBox.Show("Товар успешно добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadAdminCategoriesAndProducts();
                    AddProductPanel.Visibility = Visibility.Collapsed;
                    AdministratorPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBox.Show("Ошибка: Категория не найдена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
                string[] productParts = productInfo.Split(new string[] { "; Цена: ", "; Товаров на складе: " }, StringSplitOptions.None);
                string productName = productParts[0].Replace("Товар: ", "");
                var result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить товар '{productName}'?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                using (var dbContext = new AppDbContext())
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            var product = dbContext.Products.FirstOrDefault(p => p.Name == productName);

                            if (product != null)
                            {
                                dbContext.Products.Remove(product);
                                dbContext.SaveChanges();
                                MessageBox.Show($"Товар '{productName}' успешно удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                                LoadAdminCategoriesAndProducts();
                            }
                            else
                            {
                                MessageBox.Show("Товар не найден в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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









        // AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA


        private void ShowUsersPanel_Click(object sender, RoutedEventArgs e)
        {
            AdminSelectionPanel.Visibility = Visibility.Collapsed;
            UsersPanel.Visibility = Visibility.Visible;
            LoadUsers();

        }

        private void ShowDatabasePanel_Click(object sender, RoutedEventArgs e)
        {
            AdminSelectionPanel.Visibility = Visibility.Collapsed;
            AdministratorPanel.Visibility = Visibility.Visible;
        }

        private void BackToAdminSelectionPanel_Click(object sender, RoutedEventArgs e)
        {
            UsersPanel.Visibility = Visibility.Collapsed;
            AdministratorPanel.Visibility = Visibility.Collapsed;
            AdminSelectionPanel.Visibility = Visibility.Visible;
        }

        private void LoadUsers()
        {
            using (var dbContext = new AppDbContext())
            {
                var users = dbContext.Users.ToList();
                UsersListView.ItemsSource = null; 
                UsersListView.ItemsSource = users;
            }
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            UsersPanel.Visibility = Visibility.Collapsed;
            AddUserPanel.Visibility = Visibility.Visible;
        }

        private void CreateUserButton_Click(object sender, RoutedEventArgs e)
        {
            string name = UserNameTextBox.Text.Trim();
            string surname = UserSurnameTextBox.Text.Trim();
            string patronymic = UserPatronymicTextBox.Text.Trim();
            string login = UserLoginTextBox.Text.Trim();
            string password = UserPasswordBox.Password;
            string confirmPassword = UserPasswordConfirmBox.Password;
            string role = (RoleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(surname) ||
                string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var dbContext = new AppDbContext())
                {
                    if (dbContext.Users.Any(u => u.Login == login))
                    {
                        MessageBox.Show("Пользователь с таким логином уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var newUser = new User
                    {
                        Name = name,
                        Surname = surname,
                        Patronymic = patronymic,
                        Login = login,
                        Password = password,
                        Role = role
                    };

                    dbContext.Users.Add(newUser);
                    dbContext.SaveChanges();
                }

                MessageBox.Show("Пользователь успешно создан!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                BackToAdminPanel_Click(sender, e);
                LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении пользователя: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackToAdminPanel_Click(object sender, RoutedEventArgs e)
        {
            UpdateUserPanel.Visibility = Visibility.Collapsed;
            AddUserPanel.Visibility = Visibility.Collapsed;
            UsersPanel.Visibility = Visibility.Visible;
            UserNameTextBox.Text = string.Empty;
            UserSurnameTextBox.Text = string.Empty;
            UserPatronymicTextBox.Text = string.Empty;
            UserLoginTextBox.Text = string.Empty;
            UserPasswordBox.Password = string.Empty;
            UserPasswordConfirmBox.Password = string.Empty;
            RoleComboBox.SelectedIndex = -1;
            UpdateUserNameTextBox.Text = string.Empty;
            UpdateUserSurnameTextBox.Text = string.Empty;
            UpdateUserPatronymicTextBox.Text = string.Empty;
            UpdateUserLoginTextBox.Text = string.Empty;
            UpdateUserRoleComboBox.SelectedIndex = -1;
            UpdateUserPasswordTextBox.Text = string.Empty;
            LoadUsers();
        }

        //УДАЛЕНИЕ ПОЛЬЗОВАТЕЛЕЙ

        private void UsersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {         
            DeleteUserButton.IsEnabled = UsersListView.SelectedItem != null;
            UpdateUserButton.IsEnabled = UsersListView.SelectedItem != null;
        }

        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListView.SelectedItem == null) return;
            var selectedUser = (dynamic)UsersListView.SelectedItem;
            string loginToDelete = selectedUser.Login;

            MessageBoxResult result = MessageBox.Show(
                $"Вы действительно хотите удалить пользователя '{loginToDelete}'?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var dbContext = new AppDbContext())
                    {
                        var user = dbContext.Users.FirstOrDefault(u => u.Login == loginToDelete);
                        if (user != null)
                        {
                            dbContext.Users.Remove(user);
                            dbContext.SaveChanges();
                            MessageBox.Show("Пользователь успешно удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadUsers();
                        }
                        else
                        {
                            MessageBox.Show("Пользователь не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении пользователя: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        //ОБНОВЛЕНИЕ ПОЛЬЗОВАТЕЛЕЙ

        private void UpdateUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListView.SelectedItem is User selectedUser)
            {
                UpdateUserPanel.Visibility = Visibility.Visible;
                UsersPanel.Visibility = Visibility.Collapsed;
                UpdateUserNameTextBox.Text = selectedUser.Name;
                UpdateUserSurnameTextBox.Text = selectedUser.Surname;
                UpdateUserPatronymicTextBox.Text = selectedUser.Patronymic;
                UpdateUserLoginTextBox.Text = selectedUser.Login;
                UpdateUserRoleComboBox.SelectedItem = selectedUser.Role;
                UpdateUserPasswordTextBox.Text = selectedUser.Password;
            }
        }

        private void SaveUpdatedUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListView.SelectedItem is User selectedUser)
            {
                try
                {
                    using (var dbContext = new AppDbContext())
                    {
                        var userToUpdate = dbContext.Users.FirstOrDefault(u => u.Id == selectedUser.Id);

                        if (userToUpdate != null)
                        {
                            userToUpdate.Name = UpdateUserNameTextBox.Text.Trim();
                            userToUpdate.Surname = UpdateUserSurnameTextBox.Text.Trim();
                            userToUpdate.Patronymic = UpdateUserPatronymicTextBox.Text.Trim();
                            userToUpdate.Login = UpdateUserLoginTextBox.Text.Trim();
                            userToUpdate.Role = (UpdateUserRoleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                            userToUpdate.Password = UpdateUserPasswordTextBox.Text.Trim();
                            dbContext.SaveChanges();
                            MessageBox.Show("Данные пользователя успешно обновлены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadUsers();
                            BackToAdminPanel_Click(sender, e);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при обновлении данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }











    }
}
