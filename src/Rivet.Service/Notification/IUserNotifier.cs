namespace Rivet.Service.Notification;

/// <summary>
/// 使用者通知器介面，提供訊息顯示功能。
/// </summary>
public interface IUserNotifier
{
    /// <summary>
    /// 顯示成功訊息。
    /// </summary>
    /// <param name="message">訊息內容。</param>
    void ShowSuccess(string message);

    /// <summary>
    /// 顯示錯誤訊息。
    /// </summary>
    /// <param name="message">訊息內容。</param>
    void ShowError(string message);

    /// <summary>
    /// 顯示資訊訊息。
    /// </summary>
    /// <param name="message">訊息內容。</param>
    void ShowInfo(string message);
}
