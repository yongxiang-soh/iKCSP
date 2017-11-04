/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:KCSG.AutomatedWarehouse"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using System.Collections.Generic;
using System.Configuration;
using GalaSoft.MvvmLight.Ioc;
using KCSG.AutomatedWarehouse.Model;
using KCSG.AutomatedWarehouse.ViewModel.AutoWarehouse;
using Microsoft.Practices.ServiceLocation;

namespace KCSG.AutomatedWarehouse.ViewModel
{
    /// <summary>
    ///     This class contains static references to all the view models in the
    ///     application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        ///     Initializes a new instance of the ViewModelLocator class.
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
            SimpleIoc.Default.Register<MessageViewModel>();
            SimpleIoc.Default.Register<AutoControllerViewModel>();
            SimpleIoc.Default.Register<MaterialAutoWarehouseController>();
            SimpleIoc.Default.Register<PreProductAutoWarehouseController>();
            SimpleIoc.Default.Register<ProductAutoWarehouseController>();
            SimpleIoc.Default.Register<ConverterViewModel>();

            var setting = new SettingViewModel();
            setting = SearchTerminalSetting(setting);
            SimpleIoc.Default.Register(() => setting);
        }

        /// <summary>
        /// Instance handles main view businesses.
        /// </summary>
        public MainViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }


        /// <summary>
        ///     Application instance which holds instances to handle businesses.
        /// </summary>
        public SettingViewModel Setting
        {
            get { return ServiceLocator.Current.GetInstance<SettingViewModel>(); }
        }

        /// <summary>
        /// Instance which handles system message business.
        /// </summary>
        public MessageViewModel Message
        {
            get { return ServiceLocator.Current.GetInstance<MessageViewModel>(); }
        }

        #region Methods

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }

        /// <summary>
        /// Search terminal setting from configuration file.
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        private SettingViewModel SearchTerminalSetting(SettingViewModel setting)
        {
#if !FORGING_TERMINALS
            // Find terminal setting file path from application configuration manager.
            var terminalSettingRelativeUrl = ConfigurationManager.AppSettings["TerminalsConfigurationFile"];
            if (string.IsNullOrEmpty(terminalSettingRelativeUrl))
                return setting;

            setting.LoadTerminalSettingsFromFile(terminalSettingRelativeUrl);
#else
            setting.LoadTerminalSettingsFromFile("");
#endif
            return setting;
        }

#endregion
    }
}