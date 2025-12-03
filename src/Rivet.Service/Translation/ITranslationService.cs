namespace Rivet.Service.Translation;

/// <summary>
/// 翻譯服務介面，提供文字翻譯功能。
/// </summary>
public interface ITranslationService
{
    /// <summary>
    /// 將文字翻譯成指定語言。
    /// </summary>
    /// <param name="request">翻譯請求，包含來源文字與目標語言。</param>
    /// <param name="cancellationToken">取消權杖。</param>
    /// <returns>翻譯結果，包含成功狀態與翻譯文字或錯誤訊息。</returns>
    Task<TranslationResult> TranslateAsync(
        TranslationRequest request,
        CancellationToken cancellationToken = default);
}
