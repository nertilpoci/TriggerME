using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace TriggerMeClient.Wpf.Controls
{
    public class GenericUiComboBoxSettings
    {
        public string SelectedValuePath { get; set; }
        public string SelectedValue { get; set; }
        public string DisplayMemberPath { get; set; }
        public  ObservableCollection<object> ItemsSource { get; set; }
        

    }
}