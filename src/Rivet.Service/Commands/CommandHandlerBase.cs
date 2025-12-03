namespace Rivet.Service.Commands;

/// <summary>
/// 指令處理器基底類別，實作責任鏈模式的通用邏輯。
/// </summary>
public abstract class CommandHandlerBase : ICommandHandler
{
    private ICommandHandler? _nextHandler;

    /// <inheritdoc />
    public void SetNext(ICommandHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        _nextHandler = handler;
    }

    /// <inheritdoc />
    public async Task<CommandResult> HandleAsync(
        CommandContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (CanHandle(context))
        {
            return await ExecuteAsync(context, cancellationToken);
        }

        return _nextHandler is not null
            ? await _nextHandler.HandleAsync(context, cancellationToken)
            : CommandResult.NotHandled();
    }

    /// <summary>
    /// 判斷此處理器是否能處理指定的指令。
    /// </summary>
    /// <param name="context">指令上下文。</param>
    /// <returns>若能處理回傳 true，否則回傳 false。</returns>
    protected abstract bool CanHandle(CommandContext context);

    /// <summary>
    /// 執行指令處理邏輯。
    /// </summary>
    /// <param name="context">指令上下文。</param>
    /// <param name="cancellationToken">取消權杖。</param>
    /// <returns>指令執行結果。</returns>
    protected abstract Task<CommandResult> ExecuteAsync(
        CommandContext context,
        CancellationToken cancellationToken);
}
