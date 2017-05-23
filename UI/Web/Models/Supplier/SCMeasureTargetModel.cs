using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Validators;

namespace Web.Models.Supplier
{
    public class ScMeasureTargetModel
    {
        /// <summary>
        /// get or set identity
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// get or set ScMeasure name
        /// </summary>
        public string ScMeasureName { get; set; }
        /// <summary>
        /// get or set scmeasure identity
        /// </summary>
        public int ScMeasureId { get; set; }
        /// <summary>
        /// get or set Target
        /// </summary>
        public string Target { get; set; }      
    }
}
