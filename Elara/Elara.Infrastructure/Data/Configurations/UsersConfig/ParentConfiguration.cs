using Elara.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations.UsersConfig
{
    public class ParentConfiguration : IEntityTypeConfiguration<Parent>
    {
        public void Configure(EntityTypeBuilder<Parent> builder)
        {
            builder.HasKey(p => p.Id);
            
            builder.Property(p => p.Id)
                .ValueGeneratedNever();

            builder.HasOne(p => p.User)
                .WithOne()
                .HasForeignKey<Parent>(p => p.Id)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
