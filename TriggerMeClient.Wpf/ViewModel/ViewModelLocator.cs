/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:TriggerMeClient.Wpf"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using System;

namespace TriggerMeClient.Wpf.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        private static readonly Lazy<ViewModelLocator> lazy =
        new Lazy<ViewModelLocator>(() => new ViewModelLocator());

        public static ViewModelLocator Instance { get { return lazy.Value; } }

        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>(); 
            SimpleIoc.Default.Register<TriggersViewModel>(); 
            SimpleIoc.Default.Register<HomeViewModel>();
            SimpleIoc.Default.Register<AccountViewModel>();
            SimpleIoc.Default.Register<MyApp>();
            SimpleIoc.Default.Register<ClientViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }
        public SettingsViewModel Settings
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SettingsViewModel>();
            }
        }
        public ClientViewModel ClientViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ClientViewModel>();
            }
        }
        public TriggersViewModel Triggers
        {
            get
            {
                return ServiceLocator.Current.GetInstance<TriggersViewModel>();
            }
        }
        public HomeViewModel Home
        {
            get
            {
                return ServiceLocator.Current.GetInstance<HomeViewModel>();
            }
        }
        public AccountViewModel AccountViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AccountViewModel>();
            }
        }
        public MyApp MyApp
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MyApp>();
            }
        }
  
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}