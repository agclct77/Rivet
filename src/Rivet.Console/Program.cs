using Microsoft.Extensions.DependencyInjection;
using Rivet.Console.Menu;
using Rivet.Console.Notification;
using Rivet.Infrastructure.Clipboard;
using Rivet.Infrastructure.Configuration;
using Rivet.Infrastructure.Logging;
using Rivet.Infrastructure.Translation;
using Rivet.Service.Clipboard;
using Rivet.Service.Configuration;
using Rivet.Service.Commands;
using Rivet.Service.Notification;
using Rivet.Service.Translation;

namespace Rivet.Console;

internal class Program
{
    [STAThread]
    private static async Task Main(string[] args)
    {
        try
        {
            // 初始化服務容器
            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            // 建立命令鏈
            var commandChain = new CommandChain();

            // 註冊所有命令處理器
            commandChain.AddHandler(new CancelCommandHandler());
            // 其他處理器會在後續階段新增（Phase 4, 5）

            // 顯示主選單並取得使用者選擇
            var selectedCommand = MainMenu.DisplayAndGetChoice();

            // 建立命令上下文
            var context = new CommandContext
            {
                CommandType = selectedCommand,
                ClipboardContent = ClipboardContent.Empty()
            };

            // 執行命令
            var result = await commandChain.ExecuteAsync(context);

            // 根據結果顯示訊息
            if (result.IsHandled)
            {
                if (result.IsSuccess && result.Message is not null)
                {
                    var notifier = serviceProvider.GetRequiredService<IUserNotifier>();
                    notifier.ShowSuccess(result.Message);
                }
                else if (!result.IsSuccess && result.Message is not null)
                {
                    var notifier = serviceProvider.GetRequiredService<IUserNotifier>();
                    notifier.ShowError(result.Message);
                }
                // 若 Message 為 null（取消操作），不顯示任何訊息
            }

            // 延遲 0.5 秒後結束
            await Task.Delay(500);
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"錯誤：{ex.Message}");
            await Task.Delay(500);
        }
    }

    /// <summary>
    /// 配置依賴注入服務
    /// </summary>
    private static void ConfigureServices(IServiceCollection services)
    {
        // 日誌設定
        SerilogSetup.Initialize();

        // 註冊服務
        services.AddSingleton<IClipboardService, WindowsClipboardService>();
        services.AddSingleton<ITranslationService, GoogleTranslationService>();
        services.AddSingleton<IConfigurationService, JsonConfigurationService>();
        services.AddSingleton<IUserNotifier, ConsoleUserNotifier>();
    }
}