using Microsoft.EntityFrameworkCore;
using TranslatorTelegramBot.Models.Entities;

namespace TranslatorTelegramBot.Models.Contexts;

public class TranslatorUsersContext : DbContext
{
    public DbSet<User>? Users { get; set; }
    
    public TranslatorUsersContext(DbContextOptions options) : base(options)
    {
        //empty
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(userBuilder =>
        {
            userBuilder.HasKey(user => user.ChatId);
            userBuilder.Property(user => user.ChatId)
                .ValueGeneratedNever();

            userBuilder.Property(user => user.SourceLanguage)
                .HasColumnType("varchar(4)")
                .HasMaxLength(4)
                .IsRequired(false);

            userBuilder.Property(user => user.TargetLanguage)
                .HasColumnType("varchar(4)")
                .HasMaxLength(4)
                .IsRequired(false);
        });

        base.OnModelCreating(modelBuilder);
    }
}