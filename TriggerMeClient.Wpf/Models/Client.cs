using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;

namespace TriggerMeClient.Wpf.Models
{[Serializable]
   public class Client:ObservableObject
    {
        private bool _isUsed;
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid Identifier { get; set; }
        public bool IsUsed { get { return _isUsed; } set { _isUsed = value;RaisePropertyChanged("IsUsed"); } }

        public List<TriggerMessage> TriggerMessages { get; set; } = new List<TriggerMessage>();
       


    }
}
