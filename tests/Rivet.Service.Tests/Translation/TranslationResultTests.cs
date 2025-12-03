using Rivet.Service.Translation;

namespace Rivet.Service.Tests.Translation;

[TestFixture]
public class TranslationResultTests
{
    [Test]
    public void Success_ValidText_CreatesSuccessResult()
    {
        // Arrange
        string translatedText = "你好，世界！";

        // Act
        var result = TranslationResult.Success(translatedText);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.TranslatedText, Is.EqualTo(translatedText));
        Assert.That(result.ErrorMessage, Is.Null);
    }

    [Test]
    public void Success_NullText_ThrowsArgumentNullException()
    {
        // Arrange
        string? translatedText = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => TranslationResult.Success(translatedText!));
    }

    [Test]
    public void Success_EmptyText_ThrowsArgumentException()
    {
        // Arrange
        string translatedText = "";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => TranslationResult.Success(translatedText));
    }

    [Test]
    public void Success_WhitespaceText_ThrowsArgumentException()
    {
        // Arrange
        string translatedText = "   ";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => TranslationResult.Success(translatedText));
    }

    [Test]
    public void Failure_ValidErrorMessage_CreatesFailureResult()
    {
        // Arrange
        string errorMessage = "網路連線失敗";

        // Act
        var result = TranslationResult.Failure(errorMessage);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.TranslatedText, Is.Null);
        Assert.That(result.ErrorMessage, Is.EqualTo(errorMessage));
    }

    [Test]
    public void Failure_NullErrorMessage_ThrowsArgumentNullException()
    {
        // Arrange
        string? errorMessage = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => TranslationResult.Failure(errorMessage!));
    }

    [Test]
    public void Failure_EmptyErrorMessage_ThrowsArgumentException()
    {
        // Arrange
        string errorMessage = "";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => TranslationResult.Failure(errorMessage));
    }

    [Test]
    public void Failure_WhitespaceErrorMessage_ThrowsArgumentException()
    {
        // Arrange
        string errorMessage = "   ";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => TranslationResult.Failure(errorMessage));
    }

    [Test]
    public void TranslationResult_Record_IsImmutable()
    {
        // Arrange
        var result = TranslationResult.Success("Test");

        // Act & Assert
        var newResult = result with { TranslatedText = "Modified" };

        Assert.That(result.TranslatedText, Is.EqualTo("Test"));
        Assert.That(newResult.TranslatedText, Is.EqualTo("Modified"));
        Assert.That(result, Is.Not.EqualTo(newResult));
    }

    [Test]
    public void TranslationResult_SuccessEqualityByValue()
    {
        // Arrange
        var result1 = TranslationResult.Success("Test");
        var result2 = TranslationResult.Success("Test");

        // Act & Assert
        Assert.That(result1, Is.EqualTo(result2));
    }

    [Test]
    public void TranslationResult_FailureEqualityByValue()
    {
        // Arrange
        var result1 = TranslationResult.Failure("Error");
        var result2 = TranslationResult.Failure("Error");

        // Act & Assert
        Assert.That(result1, Is.EqualTo(result2));
    }

    [Test]
    public void TranslationResult_SuccessInequalityFromFailure()
    {
        // Arrange
        var success = TranslationResult.Success("Test");
        var failure = TranslationResult.Failure("Error");

        // Act & Assert
        Assert.That(success, Is.Not.EqualTo(failure));
    }
}
