using Google.Cloud.Translation.V2;
using Rivet.Service.Translation;
using ServiceTranslationResult = Rivet.Service.Translation.TranslationResult;

namespace Rivet.Infrastructure.Translation;

/// <summary>
/// Google 翻譯服務實作
/// 使用 Google Cloud Translation API 進行翻譯
/// </summary>
public class GoogleTranslationService : ITranslationService
{
    private readonly string _keyFilePath;
    private TranslationClient? _client;

    /// <summary>
    /// 初始化 Google 翻譯服務
    /// </summary>
    /// <param name="keyFilePath">Google Cloud 服務帳戶金鑰檔路徑</param>
    /// <exception cref="ArgumentNullException">若 keyFilePath 為 null</exception>
    public GoogleTranslationService(string keyFilePath)
    {
        ArgumentNullException.ThrowIfNull(keyFilePath);
        _keyFilePath = keyFilePath;
    }

    /// <inheritdoc />
    public async Task<ServiceTranslationResult> TranslateAsync(
        TranslationRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            // 驗證來源文字
            if (string.IsNullOrWhiteSpace(request.SourceText))
            {
                return ServiceTranslationResult.Failure("來源文字不得為空");
            }

            if (request.SourceText.Length > 5000)
            {
                return ServiceTranslationResult.Failure("翻譯字元數量不得超過5000");
            }

            // 初始化客戶端（如果未初始化）
            if (_client == null)
            {
                try
                {
                    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", _keyFilePath);
                    _client = TranslationClient.Create();
                }
                catch (Exception ex) when (ex.Message.Contains("GOOGLE_APPLICATION_CREDENTIALS"))
                {
                    return ServiceTranslationResult.Failure(
                        $"翻譯服務認證失敗，請檢查金鑰設定：{ex.Message}");
                }
                catch (Exception ex) when (ex.Message.Contains("not found"))
                {
                    return ServiceTranslationResult.Failure(
                        $"找不到翻譯服務金鑰檔：{_keyFilePath}");
                }
                catch (Exception ex)
                {
                    return ServiceTranslationResult.Failure(
                        $"無法初始化翻譯服務：{ex.Message}");
                }
            }

            // 取得目標語言代碼
            string targetLanguageCode = GetLanguageCode(request.TargetLanguage);

            // 進行翻譯
            try
            {
                var response = _client.TranslateText(request.SourceText, targetLanguageCode);

                if (response == null || string.IsNullOrEmpty(response.TranslatedText))
                {
                    return ServiceTranslationResult.Failure("翻譯服務返回空結果");
                }

                return ServiceTranslationResult.Success(response.TranslatedText);
            }
            catch (Exception ex) when (ex.Message.Contains("429") || ex.Message.Contains("quota"))
            {
                return ServiceTranslationResult.Failure(
                    "翻譯服務超過配額限制，請稍後再試");
            }
            catch (Exception ex) when (ex.Message.Contains("401") || ex.Message.Contains("403"))
            {
                return ServiceTranslationResult.Failure(
                    "翻譯服務認證失敗，請檢查金鑰設定");
            }
            catch (Exception ex) when (ex.Message.Contains("503"))
            {
                return ServiceTranslationResult.Failure(
                    "翻譯服務暫時無法使用，請稍後再試");
            }
            catch (Exception ex) when (ex is HttpRequestException || ex.InnerException is HttpRequestException)
            {
                return ServiceTranslationResult.Failure(
                    "無法連線到翻譯服務，請檢查網路連線");
            }
            catch (Exception ex)
            {
                return ServiceTranslationResult.Failure(
                    $"翻譯時發生錯誤：{ex.Message}");
            }
        }
        catch (Exception ex)
        {
            return ServiceTranslationResult.Failure(
                $"翻譯時發生未預期的錯誤：{ex.Message}");
        }
    }

    /// <summary>
    /// 將 TargetLanguage 列舉轉換為 Google API 語言代碼
    /// </summary>
    /// <param name="targetLanguage">目標語言</param>
    /// <returns>語言代碼</returns>
    private static string GetLanguageCode(TargetLanguage targetLanguage)
    {
        return targetLanguage switch
        {
            TargetLanguage.TraditionalChinese => "zh-TW",
            TargetLanguage.English => "en",
            _ => throw new ArgumentException($"不支援的目標語言：{targetLanguage}", nameof(targetLanguage))
        };
    }
}
