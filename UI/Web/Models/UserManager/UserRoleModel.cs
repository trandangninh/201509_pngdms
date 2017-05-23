using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Validators;

namespace Web.Models.UserManager
{
    [Validator(typeof(UserRoleValidator))]
    public class UserRoleModel : BaseNoisEntityModel
    {
        /// <summary>
        /// get or set name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// get or set is system name or not
        /// </summary>
        public bool IsSystem { get; set; }
        /// <summary>
        /// get or set is active or not
        /// </summary>
        public bool IsActive { get; set; }
    }
}
