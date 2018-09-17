using DotNetCore.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotNetCore.Database.Mappings
{
    public partial class ArticleFeedbackConfiguration : IEntityTypeConfiguration<ArticleFeedback>
    {
        public void Configure(EntityTypeBuilder<ArticleFeedback> builder)
        {
            builder.ToTable("ArticleFeedbacks", Constants.Database.SchemaName);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName(@"Id").HasColumnType("int").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.ArticleId).HasColumnName(@"ArticleId").HasColumnType("int").IsRequired();
            builder.Property(x => x.Comments).HasColumnName(@"Comments").HasColumnType("nvarchar(max)").IsRequired(false);
            builder.Property(x => x.Status).HasColumnName(@"Status").HasColumnType("tinyint").IsRequired();
            builder.Property(x => x.UserId).HasColumnName(@"UserId").HasColumnType("int").IsRequired();
            builder.Property(x => x.FeedbackDate).HasColumnName(@"FeedbackDate").HasColumnType("datetime").IsRequired(false);
            builder.Property(x => x.CommentDate).HasColumnName(@"CommentDate").HasColumnType("datetime").IsRequired(false);
            builder.Property(x => x.FeedbackCount).HasColumnName(@"FeedbackCount").HasColumnType("int");

            builder.HasOne(x => x.Article).WithMany(x=>x.ArticleFeedbacks).IsRequired();
        }
    }
}