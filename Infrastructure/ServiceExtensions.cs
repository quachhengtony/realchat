using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Realchat.Application.LLM;
using Realchat.Application.Repositories;
using Realchat.Application.Storage;
using Realchat.Application.Workers;
using Realchat.Infrastructure.LLM;
using Realchat.Infrastructure.Persistence.Contexts;
using Realchat.Infrastructure.Persistence.Repositories;
using Realchat.Infrastructure.Storage;
using Realchat.Infrastructure.Workers;

namespace Realchat.Infrastructure;

public static class ServiceExtensions
{
    public static void ConfigureInfrastructure(this IServiceCollection services)
    {
        var connectionString = "server=localhost;user=root;password=password;database=realchat";
        services.AddDbContext<DataContext>(opt => opt.UseMySql(connectionString, new MySqlServerVersion(new Version(7, 0, 0))));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IChatbotRepository, ChatbotRepository>();
        services.AddScoped<IScriptRepository, ScriptRepository>();
        services.AddScoped<IKnowledgeBaseRepository, KnowledgeBaseRepository>();
        services.AddScoped<IInformationChunkRepository, InformationChunkRepository>();
        services.AddScoped<IMinioAdapter, MinioAdapter>();
        services.AddScoped<IMilvusAdapter, MilvusAdapter>();
        services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        services.AddSingleton<ILLamaAdapter, LLamaAdapter>();
        // services.AddSingleton(sp =>
        // {
        //     return new HubConnectionBuilder()
        //         .WithUrl("https://0rxbgslc-7091.asse.devtunnels.ms/inferencehub")
        //         .WithAutomaticReconnect()
        //         .Build();
        // });
        services.AddSingleton<ILLamaPub, LLamaPub>();
        services.AddHostedService<MinioWorker>();
    }
}