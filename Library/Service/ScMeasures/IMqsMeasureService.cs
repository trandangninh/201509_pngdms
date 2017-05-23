using Entities.Domain.ScMeasures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Service.ScMeasures
{
    public interface IMqsMeasureService : IBaseService<MqsMeasure>
    {
        void ImportMqsMeasure(MemoryStream stream, out string message);
        void ImportScoreCardMeasure(MemoryStream stream, out string message);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="supplierId"></param>
        /// <param name="scMeasureId"></param>
        /// <returns></returns>
        Task<List<MqsMeasure>> GetMqsMeasures(DateTime date, int supplierId, int scMeasureId = 0);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="supplierId"></param>
        /// <param name="scMeasureId"></param>
        /// <returns></returns>
        Task<List<MqsMeasure>> GetMqsMeasures(DateTime fromDate, DateTime toDate, int supplierId = 0, int scMeasureId = 0);
        void ExportMqsTemplate(Stream stream, string path);

        /// <summary>
        /// get scorecard by suppplierId, scMeasureId and time
        /// </summary>
        /// <param name="supplierId"></param>
        /// <param name="scMeasureId"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        Task<MqsMeasure> GetMqsMeasureBySupplierIdAndScMeasureIdAndTime(int supplierId, int scMeasureId, DateTime time);
    }
}
