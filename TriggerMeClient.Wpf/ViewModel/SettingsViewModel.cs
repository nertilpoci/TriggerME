using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Net.Http;
using System.Net.Http.Headers;
using TriggerMeClient.Wpf.Models;
using System.Windows;

namespace TriggerMeClient.Wpf.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private string _identifier;
        public SettingsViewModel()
        {
            SaveSettingsCommand = new RelayCommand(SaveSettings);
            Identifier = Properties.Settings.Default.Client?.Identifier.ToString();
        }

        private async void SaveSettings()
        {
             HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(Properties.Settings.Default.ServerUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Properties.Settings.Default.AuthCode);
            var result = await client.GetAsync("api/Clients/"+Identifier);
            if (result.IsSuccessStatusCode)
            {

                Properties.Settings.Default.Client = await result.Content.ReadAsAsync<Client>();
                Identifier = Properties.Settings.Default.Client.Identifier.ToString();
                Properties.Settings.Default.Save();
            }
            {
                Identifier = "";
                MessageBox.Show(result.StatusCode.ToString());
            }
        }

        public RelayCommand SaveSettingsCommand { get; set; }
        public string Identifier {
             get { return _identifier; }
             set {
                _identifier = value;
                RaisePropertyChanged("Identifier");
                 }
        }
    }
}