using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using TranslatorTelegramBot.Commands;
using TranslatorTelegramBot.Models;
using TranslatorTelegramBot.Models.Enums;
using TranslatorTelegramBot.Services.Abstractions;

namespace TranslatorTelegramBot.Services;

public class TranslatorUpdateHandlerService : UpdateHandlerServiceBase
{
    private readonly ITranslator translator;
    private readonly IUserRepository userRepository;
    private readonly ILanguageManager languageManager;
    private readonly CommandManager commandManager;
    private readonly JsonSerializerOptions jsonSerializerOptions;

    public TranslatorUpdateHandlerService(ITelegramBotClient botClient,
        ITranslator translator,
        IUserRepository userRepository,
        ILanguageManager languageManager,
        JsonSerializerOptions jsonSerializerOptions,
        CommandManager commandManager) : base(botClient)
    {
        this.translator = translator;
        this.userRepository = userRepository;
        this.languageManager = languageManager;
        this.commandManager = commandManager;
        this.jsonSerializerOptions = jsonSerializerOptions;

        MessageReceived += OnMessageReceived;
        MessageEdited += OnMessageReceived;
        UnknownUpdateTypeReceived += OnUnknownUpdateTypeReceived;
        CallbackQueryReceived += OnCallbackQueryReceived;
    }

    private async Task OnMessageReceived(Message message, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        if (message.Text!.StartsWith('/'))
        {
            try
            {
                await commandManager.HandleCommandAsync(message.Text, BotClient, chatId,
                    cancellationToken);
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(exception, cancellationToken);
            }
        }
        else
        {
            var user = await userRepository.GetOrCreateUserAsync(chatId);

            if (string.IsNullOrWhiteSpace(user.SourceLanguage))
            {
                await BotClient.SendTextMessageAsync(
                    chatId: chatId,
                    text:
                    $"Source language not selected. Please set source language using {SourceLanguageSetterCommand.CommandName} command",
                    cancellationToken: cancellationToken);

                return;
            }

            if (string.IsNullOrWhiteSpace(user.TargetLanguage))
            {
                var messageText =
                    $"Target language not selected. Please set target language using {TargetLanguageSetterCommand.CommandName} command";

                if (!string.IsNullOrWhiteSpace(user.SourceLanguage))
                {
                    messageText = await translator.TranslateAsync(fromLanguage: "en",
                        toLanguage: user.SourceLanguage, text: messageText);
                }
                
                await BotClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: messageText,
                    cancellationToken: cancellationToken);

                return;
            }

            var translatedText = await translator.TranslateAsync(fromLanguage: user.SourceLanguage!,
                toLanguage: user.TargetLanguage!, text: message.Text);
            
            await BotClient.SendTextMessageAsync(
                chatId: chatId,
                text: translatedText,
                cancellationToken: cancellationToken);
        }
    }
    
    private async Task OnCallbackQueryReceived(CallbackQuery callbackQuery,
        CancellationToken cancellationToken)
    {
        LanguageChoiceResponse? languageChoiceResponse = default;
        try
        {
            languageChoiceResponse =
                JsonSerializer.Deserialize<LanguageChoiceResponse>(callbackQuery.Data!,
                    jsonSerializerOptions)!;
        }
        catch (Exception exception)
        {
            await HandleErrorAsync(exception, cancellationToken);
        }

        if (languageChoiceResponse is null)
            return;

        var chatId = callbackQuery.Message!.Chat.Id;
        var user = await userRepository.GetOrCreateUserAsync(chatId);
        if (languageChoiceResponse.Direction == LanguageDirection.Source)
        {
            await userRepository.UpdateSourceLanguage(user, languageChoiceResponse.Code);
        }
        else
        {
            await userRepository.UpdateTargetLanguage(user, languageChoiceResponse.Code);
        }

        var language = await languageManager.GetLanguageByCodeAsync(languageChoiceResponse.Code);
        await BotClient.EditMessageTextAsync(
            chatId: chatId, 
            messageId: languageChoiceResponse.MessageId, 
            text: $"{languageChoiceResponse.Direction.ToString()} language successfully changed to {language}",
            cancellationToken: cancellationToken);
    }
    
    private Task OnUnknownUpdateTypeReceived(Update update, CancellationToken cancellationToken)
    {
        return BotClient.SendTextMessageAsync(
            chatId: update.Message!.Chat.Id,
            text: "This message type is not supported. At the moment, only text messages can be translated.",
            cancellationToken: cancellationToken);
    }
    
}