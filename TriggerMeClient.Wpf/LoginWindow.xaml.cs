using IdentityModel.Client;
using IdentityModel.OidcClient;
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
using System.Windows.Shapes;
using static IdentityModel.OidcConstants;

namespace TriggerMeClient.Wpf
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow 
    {
        static OidcClient _oidcClient;
        private SystemBrowser _browser;
        private static string _authority = Properties.Settings.Default.ServerUrl;
        public LoginWindow()
        {
            InitializeComponent();
            _browser = new SystemBrowser(port: 7890, browser: webView);

            this.Loaded += LoginView_Loaded;
            this.Unloaded += LoginView_Unloaded;
        }

        private void LoginView_Unloaded(object sender, RoutedEventArgs e)
        {
            _browser.Dispose();
        }
        public event EventHandler<LoginResult> LoginFinished;
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
                Browser = _browser,
                Policy = new Policy { Discovery = new DiscoveryPolicy { RequireHttps = false } }



            };
            
            _oidcClient = new OidcClient(options);
        
            var result = await _oidcClient.LoginAsync();
            
             LoginFinished(this, result);
            
           
           

        }

    }
}
