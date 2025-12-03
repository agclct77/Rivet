namespace Rivet.Service.Clipboard;

/// <summary>
/// 剪貼簿內容記錄
/// 代表從系統剪貼簿讀取的資料
/// </summary>
public record ClipboardContent
{
    /// <summary>
    /// 剪貼簿狀態
    /// </summary>
    public required ClipboardStatus Status { get; init; }

    /// <summary>
    /// 純文字內容（僅 HasText 狀態有值）
    /// </summary>
    public string? Text { get; init; }

    /// <summary>
    /// 建立包含文字的剪貼簿內容
    /// </summary>
    /// <param name="text">文字內容</param>
    /// <returns>剪貼簿內容實體</returns>
    public static ClipboardContent WithText(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("文字內容不得為空或只含空白字元", nameof(text));

        return new ClipboardContent
        {
            Status = ClipboardStatus.HasText,
            Text = text
        };
    }

    /// <summary>
    /// 建立空白剪貼簿內容
    /// </summary>
    /// <returns>空白的剪貼簿內容實體</returns>
    public static ClipboardContent Empty()
    {
        return new ClipboardContent
        {
            Status = ClipboardStatus.Empty,
            Text = null
        };
    }

    /// <summary>
    /// 建立非純文字剪貼簿內容
    /// </summary>
    /// <returns>非純文字的剪貼簿內容實體</returns>
    public static ClipboardContent NonText()
    {
        return new ClipboardContent
        {
            Status = ClipboardStatus.NonText,
            Text = null
        };
    }
}
