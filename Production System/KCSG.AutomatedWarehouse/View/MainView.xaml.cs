using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using KCSG.AutomatedWarehouse.Interfaces;
using KCSG.AutomatedWarehouse.Model;
using KCSG.AutomatedWarehouse.ViewModel;
using Microsoft.Practices.ServiceLocation;

namespace KCSG.AutomatedWarehouse.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window, IView
    {
        public MainView()
        {
            InitializeComponent();

            Loaded += MainView_Loaded;
            
        }

        private void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext == null)
                return;

            var parentViewModel = (DataContext as ParentViewModel);
            if (parentViewModel == null)
                return;

            parentViewModel.View = this;
        }

        public System.Windows.Window FindView()
        {
            return this;
        }
    }
}
