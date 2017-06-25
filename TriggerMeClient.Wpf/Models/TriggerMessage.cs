using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace TriggerMeClient.Wpf.Models
{
    [Serializable]
    public   class TriggerMessage: INotifyPropertyChanged
    {
        public TriggerMessage()
        {

        }

        private string _action;
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;

            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(prop);
                handler(this, e);
            }
        }

        public int Id { get; set; }
        public string Name { get;set;}
        public string Description { get; set; }
        public string Secret { get; set; }
        public int ClientId { get; set; }
        public int UserId { get;set; }
        public virtual Client Client { get; set; }
        public string Action { get => _action; set { _action = value; OnPropertyChanged("Action"); } }

       

        //void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    info.AddValue("Id", this.Id);
        //    info.AddValue("Name", this.Name);
        //    info.AddValue("Description", this.Description);
        //    info.AddValue("Action", this.Action);
        //    info.AddValue("Secret", this.Secret);
        //    info.AddValue("ClientId", this.ClientId);
        //    info.AddValue("UserId", this.UserId);
        //}
    }
}
