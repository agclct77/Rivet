namespace Rivet.Service.Commands;

/// <summary>
/// 指令處理器介面，實作責任鏈模式。
/// </summary>
public interface ICommandHandler
{
    /// <summary>
    /// 設定下一個處理器。
    /// </summary>
    /// <param name="handler">下一個處理器。</param>
    /// <exception cref="ArgumentNullException">若 handler 為 null。</exception>
    void SetNext(ICommandHandler handler);

    /// <summary>
    /// 處理指令。
    /// </summary>
    /// <param name="context">指令上下文。</param>
    /// <param name="cancellationToken">取消權杖。</param>
    /// <returns>指令執行結果。</returns>
    Task<CommandResult> HandleAsync(
        CommandContext context,
        CancellationToken cancellationToken = default);
}
