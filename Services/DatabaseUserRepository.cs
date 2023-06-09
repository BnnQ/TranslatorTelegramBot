using TranslatorTelegramBot.Models.Contexts;
using TranslatorTelegramBot.Models.Entities;
using TranslatorTelegramBot.Services.Abstractions;

namespace TranslatorTelegramBot.Services;

public class DatabaseUserRepository : IUserRepository
{
    private readonly TranslatorUsersContext context;

    public DatabaseUserRepository(TranslatorUsersContext context)
    {
        this.context = context;
    }
    
    public async Task<User> GetOrCreateUserAsync(long chatId)
    {
        if (context.Users is null)
        {
            throw new InvalidOperationException("Db set 'Users' is null.");
        }

        var user = await context.Users.FindAsync(chatId);
        if (user is null)
        {
            user = new User { ChatId = chatId };
            await context.AddAsync(user);
            await context.SaveChangesAsync();
        }

        return user;
    }

    public Task UpdateSourceLanguage(User user, string newSourceLanguage)
    {
        user.SourceLanguage = newSourceLanguage;
        return context.SaveChangesAsync();
    }

    public Task UpdateTargetLanguage(User user, string newTargetLanguage)
    {
        user.TargetLanguage = newTargetLanguage;
        return context.SaveChangesAsync();
    }
    
}