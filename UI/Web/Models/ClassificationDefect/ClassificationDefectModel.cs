using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Models;
using Web.Models.ClassificationDefect;
using Web.Models.Common;
using Web.Validators;

namespace Web.ClassificationDefects
{
    [Validator(typeof(ClassificationDefectValidator))]
    public class ClassificationDefectModel : BaseNoisEntityModel
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
        /// <summary>
        /// get or set list materials
        /// </summary>
        public List<MaterialModel> Materials { get; set; }
        /// <summary>
        /// get or set list suppliers
        /// </summary>
        public List<SelectionModel> Suppliers { get; set; }
        /// <summary>
        /// constructor
        /// </summary>
        public ClassificationDefectModel()
        {
            Materials = new List<MaterialModel>();
            Suppliers = new List<SelectionModel>();
        }
    }
}
