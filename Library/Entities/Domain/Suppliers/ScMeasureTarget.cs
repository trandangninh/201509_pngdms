using Entities.Domain.ScMeasures;
using RepositoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Domain.Suppliers
{
    public class ScMeasureTarget : BaseEntity
    {
        /// <summary>
        /// get or set Score card measure identity
        /// </summary>
        public int ScMeasureId { get; set; }
        /// <summary>
        /// get or set ScMeasure entity
        /// </summary>
        public virtual ScMeasure ScMeasure { get; set; }
        /// <summary>
        /// get or set supplier identity
        /// </summary>
        public int SupplierId { get; set; }
        /// <summary>
        /// get or set supplier entity
        /// </summary>
        public virtual Supplier Supplier { get; set; }
        /// <summary>
        /// get or set target
        /// </summary>
        public string Target { get; set; }
    }
}
