using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Domain.ScoreCards
{
    public class ScoreCardObject
    {
        public ScoreCardObject()
        {
            Data = new List<float?>();
            for (var i = 0; i < 12; i++)
                Data.Add(null);
        }

        public string MsqMeasure { get; set; }
        public int ScMeasureId { get; set; }
        public string Ytd { get; set; }
        public List<float?> Data { get; set; }
        public bool AvailableEdit { get; set; }
        public bool IsBold { get; set; }
    }
}
