using MassTransit;
using RabbitMQ.IntegrationMessages;
using RequestReply.ApiGateway;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ReserveTicketConsumer>(typeof(SubmitOrderConsumerDefinition));
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});
builder.Services.AddSingleton<BusEventsCommunicationChannel<ReserveTicketResponse>>();

EndpointConvention.Map<ReserveTicketRequest>(
    new Uri("rabbitmq://localhost/ticket-reservation-requests"));

EndpointConvention.Map<ReserveTicketResponse>(
    new Uri("rabbitmq://localhost/ticket-reservation-responses"));

builder.Services.AddMassTransitHostedService();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
