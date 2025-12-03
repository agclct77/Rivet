namespace Rivet.Service.Configuration;

/// <summary>
/// 設定服務介面，提供應用程式設定讀取功能。
/// </summary>
public interface IConfigurationService
{
    /// <summary>
    /// 取得應用程式設定。
    /// </summary>
    /// <returns>設定結果，包含成功狀態與設定內容或錯誤訊息。</returns>
    ConfigurationResult GetConfiguration();
}
