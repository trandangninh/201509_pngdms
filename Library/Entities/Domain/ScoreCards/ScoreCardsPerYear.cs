using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Domain.ScoreCards
{
    public class ScoreCardsPerYear
    {
        /// <summary>
        /// get or set Suppier Name
        /// </summary>
        public string SupplierName { get; set; }
        /// <summary>
        /// get or set list Score Card
        /// </summary>
        public List<ScoreCardObject> ScoreCards { get; set; }
    }
}
