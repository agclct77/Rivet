using Serilog;
using Serilog.Core;

namespace Rivet.Infrastructure.Logging;

/// <summary>
/// Serilog 日誌設定
/// 配置日誌輸出到 %AppData%\Rivet\logs\
/// 支援 Trace 層級以記錄敏感資訊如剪貼簿內容
/// </summary>
public static class SerilogSetup
{
    /// <summary>
    /// 初始化 Serilog 日誌記錄器
    /// </summary>
    /// <returns>設定完成的 ILogger 實例</returns>
    public static ILogger Initialize()
    {
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string logDirectoryPath = Path.Combine(appDataPath, "Rivet", "logs");

        // 確保日誌目錄存在
        Directory.CreateDirectory(logDirectoryPath);

        string logFilePath = Path.Combine(logDirectoryPath, "rivet-.log");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose() // Trace 層級及以上
            .WriteTo.File(
                path: logFilePath,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7, // 保留最近 7 天的日誌
                encoding: System.Text.Encoding.UTF8)
            .CreateLogger();

        return Log.Logger;
    }

    /// <summary>
    /// 關閉日誌記錄器，確保緩衝資料被寫入
    /// </summary>
    public static void Shutdown()
    {
        Log.CloseAndFlush();
    }
}
