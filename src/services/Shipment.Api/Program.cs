using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shipment.Api.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF baðlanmak için.
builder.Services.AddDbContext<AppDbContext>(opt =>
{
  opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlConn"));
});

// Masstransit Outbox Pattern ayarlarý
builder.Services.AddMassTransit(opt =>
{
  opt.AddEntityFrameworkOutbox<AppDbContext>(o =>
  {
    o.DisableInboxCleanupService();
    o.UseSqlServer().UseBusOutbox();
  });

  opt.UsingRabbitMq((context, cfg) =>
  {
    cfg.Host(builder.Configuration.GetConnectionString("RabbitMqConn"));
    cfg.UseMessageRetry(c => c.Interval(3, TimeSpan.FromSeconds(5))); // 30 saniye olarak kullanýlabilir. 
    cfg.ConfigureEndpoints(context); // background servicelerin arka plan consumerlar üzerinden takibi için önemli.
  });


});



var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
