using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gacfox.Wealthome.Models;

public class FlowEntityConfig : IEntityTypeConfiguration<Flow>
{
    public void Configure(EntityTypeBuilder<Flow> builder)
    {
        builder.ToTable("t_flow");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd().HasColumnName("id");
        builder.Property(e => e.AccountId).IsRequired().HasColumnName("account_id");
        builder.HasOne(e => e.Account).WithMany().HasForeignKey(e => e.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(e => e.Amount).IsRequired().HasColumnName("amount");
        builder.Property(e => e.TransferId).IsRequired().HasColumnName("transfer_id");
        builder.HasOne(e => e.Transfer).WithMany(e => e.Flows).HasForeignKey(e => e.TransferId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(e => e.CreatedAt).IsRequired().HasColumnName("created_at");
    }
}