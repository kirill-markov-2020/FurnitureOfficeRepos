using System;
using System.Collections.Generic;
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
    }
}
