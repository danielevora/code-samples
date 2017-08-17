using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bsp.Data;
using Bsp.Core.Domain.Cats;

namespace Bsp.Data.Mapping.Cats {
    public partial class CatMap : EntityTypeConfiguration<Cat> {
        public override void Map(EntityTypeBuilder<Cat> builder) {
            //builder.ToTable("Cat");
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(200);
        }
    }
}