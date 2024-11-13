using System;
using System.Collections.Generic;
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

        private SqlConnection connection;

        private SqlConnection GetDatabaseConnection()
        {
            connection = new SqlConnection("Server=KIRILL-MARKOV;Database=Furniture;Integrated Security=True;");
            return connection;
        }
    }
}
