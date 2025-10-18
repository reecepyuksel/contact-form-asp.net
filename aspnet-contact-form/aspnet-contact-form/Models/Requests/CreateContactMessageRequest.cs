using System.ComponentModel.DataAnnotations;

namespace aspnet_contact_form.Models.Requests;

public class CreateContactMessageRequest
{
    [Required, MaxLength(120)]
    public string FullName { get; set; } = string.Empty;

    [Required, Phone, MaxLength(30)]
    public string Phone { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(180)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public int DepartmentId { get; set; }

    [Required, MaxLength(2000)]
    public string Message { get; set; } = string.Empty;
}
