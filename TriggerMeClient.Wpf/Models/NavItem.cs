using GalaSoft.MvvmLight;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TriggerMeClient.Wpf.Models
{
    public class NavItem : ObservableObject
    {
        private string _name;
        private object _content;
        private PackIconKind _icon;
        private Visibility _visibility=Visibility.Visible;


        public NavItem(string name, object content, PackIconKind icon,Visibility visibility )
        {
            Name = name;
            Content = content;
            Icon = icon;
            Visibility = visibility;

        }

        public string Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged("Name"); }
        }

        public object Content
        {
            get { return _content; }
            set { _content = value; RaisePropertyChanged("Content"); }
        }

        public PackIconKind Icon { get => _icon; set { _icon = value; RaisePropertyChanged("Icon");} }

        public Visibility Visibility { get => _visibility; set { _visibility = value; RaisePropertyChanged("Visibility"); } }
    }
}
