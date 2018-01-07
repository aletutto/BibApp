using BibApp.Models.Benutzer;
using BibApp.Models.Buch;
using BibApp.Models.Warenkorb;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class BibContext : IdentityDbContext<Benutzer>
{
    public BibContext(DbContextOptions<BibContext> options) : base(options) {}

    public DbSet<Benutzer> Benutzer { get; set; }
    public DbSet<Buch> Buch { get; set; }
    public DbSet<Warenkorb> Warenkorb { get; set; }
    public DbSet<Leihauftrag> Leihauftrag { get; set; }
    public DbSet<Exemplar> Exemplar { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Benutzer>(entity =>
        {
            entity.ToTable(name: "Benutzer");
        });

        builder.Ignore<IdentityUserToken<string>>();
        builder.Ignore<IdentityUserLogin<string>>();
    }
}
