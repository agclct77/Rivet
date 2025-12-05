using NSubstitute;
using NUnit.Framework;
using Rivet.Service.Clipboard;
using Rivet.Service.Commands;
using Rivet.Service.Notification;
using Rivet.Service.Translation;

namespace Rivet.Service.Tests.Commands;

/// <summary>
/// 驗證空白和只含空白字符剪貼簿的完整場景
/// </summary>
[TestFixture]
public class EmptyAndWhitespaceClipboardTests
{
    private TranslateToChineseHandler _handler = null!;
    private IClipboardService _clipboardService = null!;
    private ITranslationService _translationService = null!;
    private IUserNotifier _userNotifier = null!;

    [SetUp]
    public void SetUp()
    {
        _clipboardService = Substitute.For<IClipboardService>();
        _translationService = Substitute.For<ITranslationService>();
        _userNotifier = Substitute.For<IUserNotifier>();

        _handler = new TranslateToChineseHandler(
            _clipboardService,
            _translationService,
            _userNotifier
        );
    }

    [Test]
    [TestCase("")]
    [TestCase(" ")]
    [TestCase("\t")]
    [TestCase("   \t\n  ")]
    public async Task HandleAsync_WithEmptyOrWhitespaceClipboard_ShouldReturnErrorMessage(string content)
    {
        // Arrange
        var clipboardContent = string.IsNullOrWhiteSpace(content)
            ? ClipboardContent.Empty()
            : ClipboardContent.WithText(content);

        _clipboardService.GetContent().Returns(clipboardContent);

        var context = new CommandContext
        {
            CommandType = CommandType.TranslateToChinese,
            ClipboardContent = ClipboardContent.Empty()
        };

        // Act
        var result = await _handler.HandleAsync(context);

        // Assert
        Assert.That(result.IsHandled, Is.True);
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Message, Is.EqualTo("剪貼簿內容為空無法翻譯"));
    }

    [Test]
    public async Task HandleAsync_WithValidText_ShouldNotReturnEmptyError()
    {
        // Arrange
        _clipboardService.GetContent().Returns(ClipboardContent.WithText("Hello"));
        _translationService.TranslateAsync(Arg.Any<TranslationRequest>(), Arg.Any<CancellationToken>())
            .Returns(TranslationResult.Success("你好"));

        var context = new CommandContext
        {
            CommandType = CommandType.TranslateToChinese,
            ClipboardContent = ClipboardContent.Empty()
        };

        // Act
        var result = await _handler.HandleAsync(context);

        // Assert
        Assert.That(result.IsHandled, Is.True);
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Message, Is.EqualTo("翻譯完成，結果已複製到剪貼簿"));
    }
}
