using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gacfox.Wealthome.Models;

public class TransferEntityConfig : IEntityTypeConfiguration<Transfer>
{
    public void Configure(EntityTypeBuilder<Transfer> builder)
    {
        builder.ToTable("t_transfer");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd().HasColumnName("id");
        builder.Property(e => e.Title).IsRequired().HasMaxLength(50).HasColumnName("title");
        builder.Property(e => e.TransferTypeId).IsRequired().HasColumnName("transfer_type_id");
        builder.HasOne<TransferType>(e => e.TransferType).WithMany().HasForeignKey(e => e.TransferTypeId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(e => e.Description).HasMaxLength(255).HasColumnName("description");
        builder.Property(e => e.UserId).HasColumnName("user_id");
        builder.HasOne<User>(e => e.User).WithMany().HasForeignKey(e => e.UserId);
        builder.Property(e => e.CreatedAt).IsRequired().HasColumnName("created_at");
    }
}