using NSubstitute;
using NUnit.Framework;
using Rivet.Service.Clipboard;
using Rivet.Service.Commands;
using Rivet.Service.Notification;
using Rivet.Service.Translation;

namespace Rivet.Console.Tests;

/// <summary>
/// 驗證當剪貼簿為圖片時的錯誤訊息顯示
/// </summary>
[TestFixture]
public class ImageClipboardTests
{
    private TranslateToChineseHandler _handlerChinese = null!;
    private TranslateToEnglishHandler _handlerEnglish = null!;
    private IClipboardService _clipboardService = null!;
    private ITranslationService _translationService = null!;
    private IUserNotifier _userNotifier = null!;

    [SetUp]
    public void SetUp()
    {
        _clipboardService = Substitute.For<IClipboardService>();
        _translationService = Substitute.For<ITranslationService>();
        _userNotifier = Substitute.For<IUserNotifier>();

        _handlerChinese = new TranslateToChineseHandler(
            _clipboardService,
            _translationService,
            _userNotifier
        );

        _handlerEnglish = new TranslateToEnglishHandler(
            _clipboardService,
            _translationService,
            _userNotifier
        );
    }

    [Test]
    public async Task TranslateToChineseHandler_WithImageClipboard_ShouldReturnNonTextError()
    {
        // Arrange
        _clipboardService.GetContent().Returns(ClipboardContent.NonText());

        var context = new CommandContext
        {
            CommandType = CommandType.TranslateToChinese,
            ClipboardContent = ClipboardContent.Empty()
        };

        // Act
        var result = await _handlerChinese.HandleAsync(context);

        // Assert
        Assert.That(result.IsHandled, Is.True, "結果應被標記為已處理");
        Assert.That(result.IsSuccess, Is.False, "結果應標記為失敗");
        Assert.That(result.Message, Is.EqualTo("無法翻譯非純文字"), "應返回非純文字的錯誤訊息");
    }

    [Test]
    public async Task TranslateToEnglishHandler_WithImageClipboard_ShouldReturnNonTextError()
    {
        // Arrange
        _clipboardService.GetContent().Returns(ClipboardContent.NonText());

        var context = new CommandContext
        {
            CommandType = CommandType.TranslateToEnglish,
            ClipboardContent = ClipboardContent.Empty()
        };

        // Act
        var result = await _handlerEnglish.HandleAsync(context);

        // Assert
        Assert.That(result.IsHandled, Is.True, "結果應被標記為已處理");
        Assert.That(result.IsSuccess, Is.False, "結果應標記為失敗");
        Assert.That(result.Message, Is.EqualTo("無法翻譯非純文字"), "應返回非純文字的錯誤訊息");
    }

    [Test]
    public async Task BothHandlers_WithImageClipboard_ShouldBehaveSame()
    {
        // Arrange
        _clipboardService.GetContent().Returns(ClipboardContent.NonText());

        var contextChinese = new CommandContext
        {
            CommandType = CommandType.TranslateToChinese,
            ClipboardContent = ClipboardContent.Empty()
        };

        var contextEnglish = new CommandContext
        {
            CommandType = CommandType.TranslateToEnglish,
            ClipboardContent = ClipboardContent.Empty()
        };

        // Act
        var resultChinese = await _handlerChinese.HandleAsync(contextChinese);
        var resultEnglish = await _handlerEnglish.HandleAsync(contextEnglish);

        // Assert
        Assert.That(resultChinese.IsHandled, Is.EqualTo(resultEnglish.IsHandled));
        Assert.That(resultChinese.IsSuccess, Is.EqualTo(resultEnglish.IsSuccess));
        Assert.That(resultChinese.Message, Is.EqualTo(resultEnglish.Message));
    }
}
