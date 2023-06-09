#nullable disable
namespace TranslatorTelegramBot.Configuration;

public class BotConfiguration : ConfigurationBase
{
    public static readonly string SectionPath = nameof(BotConfiguration);
    public string BotToken { get; set; }
    public string Route { get; set; }
    public string HostAddress { get; set; }
    public string SecretToken { get; set; }
    
    public override void Validate()
    {
        ValidateNotNull(SectionPath, nameof(BotToken), BotToken);
        ValidateNotNull(SectionPath, nameof(Route), Route);
        ValidateNotNull(SectionPath, nameof(HostAddress), HostAddress);
        ValidateNotNull(SectionPath, nameof(SecretToken), SecretToken);
    }
    
}