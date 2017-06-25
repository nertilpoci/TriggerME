using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;
using myMarket_Desktop.Rules;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TriggerMeClient.Wpf.Controls;
using TriggerMeClient.Wpf.Enums;
using TriggerMeClient.Wpf.Models;
using TriggerMeClient.Wpf.Rules;

namespace TriggerMeClient.Wpf.ViewModel
{
   public class ActionsViewModel:ViewModelBase
    {
        public ActionsViewModel()
        {
            Actions = new ObservableCollection<TriggerAction>();
            NewEditCommand = new RelayCommand<object>(AddEdit);
        }

        private async void AddEdit(object obj)
        {

            TriggerAction Action = null;

            var title = "New Action";
            if (null == obj) Action = new TriggerAction();
            else
            {
                Action = obj as TriggerAction;
                title = "Edit Action";
                Action.BeginEdit();

            }



            await new AddEditGenericUiControl()
                .WithTitle(title)
                .WithModel(Action)
                .WithPositiveCallBack(Add)
                .WithNegativeCallBack((m) => { Action.CancelEdit();Action.EndEdit(); })
                 .WithValidator(new ActionValidator())
                .AddProperty(new GenericUiPropertyElement()
                {
                    PropertyName = "Name",
                    ElementType = UiElementBindingType.TextBox,
                    Label = "Name",
                    ValidationRules = { new NotEmptyValidationRule() }
                })
                .AddProperty(new GenericUiPropertyElement()
                {
                    PropertyName = "Description",
                    ElementType = UiElementBindingType.TextBox,
                    Label = "Description"
                    
                })
                .AddProperty(new GenericUiPropertyElement()
                {
                    PropertyName = "File",
                    ElementType = UiElementBindingType.TextBox,
                    Label = "File",
                    ValidationRules = {new FileMustExistValidationRule()}

                })
               
               .ShowDialog();
        }

        private void Add(object obj)
        {
           
            if (null != obj)
            {
                var action = obj as TriggerAction;
                if(action.Id!=0)
                {

                }
                
             

            }
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        private ObservableCollection<TriggerAction> _actions;

        public RelayCommand<object> NewEditCommand { get; set; }

        public ObservableCollection<TriggerAction> Actions { get => _actions; set { _actions = value; RaisePropertyChanged("Actions"); } }
    }
}
