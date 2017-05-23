using Entities.Domain.ScoreCards;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ScoreCards
{
    public interface IScoreCardService
    {
        List<ScoreCardObject> GetScoreCardData(int year, int supplierId, DateTime? fromDate = null, DateTime? toDate = null);
        void ExportScoreCardTemplate(Stream stream, int year, string path);

        /// <summary>
        /// Export multi Score Card template
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="year"></param>
        /// <param name="listScoreCardPerYear"></param>
        /// <param name="listMonthId"></param>
        /// <param name="path"></param>
        void ExportMultiScoreCardTemplate(MemoryStream stream, int year, List<ScoreCardsPerYear> listScoreCardPerYear, List<int> listMonthId, string path);     
    }
}
