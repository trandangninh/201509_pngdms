using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using Web.Models.QualityAlert;

namespace Web.Validators
{
    public class QualityAlertValidator : AbstractValidator<QualityAlertModel>
    {
        public QualityAlertValidator()
        {
            RuleFor(x => x.AlertDateTime).NotEmpty().WithMessage("Alert Date is required");
            RuleFor(x => x.Detail).NotEmpty().WithMessage("Description is required");
            //RuleFor(x => x.GCAS).NotEmpty().WithMessage("Gcas is required");
            //RuleFor(x => x.SAPLot).NotEmpty().WithMessage("Sap Lot is required");
            //RuleFor(x => x.SupplierLot).NotEmpty().WithMessage("Suplier Lot is required");
            //RuleFor(x => x.DefectedQty).NotNull().WithMessage("Defected is required");
            RuleFor(x => x.DefectedQty).GreaterThanOrEqualTo(0).WithMessage("Defected Qty must greater than 0");
        }
    }
}