using DotNetCore.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotNetCore.Database.Mappings
{
    public partial class ArticleConfiguration : IEntityTypeConfiguration<Article>
    {
        public void Configure(EntityTypeBuilder<Article> builder)
        {
            builder.ToTable("Articles", Constants.Database.SchemaName);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName(@"Id").HasColumnType("int").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.Title).HasColumnName(@"Title").HasColumnType("nvarchar(255)").IsRequired().HasMaxLength(255);
            builder.Property(x => x.Body).HasColumnName(@"Body").HasColumnType("ntext").IsRequired().HasMaxLength(int.MaxValue);
            builder.Property(x => x.Author).HasColumnName(@"Author").HasColumnType("nvarchar(255)").IsRequired().HasMaxLength(255);
            builder.Property(x => x.PublishDate).HasColumnName(@"PublishDate").HasColumnType("datetime").IsRequired(false);
            builder.Property(x => x.AddedByUserId).HasColumnName(@"AddedByUserId").HasColumnType("int").IsRequired();
            builder.Property(x => x.IsPublished).HasColumnName(@"IsPublished").HasColumnType("bit").IsRequired();

            builder.HasMany(x => x.ArticleFeedbacks).WithOne(x => x.Article).OnDelete(DeleteBehavior.Cascade);
        }
    }
}