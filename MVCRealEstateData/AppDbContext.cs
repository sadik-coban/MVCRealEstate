using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace MVCRealEstateData;

public class AppDbContext : IdentityDbContext<User, Role, Guid>
{
    public AppDbContext(DbContextOptions options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }

    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<District> Districts { get; set; }
    public virtual DbSet<Post> Posts { get; set; }
    public virtual DbSet<PostImage> PostImages { get; set; }
    public virtual DbSet<Province> Provinces { get; set; }
    public virtual DbSet<Specification> Specifications { get; set; }

}
