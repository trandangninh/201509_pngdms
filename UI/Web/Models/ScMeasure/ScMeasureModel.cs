using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Validators;

namespace Web.Models.ScMeasure
{
    [Validator(typeof(ScMeasureValidator))]
    public class ScMeasureModel : BaseNoisEntityModel
    {
        /// <summary>
        /// Get or set score card measure
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// get or set Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// get or set Note
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// display order
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// Is display this measure on score card page
        /// </summary>
        public bool IsDisplay { get; set; }
        /// <summary>
        /// Is display this measure on score card page
        /// </summary>
        public bool IsImported { get; set; }
    }
}
