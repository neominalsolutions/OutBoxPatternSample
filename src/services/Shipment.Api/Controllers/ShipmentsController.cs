using MassTransit;
using Messaging.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shipment.Api.Database;
using Shipment.Api.Migrations;

namespace Shipment.Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ShipmentsController : ControllerBase
  {
    private readonly IPublishEndpoint publishEndpoint;
    private readonly AppDbContext appDbContext;

    public ShipmentsController(IPublishEndpoint publishEndpoint, AppDbContext appDbContext)
    {
      this.publishEndpoint = publishEndpoint;
      this.appDbContext = appDbContext;
    }

    [HttpPost]
    public async Task<IActionResult> Create()
    {

      using (var tran = await this.appDbContext.Database.BeginTransactionAsync())
      {

        try
        {
          var entity = new Shipment.Api.Database.Shipment();
          entity.Address = "Istanbul";

          var @event = new ShipmentCreated(entity.Id, entity.Address);
          await this.publishEndpoint.Publish(@event);

          this.appDbContext.Shipments.Add(entity);
          await this.appDbContext.SaveChangesAsync();

          await tran.CommitAsync();
        }
        catch (Exception)
        {
          await tran.RollbackAsync();

          // sıralama önemli Publish her zaman SaveChangesAsync  den önce tanımlanmadır.
        }
      }

      return Ok("Başarılı");

    }

  }
}
