using MassTransit;
using Messaging.Models;

namespace Shipment.Tracking.Api.Consumers
{
  public class ShipmentCreatedConsumer : IConsumer<ShipmentCreated>
  {
    public async Task Consume(ConsumeContext<ShipmentCreated> context)
    {
      Console.WriteLine($"Mesaj iletilidi {context.Message}");
      await Task.CompletedTask;
    }
  }
}
