namespace Rivet.Service.Configuration;

/// <summary>
/// 設定結果記錄
/// 代表設定讀取操作的結果
/// </summary>
public record ConfigurationResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public required bool IsSuccess { get; init; }

    /// <summary>
    /// 設定內容（成功時有值）
    /// </summary>
    public AppConfiguration? Configuration { get; init; }

    /// <summary>
    /// 錯誤訊息（失敗時有值）
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// 建立成功的設定結果
    /// </summary>
    /// <param name="configuration">應用程式設定</param>
    /// <returns>成功的設定結果</returns>
    public static ConfigurationResult Success(AppConfiguration configuration)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration), "設定不得為 null");

        return new ConfigurationResult
        {
            IsSuccess = true,
            Configuration = configuration,
            ErrorMessage = null
        };
    }

    /// <summary>
    /// 建立失敗的設定結果
    /// </summary>
    /// <param name="errorMessage">錯誤訊息</param>
    /// <returns>失敗的設定結果</returns>
    public static ConfigurationResult Failure(string errorMessage)
    {
        ArgumentNullException.ThrowIfNull(errorMessage);

        if (string.IsNullOrWhiteSpace(errorMessage))
            throw new ArgumentException("錯誤訊息不得為空或只含空白字元", nameof(errorMessage));

        return new ConfigurationResult
        {
            IsSuccess = false,
            Configuration = null,
            ErrorMessage = errorMessage
        };
    }
}
