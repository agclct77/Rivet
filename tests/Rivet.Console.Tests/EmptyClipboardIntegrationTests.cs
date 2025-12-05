using NSubstitute;
using NUnit.Framework;
using Rivet.Console.Notification;
using Rivet.Service.Clipboard;
using Rivet.Service.Commands;
using Rivet.Service.Notification;
using Rivet.Service.Translation;

namespace Rivet.Console.Tests;

/// <summary>
/// 整合測試：驗證空白剪貼簿時的錯誤訊息顯示
/// </summary>
[TestFixture]
public class EmptyClipboardIntegrationTests
{
    [Test]
    public async Task TranslateToChineseHandler_WithEmptyClipboard_ShouldReturnFailedWithMessage()
    {
        // Arrange
        var clipboardService = Substitute.For<IClipboardService>();
        var translationService = Substitute.For<ITranslationService>();
        var userNotifier = Substitute.For<IUserNotifier>();

        clipboardService.GetContent().Returns(ClipboardContent.Empty());

        var handler = new TranslateToChineseHandler(clipboardService, translationService, userNotifier);

        var context = new CommandContext
        {
            CommandType = CommandType.TranslateToChinese,
            ClipboardContent = ClipboardContent.Empty()
        };

        // Act
        var result = await handler.HandleAsync(context);

        // Assert
        Assert.That(result.IsHandled, Is.True, "結果應被標記為已處理");
        Assert.That(result.IsSuccess, Is.False, "結果應標記為失敗");
        Assert.That(result.Message, Is.EqualTo("剪貼簿內容為空無法翻譯"), "應返回正確的錯誤訊息");
    }

    [Test]
    public async Task TranslateToEnglishHandler_WithEmptyClipboard_ShouldReturnFailedWithMessage()
    {
        // Arrange
        var clipboardService = Substitute.For<IClipboardService>();
        var translationService = Substitute.For<ITranslationService>();
        var userNotifier = Substitute.For<IUserNotifier>();

        clipboardService.GetContent().Returns(ClipboardContent.Empty());

        var handler = new TranslateToEnglishHandler(clipboardService, translationService, userNotifier);

        var context = new CommandContext
        {
            CommandType = CommandType.TranslateToEnglish,
            ClipboardContent = ClipboardContent.Empty()
        };

        // Act
        var result = await handler.HandleAsync(context);

        // Assert
        Assert.That(result.IsHandled, Is.True, "結果應被標記為已處理");
        Assert.That(result.IsSuccess, Is.False, "結果應標記為失敗");
        Assert.That(result.Message, Is.EqualTo("剪貼簿內容為空無法翻譯"), "應返回正確的錯誤訊息");
    }
}
