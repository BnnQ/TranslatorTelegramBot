namespace TranslatorTelegramBot.Utils.Extensions;

public static class MulticastDelegateExtensions
{
    public static IEnumerable<Task>? InvokeAll<T>(this T? multicastDelegate, Func<T, Task> invocationFunction) where T : MulticastDelegate
    {
        return multicastDelegate?.GetInvocationList()
            .Cast<T>()
            .Select(invocationFunction.Invoke)
            .ToArray();
    }
}