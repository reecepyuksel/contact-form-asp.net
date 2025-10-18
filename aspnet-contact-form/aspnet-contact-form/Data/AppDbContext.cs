using aspnet_contact_form.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace aspnet_contact_form.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
    public DbSet<Department> Departments => Set<Department>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ContactMessage>(ContactMessageConfig);
        modelBuilder.Entity<Department>(DepartmentConfig);
    }

    private void ContactMessageConfig(EntityTypeBuilder<ContactMessage> e)
    {
        e.Property(x => x.FullName).HasMaxLength(120).IsRequired();
        e.Property(x => x.Phone).HasMaxLength(30).IsRequired();
        e.Property(x => x.Email).HasMaxLength(180).IsRequired();
        e.Property(x => x.Message).HasMaxLength(2000).IsRequired();
        e.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");

        e.HasOne(x => x.Department)
         .WithMany(d => d.ContactMessages)
         .HasForeignKey(x => x.DepartmentId)
         .OnDelete(DeleteBehavior.Restrict);

        e.HasIndex(x => x.DepartmentId);
    }

    private void DepartmentConfig(EntityTypeBuilder<Department> e)
    {
        e.Property(x => x.Name).HasMaxLength(100).IsRequired();
        e.HasIndex(x => x.Name).IsUnique();
    }
}
