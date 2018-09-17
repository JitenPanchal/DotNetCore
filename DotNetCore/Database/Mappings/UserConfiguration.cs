using DotNetCore.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotNetCore.Database.Mappings
{
    public partial class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users", Constants.Database.SchemaName);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName(@"Id").HasColumnType("int").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.Name).HasColumnName(@"Name").HasColumnType("nvarchar").IsRequired().HasMaxLength(256);
            builder.Property(x => x.Email).HasColumnName(@"Email").HasColumnType("nvarchar").IsRequired().HasMaxLength(256);
            builder.Property(x => x.Password).HasColumnName(@"Password").HasColumnType("nvarchar").IsRequired().HasMaxLength(128);
            builder.Property(x => x.PasswordSalt).HasColumnName(@"PasswordSalt").HasColumnType("nvarchar").IsRequired().HasMaxLength(128);
            builder.Property(x => x.IsLockedOut).HasColumnName(@"IsLockedOut").HasColumnType("bit").IsRequired();
            builder.Property(x => x.LastLoginDate).HasColumnName(@"LastLoginDate").HasColumnType("datetime").IsRequired(false);
            builder.Property(x => x.LastActivityDate).HasColumnName(@"LastActivityDate").HasColumnType("datetime").IsRequired(false);
            builder.Property(x => x.Token).HasColumnName(@"Token").HasColumnType("nvarchar").IsRequired(false).HasMaxLength(50);
            builder.Property(x => x.ExpiresIn).HasColumnName(@"ExpiresIn").HasColumnType("int").IsRequired();
            builder.Property(x => x.UserType).HasColumnName(@"UserType").HasColumnType("tinyint").IsRequired();
        }
    }
}