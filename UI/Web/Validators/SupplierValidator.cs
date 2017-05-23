using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Models.Supplier;

namespace Web.Validators
{
    public class SupplierValidator : AbstractValidator<SupplierModel>
    {
        public SupplierValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
            RuleFor(x => x.VendorCode).NotEmpty().WithMessage("Vendor code is required.");
            RuleFor(x => x.VendorPrefixCode).NotEmpty().WithMessage("Vendor prefix code is required.");
        }
    }
}
