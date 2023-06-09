// ReSharper disable UnusedAutoPropertyAccessor.Global
#nullable disable
namespace TranslatorTelegramBot.Configuration;

public class AzureConfiguration : ConfigurationBase
{
    public static readonly string SectionPath = "Azure";
    public Translator Translator { get; set; }


    public override void Validate()
    {
        Translator.Validate();
    }
}

public class Translator : ConfigurationBase
{
    public string Endpoint { get; set; }
    public string SubscriptionKey { get; set; }
    public string Region { get; set; }
    
    public override void Validate()
    {
        ValidateNotNull(AzureConfiguration.SectionPath, nameof(Endpoint), Endpoint);
        ValidateNotNull(AzureConfiguration.SectionPath, nameof(SubscriptionKey), SubscriptionKey);
        ValidateNotNull(AzureConfiguration.SectionPath, nameof(Region), Region);
    }
    
}
