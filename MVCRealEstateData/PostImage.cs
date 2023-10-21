using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace MVCRealEstateData;

public class PostImage
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public required string Image { get; set; }
    public virtual Post? Post { get; set; }

}

public class PostImageEntityTypeConfiguration : IEntityTypeConfiguration<PostImage>
{
    public void Configure(EntityTypeBuilder<PostImage> builder)
    {
        builder
            .Property(p => p.Image)
            .IsUnicode(false)
            .IsRequired();
    }
}
