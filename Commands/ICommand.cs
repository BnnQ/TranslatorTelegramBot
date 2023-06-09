using Telegram.Bot;

namespace TranslatorTelegramBot.Commands;

public interface ICommand
{
    public static virtual string CommandName { get; } = default!;

    public Task HandleCommand(
        ITelegramBotClient botClient,
        long chatId,
        CancellationToken cancellationToken, string? arguments = null);
    
    public string GetCommandName();
}