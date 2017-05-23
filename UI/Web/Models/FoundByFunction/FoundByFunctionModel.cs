using FluentValidation;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.FoundByFunction
{    
    [Validator(typeof(FoundByFunctionValidator))]
    public class FoundByFunctionModel : BaseNoisEntityModel
    {
        public string Name { get; set; }
    }

    public class FoundByFunctionValidator : AbstractValidator<FoundByFunctionModel>
    {
        public FoundByFunctionValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
        }
    }
}