using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gacfox.Wealthome.Models;

public class UserEntityConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("t_user");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd().HasColumnName("id");
        builder.HasIndex(e => e.UserName).IsUnique();
        builder.Property(e => e.UserName).IsRequired().HasMaxLength(20).HasColumnName("user_name");
        builder.Property(e => e.DisplayName).HasMaxLength(20).HasColumnName("display_name");
        builder.Property(e => e.Email).HasMaxLength(50).HasColumnName("email");
        builder.Property(e => e.AvatarUrl).HasMaxLength(255).HasColumnName("avatar_url");
        builder.Property(e => e.RoleCode).IsRequired().HasMaxLength(10).HasColumnName("role_code");
        builder.Property(e => e.PasswordHash).IsRequired().HasMaxLength(64).HasColumnName("password_hash");
        builder.Property(e => e.Status).IsRequired().HasColumnName("status");
        builder.Property(e => e.CreatedAt).IsRequired().HasColumnName("created_at");
    }
}