using NSubstitute;
using NUnit.Framework;
using Rivet.Service.Clipboard;
using Rivet.Service.Commands;
using Rivet.Service.Notification;
using Rivet.Service.Translation;

namespace Rivet.Service.Tests.Commands;

/// <summary>
/// TranslateToChineseHandler 的單元測試
/// 驗證英文翻譯成繁體中文的功能
/// </summary>
[TestFixture]
public class TranslateToChineseHandlerTests
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
    public void Constructor_WithNullClipboardService_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new TranslateToChineseHandler(null!, _translationService, _userNotifier)
        );
    }

    [Test]
    public void Constructor_WithNullTranslationService_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new TranslateToChineseHandler(_clipboardService, null!, _userNotifier)
        );
    }

    [Test]
    public void Constructor_WithNullUserNotifier_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new TranslateToChineseHandler(_clipboardService, _translationService, null!)
        );
    }

    [Test]
    public async Task HandleAsync_WithValidEnglishText_ShouldReturnSuccess()
    {
        var englishText = "Hello, world!";
        var chineseText = "你好，世界！";

        _clipboardService.GetContent().Returns(ClipboardContent.WithText(englishText));
        _translationService.TranslateAsync(Arg.Any<TranslationRequest>(), Arg.Any<CancellationToken>())
            .Returns(TranslationResult.Success(chineseText));

        var context = new CommandContext
        {
            CommandType = CommandType.TranslateToChinese,
            ClipboardContent = ClipboardContent.Empty()
        };

        var result = await _handler.HandleAsync(context);

        Assert.That(result.IsHandled, Is.True);
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Message, Is.EqualTo("翻譯完成，結果已複製到剪貼簿"));
        _clipboardService.Received(1).SetText(chineseText);
    }

    [Test]
    public async Task HandleAsync_WithValidEnglishText_ShouldPassCorrectTranslationRequest()
    {
        var englishText = "Hello, world!";
        var chineseText = "你好，世界！";

        _clipboardService.GetContent().Returns(ClipboardContent.WithText(englishText));
        _translationService.TranslateAsync(Arg.Any<TranslationRequest>(), Arg.Any<CancellationToken>())
            .Returns(TranslationResult.Success(chineseText));

        var context = new CommandContext
        {
            CommandType = CommandType.TranslateToChinese,
            ClipboardContent = ClipboardContent.Empty()
        };

        await _handler.HandleAsync(context);

        await _translationService.Received(1).TranslateAsync(
            Arg.Is<TranslationRequest>(r =>
                r.SourceText == englishText &&
                r.TargetLanguage == TargetLanguage.TraditionalChinese),
            Arg.Any<CancellationToken>()
        );
    }

    [Test]
    public async Task HandleAsync_WithEmptyClipboard_ShouldReturnError()
    {
        _clipboardService.GetContent().Returns(ClipboardContent.Empty());

        var context = new CommandContext
        {
            CommandType = CommandType.TranslateToChinese,
            ClipboardContent = ClipboardContent.Empty()
        };

        var result = await _handler.HandleAsync(context);

        Assert.That(result.IsHandled, Is.True);
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Message, Is.EqualTo("剪貼簿內容為空無法翻譯"));
        await _translationService.DidNotReceive().TranslateAsync(Arg.Any<TranslationRequest>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task HandleAsync_WithNonTextClipboard_ShouldReturnError()
    {
        _clipboardService.GetContent().Returns(ClipboardContent.NonText());

        var context = new CommandContext
        {
            CommandType = CommandType.TranslateToChinese,
            ClipboardContent = ClipboardContent.Empty()
        };

        var result = await _handler.HandleAsync(context);

        Assert.That(result.IsHandled, Is.True);
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Message, Is.EqualTo("無法翻譯非純文字"));
        await _translationService.DidNotReceive().TranslateAsync(Arg.Any<TranslationRequest>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task HandleAsync_WithTranslationFailure_ShouldReturnError()
    {
        var englishText = "Hello, world!";
        var errorMessage = "翻譯服務暫時無法使用，請稍後再試";

        _clipboardService.GetContent().Returns(ClipboardContent.WithText(englishText));
        _translationService.TranslateAsync(Arg.Any<TranslationRequest>(), Arg.Any<CancellationToken>())
            .Returns(TranslationResult.Failure(errorMessage));

        var context = new CommandContext
        {
            CommandType = CommandType.TranslateToChinese,
            ClipboardContent = ClipboardContent.Empty()
        };

        var result = await _handler.HandleAsync(context);

        Assert.That(result.IsHandled, Is.True);
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Message, Is.EqualTo(errorMessage));
        _clipboardService.DidNotReceive().SetText(Arg.Any<string>());
    }

    [Test]
    public async Task HandleAsync_WithTranslateToEnglish_ShouldNotHandle()
    {
        var context = new CommandContext
        {
            CommandType = CommandType.TranslateToEnglish,
            ClipboardContent = ClipboardContent.Empty()
        };

        var result = await _handler.HandleAsync(context);

        Assert.That(result.IsHandled, Is.False);
        _clipboardService.DidNotReceive().GetContent();
    }

    [Test]
    public async Task HandleAsync_WithMultipleHandlersInChain_ShouldWorkCorrectly()
    {
        var englishText = "Hello";
        var chineseText = "你好";

        _clipboardService.GetContent().Returns(ClipboardContent.WithText(englishText));
        _translationService.TranslateAsync(Arg.Any<TranslationRequest>(), Arg.Any<CancellationToken>())
            .Returns(TranslationResult.Success(chineseText));

        var cancelHandler = new CancelCommandHandler();
        _handler.SetNext(cancelHandler);

        var context = new CommandContext
        {
            CommandType = CommandType.TranslateToChinese,
            ClipboardContent = ClipboardContent.Empty()
        };

        var result = await _handler.HandleAsync(context);

        Assert.That(result.IsHandled, Is.True);
        Assert.That(result.IsSuccess, Is.True);
    }
}
