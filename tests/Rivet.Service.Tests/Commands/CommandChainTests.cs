using NSubstitute;
using NUnit.Framework;
using Rivet.Service.Clipboard;
using Rivet.Service.Commands;

namespace Rivet.Service.Tests.Commands;

/// <summary>
/// CommandChain 責任鏈管理器的單元測試
/// </summary>
[TestFixture]
public class CommandChainTests
{
    private CommandChain _commandChain = null!;

    [SetUp]
    public void SetUp()
    {
        _commandChain = new CommandChain();
    }

    [Test]
    public void AddHandler_WithValidHandler_ShouldAddHandler()
    {
        // Arrange
        var handler = new CancelCommandHandler();

        // Act
        _commandChain.AddHandler(handler);

        // Assert
        // 若沒有拋出例外，代表成功添加
        Assert.Pass();
    }

    [Test]
    public void AddHandler_WithNullHandler_ShouldThrowArgumentNullException()
    {
        // Arrange
        ICommandHandler? handler = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _commandChain.AddHandler(handler!));
    }

    [Test]
    public void ExecuteAsync_WithNoHandlers_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var context = new CommandContext
        {
            CommandType = CommandType.Cancel,
            ClipboardContent = ClipboardContent.Empty()
        };

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _commandChain.ExecuteAsync(context));
    }

    [Test]
    public async Task ExecuteAsync_WithCancelCommand_ShouldHandleWithCancelHandler()
    {
        // Arrange
        var cancelHandler = new CancelCommandHandler();
        _commandChain.AddHandler(cancelHandler);

        var context = new CommandContext
        {
            CommandType = CommandType.Cancel,
            ClipboardContent = ClipboardContent.Empty()
        };

        // Act
        var result = await _commandChain.ExecuteAsync(context);

        // Assert
        Assert.That(result.IsHandled, Is.True);
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Message, Is.Null);
    }

    [Test]
    public async Task ExecuteAsync_WithUnhandledCommand_ShouldReturnNotHandled()
    {
        // Arrange
        var cancelHandler = new CancelCommandHandler();
        _commandChain.AddHandler(cancelHandler);

        var context = new CommandContext
        {
            CommandType = CommandType.TranslateToChinese,
            ClipboardContent = ClipboardContent.Empty()
        };

        // Act
        var result = await _commandChain.ExecuteAsync(context);

        // Assert
        Assert.That(result.IsHandled, Is.False);
    }

    [Test]
    public async Task ExecuteAsync_WithMultipleHandlers_ShouldChainProperly()
    {
        // Arrange
        var handler1 = new CancelCommandHandler();
        var handler2 = new CancelCommandHandler();

        _commandChain.AddHandler(handler1);
        _commandChain.AddHandler(handler2);

        var context = new CommandContext
        {
            CommandType = CommandType.Cancel,
            ClipboardContent = ClipboardContent.Empty()
        };

        // Act
        var result = await _commandChain.ExecuteAsync(context);

        // Assert - 應該由 handler1 處理，因為它先被添加
        Assert.That(result.IsHandled, Is.True);
        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public void ExecuteAsync_WithNullContext_ShouldThrowArgumentNullException()
    {
        // Arrange
        var handler = new CancelCommandHandler();
        _commandChain.AddHandler(handler);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _commandChain.ExecuteAsync(null!));
    }
}
