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
    /// Interaktionslogik für Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();
            tbUser.Text = BusinessLogic.Instance.getUsername();
        }

        private void btnSendPage_Click(object sender, RoutedEventArgs e)
        {
            SendMessage s = new SendMessage();
            Application.Current.MainWindow.Content = s;
        }

        private void btnReadPage_Click(object sender, RoutedEventArgs e)
        {
            GetMessages g = new GetMessages();
            Application.Current.MainWindow.Content = g;
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            BusinessLogic.Instance.logout();
            Login l = new Login();
            Application.Current.MainWindow.Content = l;
        }

        private async void btnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            bool result = await BusinessLogic.Instance.deleteUser();
            if(result) {
                Login l = new Login();
                Application.Current.MainWindow.Content = l;
            } else
            {
                MessageBox.Show("Konto konnte nicht gelöscht werden.", "Fehler",
                        MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
