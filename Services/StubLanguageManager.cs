using TranslatorTelegramBot.Models;
using TranslatorTelegramBot.Services.Abstractions;

namespace TranslatorTelegramBot.Services;

public class StubLanguageManager : ILanguageManager
{
    private readonly Language[] supportedLanguages =
    {
        new()
        {
            Code = "en",
            Name = "English",
            Flag = "\ud83c\uddfa\ud83c\uddf8"
        },
        new()
        {
            Code = "uk",
            Name = "Ukrainian",
            Flag = "\ud83c\uddfa\ud83c\udde6"
        },
        new()
        {
            Code = "pl",
            Name = "Polish",
            Flag = "\ud83c\uddf5\ud83c\uddf1"
        }
    };

    public Task<IEnumerable<Language>> GetLanguageCollectionAsync()
    {
        return Task.FromResult<IEnumerable<Language>>(supportedLanguages);
    }

    public Task<Language> GetLanguageByCodeAsync(string code)
    {
        return Task.FromResult(supportedLanguages.Single(language => language.Code
            .ToLowerInvariant()
            .Equals(code.ToLowerInvariant())));
    }
}