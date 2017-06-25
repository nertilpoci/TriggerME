using GalaSoft.MvvmLight;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TriggerMeClient.Wpf.Models
{
   public class TriggerAction:ObservableObject, IEditableObject
    {
        private int _id;
        private string _name;
        private string _description;
        private string _file;

        public string Name { get => _name; set { _name = value; RaisePropertyChanged("Name"); } }
        public string Description { get => _description; set { _description = value; RaisePropertyChanged("Description"); } }
        public string File { get => _file; set { _file = value; RaisePropertyChanged("File"); } }

        public int Id { get => _id; set { _id = value; RaisePropertyChanged("Id"); } }

        private Hashtable props = null;

        public void BeginEdit()
        {
            //enumerate properties
            PropertyInfo[] properties = (this.GetType()).GetProperties
                        (BindingFlags.Public | BindingFlags.Instance);

            props = new Hashtable(properties.Length - 1);

            foreach (var t in properties)
            {
                //check if there is set accessor
                if (null == t.GetSetMethod()) continue;
                var value = t.GetValue(this, null);
                props.Add(t.Name, value);
            }
        }

        public void CancelEdit()
        {
            //check for inappropriate call sequence
            if (null == props) return;

            //restore old values
            var properties = (this.GetType()).GetProperties
                (BindingFlags.Public | BindingFlags.Instance);
            foreach (var t in properties)
            {
                //check if there is set accessor
                if (null == t.GetSetMethod()) continue;
                var value = props[t.Name];
                t.SetValue(this, value, null);
            }

            
            props = null;
        }

        public void EndEdit()
        {
            //delete current values
            props = null;
        }
    }
}
