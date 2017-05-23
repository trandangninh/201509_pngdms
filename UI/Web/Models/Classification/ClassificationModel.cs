using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using FluentValidation.Attributes;
using Web.Models.QualityAlert;
using Web.Models.FoundByFunction;

namespace Web.Models.Classification
{
    [Validator(typeof(ClassificationValidator))]
    public class ClassificationModel: BaseNoisEntityModel
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Severity { get; set; }

        public int Dectability { get; set; }
        public FoundByFunctionModel FoundByFunction { get; set; }
    }

    public class ClassificationValidator : AbstractValidator<ClassificationModel>
    {
        public ClassificationValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Severity).NotEmpty().WithMessage("Severity is required");
            RuleFor(x => x.Dectability).NotEmpty().WithMessage("Dectability is required");
        }
    }
}