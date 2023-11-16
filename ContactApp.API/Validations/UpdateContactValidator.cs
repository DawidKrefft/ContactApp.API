using ContactApp.API.Models.DTO;
using FluentValidation;
using System.Text.RegularExpressions;

namespace ContactApp.API.Validations
{
    public class UpdateContactValidator : AbstractValidator<UpdateContactRequestDto>
    {
        public UpdateContactValidator()
        {
            RuleFor(request => request.FirstName)
                .NotEmpty()
                .WithMessage("FirstName is required.")
                .MaximumLength(50)
                .WithMessage("FirstName must not exceed 50 characters.");

            RuleFor(request => request.LastName)
                .NotEmpty()
                .WithMessage("LastName is required.")
                .MaximumLength(50)
                .WithMessage("LastName must not exceed 50 characters.");

            RuleFor(request => request.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email format.")
                .MaximumLength(50)
                .WithMessage("Email must not exceed 50 characters.");

            RuleFor(request => request.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .Must(BeAValidPassword)
                .WithMessage(
                    "Invalid password format. It should have at least 1 small letter, 1 big letter, 1 special character, and be at least 8 characters long."
                )
                .MaximumLength(50)
                .WithMessage("Password must not exceed 50 characters.");

            RuleFor(request => request.PhoneNumber)
                .NotEmpty()
                .WithMessage("PhoneNumber is required.")
                .Must(BeValidPhoneNumber)
                .WithMessage("Invalid phone number format. It should contain only digits.")
                .MaximumLength(50)
                .WithMessage("PhoneNumber must not exceed 50 characters.");

            RuleFor(request => request.DateOfBirth)
                .NotEmpty()
                .WithMessage("DateOfBirth is required.");

            RuleFor(request => request.CategoryName)
                .NotEmpty()
                .WithMessage("CategoryName is required.")
                .MaximumLength(50)
                .WithMessage("CategoryName must not exceed 50 characters.");

            When(
                request => request.CategoryName == "work",
                () =>
                {
                    RuleFor(request => request.SubcategoryName)
                        .NotEmpty()
                        .WithMessage("For 'work' category, you must select a subcategory.")
                        .MaximumLength(50)
                        .WithMessage("SubcategoryName must not exceed 50 characters.");
                }
            );

            When(
                request => request.CategoryName == "other",
                () =>
                {
                    RuleFor(request => request.SubcategoryName)
                        .NotEmpty()
                        .WithMessage("For 'other' category, you must provide a subcategory.")
                        .MaximumLength(50)
                        .WithMessage("SubcategoryName must not exceed 50 characters.");
                }
            );
        }

        private bool BeAValidPassword(string password)
        {
            // Add your password validation logic here
            // Example: At least 1 small letter, 1 big letter, 1 special char, and at least 8 characters
            return Regex.IsMatch(
                password,
                @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$"
            );
        }

        private bool BeValidPhoneNumber(string phoneNumber)
        {
            // Add your phone number validation logic here
            // Example: Only digits are allowed
            return Regex.IsMatch(phoneNumber, @"^\d+$");
        }
    }
}
