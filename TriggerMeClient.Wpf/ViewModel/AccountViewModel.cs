using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using TriggerMeClient.Wpf.Models;

namespace TriggerMeClient.Wpf.ViewModel
{
   public class AccountViewModel:ViewModelBase
    {
        private string _username;
        private bool _isLoggedIn;
        private bool _loginFailed;
        private Client _usedClient;
        private System.Collections.ObjectModel.ObservableCollection<Client> _clients;
        private MyApp appManager = ViewModelLocator.Instance.MyApp;
        
        public AccountViewModel()
        {
            LoginCommand = new RelayCommand(Login);
            LogoutCommand = new RelayCommand(Logout);
         
            appManager.LoginStatusChanged += _loginManger_LoginStatusChanged;
            IsLoggedIn = ViewModelLocator.Instance.Main.IsLoggedIn;
        }

        private void _loginManger_LoginStatusChanged(object sender, bool loggedIn)
        {
         IsLoggedIn = loggedIn;
            Initialize();
       }

      

        private void Logout()
        {
            appManager.Logout();
            Username = "";
        }


        public async void Initialize()
        {
            Username = Properties.Settings.Default.Username;


        }
      
        public RelayCommand LoginCommand { get; set; }
        public RelayCommand LogoutCommand { get; set; }
       
      
        public RelayCommand InitializeCommand { get; set; }
        public string Username { get => _username; set { _username = value; RaisePropertyChanged("Username"); } }
        public bool IsLoggedIn { get => _isLoggedIn; set { _isLoggedIn = value; RaisePropertyChanged("IsLoggedIn"); } }
        public bool LoginFailed { get => _loginFailed; set { _loginFailed = value; RaisePropertyChanged("LoginFailed"); } }

        public ObservableCollection<Client> Clients { get => _clients; set { _clients = value; RaisePropertyChanged("Clients"); } }

        public async void Login()
        {
         await   appManager.LoginAsync();
            Username = Properties.Settings.Default.Username;
        }

    }
}
