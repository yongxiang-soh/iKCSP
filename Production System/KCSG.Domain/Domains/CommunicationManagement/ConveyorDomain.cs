using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.CommunicationManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.CommunicationManagement
{
    public class ConveyorDomain : BaseDomain, IConveyorDomain
    {
        public ConveyorDomain(IUnitOfWork iUnitOfWork, IConfigurationService configurationService) : base(iUnitOfWork,
            configurationService)
        {
        }


        public ResponseResult<GridResponse<ConveyorItem>> Search(int communication, GridSettings gridSettings)
        {
            var result = _unitOfWork.ConveyorRepository.GetAll();

            switch (communication)
            {
                case 1:
                    result = result.Where(o => o.F05_TerminalNo.Trim() == Constants.TerminalNo.A001
                                               || o.F05_TerminalNo.Trim() == Constants.TerminalNo.A016
                                               || o.F05_TerminalNo.Trim() == Constants.TerminalNo.A017
                                               || o.F05_TerminalNo.Trim() == Constants.TerminalNo.A018);
                    break;
                case 2:
                    result = result.Where(o => o.F05_TerminalNo.Trim() == Constants.TerminalNo.A002
                                               || o.F05_TerminalNo.Trim() == Constants.TerminalNo.A005
                    );
                    break;
                case 3:

                    result = result.Where(o => o.F05_TerminalNo.Trim() == Constants.TerminalNo.A019
                                               || o.F05_TerminalNo.Trim() == Constants.TerminalNo.A003
                                               || o.F05_TerminalNo.Trim() == Constants.TerminalNo.A004);

                    break;
            }

            var itemCount = result.Count();
            OrderByAndPaging(ref result, gridSettings);
            var lstResult = AutoMapper.Mapper.Map<IEnumerable<TM05_Conveyor>, IEnumerable<ConveyorItem>>(result);
            var resultModel = new GridResponse<ConveyorItem>(lstResult, itemCount);
            return new ResponseResult<GridResponse<ConveyorItem>>(resultModel, true);
        }

        public TM05_Conveyor GetConveyor(string code)
        {
            return _unitOfWork.ConveyorRepository.GetById(code.Trim());
        }

        public void UpdateConveyor(TM05_Conveyor model)
        {
            _unitOfWork.ConveyorRepository.Update(model);
            _unitOfWork.Commit();
        }
    }
}