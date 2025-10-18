using aspnet_contact_form.Data;
using aspnet_contact_form.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace aspnet_contact_form.Controllers;

public class ContactController(AppDbContext db) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await FillDepartments();
        return View(new ContactMessage());
    }

    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> Create(ContactMessage model)
    {
        if (!await db.Departments.AnyAsync(d => d.Id == model.DepartmentId))
            ModelState.AddModelError(nameof(model.DepartmentId), "Geçersiz departman.");

        if (!ModelState.IsValid)
        {
            await FillDepartments();
            return View(model);
        }

        model.CreatedAtUtc = DateTime.UtcNow;
        db.ContactMessages.Add(model);
        await db.SaveChangesAsync();

        TempData["ok"] = true;
        return RedirectToAction(nameof(Create));
    }
    public async Task<IActionResult> Index()
    {
        var list = await db.ContactMessages.Include(x => x.Department)
                                           .OrderByDescending(x => x.Id)
                                           .ToListAsync();
        return View(list);
    }

    private async Task FillDepartments()
    {
        ViewBag.Departments = await db.Departments
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
            .ToListAsync();
    }
}
