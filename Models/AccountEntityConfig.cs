using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gacfox.Wealthome.Models;

public class AccountEntityConfig : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("t_account");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd().HasColumnName("id");
        builder.Property(e => e.AccountName).IsRequired().HasMaxLength(50).HasColumnName("account_name");
        builder.Property(e => e.AccountType).IsRequired().HasColumnName("account_type");
        builder.Property(e => e.Balance).IsRequired().HasDefaultValue(0.00m).HasColumnName("balance");
        builder.Property(e => e.UserId).IsRequired().HasColumnName("user_id");
        builder.HasOne<User>(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.Property(e => e.CreatedAt).IsRequired().HasColumnName("created_at");
    }
}