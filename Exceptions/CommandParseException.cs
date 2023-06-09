namespace TranslatorTelegramBot.Exceptions;

public class CommandParseException : ApplicationException
{
    public CommandParseException(string details) : base(
        $"Error occured during command parsing. Details: {details}")
    {
    }
    
}