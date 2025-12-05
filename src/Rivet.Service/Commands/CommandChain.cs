using Rivet.Service.Commands;

namespace Rivet.Service.Commands;

/// <summary>
/// 指令責任鏈管理器，負責協調多個指令處理器的執行
/// </summary>
public class CommandChain
{
    private ICommandHandler? _firstHandler;

    /// <summary>
    /// 將處理器加入責任鏈
    /// </summary>
    /// <param name="handler">要加入的處理器</param>
    /// <exception cref="ArgumentNullException">handler 為 null</exception>
    public void AddHandler(ICommandHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        if (_firstHandler is null)
        {
            _firstHandler = handler;
        }
        else
        {
            // 找到鏈的末端並加入新處理器
            AppendToChain(_firstHandler, handler);
        }
    }

    /// <summary>
    /// 執行指令
    /// </summary>
    /// <param name="context">指令上下文</param>
    /// <param name="cancellationToken">取消權杖</param>
    /// <returns>指令執行結果</returns>
    /// <exception cref="InvalidOperationException">沒有註冊任何處理器</exception>
    public async Task<CommandResult> ExecuteAsync(
        CommandContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (_firstHandler is null)
            throw new InvalidOperationException("責任鏈中沒有任何處理器");

        return await _firstHandler.HandleAsync(context, cancellationToken);
    }

    /// <summary>
    /// 遞迴地將處理器附加到鏈的末端
    /// </summary>
    private static void AppendToChain(ICommandHandler current, ICommandHandler handler)
    {
        // 使用反射獲取 _nextHandler 欄位，以判斷是否已有下一個處理器
        var nextHandlerField = current.GetType().BaseType?.GetField(
            "_nextHandler",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
        );

        if (nextHandlerField == null)
        {
            // 無法通過反射存取，直接使用 SetNext
            current.SetNext(handler);
            return;
        }

        var nextHandler = (ICommandHandler?)nextHandlerField.GetValue(current);

        if (nextHandler == null)
        {
            // 找到末端，設置新處理器
            current.SetNext(handler);
        }
        else
        {
            // 繼續遞迴找末端
            AppendToChain(nextHandler, handler);
        }
    }
}
