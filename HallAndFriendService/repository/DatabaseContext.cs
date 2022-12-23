using WebApplication2.models;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.repository;
class DatabaseContext : DbContext
{
    public DbSet<Attempt> Attempts => Set<Attempt>();

    public DatabaseContext() => Database.EnsureCreated(); 
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Username=postgres;Password=qwerty;Database=princess");
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {

        base.OnModelCreating(builder);
    }
}