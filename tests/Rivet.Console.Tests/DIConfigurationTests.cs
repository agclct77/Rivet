using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Rivet.Infrastructure.Configuration;
using Rivet.Infrastructure.Translation;
using Rivet.Service.Configuration;
using Rivet.Service.Translation;

namespace Rivet.Console.Tests;

/// <summary>
/// 驗證依賴注入配置的測試
/// </summary>
[TestFixture]
public class DIConfigurationTests
{
    [Test]
    public void DIContainer_ShouldResolveAllRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // 手動配置服務（與 Program.cs 中的 ConfigureServices 相同）
        services.AddSingleton<IConfigurationService, JsonConfigurationService>();
        services.AddSingleton<ITranslationService>(provider =>
        {
            var configService = provider.GetRequiredService<IConfigurationService>();
            var configResult = configService.GetConfiguration();

            if (!configResult.IsSuccess)
            {
                return new GoogleTranslationService(string.Empty);
            }

            return new GoogleTranslationService(configResult.Configuration!.Services.GoogleTranslation.KeyFilePath);
        });

        var serviceProvider = services.BuildServiceProvider();

        // Act & Assert - 應該能夠解析所有服務而不拋出異常
        Assert.DoesNotThrow(() =>
        {
            var configService = serviceProvider.GetRequiredService<IConfigurationService>();
            Assert.That(configService, Is.Not.Null);
            Assert.That(configService, Is.InstanceOf<JsonConfigurationService>());

            var translationService = serviceProvider.GetRequiredService<ITranslationService>();
            Assert.That(translationService, Is.Not.Null);
            Assert.That(translationService, Is.InstanceOf<GoogleTranslationService>());
        });
    }

    [Test]
    public void DIContainer_TranslationServiceFactory_ShouldHandleConfigurationFailure()
    {
        // Arrange
        var services = new ServiceCollection();

        // 使用虛擬配置服務（會失敗）
        var mockConfigService = new FailingConfigurationService();
        services.AddSingleton<IConfigurationService>(mockConfigService);
        services.AddSingleton<ITranslationService>(provider =>
        {
            var configService = provider.GetRequiredService<IConfigurationService>();
            var configResult = configService.GetConfiguration();

            if (!configResult.IsSuccess)
            {
                return new GoogleTranslationService(string.Empty);
            }

            return new GoogleTranslationService(configResult.Configuration!.Services.GoogleTranslation.KeyFilePath);
        });

        var serviceProvider = services.BuildServiceProvider();

        // Act & Assert - 即使配置失敗，也應該能夠創建服務
        Assert.DoesNotThrow(() =>
        {
            var translationService = serviceProvider.GetRequiredService<ITranslationService>();
            Assert.That(translationService, Is.Not.Null);
            Assert.That(translationService, Is.InstanceOf<GoogleTranslationService>());
        });
    }
}

/// <summary>
/// 用於測試的配置服務實現（會失敗）
/// </summary>
public class FailingConfigurationService : IConfigurationService
{
    public ConfigurationResult GetConfiguration()
    {
        return ConfigurationResult.Failure("測試：無法讀取設定檔");
    }
}
