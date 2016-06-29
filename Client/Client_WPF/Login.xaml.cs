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

namespace Client
{
    /// <summary>
    /// Interaktionslogik für Page1.xaml
    /// </summary>
    public partial class Login : Page
    {
        public Login()
        {
            InitializeComponent();
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string pwd = txtPwd.Password.Trim();
            if (username != "" && pwd != "" && username != "Benutzername" && pwd != "Password")
            {
                bool result = await BusinessLogic.Instance.login(username, pwd);
                if (result)
                {
                    Home h = new Home();
                    Application.Current.MainWindow.Content = h;
                }
                else
                {
                    MessageBox.Show("Fehler bei der Anmeldung." + Environment.NewLine
                        + "Bitte versuchen Sie es erneut!",
                        "Fehler",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            } else
            {
                MessageBox.Show("Benutzername oder Kennwort ungültig.", "Fehler",
                        MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string pwd = txtPwd.Password.Trim();
            if (username != "" && pwd != "" && username != "Benutzername" && pwd != "Password")
            {
                bool result = await BusinessLogic.Instance.register(username, pwd);
                if (result)
                {
                    Home h = new Home();
                    Application.Current.MainWindow.Content = h;
                }
                else
                {
                    MessageBox.Show("Fehler bei der Registrierung." + Environment.NewLine
                        + "Bitte versuchen Sie es erneut!",
                        "Fehler",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            } else
            {
                MessageBox.Show("Benutzername oder Kennwort ungültig.", "Fehler",
                        MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtUsername_GotFocus(object sender, RoutedEventArgs e)
        {
            if(txtUsername.Text == "Benutzername")
                txtUsername.Text = "";

        }

        private void txtUsername_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtUsername.Text == "")
                txtUsername.Text = "Benutzername";
        }

       
        private void txtPwd_GotFocus(object sender, RoutedEventArgs e)
        {
            if(txtPwd.Password == "Password")
                txtPwd.Password = "";
        }

        private void txtPwd_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtPwd.Password == "")
                txtPwd.Password = "Password";
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(("Gruppe 1:" + Environment.NewLine +
                        "Julia Bracht, Nicolas Burchert" + Environment.NewLine 
                        + "Lennart Giesen und Julius Wessing"), "Info",
                        MessageBoxButton.OK, 
                        MessageBoxImage.Information);
        }
    }
}
