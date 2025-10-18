using System.Net;
using aspnet_contact_form.Data;
using aspnet_contact_form.Models;
using DotNetEnv;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var envPath = Path.Combine(builder.Environment.ContentRootPath, ".env");
try
{
    if (File.Exists(envPath))
    {
        Env.Load(envPath);
        Console.WriteLine($"[ENV] .env yüklendi: {envPath}");
    }
    else
    {
        Console.WriteLine($"[ENV] .env bulunamadý: {envPath} (ENV deðiþkenleri yalnýzca process'ten okunacak)");
    }
}
catch (Exception ex)
{
    Console.WriteLine("[ENV] .env yüklenirken hata: " + ex.Message);
}

string dbServer = Environment.GetEnvironmentVariable("DB_SERVER") ?? @"(localdb)\MSSQLLocalDB";
string dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "ContactFormDb";
string dbUser = Environment.GetEnvironmentVariable("DB_USER");
string dbPass = Environment.GetEnvironmentVariable("DB_PASS");

string Mask(string? s) => string.IsNullOrEmpty(s) ? "(empty)" : new string('*', Math.Max(3, s.Length / 2));
Console.WriteLine($"[ENV] DB_SERVER={dbServer}");
Console.WriteLine($"[ENV] DB_NAME  ={dbName}");
Console.WriteLine($"[ENV] DB_USER  ={(string.IsNullOrWhiteSpace(dbUser) ? "(Trusted_Connection)" : dbUser)}");
Console.WriteLine($"[ENV] DB_PASS  ={(string.IsNullOrWhiteSpace(dbUser) ? "(Trusted_Connection)" : Mask(dbPass))}");

try
{
    var hostPart = dbServer.Contains(',') ? dbServer.Split(',')[0] : dbServer;
    if (!hostPart.Contains('\\'))
    {
        var addrs = Dns.GetHostAddresses(hostPart);
        Console.WriteLine($"[DNS] {hostPart} -> {string.Join(", ", addrs.Select(a => a.ToString()))}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"[DNS] {dbServer} çözümlenemedi: {ex.Message}");
}

string connectionString;
if (!string.IsNullOrWhiteSpace(dbUser) && !string.IsNullOrWhiteSpace(dbPass))
{
    connectionString =
        $"Server={dbServer};Database={dbName};User Id={dbUser};Password={dbPass};" +
        $"Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;Connection Timeout=15";
}
else
{
    connectionString =
        $"Server={dbServer};Database={dbName};Trusted_Connection=True;" +
        $"Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;Connection Timeout=15";
}

try
{
    var csb = new SqlConnectionStringBuilder(connectionString);
    if (csb.ContainsKey("Password")) csb.Password = "****";
    Console.WriteLine($"[DB] ConnectionString={csb.ConnectionString}");
}
catch
{
    Console.WriteLine("[DB] ConnectionString (maskeli) yazýlamadý.");
}

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(connectionString, sql =>
    {
        sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
        sql.CommandTimeout(30);
    });

});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    try
    {
        Console.WriteLine("[DB] Migrate() baþlýyor...");
        db.Database.Migrate();
        Console.WriteLine("[DB] Migrate() OK");

        using var con = new SqlConnection(connectionString);
        await con.OpenAsync();
        using var cmd = new SqlCommand("SELECT 1", con);
        var r = await cmd.ExecuteScalarAsync();
        Console.WriteLine("[DB] Test SELECT 1 -> " + r);

        if (!db.Departments.Any())
        {
            db.Departments.AddRange(
                new Department { Name = "Muhasebe" },
                new Department { Name = "Teknik Destek" },
                new Department { Name = "Ýnsan Kaynaklarý" }
            );
            await db.SaveChangesAsync();
            Console.WriteLine("[Seed] Varsayýlan departmanlar eklendi.");
        }
        else
        {
            Console.WriteLine("[Seed] Departments zaten var, seed atlanýyor.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("[DB ERROR] Migration/Seed sýrasýnda hata: " + ex);
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

app.MapGet("/health/db", async (IServiceProvider sp) =>
{
    try
    {
        await using var scope = sp.CreateAsyncScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var canConnect = await ctx.Database.CanConnectAsync();
        if (!canConnect) return Results.Problem("Cannot connect.");
        return Results.Ok("DB OK");
    }
    catch (Exception ex)
    {
        return Results.Problem("DB ERROR: " + ex.Message);
    }
});

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
