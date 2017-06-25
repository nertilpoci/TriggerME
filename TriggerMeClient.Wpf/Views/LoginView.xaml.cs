using IdentityModel.OidcClient;
using MaterialDesignThemes.Wpf;
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

namespace TriggerMeClient.Wpf.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        static OidcClient _oidcClient;
        private SystemBrowser _browser;
        private static string _authority = Properties.Settings.Default.ServerUrl;

        public LoginView()
        {
            InitializeComponent();
        //    _browser = new SystemBrowser(port: 7890, browser: webView);
            this.Loaded += LoginView_Loaded;
            this.Unloaded += LoginView_Unloaded;
        }

        private void LoginView_Unloaded(object sender, RoutedEventArgs e)
        {
            _browser.Dispose();
        }

        private async void LoginView_Loaded(object sender, RoutedEventArgs e)
        {
            string redirectUri = string.Format("http://127.0.0.1:7890/");
            var options = new OidcClientOptions
            {
                Authority = _authority,
                ClientId = "native.hybrid",
                RedirectUri = redirectUri,
                Scope = "openid profile api1 email",
                FilterClaims = false,
                Browser = _browser

            };
            _oidcClient = new OidcClient(options);

            var result = await _oidcClient.LoginAsync();
            if (!result.IsError)
            {
                Properties.Settings.Default.AuthCode = result.AccessToken;
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.AuthCode = "";
                Properties.Settings.Default.Save();
                MessageBox.Show(result.Error);
            }
            DialogHost.CloseDialogCommand.Execute(null, null);

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogHost.CloseDialogCommand.Execute(null,null);
        }
    }
}
