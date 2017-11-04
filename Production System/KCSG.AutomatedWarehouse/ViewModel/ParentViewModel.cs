using GalaSoft.MvvmLight;
using KCSG.AutomatedWarehouse.Interfaces;

namespace KCSG.AutomatedWarehouse.ViewModel
{
    public class ParentViewModel: ViewModelBase
    {
        /// <summary>
        /// View which relates with view model.
        /// </summary>
        public IView View { get; set; }   
    }
}