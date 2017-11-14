using BibApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

public class BibContext : DbContext
{
    public BibContext(DbContextOptions<BibContext> options) : base(options)
    {}

    // Mehrere Models möglich!
    public DbSet<Benutzer> Benutzers { get; set; }
    public DbSet<Buch> Buecher { get; set; }

}
