using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.ClassificationDefects;

namespace Web.Validators
{
    class ClassificationDefectValidator : AbstractValidator<ClassificationDefectModel>
    {
        public ClassificationDefectValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        }
    }
}
