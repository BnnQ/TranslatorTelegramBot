#nullable disable
namespace TranslatorTelegramBot.Models;

public class Language
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Flag { get; set; }

    public override string ToString()
    {
        return $"{Name}{Flag}";
    }
    
}