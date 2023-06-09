using System.Text.Json;
using Telegram.Bot.Types.ReplyMarkups;
using TranslatorTelegramBot.Models;
using TranslatorTelegramBot.Models.Enums;

namespace TranslatorTelegramBot.Utils;

public static class TelegramUtilities
{
    public static InlineKeyboardMarkup ParseLanguageCollectionAsKeyboardMarkup(IEnumerable<Language>
        languages, int columns, LanguageDirection languageDirection, int queryMessageId, JsonSerializerOptions serializerOptions)
    {
        var buttonList = new List<IEnumerable<InlineKeyboardButton>>();

        var row = new List<InlineKeyboardButton>();
        foreach (var language in languages)
        {
            var callbackData = new LanguageChoiceResponse
            {
                Code = language.Code, Direction = languageDirection, MessageId = queryMessageId
            };

            row.Add(InlineKeyboardButton.WithCallbackData(text: language.ToString(),
                callbackData: JsonSerializer.Serialize(value: callbackData,
                    options: serializerOptions)));
            
            if (row.Count == columns)
            {
                buttonList.Add(row.ToArray());
                row.Clear();
            }
            
        }

        if (row.Count > 0)
        {
            buttonList.Add(row.ToArray());
        }

        var replyMarkup = new InlineKeyboardMarkup(buttonList);
        return replyMarkup;
    }
    
}