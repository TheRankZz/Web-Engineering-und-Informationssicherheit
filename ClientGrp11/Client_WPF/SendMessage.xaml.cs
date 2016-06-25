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
    /// Interaktionslogik für SendMessage.xaml
    /// </summary>
    public partial class SendMessage : Page
    {
        public SendMessage()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Home h = new Home();
            Application.Current.MainWindow.Content = h;
        }

        private async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            string msg = Util.GetString(txtMessage);
            bool result = await BusinessLogic.Instance.sendMessage(txtReceiver.Text, msg);
            if(result)
            {
                MessageBox.Show("Nachricht wurde gesendet.", "Meldung",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                Home h = new Home();
                Application.Current.MainWindow.Content = h;
            }
            else
            {
                MessageBox.Show("Nachricht konnte nicht gesendet werden.", "Fehler",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
