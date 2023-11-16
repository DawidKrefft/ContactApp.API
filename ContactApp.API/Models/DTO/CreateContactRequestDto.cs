namespace ContactApp.API.Models.DTO
{
    public class CreateContactRequestDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string CategoryName { get; set; }
        public string? SubcategoryName { get; set; }
    }
}
