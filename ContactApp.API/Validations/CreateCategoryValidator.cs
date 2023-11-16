using ContactApp.API.Models.DTO;
using FluentValidation;

namespace ContactApp.API.Validations
{
    public class CreateCategoryValidator : AbstractValidator<CreateCategoryRequestDto>
    {
        public CreateCategoryValidator()
        {
            RuleFor(category => category.Name)
                .NotEmpty()
                .WithMessage("Category name is required.")
                .MaximumLength(50)
                .WithMessage("Category name cannot exceed 50 characters.")
                .Matches("^[a-z0-9-]+$")
                .WithMessage(
                    "Category name can only contain lowercase letters, numbers, and hyphens."
                )
                .Must(name => !name.Contains(" "))
                .WithMessage("Category name cannot contain spaces.");
        }
    }
}
