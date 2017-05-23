using FluentValidation;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Models.Common;

namespace Web.Models.UserManager
{
    [Validator(typeof(User_ModelValidator))]
    public class User_Model : BaseNoisEntityModel
    {
        public int UserId { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string PhoneNumber { get; set; }
        public bool Active { get; set; }
        //public UserRoleViewModel Role { get; set; }
        public List<UserRoleViewModel> Roles { get; set; }
        public string ResetPassword { get; set; }
        public List<DepartmentViewModel> Departments { get; set; }
        public User_Model()
        {
            Roles = new List<UserRoleViewModel>();
        }
    }

    public class User_ModelValidator : AbstractValidator<User_Model>
    {
        public User_ModelValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password id required");
            RuleFor(x => x.Password).Length(6, int.MaxValue).WithMessage("Password least 6 characters");
            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Confirm Password id required");
            RuleFor(x => x.ConfirmPassword).Length(6, 16).WithMessage("Confirm Password least 6 characters");
            RuleFor(x => x.Password).Equal(u => u.ConfirmPassword).WithMessage("Password and ConfirmPassword not match");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid Email");
        }
    }

    public class UserRoleViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public UserRoleViewModel()
        {
            Id = 0;
            Name = "";
        }
    }
}