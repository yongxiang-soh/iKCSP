using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Interfaces.TabletisingCommandSubSystem;

namespace KCSG.Domain.Domains.TabletisingCommandSubSystem
{
    public class ManagementOfProductLabelDomain:IManagementOfProductLabelDomain
    {
        #region Properties

        /// <summary>
        /// Provides repositories to access database.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Constructors
        
        /// <summary>
        /// Initiate instance with IoC.
        /// </summary>
        /// <param name="unitOfWork"></param>
        public ManagementOfProductLabelDomain(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion



        public TM09_Product GetProductItem(string productCode)
        {
            var product = _unitOfWork.ProductRepository.Get(i => i.F09_ProductCode.Trim().Equals(productCode));
            return product;
        }


        public TX42_KndCmd GetKneadingCommand(string commandNo, string lotNo)
        {
            var kneadingCommand =
                _unitOfWork.KneadingCommandRepository.Get(
                    i => i.F42_KndCmdNo.Trim().Equals(commandNo) && i.F42_PrePdtLotNo.Trim().Equals(lotNo));
            return kneadingCommand;
        }


        //public TM14_Device GetDevice(string commandNo, string lotNo, string tableLine)
        //{
        //    var device = _unitOfWork.DeviceRepository.Get(i => i.F14_DeviceCode.Trim().Equals(tableLine));
        //    return device;
        //}

        public int? GetCSNo1(string commandNo, string preProductLotNo)
        {
            var tableCommand =
                _unitOfWork.TabletCommandRepository.Get(
                    i => i.F41_KndCmdNo.Trim().Equals(commandNo) && i.F41_PrePdtLotNo.Trim().Equals(preProductLotNo));


            if (tableCommand != null)
            {
                var device =
                    _unitOfWork.DeviceRepository.Get(
                        i => i.F14_DeviceCode.Trim().Equals(tableCommand.F41_TabletLine.Trim()));
                if (device != null)
                {
                    return device.F14_CSNumber;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                return 1;
            }
        }
    }
}
