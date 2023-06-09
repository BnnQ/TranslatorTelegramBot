using Telegram.Bot;

namespace TranslatorTelegramBot.Commands;

public class StartCommand : ICommand
{
    public static string CommandName => "/start";

    public Task HandleCommand(ITelegramBotClient botClient,
        long chatId,
        CancellationToken cancellationToken,
        string? arguments = null)
    {
        var message =
            $"Welcome to translator bot. To get started, select the source language and the target language using the appropriate commands {SourceLanguageSetterCommand.CommandName} and {TargetLanguageSetterCommand.CommandName}. Then just send a message to the bot and it will send you a translation of your message.";

        return botClient.SendTextMessageAsync(
            chatId: chatId,
            text: message,
            cancellationToken: cancellationToken);
    }

    public string GetCommandName() => CommandName;
}