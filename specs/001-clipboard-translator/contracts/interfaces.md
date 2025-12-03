# 介面合約：剪貼簿翻譯工具

**Branch**: `001-clipboard-translator`  
**Date**: 2024-12-04  
**Status**: 已完成

## 概述

本文件定義 Rivet.Service 層的核心介面，這些介面由 Rivet.Infrastructure 實作。
遵循依賴反轉原則（DIP），高階模組不依賴低階模組，兩者都依賴抽象。

---

## 1. IClipboardService（剪貼簿服務）

負責與系統剪貼簿互動。

### 介面定義

```csharp
namespace Rivet.Service.Clipboard;

/// <summary>
/// 剪貼簿服務介面，提供讀取與寫入剪貼簿的功能。
/// </summary>
public interface IClipboardService
{
    /// <summary>
    /// 取得剪貼簿目前的內容。
    /// </summary>
    /// <returns>剪貼簿內容，包含狀態與文字（如有）。</returns>
    ClipboardContent GetContent();
    
    /// <summary>
    /// 將文字寫入剪貼簿。
    /// </summary>
    /// <param name="text">要寫入的文字。</param>
    void SetText(string text);
}
```

### 行為規範

| 方法 | 前置條件 | 後置條件 | 例外處理 |
|------|----------|----------|----------|
| GetContent | 無 | 回傳有效的 ClipboardContent | 存取失敗時回傳 NonText 狀態 |
| SetText | text 不為 null | 剪貼簿內容更新為指定文字 | 若 text 為 null 拋出 ArgumentNullException |

---

## 2. ITranslationService（翻譯服務）

負責文字翻譯功能。

### 介面定義

```csharp
namespace Rivet.Service.Translation;

/// <summary>
/// 翻譯服務介面，提供文字翻譯功能。
/// </summary>
public interface ITranslationService
{
    /// <summary>
    /// 將文字翻譯成指定語言。
    /// </summary>
    /// <param name="request">翻譯請求，包含來源文字與目標語言。</param>
    /// <param name="cancellationToken">取消權杖。</param>
    /// <returns>翻譯結果，包含成功狀態與翻譯文字或錯誤訊息。</returns>
    Task<TranslationResult> TranslateAsync(
        TranslationRequest request, 
        CancellationToken cancellationToken = default);
}
```

### 行為規範

| 方法 | 前置條件 | 後置條件 | 例外處理 |
|------|----------|----------|----------|
| TranslateAsync | request 不為 null，SourceText 不為空白 | 回傳有效的 TranslationResult | 網路錯誤、API 錯誤回傳 Failure 結果 |

### 錯誤訊息對照

| 錯誤情境 | ErrorMessage |
|----------|--------------|
| 網路連線失敗 | 無法連線到翻譯服務，請檢查網路連線 |
| API 認證失敗 | 翻譯服務認證失敗，請檢查金鑰設定 |
| 超過字元限制 | 翻譯字元數量不得超過5000 |
| 服務暫時不可用 | 翻譯服務暫時無法使用，請稍後再試 |
| 未知錯誤 | 翻譯時發生錯誤：{技術細節} |

---

## 3. IConfigurationService（設定服務）

負責讀取應用程式設定。

### 介面定義

```csharp
namespace Rivet.Service.Configuration;

/// <summary>
/// 設定服務介面，提供應用程式設定讀取功能。
/// </summary>
public interface IConfigurationService
{
    /// <summary>
    /// 取得應用程式設定。
    /// </summary>
    /// <returns>設定結果，包含成功狀態與設定內容或錯誤訊息。</returns>
    ConfigurationResult GetConfiguration();
}

// ConfigurationResult 定義請參考 data-model.md 第 6 節
```

### 行為規範

| 方法 | 前置條件 | 後置條件 | 例外處理 |
|------|----------|----------|----------|
| GetConfiguration | 無 | 回傳有效的 ConfigurationResult | 設定檔不存在或格式錯誤回傳 Failure 結果 |

### 錯誤訊息對照

| 錯誤情境 | ErrorMessage |
|----------|--------------|
| 設定檔不存在 | 找不到設定檔，請在以下路徑建立設定檔：{path} |
| 設定檔格式錯誤 | 設定檔格式錯誤，請檢查 JSON 格式 |
| 金鑰檔不存在 | 找不到翻譯服務金鑰檔：{path} |

---

## 4. ICommandHandler（指令處理器）

責任鏈模式的處理器介面。

### 介面定義

```csharp
namespace Rivet.Service.Commands;

/// <summary>
/// 指令處理器介面，實作責任鏈模式。
/// </summary>
public interface ICommandHandler
{
    /// <summary>
    /// 設定下一個處理器。
    /// </summary>
    /// <param name="handler">下一個處理器。</param>
    void SetNext(ICommandHandler handler);
    
    /// <summary>
    /// 處理指令。
    /// </summary>
    /// <param name="context">指令上下文。</param>
    /// <param name="cancellationToken">取消權杖。</param>
    /// <returns>若已處理回傳 true，否則回傳 false 並傳遞給下一個處理器。</returns>
    Task<CommandResult> HandleAsync(
        CommandContext context, 
        CancellationToken cancellationToken = default);
}

/// <summary>
/// 指令執行結果。
/// </summary>
public record CommandResult
{
    public bool IsHandled { get; init; }
    public bool IsSuccess { get; init; }
    public string? Message { get; init; }
    
    public static CommandResult Handled(bool success, string message)
        => new() { IsHandled = true, IsSuccess = success, Message = message };
    
    public static CommandResult NotHandled()
        => new() { IsHandled = false };
    
    public static CommandResult Cancelled()
        => new() { IsHandled = true, IsSuccess = true, Message = null };
}
```

