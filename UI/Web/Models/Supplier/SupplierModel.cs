using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Validators;

namespace Web.Models.Supplier
{
    [Validator(typeof(SupplierValidator))]
    public class SupplierModel : BaseNoisEntityModel
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
        /// display order
        /// </summary>
        public int DisplayOrder { get; set; }
        public string VendorContact { get; set; }
        public LocationTypeModel LocationType { get; set; }
    }

    public class LocationTypeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public LocationTypeModel()
        {
            Id = 0;
            Name = string.Empty;
        }
        public LocationTypeModel(int id)
        {
            Id = id;
            switch(id)
            {
                case 1: { Name = "Local"; break; }
                case 2: { Name = "External"; break; }
                default: { Name = string.Empty; break; }
            }
        }
    }
}
