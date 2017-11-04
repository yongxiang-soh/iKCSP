using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Models.MaterialManagement;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office.Word;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Domain.Interfaces.Services;

namespace KCSG.Domain.Domains.MaterialManagement
{
    public class InterFloorMovementOfMaterialDomain : BaseDomain, IInterFloorMovementOfMaterialDomain
    {


        #region Constructor
        public InterFloorMovementOfMaterialDomain(IUnitOfWork unitOfWork, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {

        }
        #endregion

        //Check [tm05_conveyor] record 
        public bool CheckRecordTm05(string terminalNo)
        {
            var lstConveyor =
                _unitOfWork.ConveyorRepository.GetMany(
                    i =>
                        i.F05_TerminalNo.Trim().Equals(Constants.TerminalNo.A018) ||
                        i.F05_TerminalNo.Trim().Equals(terminalNo));
            if (lstConveyor.Any())
                return true;
            return false;

        }
        //Check status [tm05_conveyor]
        public bool CheckConveyorStatus(string terminalNo)
        {
            var conveyorStatus = Constants.F05_StrRtrSts.Error.ToString("D");
            var lstConveyor =
                _unitOfWork.ConveyorRepository.GetMany(
                    i =>
                        i.F05_TerminalNo.Trim().Equals(terminalNo) ||
                        i.F05_TerminalNo.Trim().Equals(Constants.TerminalNo.A018));
            lstConveyor = lstConveyor.Where(i => i.F05_StrRtrSts.Equals(conveyorStatus));
            if (lstConveyor.Any())
                return false;
            return true;
        }
        //Check Warehouse status 
        public bool CheckedWarehouseStatus()
        {
            var lstDevice =
                _unitOfWork.DeviceRepository.GetMany(
                    x =>
                        x.F14_DeviceCode.Trim() == Constants.DeviceCode.ATW001 ||
                        x.F14_DeviceStatus.Trim() == Constants.F14_DeviceStatus.Offline.ToString() ||
                        x.F14_DeviceStatus.Trim() == Constants.F14_DeviceStatus.Error.ToString() ||
                        x.F14_UsePermitClass.Trim() == Constants.F14_UsePermitClass.Prohibited.ToString());
            if (lstDevice.Any())
                return true;
            return false;
        }

        public string CreateOrUpdate(int from, int to, string terminalNo)
        {
            //select record from tm05 with f05_terminalno as current termianlNo
            var startConveyor = _unitOfWork.ConveyorRepository.Get(i => i.F05_TerminalNo.Trim().Equals(terminalNo));

            //Insert Or Update tx48_nomanage
            var isNoManage = true;
            var sequenceNo = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNoManage,
                Constants.GetColumnInNoManager.MtrWhsCmdNo, 1, 0, 0, 0, 0);

            var destinationTerminal = terminalNo == Constants.TerminalNo.A018
                ? Constants.TerminalNo.A017
                : Constants.TerminalNo.A018;

            var destinationConveyor = _unitOfWork.ConveyorRepository.Get(x => x.F05_TerminalNo.Trim().Equals(destinationTerminal));

            //InsertTX34
            InsertTX34(Constants.F34_CommandNo.Move, sequenceNo, Constants.CommandType.CmdType_0, Constants.TX34_StrRtrType.Material.ToString("D"),
                Constants.TC_CMDSTS.TC_CMDSTS_0, null, startConveyor.F05_ConveyorCode, destinationConveyor.F05_ConveyorCode, terminalNo,
                Constants.PictureNo.TCRM091F);
            

            _unitOfWork.Commit();
            var message = "Shelf No [" + startConveyor.F05_ConveyorCode + " -> " + destinationConveyor.F05_ConveyorCode + "] moving …";
            return message;
        }

        /// <summary>
        /// Complete inter-floor movement.
        /// </summary>
        /// <param name="terminalNo"></param>
        public async Task<IList<TX34_MtrWhsCmd>> Complete(string terminalNo)
        {
            var commands = _unitOfWork.MaterialWarehouseCommandRepository.GetAll();
            commands =
                commands.Where(x => x.F34_PictureNo.Equals("TCRM091F") && x.F34_TerminalNo.Trim().Equals(terminalNo));

            foreach (var command in commands)
            {
                if (command.F34_Status.Trim().Equals("6"))
                    command.F34_Status = "C";
                else
                    command.F34_Status = "D";
            }

            await _unitOfWork.CommitAsync();

            return await commands.ToListAsync();
        }
    }
}
