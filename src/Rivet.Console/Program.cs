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
    private static void Main(string[] args)
    {
        try
        {
            // 使用 RunSynchronously 執行非同步 Main 邏輯
            MainAsync(args).Wait();
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"錯誤：{ex.Message}");
            Task.Delay(500).Wait();
        }
    }

    private static async Task MainAsync(string[] args)
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

            // Phase 4: 翻譯成繁體中文
            var translateToChineseService = new Rivet.Service.Commands.TranslateToChineseHandler(
                serviceProvider.GetRequiredService<IClipboardService>(),
                serviceProvider.GetRequiredService<ITranslationService>(),
                serviceProvider.GetRequiredService<IUserNotifier>()
            );
            commandChain.AddHandler(translateToChineseService);

            // Phase 5: 翻譯成英文
            var translateToEnglishService = new Rivet.Service.Commands.TranslateToEnglishHandler(
                serviceProvider.GetRequiredService<IClipboardService>(),
                serviceProvider.GetRequiredService<ITranslationService>(),
                serviceProvider.GetRequiredService<IUserNotifier>()
            );
            commandChain.AddHandler(translateToEnglishService);

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
            var notifier = serviceProvider.GetRequiredService<IUserNotifier>();

            if (result.IsHandled)
            {
                if (result.IsSuccess && result.Message is not null)
                {
                    notifier.ShowSuccess(result.Message);
                }
                else if (!result.IsSuccess && result.Message is not null)
                {
                    notifier.ShowError(result.Message);
                }
                else if (result.Message is null)
                {
                    // 取消操作，不顯示訊息
                }
            }
            else
            {
                // 沒有任何處理器處理此命令
                notifier.ShowError("未知命令");
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

        // 先註冊配置服務以便讀取金鑰路徑
        services.AddSingleton<IConfigurationService, JsonConfigurationService>();

        // 使用工廠註冊 GoogleTranslationService，以便從配置取得金鑰路徑
        services.AddSingleton<ITranslationService>(provider =>
        {
            var configService = provider.GetRequiredService<IConfigurationService>();
            var configResult = configService.GetConfiguration();

            if (!configResult.IsSuccess)
            {
                // 若設定失敗，使用空字符串（之後會在 TranslateAsync 時報錯）
                // 這樣確保服務容器可以成功建立
                return new GoogleTranslationService(string.Empty);
            }

            return new GoogleTranslationService(configResult.Configuration!.Services.GoogleTranslation.KeyFilePath);
        });

        // 註冊其他服務
        services.AddSingleton<IClipboardService, WindowsClipboardService>();
        services.AddSingleton<IUserNotifier, ConsoleUserNotifier>();
    }
}