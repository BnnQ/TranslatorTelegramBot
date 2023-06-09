using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TranslatorTelegramBot.Utils.Extensions;

namespace TranslatorTelegramBot.Services.Abstractions;

public abstract class UpdateHandlerServiceBase
{
    #region Delegates & Events

    public delegate Task MessageReceivedEventHandler(Message message, CancellationToken cancellationToken);
    public event MessageReceivedEventHandler? MessageReceived;
    
    public delegate Task MessageEditedEventHandler(Message editedMessage, CancellationToken cancellationToken);
    public event MessageEditedEventHandler? MessageEdited;

    public delegate Task UnknownUpdateTypeReceivedEventHandler(Update update, CancellationToken cancellationToken);
    public event UnknownUpdateTypeReceivedEventHandler? UnknownUpdateTypeReceived;

    public delegate Task CallbackQueryReceivedEventHandler(CallbackQuery callbackQuery, CancellationToken cancellationToken);
    public event CallbackQueryReceivedEventHandler? CallbackQueryReceived;

    public delegate Task ErrorOccuredEventHandler(Exception exception, CancellationToken cancellationToken);
    public event ErrorOccuredEventHandler? ErrorOccured;

    #endregion

    protected readonly ITelegramBotClient BotClient;
    protected UpdateHandlerServiceBase(ITelegramBotClient botClient)
    {
        BotClient = botClient;
    }

    public Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        var handlers = update.Type switch
        {
            UpdateType.Message when update.Message!.Type == MessageType.Text => MessageReceived?.InvokeAll(handler => handler(update.Message!, cancellationToken)),
            UpdateType.CallbackQuery => CallbackQueryReceived?.InvokeAll(handler => handler(update.CallbackQuery!, cancellationToken)),
            UpdateType.EditedMessage => MessageEdited?.InvokeAll(handler => handler(update.Message!, cancellationToken)),
            _ => UnknownUpdateTypeReceived?.InvokeAll(handler => handler(update, cancellationToken))
        };

        return handlers is not null ? Task.WhenAll(handlers) : Task.CompletedTask;
    }

    public Task HandleErrorAsync(Exception exception, CancellationToken cancellationToken)
    {
        return ErrorOccured?.Invoke(exception, cancellationToken) ?? Task.CompletedTask;
    }
    
}