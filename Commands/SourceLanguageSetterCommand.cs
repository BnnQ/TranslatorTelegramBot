using System.Text.Json;
using Telegram.Bot;
using TranslatorTelegramBot.Models.Enums;
using TranslatorTelegramBot.Services.Abstractions;
using TranslatorTelegramBot.Utils;

namespace TranslatorTelegramBot.Commands;

public class SourceLanguageSetterCommand : ICommand
{
    public static string CommandName => "/set_source_language";
    
    private readonly ILanguageManager languageManager;
    private readonly JsonSerializerOptions serializerOptions;
    private readonly int replyKeyboardColumns;

    public SourceLanguageSetterCommand(ILanguageManager languageManager,
        JsonSerializerOptions serializerOptions, int replyKeyboardColumns)
    {
        this.languageManager = languageManager;
        this.serializerOptions = serializerOptions;
        this.replyKeyboardColumns = replyKeyboardColumns;
    }

    public async Task HandleCommand(ITelegramBotClient botClient,
        long chatId,
        CancellationToken cancellationToken,
        string? arguments = null)
    {
        var messageId = (await botClient.SendTextMessageAsync(chatId: chatId, text: "Processing...",
            cancellationToken: cancellationToken)).MessageId;

        var languages = await languageManager.GetLanguageCollectionAsync();
        var replyMarkup =
            TelegramUtilities.ParseLanguageCollectionAsKeyboardMarkup(languages, replyKeyboardColumns,
                LanguageDirection.Source,
                messageId, serializerOptions);

        await botClient.EditMessageTextAsync(
            chatId: chatId,
            messageId: messageId,
            text: "Choose a source language",
            replyMarkup: replyMarkup,
            cancellationToken: cancellationToken);
    }

    public string GetCommandName() => CommandName;

}