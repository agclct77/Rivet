namespace Rivet.Service.Commands;

/// <summary>
/// 指令執行結果記錄
/// 代表指令執行操作的結果
/// </summary>
public record CommandResult
{
    /// <summary>
    /// 指令是否已被處理
    /// </summary>
    public required bool IsHandled { get; init; }

    /// <summary>
    /// 指令是否執行成功（僅在 IsHandled=true 時有效）
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// 成功/錯誤訊息（僅在 IsHandled=true 時有值）
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    /// 建立已處理且成功的指令結果
    /// </summary>
    /// <param name="message">成功訊息</param>
    /// <returns>已處理且成功的指令結果</returns>
    public static CommandResult Handled(string message)
    {
        ArgumentNullException.ThrowIfNull(message);

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("成功訊息不得為空或只含空白字元", nameof(message));

        return new CommandResult
        {
            IsHandled = true,
            IsSuccess = true,
            Message = message
        };
    }

    /// <summary>
    /// 建立已處理但失敗的指令結果
    /// </summary>
    /// <param name="errorMessage">錯誤訊息</param>
    /// <returns>已處理但失敗的指令結果</returns>
    public static CommandResult Failed(string errorMessage)
    {
        ArgumentNullException.ThrowIfNull(errorMessage);

        if (string.IsNullOrWhiteSpace(errorMessage))
            throw new ArgumentException("錯誤訊息不得為空或只含空白字元", nameof(errorMessage));

        return new CommandResult
        {
            IsHandled = true,
            IsSuccess = false,
            Message = errorMessage
        };
    }

    /// <summary>
    /// 建立未被處理的指令結果
    /// </summary>
    /// <returns>未被處理的指令結果</returns>
    public static CommandResult NotHandled()
    {
        return new CommandResult
        {
            IsHandled = false,
            IsSuccess = false,
            Message = null
        };
    }

    /// <summary>
    /// 建立取消操作的指令結果
    /// </summary>
    /// <returns>取消操作的指令結果</returns>
    public static CommandResult Cancelled()
    {
        return new CommandResult
        {
            IsHandled = true,
            IsSuccess = true,
            Message = null
        };
    }
}
