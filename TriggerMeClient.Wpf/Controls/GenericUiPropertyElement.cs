using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TriggerMeClient.Wpf.Enums;

namespace TriggerMeClient.Wpf.Controls
{
  public  class GenericUiPropertyElement
    {
    
        
        public  string Label { get; set; }
        public  string PropertyName { get; set; }
        public  UiElementBindingType ElementType { get; set; }
        public  GenericUiComboBoxSettings ComboBoxSettings { get; set; }
       public List<ValidationRule> ValidationRules { get; set; }=new List<ValidationRule>();
    }
}
