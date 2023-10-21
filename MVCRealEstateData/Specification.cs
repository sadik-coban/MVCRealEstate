using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace MVCRealEstateData;

public class Specification
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new HashSet<Post>();

}

public class SpecificationEntityTypeConfiguration : IEntityTypeConfiguration<Specification>
{
    public void Configure(EntityTypeBuilder<Specification> builder)
    {
        builder
            .HasIndex(p => new { p.Name })
            .IsUnique();

        builder
            .Property(p => p.Name)
            .HasMaxLength(400)
            .IsRequired();
    }
}