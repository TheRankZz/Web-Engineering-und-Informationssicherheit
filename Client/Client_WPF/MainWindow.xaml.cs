using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
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
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private string txtUserPlaceholder = "Benutzername";
        //private string txtPwdPlaceholder = "Passwort";
        //private Models.User user { get; set; }
        //HttpClient client;
        //private static RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();

        public MainWindow()
        {
            InitializeComponent();
            Login p = new Login();
            this.Content = p;

            //txtUsername.Text = txtUserPlaceholder;
            //txtPwd.Password = txtPwdPlaceholder;
            //tabControl.IsEnabled = false;


            //client = new HttpClient();
            //client.BaseAddress = new Uri("http://10.60.70.15/");
            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        //private async void btnRegister_Click(object sender, RoutedEventArgs e)
        //{
            //Models.RegisterRequest request = new Models.RegisterRequest();

            ////Den salt_masterkey erstellen
            //byte[] salt_masterkey = Encrypt.createSaltMasterKey();
            //request.salt_masterkey = Convert.ToBase64String(salt_masterkey);

            ////Das Schlüsselpaar erstellen
            //string publickey = null;
            //string privatekey = null;
            //Encrypt.createNewKeyPair(out privatekey, out publickey);

            ////Den PublicKey in Base64 konventieren
            //request.pubkey = Util.StringToBase64String(publickey);

            ////Den PrivateKey verschlüsseln und in Base64 konventieren
            //string encryptedprivkey = Encrypt.encryptPrivatekey(privatekey, txtPwd.Password, salt_masterkey);
            //request.privkey_enc = encryptedprivkey;


            //HttpResponseMessage response = await client.PostAsJsonAsync("/" + txtUsername.Text, request);
            //if (response.IsSuccessStatusCode)
            //{
            //    MessageBox.Show("Der Benutzer wurde angelegt!", "Meldung", MessageBoxButton.OK, MessageBoxImage.Information);
            //} else
            //{
            //    MessageBox.Show("Der Benutzer nicht wurde angelegt!", "Meldung", MessageBoxButton.OK, MessageBoxImage.Error);
            //}


                //string testmsg = "Das ist eine Testnachricht!";

                //string cipher = Encrypt.encrptMessage(testmsg, newpublickey);

                //string decrpytMsg = Encrypt.decryptMessage(cipher, decrypetedprivkey);
       
        //}

        //private async void btnLogin_Click(object sender, RoutedEventArgs e)
        //{
        //    Boolean result = false;
        //    HttpResponseMessage response = await client.GetAsync("/" + txtUsername.Text);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        user = new Models.User();
        //        Models.LoginResponse loginresponse = await response.Content.ReadAsAsync<Models.LoginResponse>();

        //        try
        //        {
        //            user.publickey = Util.Base64StringToString(loginresponse.pubkey);
        //            user.salt_masterkey = Convert.FromBase64String(loginresponse.salt_masterkey);
        //            user.privatekey = Encrypt.decryptPrivatekey(loginresponse.privkey_enc, txtPwd.Password, user.salt_masterkey);
        //            result = true;
        //        }
        //        catch
        //        {
        //            MessageBox.Show("Fehler beim Login", "Meldung", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //    }

        //    if (result)
        //    {
        //        txtPwd.IsEnabled = !txtPwd.IsEnabled;
        //        txtUsername.IsEnabled = !txtUsername.IsEnabled;
        //        tabControl.IsEnabled = !tabControl.IsEnabled;
        //        btnLogin.IsEnabled = !btnLogin.IsEnabled;
        //        btnRegister.IsEnabled = !btnRegister.IsEnabled;
        //    }
        //}

        //private async void btnSend_Click(object sender, RoutedEventArgs e)
        //{
        //    HttpResponseMessage response = await client.GetAsync("/" + txtReceiver.Text + "/publickey");
        //    if (response.IsSuccessStatusCode)
        //    {
        //        Models.PubkeyResponse pubkeyresponse = await response.Content.ReadAsAsync<Models.PubkeyResponse>();
        //        string publickeyfromreceiver = Util.Base64StringToString(pubkeyresponse.pubkey);
        //    }
        //}

        //private void btnGetMessage_Click(object sender, RoutedEventArgs e)
        //{

        //}

        







        //private void txtUsername_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    txtUsername.Text = "";
        //}

        //private void txtUsername_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    //txtUsername.Text = txtUserPlaceholder;
        //}

        //private void txtPwd_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    //txtPwd.Password = txtPwdPlaceholder;
        //}

        //private void txtPwd_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    txtPwd.Password = "";
        //}



       


 

        
    }
}
