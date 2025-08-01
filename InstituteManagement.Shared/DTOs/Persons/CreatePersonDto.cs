using System.ComponentModel.DataAnnotations;

namespace InstituteManagement.Shared.DTOs.Persons;

public class CreatePersonDto
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string NationalCode { get; set; } = default!;
    public bool IsVerified { get; set; }
}
