using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using TriggerMeClient.Wpf.Models;

namespace TriggerMeClient.Wpf.ViewModel
{
   public class ClientViewModel:ViewModelBase
    {

      
        private Client _usedClient;
        private System.Collections.ObjectModel.ObservableCollection<Client> _clients;
        private MyApp appManager = ViewModelLocator.Instance.MyApp;
        public  ClientViewModel()=> ClientViewModelAsync();
        public  void ClientViewModelAsync()
        {
          
            RefreshCommand = new RelayCommand(RefreshClients);
            ChangeClientCommand = new RelayCommand<Client>(ChangeClient);
            appManager.LoginStatusChanged +=  _loginManger_LoginStatusChangedAsync;

           
                InitializeAsync();
            
           
            
               
            
        }

        async Task InitializeAsync()
        {

            Clients = new ObservableCollection<Client>(await GetClients());
            var currentclient = Properties.Settings.Default.Client;
            if(null!=currentclient)
            {
                var serverClient = Clients.SingleOrDefault(z => z.Identifier == currentclient.Identifier);
                if(serverClient!=null)
                {
                    serverClient.IsUsed = true;
                    UsedClient = serverClient;
                    Properties.Settings.Default.Client = serverClient;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    Properties.Settings.Default.Client = null;
                    Properties.Settings.Default.Save();
                    appManager.NotifyClientChanged(true);
                }
            }
        }
        private void ChangeClient(Client obj)
        {


           if(UsedClient!=null) UsedClient.IsUsed= false;
            UsedClient = obj;
            UsedClient.IsUsed = true;
            Properties.Settings.Default.Client = UsedClient;
            Properties.Settings.Default.Save();

            appManager.NotifyClientChanged(true);
        }

        private  void _loginManger_LoginStatusChangedAsync(object sender, bool loggedIn)
        {
          if(loggedIn)   InitializeAsync();
        }

      
        public void RefreshClients()
        {
             InitializeAsync();
        }

       
        public RelayCommand RefreshCommand { get; set; }
        public RelayCommand<Client> ChangeClientCommand { get; set; }

        public async Task<List<Client>> GetClients()
        {

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Properties.Settings.Default.ServerUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Properties.Settings.Default.AuthCode);
            var result = await client.GetAsync("api/Clients?includeTriggers=true");
            if (result.IsSuccessStatusCode)
            {
                ViewModelLocator.Instance.Main.AddSnacbkarSuccessMessageToQueeue("Loaded Clients");

                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<Client>>(await result.Content.ReadAsStringAsync());
            }
            else if(result.StatusCode== System.Net.HttpStatusCode.Unauthorized)
            {
                ViewModelLocator.Instance.Main.AddSnacbkarErrorMessageToQueeue("User not authorized. Please Login");

                appManager.Logout();
                return new List<Client>();
            }
            else
            {
                ViewModelLocator.Instance.Main.AddSnacbkarErrorMessageToQueeue("Failed to load clients. Please try again" );

                return new List<Client>();
            }


        }
        public RelayCommand InitializeCommand { get; set; }
        public Client UsedClient { get => _usedClient; set { _usedClient = value; RaisePropertyChanged("UsedClient"); RefreshCommand.RaiseCanExecuteChanged(); } }
      
        public ObservableCollection<Client> Clients { get => _clients; set { _clients = value; RaisePropertyChanged("Clients"); } }

       
    }
}
