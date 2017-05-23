using Entities.Domain.ClassificationDefects;
using RepositoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Domain.Suppliers
{
    public class Supplier : BaseEntity
    {
        /// <summary>
        /// get or set Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// get or set Note
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// get or set vendorCode
        /// </summary>
        public string VendorCode { get; set; }
        /// <summary>
        /// get or set vendor prefix code
        /// </summary>
        public string VendorPrefixCode { get; set; }
        /// <summary>
        /// get or set vendor contact
        /// </summary>
        public string VendorContact { get; set; }
        /// <summary>
        /// get or set location type identity
        /// </summary>
        public int LocationTypeId { get; set; }
        /// <summary>
        /// get or set lacation type
        /// </summary>       
        public LocationType LocationType
        {
            get { return (LocationType)LocationTypeId; }
            set { LocationTypeId = (int)value; }
        }
        /// <summary>
        /// display order
        /// </summary>
        public int DisplayOrder { get; set; }

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

        private ICollection<ClassificationDefect> _classificationDefects;
        public virtual ICollection<ClassificationDefect> ClassificationDefects
        {
            get { return _classificationDefects ?? (_classificationDefects = new List<ClassificationDefect>()); }
            protected set { _classificationDefects = value; }
        }
    }

    public enum LocationType
    {
        Local = 1,
        External = 2,
    }
}
