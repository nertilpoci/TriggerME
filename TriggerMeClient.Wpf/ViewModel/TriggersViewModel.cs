using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using myMarket_Desktop.Rules;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TriggerMeClient.Wpf.Enums;
using TriggerMeClient.Wpf.Models;

namespace TriggerMeClient.Wpf.ViewModel
{
   public class TriggersViewModel: ViewModelBase
    {
        private MyApp appManager = ViewModelLocator.Instance.MyApp;
        private ObservableCollection<TriggerMessage> _triggerMessages;

        public ObservableCollection<TriggerMessage> TriggerMessages { get => _triggerMessages; set { _triggerMessages = value; RaisePropertyChanged("TriggerMessages"); } }

        public TriggersViewModel()
        {
            RefreshCommand = new RelayCommand(RefreshClients);
            appManager.ClientChanged += AppManager_ClientChanged;
            SetActionCommand = new RelayCommand<int>(SetAction);
            GetClients(); //todo Make async contructors
        }

        
        public async Task GetClients()
        {
            
            TriggerMessages = new ObservableCollection<TriggerMessage>(await GetTriggerMessages());
        }
        public void RefreshClients()
        {
            GetClients();
        }
        public RelayCommand RefreshCommand { get; set; }

        private void SetAction(int id)
        {

            if (null== Properties.Settings.Default.Actions)
            {
                Properties.Settings.Default.Actions = new System.Collections.Specialized.StringCollection();
                Properties.Settings.Default.Save();
            }
            var actions= Properties.Settings.Default.Actions.Cast<string>().ToList();
            if (id == 0) return;
            System.Windows.Forms.OpenFileDialog dialog = new OpenFileDialog();
            var result = dialog.ShowDialog();
           
            if(result==DialogResult.OK)
            {
                if (actions.Any(z => z.StartsWith(id.ToString()+"|")))
                    {
                    var action = actions.Single(z => z.StartsWith(id.ToString() + "|"));
                    action = id + "|" + dialog.FileName;
                    Properties.Settings.Default.Actions.Clear();
                    Properties.Settings.Default.Actions.AddRange(actions.ToArray());
                }
                else
                {
                    Properties.Settings.Default.Actions.Add(id + "|" + dialog.FileName);
                }
                Properties.Settings.Default.Save();
            }
           
        }

        public RelayCommand<object> ShowAddEditCommand { get; set; }
        public RelayCommand<int> SetActionCommand { get; set; }
     

       

        private  void AppManager_ClientChanged(object sender, bool e)
        {
            GetClients();
        }

        public async  Task<List<TriggerMessage>> GetTriggerMessages()
        {
            if (Properties.Settings.Default.Client == null) return new List<TriggerMessage>();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Properties.Settings.Default.ServerUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Properties.Settings.Default.AuthCode);
            var result =await client.GetAsync("api/Triggers/" + Properties.Settings.Default.Client.Id);
            if (result.IsSuccessStatusCode)
            {

                var newTriggers=Newtonsoft.Json.JsonConvert.DeserializeObject<List<TriggerMessage>>(await result.Content.ReadAsStringAsync());

                if (null == Properties.Settings.Default.Actions)
                {
                    Properties.Settings.Default.Actions = new System.Collections.Specialized.StringCollection();
                    Properties.Settings.Default.Save();
                }
                var actions = Properties.Settings.Default.Actions.Cast<string>().ToList();
                foreach (var trigger in newTriggers)
                {
                    var action = actions.SingleOrDefault(z => z.StartsWith(trigger.Id + "|"));
                    trigger.Action = action?.Split('|')[1];
                }
                ViewModelLocator.Instance.Main.AddSnacbkarSuccessMessageToQueeue("Loaded Triggers");
                return newTriggers;
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                ViewModelLocator.Instance.Main.AddSnacbkarErrorMessageToQueeue("User not authorized. Please Login");

                appManager.Logout();
                return new List<TriggerMessage>();
            }
            else
            {
                ViewModelLocator.Instance.Main.AddSnacbkarErrorMessageToQueeue("Failed to load triggers. Please try again");

                return new List<TriggerMessage>();
            }
        }
    }
}
