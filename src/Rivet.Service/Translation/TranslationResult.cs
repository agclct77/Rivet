namespace Rivet.Service.Translation;

/// <summary>
/// 翻譯結果記錄
/// 代表翻譯操作的結果
/// </summary>
public record TranslationResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public required bool IsSuccess { get; init; }

    /// <summary>
    /// 翻譯後文字（成功時有值）
    /// </summary>
    public string? TranslatedText { get; init; }

    /// <summary>
    /// 錯誤訊息（失敗時有值）
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// 建立成功的翻譯結果
    /// </summary>
    /// <param name="translatedText">翻譯後的文字</param>
    /// <returns>成功的翻譯結果</returns>
    public static TranslationResult Success(string translatedText)
    {
        ArgumentNullException.ThrowIfNull(translatedText);

        if (string.IsNullOrWhiteSpace(translatedText))
            throw new ArgumentException("翻譯文字不得為空或只含空白字元", nameof(translatedText));

        return new TranslationResult
        {
            IsSuccess = true,
            TranslatedText = translatedText,
            ErrorMessage = null
        };
    }

    /// <summary>
    /// 建立失敗的翻譯結果
    /// </summary>
    /// <param name="errorMessage">錯誤訊息</param>
    /// <returns>失敗的翻譯結果</returns>
    public static TranslationResult Failure(string errorMessage)
    {
        ArgumentNullException.ThrowIfNull(errorMessage);

        if (string.IsNullOrWhiteSpace(errorMessage))
            throw new ArgumentException("錯誤訊息不得為空或只含空白字元", nameof(errorMessage));

        return new TranslationResult
        {
            IsSuccess = false,
            TranslatedText = null,
            ErrorMessage = errorMessage
        };
    }
}
