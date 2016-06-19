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

namespace Client_WPF
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
            bool result = await BusinessLogic.Instance.login(txtUsername.Text, txtPwd.Password);
            if(result)
            {
                Home h = new Home();
                Application.Current.MainWindow.Content = h;
            } else
            {
                MessageBox.Show("Fehler bei der Anmeldung." + Environment.NewLine
                    + "Bitte versuchen Sie es erneut!",
                    "Fehler",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            bool result = await BusinessLogic.Instance.register(txtUsername.Text, txtPwd.Password);
            if(result)
            {
                Home h = new Home();
                Application.Current.MainWindow.Content = h;
            } else
            {
                MessageBox.Show("Fehler bei der Registrierung." + Environment.NewLine 
                    +"Bitte versuchen Sie es erneut!", 
                    "Fehler",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
