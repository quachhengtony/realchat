using Realchat.Services.LLamaService.Extensions;
using Realchat.Services.LLamaService.Hubs;
using Realchat.Services.LLamaService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<ILLamaService, LLamaService>();
builder.Services.ConfigureCorsPolicy();
builder.Services.AddSignalR().AddHubOptions<InferenceHub>(opt =>
{
    opt.KeepAliveInterval = TimeSpan.FromMinutes(15);
    opt.ClientTimeoutInterval = TimeSpan.FromMinutes(35);
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

//app.UseAuthorization();

app.UseCors("RealchatClient");

app.MapControllers();

app.MapHub<InferenceHub>("/inferencehub");

app.Run();
