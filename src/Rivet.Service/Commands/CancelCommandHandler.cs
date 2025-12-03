using Rivet.Service.Commands;

namespace Rivet.Service.Commands;

/// <summary>
/// 取消指令處理器，處理使用者按下 Esc 鍵的情況
/// </summary>
public class CancelCommandHandler : CommandHandlerBase
{
    /// <inheritdoc />
    protected override bool CanHandle(CommandContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.CommandType == CommandType.Cancel;
    }

    /// <inheritdoc />
    protected override Task<CommandResult> ExecuteAsync(
        CommandContext context,
        CancellationToken cancellationToken = default)
    {
        // 取消操作不需要進行任何實際操作，直接回傳取消結果
        return Task.FromResult(CommandResult.Cancelled());
    }
}
