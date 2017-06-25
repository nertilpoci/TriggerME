using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using FluentValidation;
using GalaSoft.MvvmLight.Command;

using myMarket_Desktop.Rules;
using MaterialDesignThemes.Wpf;
using GalaSoft.MvvmLight;
using TriggerMeClient.Wpf.Enums;

namespace TriggerMeClient.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for AddEditGenericUiControl.xaml
    /// </summary>
    public partial class AddEditGenericUiControl : UserControl 
    {
        public object Model;
        public IValidator Validator;
        public List<GenericUiPropertyElement> Properties;
        public RelayCommand AddUpdateCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }

        public String Title { get; set; }

     

        public Action<object> SaveEditAction { get; set; }
        public Action<object> CancelAction { get; set; }




        public AddEditGenericUiControl()
        {
            InitializeComponent();
            AddUpdateCommand = new RelayCommand(Save, CanAddOrSave);
            CancelCommand=new RelayCommand(Cancel);
            this.DataContext = this;
        }
        void UpdateCommands()
        {

            AddUpdateCommand.RaiseCanExecuteChanged();
            CancelCommand.RaiseCanExecuteChanged();

        }




       

        private void AddElement(GenericUiPropertyElement element)
        {

            if (element.ElementType == UiElementBindingType.TextBox)
            {
                MainGrid.RowDefinitions.Add(new RowDefinition());




                var textBox = new TextBox();
                textBox.SetValue(HintAssist.HintProperty, element.Label);
                textBox.FontSize = 18;
                textBox.Style = this.FindResource("MaterialDesignFloatingHintTextBox") as Style;
                Binding myBinding = new Binding
                {
                    Source = Model,
                    Path = new PropertyPath(element.PropertyName),
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    ValidatesOnDataErrors = true,

                };
                element.ValidationRules.ForEach(x =>
                {
                    myBinding.ValidationRules.Add(x);

                });

                BindingOperations.SetBinding(textBox, TextBox.TextProperty, myBinding);
                Grid.SetRow(textBox, MainGrid.RowDefinitions.Count - 1);
                MainGrid.Children.Add(textBox);
            }
            else if (element.ElementType == UiElementBindingType.ComboBox)
            {
                MainGrid.RowDefinitions.Add(new RowDefinition());
                ComboBox combobox = new ComboBox();
                //var grdEncoding = new ASCIIEncoding();
                //var str =
                //    String.Format(comboBoxString, element.Label, MainGrid.RowDefinitions.Count - 1,
                //        element.ComboBoxSettings.SelectedValuePath, element.ComboBoxSettings.SelectedValue,
                //        element.ComboBoxSettings.DisplayMemberPath);
                //var grdBytes = grdEncoding.GetBytes(str);
                //combobox = (ComboBox)XamlReader.Load(new MemoryStream(grdBytes));

                Grid.SetRow(combobox, MainGrid.RowDefinitions.Count - 1);
                combobox.SetBinding(
   ItemsControl.ItemsSourceProperty,
   new Binding { Source = element.ComboBoxSettings.ItemsSource });
                combobox.DisplayMemberPath = element.ComboBoxSettings.DisplayMemberPath;
                combobox.SelectedValuePath = element.ComboBoxSettings.SelectedValuePath;
                Binding myBinding = new Binding
                {
                    Source = Model,
                    Path = new PropertyPath(element.ComboBoxSettings.SelectedValue),
                 

                };
                combobox.SetBinding(
   Selector.SelectedValueProperty,
   myBinding);
                combobox.SetValue(HintAssist.HintProperty, element.Label);
                combobox.SetValue(HintAssist.IsFloatingProperty,true);
                MainGrid.Children.Add(combobox);



            }
        }

        private void BuildDialog()
        {
            Properties?.ForEach(x => { if (x != null) AddElement(x); });
        }

        public async Task<object> ShowDialog()
        {
            // update command can add 
            if (null == Model) throw new ArgumentNullException("Model", "Model cannot be null. Call WithModel(your model) before calling show dialog");
           ((ObservableObject)Model).PropertyChanged += (s, e) => { UpdateCommands(); };
            // Nderto Dialogun
            BuildDialog();

            return await DialogHost.Show(this, "RootDialog");

        }

        public void Save()
        {
            SaveEditAction?.Invoke(Model);
        }
        public void Cancel()
        {
            DialogHost.CloseDialogCommand.Execute(null, null);
            CancelAction?.Invoke(Model);
        }
        public bool CanAddOrSave()
        {
                 return Validator == null || Validator.Validate(Model).IsValid;
            
        }




        #region chaining

        public AddEditGenericUiControl WithTitle(string title)
        {
            Title = title;
            return this;
        }

        public AddEditGenericUiControl WithModel(object model)
        {
            Model = model;
            return this;
        }

        public AddEditGenericUiControl AddProperty(GenericUiPropertyElement property)
        {
            if (null == Properties) Properties = new List<GenericUiPropertyElement>();
            Properties.Add(property);
            return this;
        }
        public AddEditGenericUiControl WithValidator(IValidator validator)
        {
            Validator = validator;
            return this;
        }
        public AddEditGenericUiControl WithPositiveCallBack(Action<object> callback)
        {

            SaveEditAction = callback;
            return this;
        }
        public AddEditGenericUiControl WithNegativeCallBack(Action<object> callback)
        {

            CancelAction = callback;
            return this;
        }


        #endregion

    }
}

