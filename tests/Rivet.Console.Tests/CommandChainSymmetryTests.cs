using NSubstitute;
using NUnit.Framework;
using Rivet.Console.Notification;
using Rivet.Service.Clipboard;
using Rivet.Service.Commands;
using Rivet.Service.Notification;
using Rivet.Service.Translation;

namespace Rivet.Console.Tests;

/// <summary>
/// 驗證命令鏈中翻譯成中文和翻譯成英文在空白剪貼簿時的行為對稱性
/// </summary>
[TestFixture]
public class CommandChainSymmetryTests
{
    private CommandChain _commandChain = null!;
    private IClipboardService _clipboardService = null!;
    private ITranslationService _translationService = null!;
    private IUserNotifier _userNotifier = null!;

    [SetUp]
    public void SetUp()
    {
        _clipboardService = Substitute.For<IClipboardService>();
        _translationService = Substitute.For<ITranslationService>();
        _userNotifier = Substitute.For<IUserNotifier>();

        _commandChain = new CommandChain();

        // 與 Program.cs 中的順序相同
        _commandChain.AddHandler(new CancelCommandHandler());
        _commandChain.AddHandler(new TranslateToChineseHandler(
            _clipboardService,
            _translationService,
            _userNotifier
        ));
        _commandChain.AddHandler(new TranslateToEnglishHandler(
            _clipboardService,
            _translationService,
            _userNotifier
        ));
    }

    [Test]
    public async Task ExecuteAsync_TranslateToChinese_WithEmptyClipboard_ShouldReturnError()
    {
        // Arrange
        _clipboardService.GetContent().Returns(ClipboardContent.Empty());

        var context = new CommandContext
        {
            CommandType = CommandType.TranslateToChinese,
            ClipboardContent = ClipboardContent.Empty()
        };

        // Act
        var result = await _commandChain.ExecuteAsync(context);

        // Assert
        Assert.That(result.IsHandled, Is.True, "結果應被標記為已處理");
        Assert.That(result.IsSuccess, Is.False, "結果應標記為失敗");
        Assert.That(result.Message, Is.EqualTo("剪貼簿內容為空無法翻譯"),
            "應返回空白剪貼簿的錯誤訊息");
    }

    [Test]
    public async Task ExecuteAsync_TranslateToEnglish_WithEmptyClipboard_ShouldReturnError()
    {
        // Arrange
        _clipboardService.GetContent().Returns(ClipboardContent.Empty());

        var context = new CommandContext
        {
            CommandType = CommandType.TranslateToEnglish,
            ClipboardContent = ClipboardContent.Empty()
        };

        // Act
        var result = await _commandChain.ExecuteAsync(context);

        // Assert
        Assert.That(result.IsHandled, Is.True, "結果應被標記為已處理");
        Assert.That(result.IsSuccess, Is.False, "結果應標記為失敗");
        Assert.That(result.Message, Is.EqualTo("剪貼簿內容為空無法翻譯"),
            "應返回空白剪貼簿的錯誤訊息");
    }

    [Test]
    public async Task ExecuteAsync_BothHandlers_ShouldBehaveSame_WhenClipboardEmpty()
    {
        // Arrange
        _clipboardService.GetContent().Returns(ClipboardContent.Empty());

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
        var resultChinese = await _commandChain.ExecuteAsync(contextChinese);
        var resultEnglish = await _commandChain.ExecuteAsync(contextEnglish);

        // Assert - 兩者應該有相同的行為
        Assert.That(resultChinese.IsHandled, Is.EqualTo(resultEnglish.IsHandled),
            "IsHandled 應該相同");
        Assert.That(resultChinese.IsSuccess, Is.EqualTo(resultEnglish.IsSuccess),
            "IsSuccess 應該相同");
        Assert.That(resultChinese.Message, Is.EqualTo(resultEnglish.Message),
            "Message 應該相同");
    }
}
