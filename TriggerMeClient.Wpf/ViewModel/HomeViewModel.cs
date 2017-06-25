using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriggerMeClient.Wpf.Models;

namespace TriggerMeClient.Wpf.ViewModel
{
  public  class HomeViewModel:ViewModelBase
    {
        private Client _client;
        private MyApp appManager = ViewModelLocator.Instance.MyApp;
        private ObservableCollection<TriggerMessage> _output = new ObservableCollection<TriggerMessage>();
        public HomeViewModel()
        {
       
            InitializeCommand = new RelayCommand(Initialize);
            appManager.ClientChanged += AppManager_ClientChanged;
        }

        private void AppManager_ClientChanged(object sender, bool e)
        {
            Initialize();
        }

        public RelayCommand InitializeCommand { get; set; }
        public Client Client { get => _client; set { _client = value; RaisePropertyChanged("Client"); } }

        public ObservableCollection<TriggerMessage> Output { get => _output; set { _output = value; RaisePropertyChanged("Output"); } }

        public void Initialize()
        {
            Client = Properties.Settings.Default.Client;


        }
    }
}
