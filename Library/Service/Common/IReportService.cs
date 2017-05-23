using System.Collections.Generic;
using System.IO;
using Entities.Domain;
using Entities.Domain.QualityAlerts;

namespace Service.Common
{
    public  interface IReportService
    {
        /// <summary>
        /// export making report to xlsx
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="report"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        void ExportMakingToXlsx(Stream stream, List<MeetingReport> report, string path);

        string checkColor(string target, string number);

        string checkColorPacking(string target, string number);


        void ExportQualityAlertToXlsx(Stream stream, List<QualityAlertFullObject> report, string path);

        void ExportDdsMeetingToXlsx(Stream stream, List<MeetingReport> listMeetingReport, string filePath);

        /// <summary>
        /// export complaint letter
        /// </summary>
        /// <param name="streamSource"></param>
        /// <param name="streamDestination"></param>
        /// <param name="listQualityAlert"></param>
        /// <param name="path"></param>
        void ExportComplaintLetterToXlsx(Stream streamSource, Stream streamDestination, List<QualityAlertFullObject> listQualityAlert, string path);
    }
}
