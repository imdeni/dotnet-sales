using Microsoft.EntityFrameworkCore;
using TechnicalTest.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<ComCustomerModel> ComCustomers { get; set; }
    public DbSet<SoOrderModel> SoOrders { get; set; }
    public DbSet<SoItemModel> SoItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ComCustomerModel>().ToTable("COM_CUSTOMER");
        modelBuilder.Entity<SoOrderModel>().ToTable("SO_ORDER");
        modelBuilder.Entity<SoItemModel>().ToTable("SO_ITEM");

        modelBuilder.Entity<SoOrderModel>()
            .HasOne(o => o.ComCustomer)
            .WithMany(c => c.SoOrders)
            .HasForeignKey(o => o.ComCustomerId);
    }
}
