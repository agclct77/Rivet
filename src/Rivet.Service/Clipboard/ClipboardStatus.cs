namespace Rivet.Service.Clipboard;

/// <summary>
/// 剪貼簿的狀態列舉
/// </summary>
public enum ClipboardStatus
{
    /// <summary>
    /// 包含有效的純文字內容
    /// </summary>
    HasText = 0,

    /// <summary>
    /// CF_UNICODETEXT 為空字串
    /// </summary>
    Empty = 1,

    /// <summary>
    /// 非純文字格式（如圖片）
    /// </summary>
    NonText = 2
}
