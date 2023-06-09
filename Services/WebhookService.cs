using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TranslatorTelegramBot.Configuration;

namespace TranslatorTelegramBot.Services;

public class WebhookService : IHostedService
{
    private readonly ILogger<WebhookService> logger;
    private readonly IServiceProvider serviceProvider;
    private readonly IOptions<BotConfiguration> botOptions;
    
    public WebhookService(IServiceProvider serviceProvider, IOptions<BotConfiguration> botOptions, ILoggerFactory loggerFactory)
    {
        this.serviceProvider = serviceProvider;
        this.botOptions = botOptions;
        logger = loggerFactory.CreateLogger<WebhookService>();
    }

    private ITelegramBotClient botClient = default!;
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

        var botUrl = $"{botOptions.Value.HostAddress}{botOptions.Value.Route}";

        try
        {
            await botClient.SetWebhookAsync(
                url: botUrl,
                allowedUpdates: new[]
                {
                    UpdateType.Message, UpdateType.EditedMessage, UpdateType.CallbackQuery,
                    UpdateType.Unknown
                },
                secretToken: botOptions.Value.SecretToken,
                cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogCritical("Failed to set Telegram webhook: {ErrorMessage}", exception.Message);
            throw;
        }
        
        logger.LogInformation("[POST] Successfully setted Telegram webhook");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            await botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogCritical("[POST] Failed to unset Telegram webhook: {ErrorMessage}", exception.Message);
        }
        
        logger.LogInformation("[POST] Successfully unsetted Telegram webhook");
    }
    
}