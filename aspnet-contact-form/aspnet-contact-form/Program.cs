using System;
using System.Linq;
using aspnet_contact_form.Data;
using aspnet_contact_form.Models;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

string dbServer = Environment.GetEnvironmentVariable("DB_SERVER") ?? @"(localdb)\MSSQLLocalDB";
string dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "ContactFormDb";
string dbUser = Environment.GetEnvironmentVariable("DB_USER");
string dbPass = Environment.GetEnvironmentVariable("DB_PASS");

string connectionString;

if (!string.IsNullOrWhiteSpace(dbUser) && !string.IsNullOrWhiteSpace(dbPass))
{
    connectionString = $"Server={dbServer};Database={dbName};User Id={dbUser};Password={dbPass};TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=True";
}
else
{
    connectionString = $"Server={dbServer};Database={dbName};Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=True";
}

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(connectionString, sql =>
    {
        sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
        sql.CommandTimeout(30);
    })
);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    try
    {
        db.Database.Migrate();

        if (!db.Departments.Any())
        {
            db.Departments.AddRange(
                new Department { Name = "Muhasebe" },
                new Department { Name = "Teknik Destek" },
                new Department { Name = "Ýnsan Kaynaklarý" }
            );
            db.SaveChanges();
            Console.WriteLine("[Seed] Varsayýlan departmanlar eklendi.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("[DB ERROR] Migration/Seed sýrasýnda hata: " + ex.Message);
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapGet("/ping", () => "pong");
app.MapGet("/api/departments/ping", () => "departments-ok");

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
