using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Shipment.Api.Database
{
  public class AppDbContext:DbContext
  {

    public DbSet<Shipment> Shipments { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> opt):base(opt)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      // outbox pattern için gönderilemeyen eventlerin yada commandların tutulduğu tablolar.
      modelBuilder.AddOutboxMessageEntity();
      modelBuilder.AddOutboxStateEntity();
    }
  }
}
