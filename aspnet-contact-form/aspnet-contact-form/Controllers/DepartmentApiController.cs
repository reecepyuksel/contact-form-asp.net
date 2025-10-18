using aspnet_contact_form.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace aspnet_contact_form.Controllers;

[ApiController]
[Route("api/departments")]
public class DepartmentApiController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetDepartments()
    {
        var departments = await db.Departments
            .OrderBy(d => d.Name)
            .Select(d => new
            {
                d.Id,
                d.Name
            })
            .ToListAsync();

        if (departments.Count == 0)
            return NotFound(new { message = "Departman bulunamadı." });

        return Ok(departments);
    }
}
