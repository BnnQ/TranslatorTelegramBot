namespace TranslatorTelegramBot.Models.Entities;

public class User
{
    public long ChatId { get; set; }
    public string? SourceLanguage { get; set; }
    public string? TargetLanguage { get; set; }
}