using Microsoft.EntityFrameworkCore;

namespace Gacfox.Wealthome.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transfer> Transfers { get; set; }
    public DbSet<TransferType> TransferTypes { get; set; }
    public DbSet<Flow> Flows { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 从程序集中引入所有数据表映射配置类
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}