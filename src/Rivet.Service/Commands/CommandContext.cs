using Rivet.Service.Clipboard;

namespace Rivet.Service.Commands;

/// <summary>
/// 指令上下文記錄
/// 傳遞給責任鏈處理器的上下文資訊
/// </summary>
public record CommandContext
{
    /// <summary>
    /// 指令類型
    /// </summary>
    public required CommandType CommandType { get; init; }

    /// <summary>
    /// 剪貼簿內容
    /// </summary>
    public required ClipboardContent ClipboardContent { get; init; }
}
