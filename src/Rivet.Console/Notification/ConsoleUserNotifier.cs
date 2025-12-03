using Spectre.Console;
using Rivet.Service.Notification;

namespace Rivet.Console.Notification;

/// <summary>
/// Console 使用者通知器實作
/// 使用 Spectre.Console 顯示彩色訊息
/// </summary>
public class ConsoleUserNotifier : IUserNotifier
{
    /// <inheritdoc />
    public void ShowSuccess(string message)
    {
        ArgumentNullException.ThrowIfNull(message);
        AnsiConsole.MarkupLine($"[green]✓ {EscapeMarkup(message)}[/]");
    }

    /// <inheritdoc />
    public void ShowError(string message)
    {
        ArgumentNullException.ThrowIfNull(message);
        AnsiConsole.MarkupLine($"[red]✗ {EscapeMarkup(message)}[/]");
    }

    /// <inheritdoc />
    public void ShowInfo(string message)
    {
        ArgumentNullException.ThrowIfNull(message);
        AnsiConsole.MarkupLine($"[blue]ℹ {EscapeMarkup(message)}[/]");
    }

    /// <summary>
    /// 逃逸 Spectre.Console 的特殊字元
    /// </summary>
    /// <param name="text">原始文字</param>
    /// <returns>逃逸後的文字</returns>
    private static string EscapeMarkup(string text)
    {
        return text
            .Replace("[", "[[")
            .Replace("]", "]]");
    }
}
