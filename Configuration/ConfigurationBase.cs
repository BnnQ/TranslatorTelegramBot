namespace TranslatorTelegramBot.Configuration;

public abstract class ConfigurationBase
{
    public abstract void Validate();

    protected void ValidateNotNull<T>(string sectionPath, string propertyName, T propertyValue)
    {
        var isValid = true;
        if (propertyValue is string stringPropetyValue)
        {
            isValid = !string.IsNullOrWhiteSpace(stringPropetyValue);
        }
        else
        {
            isValid = propertyValue is not null;
        }


        if (!isValid)
        {
            throw new InvalidOperationException($"'{sectionPath}:{propertyName}' configuration value is not provided.");
        }
        
    }
    
}