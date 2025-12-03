using System.Text.Json;
using Rivet.Service.Configuration;

namespace Rivet.Infrastructure.Configuration;

/// <summary>
/// JSON 設定服務實作
/// 從 %AppData%\Rivet\config.json 讀取設定
/// </summary>
public class JsonConfigurationService : IConfigurationService
{
    private readonly string _configPath;

    /// <summary>
    /// 初始化 JSON 設定服務
    /// </summary>
    public JsonConfigurationService()
    {
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        _configPath = Path.Combine(appDataPath, "Rivet", "config.json");
    }

    /// <inheritdoc />
    public ConfigurationResult GetConfiguration()
    {
        try
        {
            if (!File.Exists(_configPath))
            {
                return ConfigurationResult.Failure(
                    $"找不到設定檔，請在以下路徑建立設定檔：{_configPath}");
            }

            string jsonContent = File.ReadAllText(_configPath);

            // 使用 JsonSerializer 反序列化
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var configData = JsonSerializer.Deserialize<ConfigurationData>(jsonContent, options);

            if (configData?.Services?.GoogleTranslation?.KeyFilePath == null)
            {
                return ConfigurationResult.Failure("設定檔格式錯誤，請檢查 JSON 格式");
            }

            string keyFilePath = configData.Services.GoogleTranslation.KeyFilePath;

            if (!File.Exists(keyFilePath))
            {
                return ConfigurationResult.Failure(
                    $"找不到翻譯服務金鑰檔：{keyFilePath}");
            }

            var config = new AppConfiguration
            {
                Services = new ServiceConfiguration
                {
                    GoogleTranslation = new GoogleTranslationConfig
                    {
                        KeyFilePath = keyFilePath
                    }
                }
            };

            return ConfigurationResult.Success(config);
        }
        catch (JsonException)
        {
            return ConfigurationResult.Failure("設定檔格式錯誤，請檢查 JSON 格式");
        }
        catch (Exception ex)
        {
            return ConfigurationResult.Failure($"讀取設定檔時發生錯誤：{ex.Message}");
        }
    }

    /// <summary>
    /// 內部用於 JSON 反序列化的資料類別
    /// </summary>
    private record ConfigurationData(
        ServiceConfigurationData? Services);

    private record ServiceConfigurationData(
        GoogleTranslationConfigData? GoogleTranslation);

    private record GoogleTranslationConfigData(
        string? KeyFilePath);
}
