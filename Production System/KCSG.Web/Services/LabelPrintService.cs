using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using KCSG.Core.Constants;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Models;

namespace KCSG.Web.Services
{
    public class LabelPrintService : ILabelPrintService
    {
        #region Properties
        
        /// <summary>
        /// List of external printers in the system.
        /// </summary>
        public IList<PrinterSetting> ExternalPrinters { get; set; }

        /// <summary>
        /// List of internal printers in the system.
        /// </summary>
        public IList<PrinterSetting> InternalPrinters { get; set; }

        /// <summary>
        /// List of pre-product printer.
        /// </summary>
        public IList<PrinterSetting> PreProductPrinters { get; set; }
        
        /// <summary>
        /// List of internal labels with their content.
        /// </summary>
        public Dictionary<string, string> InternalLabelOriginalContent { get; set; }
        
        /// <summary>
        /// Pre-product label original text.
        /// </summary>
        public string PreProductLabelOrignalText { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Broadcast print command to label printer base on IP Address and port.
        /// </summary>
        /// <param name="labelPrintEndPoint"></param>
        /// <param name="command"></param>
        public void Print(IPEndPoint labelPrintEndPoint, string command)
        {
            using (var tcpClient = new TcpClient())
            {
                // Connect to printer.
                tcpClient.Connect(labelPrintEndPoint);

                // Find connection stream.
                using (var networkStream = tcpClient.GetStream())
                {
                    // Initiate writer stream from network stream.
                    using (var streamWriter = new StreamWriter(networkStream))
                    {
                        // Automatically flush content to stream.
                        streamWriter.AutoFlush = true;

                        // Write data to stream.
                        streamWriter.WriteLine(command);
                    }
                }
            }
        }

        /// <summary>
        /// Find content which can be printed by using label name.
        /// </summary>
        /// <param name="labelName"></param>
        /// <returns></returns>
        public string FindInternalPrintContent(string labelName)
        {
            if (InternalLabelOriginalContent == null || !InternalLabelOriginalContent.ContainsKey(labelName))
                return string.Empty;

            return InternalLabelOriginalContent[labelName];
        }

        /// <summary>
        /// Find external print content.
        /// </summary>
        /// <param name="labelName"></param>
        /// <returns></returns>
        public string FindExternalPrintContent(string labelName)
        {
            if (ExternalLabelOriginalContent == null || !ExternalLabelOriginalContent.ContainsKey(labelName))
                return string.Empty;

            return ExternalLabelOriginalContent[labelName];
        }

        public Dictionary<string, string> ExternalLabelOriginalContent { get; set; }

        #endregion
    }
}