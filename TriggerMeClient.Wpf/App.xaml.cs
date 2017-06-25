using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TriggerMeClient.Wpf.ViewModel;

namespace TriggerMeClient.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
         

            ViewModelLocator.Instance.Main.AddSnacbkarErrorMessageToQueeue("Unexpected error has occured. Please check your internet connection");
            ViewModelLocator.Instance.Main.AddSnacbkarErrorMessageToQueeue("Ex: " + e.Exception.Message);


            e.Handled = true;
        }

       
    }
}
