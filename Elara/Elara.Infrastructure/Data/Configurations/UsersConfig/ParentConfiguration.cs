using Elara.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Elara.Infrastructure.Data.Configurations.UsersConfig
{
    public class ParentConfiguration : IEntityTypeConfiguration<Parent>
    {
        public void Configure(EntityTypeBuilder<Parent> builder)
        {
            builder.HasOne(p=>p.parent).WithOne().
                HasForeignKey<Parent>(p=>p.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Childrens)
              .WithOne()
              .HasForeignKey("ParentId")
              .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
