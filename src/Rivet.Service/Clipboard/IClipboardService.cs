using Rivet.Service.Clipboard;

namespace Rivet.Service.Clipboard;

/// <summary>
/// 剪貼簿服務介面，提供讀取與寫入剪貼簿的功能。
/// </summary>
public interface IClipboardService
{
    /// <summary>
    /// 取得剪貼簿目前的內容。
    /// </summary>
    /// <returns>剪貼簿內容，包含狀態與文字（如有）。</returns>
    ClipboardContent GetContent();

    /// <summary>
    /// 將文字寫入剪貼簿。
    /// </summary>
    /// <param name="text">要寫入的文字。</param>
    /// <exception cref="ArgumentNullException">若 text 為 null。</exception>
    void SetText(string text);
}
