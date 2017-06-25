using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using IdentityModel.Client;
using MaterialDesignThemes.Wpf;
using Microsoft.AspNet.SignalR.Client;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using TriggerMeClient.Wpf.Models;
using TriggerMeClient.Wpf.Views;
using System;
using System.Windows.Threading;
using System.Linq;
using System.Diagnostics;
using TriggerMeClient.Wpf.Enums;

namespace TriggerMeClient.Wpf.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private bool _isLoggedIn;
        private bool _noClient;
        private string _username;
        private string _conectionState = "Disconnected";
        private string _statusMessage;
        private bool _internetAvailable;
        private SolidColorBrush _color = new SolidColorBrush(Colors.Red);
        private SnackbarMessageQueue _mesageQueue = new SnackbarMessageQueue();
        private MyApp appManager = ViewModelLocator.Instance.MyApp;
        public HubConnection HubConnection;
        private ObservableCollection<NavItem> _navItems;
        private NavItem _currentMenu;
        public MainViewModel()
        {
            appManager.LoginStatusChanged += _loginManger_LoginStatusChanged;
            appManager.ClientChanged += AppManager_ClientChanged;
        }

        private void AppManager_ClientChanged(object sender, bool e)
        {
            this.HubConnection?.Stop();
            if (e)
            {
                Connect(Properties.Settings.Default.AuthCode, Properties.Settings.Default.Client.Identifier.ToString());
                SetNavItems(AppStatus.Ok);
                GoToMenu(Constants.ClientsMenu);
            }
            else
            {
                Startup();

            }
        }

        private void _loginManger_LoginStatusChanged(object sender, bool e)
        {
            this.HubConnection?.Stop();
            Startup();
        }

        public void AddSnacbkarErrorMessageToQueeue(object content)
        {
            MesageQueue.Enqueue(new SnackbarMessage { Content = content, Foreground = new System.Windows.Media.SolidColorBrush(Colors.Red) });

        }
        public void AddSnacbkarSuccessMessageToQueeue(object content)
        {
            MesageQueue.Enqueue(new SnackbarMessage { Content = content });

        }
       
        public async Task<bool> Login()
        {
            TaskCompletionSource<bool> loginTask = new TaskCompletionSource<bool>();
            var loginWindow = new LoginWindow();
            loginWindow.LoginFinished += (sender, login) =>
            {
                if (login.IsError)
                {
                    Properties.Settings.Default.AuthCode = "";
                    Properties.Settings.Default.Username = "";
                    Properties.Settings.Default.Save();
                    loginWindow.Close();
                    loginTask.SetResult(false);
                }
                else
                {
                    Properties.Settings.Default.AuthCode = login.AccessToken;
                    Properties.Settings.Default.Username = login.User.Identity.Name;
                    Properties.Settings.Default.Save();
                    loginWindow.Close();
                    Username = login.User.Identity.Name;
                    loginTask.SetResult(true);
                }

            };


            loginWindow.ShowDialog();
            return await loginTask.Task;
        }
        public void GoToMenu(string menuName)
        {
            CurrentMenu = NavItems.SingleOrDefault(z => z.Name == menuName);
        }
        void SetNavItems(AppStatus status)
        {

            switch (status)
            {
                case  AppStatus.Ok :
                    NavItems = new ObservableCollection<NavItem>
                             {
                               new NavItem(Constants.HomeMenu,new HomeView(), PackIconKind.Home,Visibility.Visible),
                               new NavItem(Constants.ClientsMenu,new ClientsView(),PackIconKind.AccountMultiple,Visibility.Visible),
                               new NavItem(Constants.TriggersMenu,new TriggersView(),PackIconKind.VectorCombine,Visibility.Visible),
                               new NavItem(Constants.AccountMenu, new AccountView(),PackIconKind.Account,Visibility.Visible)
                             };
                    GoToMenu(Constants.HomeMenu);
                    break;
                case AppStatus.NotLoggedIn:
                    NavItems = new ObservableCollection<NavItem>
                             {
                               
                               new NavItem("Account", new AccountView(),PackIconKind.Account,Visibility.Visible)
                             };
                    GoToMenu(Constants.AccountMenu);
                    break;
                case AppStatus.NoClient:
                    NavItems = new ObservableCollection<NavItem>
                             {
                             
                               new NavItem("Clients",new ClientsView(),PackIconKind.VectorCombine,Visibility.Visible),
                               new NavItem("Account", new AccountView(),PackIconKind.Account,Visibility.Visible)
                             };
                    GoToMenu(Constants.ClientsMenu);
                    break;
             


            }
           
            
        }
        public async void Startup()
        {

            if (string.IsNullOrEmpty(Properties.Settings.Default.AuthCode))
            {

                IsLoggedIn = false;
                SetNavItems(AppStatus.NotLoggedIn);


            }
            else
            {
                var tokenValid = await IsAuthTokenValid(Properties.Settings.Default.AuthCode, Properties.Settings.Default.ServerUrl);
                if (!tokenValid)
                {

                    IsLoggedIn = false;
                    SetNavItems(AppStatus.NotLoggedIn);


                }
                else
                {
                    IsLoggedIn = true;
                    Username = Properties.Settings.Default.Username;
                    if (Properties.Settings.Default.Client == null)
                    {
                        ConectionState = "You need to set a client first. Go to Clients";
                        SetNavItems(AppStatus.NoClient);

                        return;
                    }
                    SetNavItems(AppStatus.Ok);
                    Connect(Properties.Settings.Default.AuthCode, Properties.Settings.Default.Client.Identifier.ToString());

                }
            }
        }
        public async Task<bool> IsAuthTokenValid(string authCode, string authority)
        {
            var client = new IntrospectionClient(
           $"{authority}/connect/introspect",
           "api1", "secret");

            var request = new IntrospectionRequest
            {
                Token = Properties.Settings.Default.AuthCode
            };
            try
            {
                var result = await client.SendAsync(request);
                return result.IsActive;
            }
            catch
            {
                AddSnacbkarErrorMessageToQueeue("Error contacting the server");
                return false;
            }


           

        }
        public ObservableCollection<NavItem> NavItems { get { return _navItems; } set { _navItems = value; RaisePropertyChanged("NavItems"); } }
        public void Connect(string authToken, string identifier)
        {
            HubConnection = new HubConnection(Properties.Settings.Default.ServerUrl, true);
            HubConnection.Headers.Add(new KeyValuePair<string, string>("Authorization", "Bearer " + authToken));
            HubConnection.Headers.Add(new KeyValuePair<string, string>("ClientIdentifier", identifier));

            HubConnection.TraceLevel = TraceLevels.All;
            HubConnection.TraceWriter = System.Console.Out;
            IHubProxy myHubProxy = HubConnection.CreateHubProxy("ClientHub");


            myHubProxy.On<TriggerMessage>("sendTrigger", OnTrigered);
            HubConnection.StateChanged += HubConnection_StateChanged;

            HubConnection.Start().Wait();


        }

        SolidColorBrush red = new SolidColorBrush(Colors.Red);
        SolidColorBrush green = new SolidColorBrush(Colors.Green);
        SolidColorBrush orange = new SolidColorBrush(Colors.Orange);
        private void HubConnection_StateChanged(StateChange obj)
        {

            switch (obj.NewState)
            {
                case ConnectionState.Connected:
                    ConectionState = "Connected";
                    Color = green;
                    break;
                case ConnectionState.Disconnected:
                    ConectionState = "Disconnected";
                    Color = red;
                    break;
                case ConnectionState.Connecting:
                    ConectionState = "Connecting....";
                    Color = orange;
                    break;
                case ConnectionState.Reconnecting:
                    ConectionState = "Reconnecting....";
                    Color = orange;
                    break;

            }


        }

        public void OnTrigered(TriggerMessage trigger)
        {
            //todo store local clients differntly then properties

            var actions = Properties.Settings.Default.Actions.Cast<string>().ToList();


            if (actions.Any(z => z.StartsWith(trigger.Id.ToString() + "|")))
            {
                var action = actions.Single(z => z.StartsWith(trigger.Id.ToString() + "|"));
                Process.Start(action.Split('|')[1]);

                trigger.Action = action.Split('|')[1];


            }
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                if (ViewModelLocator.Instance.Home.Output.Count > 2000) ViewModelLocator.Instance.Home.Output.Clear();
                ViewModelLocator.Instance.Home.Output.Insert(0, trigger);
            }));

        }

        public string Username { get => _username; set { _username = value; RaisePropertyChanged("Username"); } }

        public string ConectionState { get => _conectionState; set { _conectionState = value; RaisePropertyChanged("ConectionState"); } }

        public string StatusMessage { get => _statusMessage; set { _statusMessage = value; RaisePropertyChanged("StatusMessage"); } }

        public bool NoClient { get => _noClient; set { _noClient = value; RaisePropertyChanged("NoClient"); } }

        public bool IsLoggedIn { get => _isLoggedIn; set { _isLoggedIn = value; RaisePropertyChanged("IsLoggedIn"); } }

        public SolidColorBrush Color { get => _color; set { _color = value; RaisePropertyChanged("Color"); } }

        public SnackbarMessageQueue MesageQueue { get => _mesageQueue; set { _mesageQueue = value; RaisePropertyChanged("MesageQueue"); } }

        public bool InternetAvailable { get => _internetAvailable; set { _internetAvailable = value; RaisePropertyChanged("InternetAvailable"); } }

        public NavItem CurrentMenu { get => _currentMenu; set { _currentMenu = value;RaisePropertyChanged("CurrentMenu"); } }
    }
}