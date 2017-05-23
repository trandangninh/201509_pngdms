using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Models.ScMeasure;

namespace Web.Validators
{
    class ScMeasureValidator : AbstractValidator<ScMeasureModel>
    {
        public ScMeasureValidator()
        {
            RuleFor(x => x.Code).NotEmpty().WithMessage("Code is required.");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        }
    }
}
