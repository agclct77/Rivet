namespace Rivet.Service.Configuration;

/// <summary>
/// 應用程式設定記錄
/// 從 config.json 讀取的設定資訊
/// </summary>
public record AppConfiguration
{
    /// <summary>
    /// 服務設定
    /// </summary>
    public required ServiceConfiguration Services { get; init; }
}

/// <summary>
/// 服務設定記錄
/// </summary>
public record ServiceConfiguration
{
    /// <summary>
    /// Google 翻譯服務設定
    /// </summary>
    public required GoogleTranslationConfig GoogleTranslation { get; init; }
}

/// <summary>
/// Google 翻譯服務設定記錄
/// </summary>
public record GoogleTranslationConfig
{
    /// <summary>
    /// 金鑰檔案路徑
    /// </summary>
    public required string KeyFilePath { get; init; }
}
