using FluentValidation;
using IM.Common.Model.EntityModels;
using System;

namespace IM.UserManagement.Validator
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserModel>
    {
        public RegisterUserValidator()
        {
            RuleFor(a => a.FirstName)
                   .NotNull()
                   .NotEmpty()
                   .WithMessage("FirstName is Required")
                   .WithErrorCode(IMStatusCodes.REQUIRED_FIELD_CODE)
                   .MaximumLength(50)
                   .WithMessage("Invalid {PropertyName}")
                   .WithErrorCode(IMStatusCodes.LENGTH_EXCEEDED_CODE);

            RuleFor(a => a.LastName)
                   .NotNull()
                   .NotEmpty()
                   .WithMessage("LastName is Required")
                   .WithErrorCode(IMStatusCodes.REQUIRED_FIELD_CODE)
                   .MaximumLength(50)
                   .WithMessage("Invalid {PropertyName}")
                   .WithErrorCode(IMStatusCodes.LENGTH_EXCEEDED_CODE);


            RuleFor(a => a.Email)
                   .NotNull()
                   .NotEmpty()
                   .WithMessage($"FirstName is Required - ERROR CODE :- {IMStatusCodes.REQUIRED_FIELD_CODE}")
                   .WithErrorCode(IMStatusCodes.REQUIRED_FIELD_CODE)
                   .EmailAddress()
                   .WithMessage($"InValid Email - ERROR CODE :- {IMStatusCodes.INVALID_EMAIL_CODE}")
                   .WithErrorCode(IMStatusCodes.INVALID_EMAIL_CODE)
                   .MaximumLength(50)
                   .WithMessage("Invalid {PropertyName}")
                   .WithErrorCode(IMStatusCodes.LENGTH_EXCEEDED_CODE);

            RuleFor(a => a.PhoneNumber)
                   .NotNull()
                   .NotEmpty()
                   .WithMessage("PhoneNumber is Required")
                   .WithErrorCode(IMStatusCodes.REQUIRED_FIELD_CODE)
                   .MaximumLength(50)
                   .WithMessage("Invalid {PropertyName}")
                   .WithErrorCode(IMStatusCodes.LENGTH_EXCEEDED_CODE);

            RuleFor(p => p.DateofBirth).Must(BeAValidAge).WithMessage("Invalid {PropertyName}");

            RuleFor(a => a.UserAddress).SetValidator(new UserAddressValidator());
        }

        protected bool BeAValidAge(DateTime date)
        {
            int currentYear = DateTime.Now.Year;
            int dobYear = date.Year;

            if (dobYear <= currentYear && dobYear > (currentYear - 120))
            {
                return true;
            }

            return false;
        }
    }
}
