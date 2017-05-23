using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.ScoreCard
{
    public class ScoreCardModel
    {
        public int Id { get; set; }
        public string MsqMeasure { get; set; }
        public int ScMeasureId { get; set; }
        public string Ytd { get; set; }
        public string Jul { get; set; }
        public string Aug { get; set; }
        public string Sep { get; set; }
        public string Oct { get; set; }
        public string Nov { get; set; }
        public string Dec { get; set; }
        public string Jan { get; set; }
        public string Feb { get; set; }
        public string Mar { get; set; }
        public string Apr { get; set; }
        public string May { get; set; }
        public string Jun { get; set; }
        public bool AvailableEdit { get; set; }
        public bool IsBold { get; set; }
    }
}