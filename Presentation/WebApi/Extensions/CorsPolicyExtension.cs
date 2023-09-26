namespace Realchat.WebApi.Extensions;

public static class CorsPolicyExtension
{
    public static void ConfigureCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(opt => opt.AddPolicy(name: "RealchatClient",
        policy =>
        {
            policy.WithOrigins("https://localhost:7190", "http://127.0.0.1:3000", "http://localhost:3000").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
        }));
    }
}