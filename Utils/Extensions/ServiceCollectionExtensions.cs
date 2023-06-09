using TranslatorTelegramBot.Services;
using TranslatorTelegramBot.Services.Builders;

namespace TranslatorTelegramBot.Utils.Extensions;

public static class ServiceCollectionExtensions
{
    public delegate void ConfigureCommands(IServiceProvider serviceProvider, CommandManagerBuilder builder);
    public static IServiceCollection AddCommandManager(this IServiceCollection serviceCollection,
        ConfigureCommands configureCommands)
    {
        serviceCollection.AddScoped<CommandManager>(serviceProvider =>
        {
            var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>()
                .CreateScope();
            
            var builder = new CommandManagerBuilder(scope);
            configureCommands(scope.ServiceProvider, builder);

            return builder.Build();
        });

        return serviceCollection;
    }
    
}