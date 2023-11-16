using ContactApp.API.Models.DTO;
using FluentValidation;

namespace ContactApp.API.Validations
{
    public class UpdateSubcategoryValidator : AbstractValidator<UpdateSubcategoryRequestDto>
    {
        public UpdateSubcategoryValidator()
        {
            RuleFor(subcategory => subcategory.Name)
                .NotEmpty()
                .WithMessage("Subcategory name is required.")
                .MaximumLength(50)
                .WithMessage("Subcategory name cannot exceed 50 characters.")
                .Matches("^[a-z0-9-]+$")
                .WithMessage(
                    "Subcategory name can only contain lowercase letters, numbers, and hyphens."
                )
                .Must(name => !name.Contains(" "))
                .WithMessage("Subcategory name cannot contain spaces.");

            RuleFor(subcategory => subcategory.CategoryId)
                .NotNull()
                .NotEmpty()
                .WithMessage("Category ID is required.")
                .Must(id => id.ToString().Length <= 10)
                .WithMessage("Category ID cannot exceed 10 digits.");
        }
    }
}
