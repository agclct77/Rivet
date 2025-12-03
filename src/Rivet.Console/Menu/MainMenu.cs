using Spectre.Console;
using Rivet.Service.Commands;

namespace Rivet.Console.Menu;

/// <summary>
/// 主選單顯示與處理類別，使用 Spectre.Console 提供使用者互動介面
/// </summary>
public class MainMenu
{
    /// <summary>
    /// 顯示主選單並等待使用者選擇
    /// </summary>
    /// <returns>使用者選擇的指令類型，或 Cancel 如果使用者按下 Esc</returns>
    public static CommandType DisplayAndGetChoice()
    {
        AnsiConsole.Clear();

        // 使用 Spectre.Console 的 SelectionPrompt 顯示選單
        var prompt = new SelectionPrompt<MenuOption>()
            .Title("[bold green]請選擇功能：[/]")
            .AddChoices(
                new MenuOption("翻譯成繁體中文", CommandType.TranslateToChinese),
                new MenuOption("翻譯成英文", CommandType.TranslateToEnglish),
                new MenuOption("取消", CommandType.Cancel)
            )
            .UseConverter(opt => opt.DisplayName);

        var selected = AnsiConsole.Prompt(prompt);
        return selected.CommandType;
    }

    /// <summary>
    /// 選單選項內部類別
    /// </summary>
    private class MenuOption
    {
        public string DisplayName { get; }
        public CommandType CommandType { get; }

        public MenuOption(string displayName, CommandType commandType)
        {
            DisplayName = displayName;
            CommandType = commandType;
        }
    }
}
