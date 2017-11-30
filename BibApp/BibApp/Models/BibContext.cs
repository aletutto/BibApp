using BibApp.Models;
using BibApp.Models.Warenkorb;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

public class BibContext : IdentityDbContext<Benutzer>
{
    public BibContext(DbContextOptions<BibContext> options) : base(options)
    {}

    // Mehrere Models möglich!
    public DbSet<Benutzer> Benutzers { get; set; }
    public DbSet<Buch> Buecher { get; set; }
    public DbSet<Korb> Warenkoerbe { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Benutzer>(entity =>
            {
                entity.ToTable(name: "Benutzers");
            });

        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }

}
