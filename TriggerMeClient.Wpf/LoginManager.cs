using GalaSoft.MvvmLight;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TriggerMeClient.Wpf.Models;
using TriggerMeClient.Wpf.ViewModel;

namespace TriggerMeClient.Wpf
{
   public class MyApp:ViewModelBase
    {
        
        public MyApp()
        {
           
        }

        public  event EventHandler<bool> LoginStatusChanged;
        public event EventHandler<bool> ClientChanged;
        private  bool isLoggedIn;
        public void SaveClients(List<Client> clients)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, clients);
                ms.Position = 0;
                byte[] buffer = new byte[(int)ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                Properties.Settings.Default.Clients = Convert.ToBase64String(buffer);
                Properties.Settings.Default.Save();
            }
        }
        public List<Client> LoadClients()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.Clients)) return new List<Client>();

            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(Properties.Settings.Default.Clients)))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return (List<Client>)bf.Deserialize(ms);
            }
        }

        public  bool IsLoggedIn { get => isLoggedIn; set { isLoggedIn = value; } }

        public  async Task LoginAsync()
        {
            var loginSucceded = await ViewModelLocator.Instance.Main.Login();
            if (loginSucceded)
            {
                LoginStatusChanged(null, true);
            }
            else
            {
                ViewModelLocator.Instance.Main.AddSnacbkarErrorMessageToQueeue("Login Failed Please try again");

                LoginStatusChanged(null, true);
            }

        }
        public void Logout()
        {
            Properties.Settings.Default.AuthCode = "";
            Properties.Settings.Default.Username = "";
            Properties.Settings.Default.Save();
            LoginStatusChanged(this, false);


        }
        public void NotifyClientChanged(bool hasValue)
        {
            ClientChanged(this, hasValue);

        }
    }


    }

