using Rivet.Service.Clipboard;

namespace Rivet.Service.Tests.Clipboard;

[TestFixture]
public class ClipboardContentTests
{
    [Test]
    public void WithText_ValidText_CreatesClipboardContentWithStatus()
    {
        // Arrange
        string text = "Hello, World!";

        // Act
        var content = ClipboardContent.WithText(text);

        // Assert
        Assert.That(content.Status, Is.EqualTo(ClipboardStatus.HasText));
        Assert.That(content.Text, Is.EqualTo(text));
    }

    [Test]
    public void WithText_NullText_ThrowsArgumentNullException()
    {
        // Arrange
        string? text = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => ClipboardContent.WithText(text!));
    }

    [Test]
    public void WithText_EmptyText_ThrowsArgumentException()
    {
        // Arrange
        string text = "";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => ClipboardContent.WithText(text));
    }

    [Test]
    public void WithText_WhitespaceText_ThrowsArgumentException()
    {
        // Arrange
        string text = "   ";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => ClipboardContent.WithText(text));
    }

    [Test]
    public void Empty_CreatesEmptyClipboardContent()
    {
        // Act
        var content = ClipboardContent.Empty();

        // Assert
        Assert.That(content.Status, Is.EqualTo(ClipboardStatus.Empty));
        Assert.That(content.Text, Is.Null);
    }

    [Test]
    public void NonText_CreatesNonTextClipboardContent()
    {
        // Act
        var content = ClipboardContent.NonText();

        // Assert
        Assert.That(content.Status, Is.EqualTo(ClipboardStatus.NonText));
        Assert.That(content.Text, Is.Null);
    }

    [Test]
    public void ClipboardContent_Record_IsImmutable()
    {
        // Arrange
        var content = ClipboardContent.WithText("Test");

        // Act & Assert - 嘗試修改應該產生新物件
        var newContent = content with { Text = "Modified" };

        Assert.That(content.Text, Is.EqualTo("Test"));
        Assert.That(newContent.Text, Is.EqualTo("Modified"));
        Assert.That(content, Is.Not.EqualTo(newContent));
    }

    [Test]
    public void ClipboardContent_EqualityByValue()
    {
        // Arrange
        var content1 = ClipboardContent.WithText("Test");
        var content2 = ClipboardContent.WithText("Test");

        // Act & Assert
        Assert.That(content1, Is.EqualTo(content2));
    }

    [Test]
    public void ClipboardContent_InequalityByValue()
    {
        // Arrange
        var content1 = ClipboardContent.WithText("Test1");
        var content2 = ClipboardContent.WithText("Test2");

        // Act & Assert
        Assert.That(content1, Is.Not.EqualTo(content2));
    }
}
