using System.ComponentModel.DataAnnotations;

namespace InstituteManagement.Shared.DTOs.Persons;

public class PersonDto
{
    public Guid Id { get; set; }
    public string NationalCode { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public bool IsVerified { get; set; }
}
