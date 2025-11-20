using FluentValidation;
using PROG2112ClaimsPOE.Models;

namespace PROG2112ClaimsPOE.Validators
{
    public class ClaimModelValidator : AbstractValidator<ClaimModel>
    {
        public ClaimModelValidator()
        {
            RuleFor(x => x.LectureName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LectureSurname).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LecturerEmail).NotEmpty().EmailAddress();
            RuleFor(x => x.HoursWorked).GreaterThan(0).WithMessage("Hours must be > 0");
            RuleFor(x => x.HourlyRate).GreaterThan(0).WithMessage("Rate must be > 0");
            RuleFor(x => x.Payment).GreaterThanOrEqualTo(0);
        }
    }
}
