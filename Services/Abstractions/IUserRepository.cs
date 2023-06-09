using TranslatorTelegramBot.Models.Entities;

namespace TranslatorTelegramBot.Services.Abstractions;

public interface IUserRepository
{
    public Task<User> GetOrCreateUserAsync(long chatId);

    public Task UpdateSourceLanguage(User user, string newSourceLanguage);
    public Task UpdateTargetLanguage(User user, string newTargetLanguage);
    
}