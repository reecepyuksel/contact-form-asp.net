using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspnet_contact_form.Models;

[Table("Departments")]
public class Department
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public ICollection<ContactMessage> ContactMessages { get; set; } = new List<ContactMessage>();
}