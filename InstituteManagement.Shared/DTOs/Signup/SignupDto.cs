namespace InstituteManagement.Shared.DTOs.Signup
{
    public class SignupDto
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public NationalityCode NationalityCode { get; set; }
        public string NationalId { get; set; } = default!;
        public DateOnly Birthday { get; set; } = default!;
        public string Email { get; set; } = default!;
    }
}
