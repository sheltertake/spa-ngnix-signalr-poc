using MediatR;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.EnableDetailedErrors = true;
});

builder.Services.AddMediatR(typeof(NotificationHub));

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "localhost",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200", "http://localhost:9001")
                .AllowAnyHeader()
                .WithMethods("GET", "POST")
                .AllowCredentials();
        });
});

var app = builder.Build();

app.UseStaticFiles();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("localhost");

app.UseHttpsRedirection();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<NotificationHub>("/notifications");
});
app.UseSpa(spa =>
{
    if (app.Environment.IsDevelopment())
    {
        spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
    }
});
app.Run();
public interface IPushNotificationHubClient
{
    Task PongAsync(Guid newGuid);
}
public interface IInvokeNotificationHubClient
{
    Task PingAsync();
}
public class NotificationHub : Hub<IPushNotificationHubClient>, IInvokeNotificationHubClient
{
    private readonly IMediator _mediator;
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(
        IMediator mediator,
        ILogger<NotificationHub> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public Task PingAsync()
    {

        return _mediator.Publish(new PingCommand(Context.ConnectionId));
    }
}

public class PingCommand : INotification
{
    public string ConnectionId { get; }

    public PingCommand(string connectionId)
    {
        ConnectionId = connectionId;
    }
}

public class PingCommandNotificationHandler : INotificationHandler<PingCommand>
{
    private readonly IMediator _mediator;
    private readonly ILogger<PingCommandNotificationHandler> _logger;

    public PingCommandNotificationHandler(
        IMediator mediator,
        ILogger<PingCommandNotificationHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public Task Handle(PingCommand notification, CancellationToken cancellationToken)
    { 
        return _mediator.Publish(new PongCommand(notification.ConnectionId), cancellationToken);
    }
}


public class PongCommand : INotification
{
    public string ConnectionId { get; }

    public PongCommand(string connectionId)
    {
        ConnectionId = connectionId;
    }
}


public class PongCommandNotificationHandler : INotificationHandler<PongCommand>
{
    private readonly IHubContext<NotificationHub, IPushNotificationHubClient> _hubContext;
    private readonly ILogger<PongCommandNotificationHandler> _logger;

    public PongCommandNotificationHandler(
        IHubContext<NotificationHub, IPushNotificationHubClient> hubContext,
        ILogger<PongCommandNotificationHandler> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public Task Handle(PongCommand notification, CancellationToken cancellationToken)
    {
        return _hubContext.Clients.Clients(notification.ConnectionId).PongAsync(Guid.NewGuid());
    }
}