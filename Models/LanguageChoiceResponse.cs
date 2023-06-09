#nullable disable
using TranslatorTelegramBot.Models.Enums;

namespace TranslatorTelegramBot.Models;

public class LanguageChoiceResponse
{
    public string Code { get; set; }
    public LanguageDirection Direction { get; set; }
    public int MessageId { get; set; }
}