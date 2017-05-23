using Entities.Domain.Suppliers;
using RepositoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Domain.ScMeasures
{
    public class ScMeasure : BaseEntity
    {
        /// <summary>
        /// Get or set score card measure code
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// get or set name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// get or set note
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// get or set display order
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// Is display on score card table
        /// </summary>
        public bool IsDisplay { get; set; }
        /// <summary>
        /// Get or Set Formula
        /// </summary>
        public string Formula { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int YtdCalculatedTypeId { get; set; }
        public YtdCalculatedType YtdCalculatedType
        {
            get { return (YtdCalculatedType)YtdCalculatedTypeId; }
            set { YtdCalculatedTypeId = (int)value; }
        }
        /// <summary>
        /// Is imported data from excel file
        /// </summary>
        public bool IsImported { get; set; }

        /// <summary>
        /// Product line
        /// </summary>
        private ICollection<ScMeasureTarget> _scMeasureTargets;
        /// <summary>
        /// get or set list ScMeasure target
        /// </summary>
        public virtual ICollection<ScMeasureTarget> ScMeasureTargets
        {
            get { return _scMeasureTargets ?? (_scMeasureTargets = new List<ScMeasureTarget>()); }
            protected set { _scMeasureTargets = value; }
        }
        /// <summary>
        /// Get or set display style
        /// </summary>
        public bool IsBold { get; set; }
    }
    public enum YtdCalculatedType
    {
        LatestData = 10,
        Sum = 20,
        Average = 30
    }
}
