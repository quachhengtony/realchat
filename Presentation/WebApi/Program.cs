using Realchat.WebApi.Extensions;
using Realchat.Infrastructure;
using Realchat.Application;
using Realchat.WebApi.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.ConfigureApplication();
builder.Services.ConfigureInfrastructure();
builder.Services.ConfigureCorsPolicy();
builder.Services.AddSignalR().AddHubOptions<ChatHub>(opt =>
{
    opt.KeepAliveInterval = TimeSpan.FromMinutes(15);
    opt.ClientTimeoutInterval = TimeSpan.FromMinutes(30);
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.WebHost.ConfigureKestrel(cfg =>
{
    cfg.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// app.UseAuthorization();

app.UseErrorHandler();

app.UseCors("RealchatClient");

app.MapControllers();

app.MapHub<ChatHub>("/chathub");

app.Run();
