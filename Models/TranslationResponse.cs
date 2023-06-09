// ReSharper disable CollectionNeverUpdated.Global
#nullable disable
namespace TranslatorTelegramBot.Models;

public class TranslationResponse
{
    public List<Translation> Translations { get; set; }
}

public class Translation
{
    public string Text { get; set; }
}