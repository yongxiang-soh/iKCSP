﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.Inquiry
{
    public interface IInquiryKneadingLineNoDomain
    {
        /// <summary>
        /// Load kneading commands from database.
        /// </summary>
        /// <param name="gridSettings"></param>
        /// <param name="kneadingCommandline"></param>
        /// <returns></returns>
        Task<ResponseResult<GridResponse<object>>> LoadKneadingCommands(GridSettings gridSettings,
            Constants.KndLine kneadingCommandline);

        /// <summary>
        /// Start kneading commands.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <param name="kneadingCommandResults"></param>
        /// <param name="kneadingLine"></param>
        //Task StartKneadingCommand(string terminalNo,
        //    IList<PdtPlnItem> kneadingCommandResults, Constants.KndLine kneadingLine);

        /// <summary>
        /// Interrupt kneading command by using terminal number and list of kneading command results.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <param name="kneadingCommandResults"></param>
        //Task InterruptKneadingCommand(string terminalNo,
        //    IEnumerable<PdtPlnItem> kneadingCommandResults);

        /// <summary>
        /// Stop kneading command.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <param name="kneadingCommandResult"></param>
        //Task StopKneadingCommand(string terminalNo, PdtPlnItem kneadingCommandResult);
    }
}