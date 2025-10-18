using aspnet_contact_form.Data;
using aspnet_contact_form.Models;
using aspnet_contact_form.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace aspnet_contact_form.Controllers;

[ApiController]
[Route("api/contacts")]
[Produces("application/json")]
public class ContactApiController(AppDbContext db) : ControllerBase
{
    [HttpPost]
    [Consumes("application/json")]
    public async Task<IActionResult> Create([FromBody] CreateContactMessageRequest req, CancellationToken ct)
    {
        if (req is null)
            return BadRequest(new { message = "Gövde (body) boş gönderildi." });

        if (!ModelState.IsValid)
            return BadRequest(new { message = "Eksik veya hatalı alanlar var.", errors = ModelState });

        var exists = await db.Departments.AnyAsync(d => d.Id == req.DepartmentId, ct);
        if (!exists)
            return NotFound(new { message = "Departman bulunamadı." });

        var entity = new ContactMessage
        {
            FullName = req.FullName,
            Phone = req.Phone,
            Email = req.Email,
            DepartmentId = req.DepartmentId,
            Message = req.Message,
            CreatedAtUtc = DateTime.UtcNow
        };

        db.ContactMessages.Add(entity);
        await db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, new
        {
            success = true,
            message = "Kayıt başarıyla oluşturuldu.",
            id = entity.Id
        });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
    {
        var item = await db.ContactMessages
            .AsNoTracking()
            .Select(x => new
            {
                x.Id,
                x.FullName,
                x.Email,
                x.Phone,
                x.Message,
                x.DepartmentId,
                x.CreatedAtUtc
            })
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return item is null
            ? NotFound(new { message = "Kayıt bulunamadı." })
            : Ok(item);
    }
}
