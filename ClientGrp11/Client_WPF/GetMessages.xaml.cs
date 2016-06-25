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
    /// Interaktionslogik für GetMessages.xaml
    /// </summary>
    public partial class GetMessages : Page
    {
        public GetMessages()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Home h = new Home();
            Application.Current.MainWindow.Content = h;
        }

        private async void btnGetMessage_Click(object sender, RoutedEventArgs e)
        {
            var msg = await BusinessLogic.Instance.getMessage();
            if(msg != null)
            {
                txtSender.Text = msg.sender;
                txtMessage.Document.Blocks.Clear();
                txtMessage.Document.Blocks.Add(new Paragraph(new Run(msg.content)));
            } else
            {
                MessageBox.Show("Es wurde keine Nachricht gefunden.", "Medlung",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
