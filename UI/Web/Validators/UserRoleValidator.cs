using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Models.UserManager;

namespace Web.Validators
{
    public class UserRoleValidator : AbstractValidator<UserRoleModel>
    {
        public UserRoleValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        }
    }
}