### 行為規範

| 方法 | 前置條件 | 後置條件 | 例外處理 |
|------|----------|----------|----------|
| SetNext | handler 不為 null | 設定下一個處理器 | 若 handler 為 null 拋出 ArgumentNullException |
| HandleAsync | context 不為 null | 回傳處理結果 | 處理過程中的錯誤包裝在 CommandResult 中回傳 |

---

## 4.1 CommandHandlerBase（指令處理器基底類別）

責任鏈模式的抽象基底類別，提供通用的鏈結邏輯。

### 類別定義

```csharp
namespace Rivet.Service.Commands;

/// <summary>
/// 指令處理器基底類別，實作責任鏈模式的通用邏輯。
/// </summary>
public abstract class CommandHandlerBase : ICommandHandler
{
    private ICommandHandler? _nextHandler;
    
    /// <inheritdoc />
    public void SetNext(ICommandHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        _nextHandler = handler;
    }
    
    /// <inheritdoc />
    public async Task<CommandResult> HandleAsync(
        CommandContext context, 
        CancellationToken cancellationToken = default)
    {
        if (CanHandle(context))
        {
            return await ExecuteAsync(context, cancellationToken);
        }
        
        return _nextHandler is not null 
            ? await _nextHandler.HandleAsync(context, cancellationToken)
            : CommandResult.NotHandled();
    }
    
    /// <summary>
    /// 判斷此處理器是否能處理指定的指令。
    /// </summary>
    /// <param name="context">指令上下文。</param>
    /// <returns>若能處理回傳 true，否則回傳 false。</returns>
    protected abstract bool CanHandle(CommandContext context);
    
    /// <summary>
    /// 執行指令處理邏輯。
    /// </summary>
    /// <param name="context">指令上下文。</param>
    /// <param name="cancellationToken">取消權杖。</param>
    /// <returns>指令執行結果。</returns>
    protected abstract Task<CommandResult> ExecuteAsync(
        CommandContext context, 
        CancellationToken cancellationToken);
}
```

### 行為規範

| 方法 | 前置條件 | 後置條件 | 例外處理 |
|------|----------|----------|----------|
| CanHandle | context 不為 null | 回傳是否能處理 | 不應拋出例外 |
| ExecuteAsync | CanHandle 回傳 true | 回傳處理結果 | 錯誤包裝在 CommandResult 中 |

---

## 5. IUserNotifier（使用者通知器）

負責向使用者顯示訊息。

### 介面定義

```csharp
namespace Rivet.Service.Notification;

/// <summary>
/// 使用者通知器介面，提供訊息顯示功能。
/// </summary>
public interface IUserNotifier
{
    /// <summary>
    /// 顯示成功訊息。
    /// </summary>
    /// <param name="message">訊息內容。</param>
    void ShowSuccess(string message);
    
    /// <summary>
    /// 顯示錯誤訊息。
    /// </summary>
    /// <param name="message">訊息內容。</param>
    void ShowError(string message);
    
    /// <summary>
    /// 顯示資訊訊息。
    /// </summary>
    /// <param name="message">訊息內容。</param>
    void ShowInfo(string message);
}
```

### 行為規範

| 方法 | 前置條件 | 後置條件 | 例外處理 |
|------|----------|----------|----------|
| ShowSuccess | message 不為 null | 訊息已顯示給使用者 | 不拋出例外 |
| ShowError | message 不為 null | 訊息已顯示給使用者 | 不拋出例外 |
| ShowInfo | message 不為 null | 訊息已顯示給使用者 | 不拋出例外 |

---

## 使用者訊息對照表

根據 FR-005、FR-006、FR-007、FR-014 的要求，定義標準訊息：

| 情境 | 訊息類型 | 訊息內容 |
|------|----------|----------|
| 翻譯成功 | Success | 翻譯完成，結果已複製到剪貼簿 |
| 剪貼簿為空 | Error | 剪貼簿內容為空無法翻譯 |
| 非純文字內容 | Error | 無法翻譯非純文字 |
| 設定檔不存在 | Error | 找不到設定檔，請在以下路徑建立設定檔：%AppData%\Rivet\config.json |
| 金鑰檔不存在 | Error | 找不到翻譯服務金鑰檔：{path} |
| 翻譯服務錯誤 | Error | {錯誤訊息} |
| 使用者取消 | (無訊息) | (直接結束程式) |

---

## 依賴關係

```
IUserNotifier ◄────────── Console 專案實作
       ▲
       │
       │ 使用
       │
┌──────┴──────┐
│ CommandHandler │
├─────────────────┤
│ 使用 ▼         │
│  IClipboardService ◄─── Infrastructure 實作
│  ITranslationService ◄─ Infrastructure 實作
│  IConfigurationService ◄ Infrastructure 實作
└─────────────────┘
```
