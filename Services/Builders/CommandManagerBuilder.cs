using TranslatorTelegramBot.Commands;
// ReSharper disable ParameterHidesMember

namespace TranslatorTelegramBot.Services.Builders;

public class CommandManagerBuilder
{
    private readonly IList<ICommand> commandCollection;
    private readonly IServiceScope serviceScope;

    public CommandManagerBuilder(IServiceScope serviceScope)
    {
        commandCollection = new List<ICommand>();
        this.serviceScope = serviceScope;
    }

    public CommandManagerBuilder RegisterCommand(ICommand commandBase)
    {
        commandCollection.Add(commandBase);
        return this;
    }

    public CommandManager Build()
    {
        return new CommandManager(commandCollection, serviceScope);
    }
    
}