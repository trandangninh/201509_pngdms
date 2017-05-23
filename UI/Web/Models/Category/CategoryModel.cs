using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Validators;

namespace Web.Models.Category
{
    [Validator(typeof(CategoryValidator))]
    public class CategoryModel : BaseNoisEntityModel
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
        /// display order
        /// </summary>
        public int DisplayOrder { get; set; }
    }
}
