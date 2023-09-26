namespace Realchat.Services.LLamaService.Extensions
{
    public static class CorsPolicyExtension
    {
        public static void ConfigureCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(opt => opt.AddPolicy(name: "RealchatClient", policy =>
            {
                policy.WithOrigins("https://10.230.191.178:7090", "https://localhost:7090").AllowAnyMethod().AllowAnyHeader();
            }));
        }
    }
}
