using ContactApp.API.Models.DTO;
using FluentValidation;

namespace ContactApp.API.Validations
{
    public class CreateSubcategoryValidator : AbstractValidator<CreateSubcategoryRequestDto>
    {
        public CreateSubcategoryValidator()
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
                .NotEmpty()
                .WithMessage("Category ID is required.")
                .Must(id => id.ToString().Length <= 20)
                .WithMessage("Category ID cannot exceed 20 digits.");
        }
    }
}
