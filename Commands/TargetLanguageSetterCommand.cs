using System.Text.Json;
using Telegram.Bot;
using TranslatorTelegramBot.Models.Enums;
using TranslatorTelegramBot.Services.Abstractions;
using TranslatorTelegramBot.Utils;

namespace TranslatorTelegramBot.Commands;

public class TargetLanguageSetterCommand : ICommand
{
    public static string CommandName => "/set_target_language";
    
    private readonly IUserRepository userRepository;
    private readonly ITranslator translator;
    private readonly ILanguageManager languageManager;
    private readonly JsonSerializerOptions serializerOptions;
    private readonly int replyKeyboardColumns;

    public TargetLanguageSetterCommand(IUserRepository userRepository,
        ITranslator translator,
        ILanguageManager languageManager,
        JsonSerializerOptions serializerOptions,
        int replyKeyboardColumns)
    {
        this.userRepository = userRepository;
        this.translator = translator;
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

        var user = await userRepository.GetOrCreateUserAsync(chatId);
        var messageText = "Choose a target language";
        if (!string.IsNullOrWhiteSpace(user.SourceLanguage))
        {
            messageText = await translator.TranslateAsync(fromLanguage: "en",
                toLanguage: user.SourceLanguage, messageText);
        }

        var languages = await languageManager.GetLanguageCollectionAsync();
        var replyMarkup =
            TelegramUtilities.ParseLanguageCollectionAsKeyboardMarkup(languages, replyKeyboardColumns, LanguageDirection.Target,
                messageId, serializerOptions);
        
        await botClient.EditMessageTextAsync(
            chatId: chatId,
            messageId: messageId,
            text: messageText,
            replyMarkup: replyMarkup,
            cancellationToken: cancellationToken);
    }

    public string GetCommandName() => CommandName;
}