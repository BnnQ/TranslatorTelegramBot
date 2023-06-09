using System.Text;
using System.Text.RegularExpressions;
using Telegram.Bot;
using TranslatorTelegramBot.Commands;
using TranslatorTelegramBot.Exceptions;

namespace TranslatorTelegramBot.Services;

public sealed partial class CommandManager : IDisposable
{
    private readonly IEnumerable<ICommand> commandCollection;
    private readonly IServiceScope serviceScope;

    public CommandManager(IEnumerable<ICommand> commandCollection, IServiceScope serviceScope)
    {
        this.commandCollection = commandCollection;
        this.serviceScope = serviceScope;
    }

    public IEnumerable<ICommand> GetRegisteredCommandCollection()
    {
        return commandCollection;
    }

    public async Task HandleCommandAsync(string command,
        ITelegramBotClient botClient,
        long chatId,
        CancellationToken cancellationToken)
    {
        const string Whitespace = " ";

        string? arguments = default;
        string commandName;
        try
        {
            var splittedCommand = GetAnyWhitespaceCharactersPattern()
                .Replace(command.Trim(), replacement: Whitespace)
                .Split(Whitespace, StringSplitOptions.RemoveEmptyEntries);
            commandName = splittedCommand.First();

            if (splittedCommand.Length > 1)
            {
                arguments = splittedCommand.Skip(1)
                    .Aggregate(seed: new StringBuilder(),
                        (resultBuilder, argument) => resultBuilder.Append($" {argument}"),
                        resultBuilder => resultBuilder.ToString());
            }
        }
        catch (Exception exception)
        {
            throw new CommandParseException(exception.Message);
        }

        var registeredCommand =
            commandCollection.FirstOrDefault(item => item.GetCommandName()
                .Equals(commandName));

        if (registeredCommand is null)
        {
            throw new CommandNotRegisteredException(commandName);
        }

        await registeredCommand.HandleCommand(botClient, chatId, cancellationToken, arguments);
    }

    [GeneratedRegex("\\s{2,}")]
    private static partial Regex GetAnyWhitespaceCharactersPattern();

    #region Implementation of IDisposable

    private bool disposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool needDisposing)
    {
        if (!disposed)
        {
            if (needDisposing)
            {
                serviceScope.Dispose();
            }

            disposed = true;
        }
    }

    ~CommandManager()
    {
        Dispose(false);
    }

    #endregion
}