using IdentityModel.OidcClient;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TriggerMeClient.Wpf.ViewModel;
using TriggerMeClient.Wpf.Views;

namespace TriggerMeClient.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            NetworkChange.NetworkAvailabilityChanged += NetworkAvailabilityChanged;
            try
            {
                Ping myPing = new Ping();

                PingReply reply = myPing.Send("www.google.com", 1000);
                ViewModelLocator.Instance.Main.InternetAvailable = reply != null;
            }
            catch
            {
                ViewModelLocator.Instance.Main.InternetAvailable = false;
            }

            ni.Icon = new System.Drawing.Icon("icon.ico");
           



            ni.DoubleClick +=
                delegate (object sender, EventArgs args)
                {

                    this.WindowState = WindowState.Normal;
                    this.Show();
                };
        }
        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                this.Hide();
                ni.Visible = true;
                ni.Text = "TriggerME";
                ni.BalloonTipText = "Minimized to tray";
                ni.BalloonTipTitle = "TriggerME";
                ni.ShowBalloonTip(5);
            }
            else
            {
                this.Show();
                ni.Visible = false;
            }
            base.OnStateChanged(e);
        }
        private void NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            ViewModelLocator.Instance.Main.InternetAvailable = e.IsAvailable;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModelLocator.Instance.Main.Startup();
        }
        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //until we had a StaysOpen glag to Drawer, this will help with scroll bars
            var dependencyObject = Mouse.Captured as DependencyObject;
            while (dependencyObject != null)
            {
                if (dependencyObject is ScrollBar) return;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            MenuToggleButton.IsChecked = false;
        }

    }
}

