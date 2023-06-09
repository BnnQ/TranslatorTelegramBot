using TranslatorTelegramBot.Models;

namespace TranslatorTelegramBot.Services.Abstractions;

public interface ILanguageManager
{
    public Task<IEnumerable<Language>> GetLanguageCollectionAsync();
    public Task<Language> GetLanguageByCodeAsync(string code);
}