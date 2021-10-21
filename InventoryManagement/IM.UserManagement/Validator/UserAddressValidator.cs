using FluentValidation;
using IM.Common.Model.EntityModels;

namespace IM.UserManagement.Validator
{
    public class UserAddressValidator : AbstractValidator<UserAddress>
    {

        public UserAddressValidator()
        {
            RuleFor(a => a.Address)
                   .NotNull()
                   .NotEmpty()
                   .WithErrorCode(IMStatusCodes.REQUIRED_FIELD_CODE)
                   .WithMessage("Invalid {PropertyName}");

            RuleFor(a => a.City)
                   .NotNull()
                   .NotEmpty()
                   .WithMessage("City is Required")
                   .WithErrorCode(IMStatusCodes.REQUIRED_FIELD_CODE)
                   .MaximumLength(50)
                   .WithMessage("Invalid {PropertyName}")
                   .WithErrorCode(IMStatusCodes.LENGTH_EXCEEDED_CODE);

            RuleFor(a => a.State)
                   .NotNull()
                   .NotEmpty()
                   .WithMessage("Address is Required")
                   .WithErrorCode(IMStatusCodes.REQUIRED_FIELD_CODE)
                   .MaximumLength(50)
                   .WithMessage("Invalid {PropertyName}")
                   .WithErrorCode(IMStatusCodes.LENGTH_EXCEEDED_CODE);

            RuleFor(a => a.ZipCode)
                   .NotNull()
                   .NotEmpty()
                   .WithMessage("Address is Required")
                   .WithErrorCode(IMStatusCodes.REQUIRED_FIELD_CODE)
                   .Matches("^(([0-9]{5})*-([0-9]{4}))|([0-9]{5})$")
                   .WithErrorCode(IMStatusCodes.LENGTH_EXCEEDED_CODE);
        }
    }
}
