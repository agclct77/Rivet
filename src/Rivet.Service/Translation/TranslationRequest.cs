namespace Rivet.Service.Translation;

/// <summary>
/// 翻譯請求記錄
/// 代表使用者發起的翻譯請求
/// </summary>
public record TranslationRequest
{
    /// <summary>
    /// 來源文字
    /// </summary>
    public required string SourceText { get; init; }

    /// <summary>
    /// 目標語言
    /// </summary>
    public required TargetLanguage TargetLanguage { get; init; }
}
