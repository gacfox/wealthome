using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gacfox.Wealthome.Models;

public class TransferTypeEntityConfig : IEntityTypeConfiguration<TransferType>
{
    public void Configure(EntityTypeBuilder<TransferType> builder)
    {
        builder.ToTable("t_transfer_type");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd().HasColumnName("id");
        builder.Property(e => e.Name).IsRequired().HasMaxLength(20).HasColumnName("name");
        builder.Property(e => e.InoutType).IsRequired().HasColumnName("inout_type");
    }
}