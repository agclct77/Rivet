using Rivet.Service.Clipboard;
using Rivet.Service.Notification;
using Rivet.Service.Translation;

namespace Rivet.Service.Commands;

/// <summary>
/// 翻譯成繁體中文的指令處理器
/// 責任鏈中的一個具體處理器，負責將剪貼簿英文內容翻譯成繁體中文
/// </summary>
public class TranslateToChineseHandler : CommandHandlerBase
{
    private readonly IClipboardService _clipboardService;
    private readonly ITranslationService _translationService;
    private readonly IUserNotifier _userNotifier;

    /// <summary>
    /// 初始化 TranslateToChineseHandler 實體
    /// </summary>
    /// <param name="clipboardService">剪貼簿服務</param>
    /// <param name="translationService">翻譯服務</param>
    /// <param name="userNotifier">使用者通知器</param>
    public TranslateToChineseHandler(
        IClipboardService clipboardService,
        ITranslationService translationService,
        IUserNotifier userNotifier)
    {
        ArgumentNullException.ThrowIfNull(clipboardService);
        ArgumentNullException.ThrowIfNull(translationService);
        ArgumentNullException.ThrowIfNull(userNotifier);

        _clipboardService = clipboardService;
        _translationService = translationService;
        _userNotifier = userNotifier;
    }

    /// <summary>
    /// 判斷是否能處理此指令
    /// 僅當指令類型為 TranslateToChinese 時才能處理
    /// </summary>
    /// <param name="context">指令上下文</param>
    /// <returns>是否能處理</returns>
    protected override bool CanHandle(CommandContext context)
    {
        return context.CommandType == CommandType.TranslateToChinese;
    }

    /// <summary>
    /// 執行翻譯成繁體中文的操作
    /// 流程：讀取剪貼簿 → 驗證內容 → 翻譯 → 寫入剪貼簿 → 顯示結果訊息
    /// </summary>
    /// <param name="context">指令上下文</param>
    /// <param name="cancellationToken">取消權杖</param>
    /// <returns>執行結果</returns>
    protected override async Task<CommandResult> ExecuteAsync(
        CommandContext context,
        CancellationToken cancellationToken)
    {
        // 保存原始剪貼簿內容，以便在翻譯失敗時恢復
        var originalContent = _clipboardService.GetContent();

        try
        {
            // 1. 讀取剪貼簿內容
            var clipboardContent = originalContent;

            // 2. 驗證剪貼簿內容
            // 檢查非純文字內容
            if (clipboardContent.Status == ClipboardStatus.NonText)
            {
                return CommandResult.Failed("無法翻譯非純文字");
            }

            // 檢查空白剪貼簿
            if (clipboardContent.Status == ClipboardStatus.Empty)
            {
                return CommandResult.Failed("剪貼簿內容為空無法翻譯");
            }

            // 3. 翻譯文字
            var translationRequest = new TranslationRequest
            {
                SourceText = clipboardContent.Text!,
                TargetLanguage = TargetLanguage.TraditionalChinese
            };

            var translationResult = await _translationService.TranslateAsync(
                translationRequest,
                cancellationToken);

            // 4. 檢查翻譯結果
            if (!translationResult.IsSuccess)
            {
                // 翻譯失敗時，保留原始剪貼簿內容
                return CommandResult.Failed(translationResult.ErrorMessage!);
            }

            // 5. 寫入剪貼簿
            _clipboardService.SetText(translationResult.TranslatedText!);

            // 6. 顯示成功訊息並回傳
            return CommandResult.Handled("翻譯完成，結果已複製到剪貼簿");
        }
        catch (Exception ex)
        {
            // 捕捉未預期的錯誤時，保留原始剪貼簿內容
            return CommandResult.Failed($"翻譯時發生錯誤：{ex.Message}");
        }
    }
}
