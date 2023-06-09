using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using TranslatorTelegramBot.Services.Abstractions;

namespace TranslatorTelegramBot.Controllers;

[Route(template: "[controller]")]
[ApiController]
public class BotController : ControllerBase
{
    private readonly ILogger<BotController> logger;

    public BotController(ILoggerFactory loggerFactory)
    {
        logger = loggerFactory.CreateLogger<BotController>();
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update update,
        [FromServices] UpdateHandlerServiceBase updateHandler,
        CancellationToken cancellationToken)
    {
        updateHandler.MessageReceived += OnMessageReceived;
        updateHandler.ErrorOccured += OnErrorOccured;
        updateHandler.UnknownUpdateTypeReceived += OnUnknownUpdateTypeReceived;

        await updateHandler.HandleUpdateAsync(update, cancellationToken);
        return Ok();
    }

    #region Logging

    private Task OnMessageReceived(Message message, CancellationToken _)
    {
        logger.LogInformation("[POST] Received an update message: {MessageText}", message.Text);
        return Task.CompletedTask;
    }
    
    private Task OnUnknownUpdateTypeReceived(Update update, CancellationToken cancellationtoken)
    {
        logger.LogWarning("[POST] Retrieved update with unsupported type: {UpdateType} ({InternalUpdateType})", update.Type, update.Message?.Type);
        return Task.CompletedTask;
    }

    private Task OnErrorOccured(Exception exception, CancellationToken _)
    {
        logger.LogWarning("[POST] An error occured during processing update: {ErrorMessage}",
            exception.Message);
        return Task.CompletedTask;
    }

    #endregion

}