using Elara.Domain.Entities.Chat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations.ChatConfig
{
    public class ChatAnalysisReportConfiguration : IEntityTypeConfiguration<ChatAnalysisReport>
    {
        public void Configure(EntityTypeBuilder<ChatAnalysisReport> builder)
        {
            builder.ToTable("ChatAnalysisReports");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.PublicId)
                .IsRequired()
                .HasDefaultValueSql("gen_random_uuid()");

            builder.HasIndex(r => r.PublicId)
                .IsUnique()
                .HasDatabaseName("IX_ChatAnalysisReports_PublicId");

            builder.Property(r => r.Title).HasMaxLength(200).IsRequired();
            builder.Property(r => r.ReportText).IsRequired();

            builder.HasIndex(r => r.ConversationId)
                .IsUnique()
                .HasDatabaseName("IX_ChatAnalysisReports_ConversationId");

            builder.HasIndex(r => r.StudentId)
                .HasDatabaseName("IX_ChatAnalysisReports_StudentId");

            builder.HasOne(r => r.Conversation)
                .WithOne()
                .HasForeignKey<ChatAnalysisReport>(r => r.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Student)
                .WithMany()
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
