# 研究文件：剪貼簿翻譯工具

**Branch**: `001-clipboard-translator`  
**Date**: 2024-12-04  
**Status**: 已完成

## 研究摘要

本文件整理實作剪貼簿翻譯工具所需的技術研究，包括各依賴項的最佳實踐與整合模式。

---

## 1. 責任鏈模式（Chain of Responsibility）

### 決策
採用責任鏈模式處理指令分派，每個 Handler 負責判斷是否能處理該指令。

### 理由
- **開放封閉原則**：新增功能只需新增 Handler，不修改既有程式碼
- **解耦合**：指令發送者不需知道具體處理者
- **靈活排序**：可動態調整處理順序

### 考慮的替代方案
| 方案 | 排除原因 |
|------|----------|
| Switch-case 分派 | 違反開放封閉原則，新增功能需修改分派邏輯 |
| 策略模式 + 工廠 | 較複雜，且需要維護對應表 |
| 命令模式 | 適合撤銷/重做場景，本專案不需要 |

### 實作要點
```csharp
public interface ICommandHandler
{
    void SetNext(ICommandHandler handler);
    Task<bool> HandleAsync(CommandContext context);
}

public abstract class CommandHandlerBase : ICommandHandler
{
    private ICommandHandler? _next;
    
    public void SetNext(ICommandHandler handler) => _next = handler;
    
    public async Task<bool> HandleAsync(CommandContext context)
    {
        if (CanHandle(context))
        {
            await ExecuteAsync(context);
            return true;
        }
        return _next != null && await _next.HandleAsync(context);
    }
    
    protected abstract bool CanHandle(CommandContext context);
    protected abstract Task ExecuteAsync(CommandContext context);
}
```

---

## 2. Google Cloud Translation API

### 決策
使用 Google.Cloud.Translation.V2 NuGet 套件，以 JSON 金鑰檔認證。

### 理由
- **官方支援**：Google 維護的 .NET 客戶端程式庫
- **簡單整合**：提供高階 API，減少樣板程式碼
- **彈性認證**：支援多種認證方式

### 考慮的替代方案
| 方案 | 排除原因 |
|------|----------|
| REST API 直接呼叫 | 需自行處理認證、序列化、錯誤重試 |
| Azure Translator | 架構已設計可替換，初期選用 Google |
| DeepL API | 架構已設計可替換，初期選用 Google |

### 實作要點
```csharp
// 認證設定
var credential = GoogleCredential.FromFile(keyFilePath);
var client = TranslationClient.Create(credential);

// 翻譯呼叫
var response = await client.TranslateTextAsync(
    text: sourceText,
    targetLanguage: "zh-TW",  // 或 "en"
    sourceLanguage: null      // 自動偵測
);
```

### 設定檔格式 (%AppData%\Rivet\config.json)
```json
{
  "services": {
    "googleTranslation": {
      "keyFilePath": "C:\\path\\to\\service-account-key.json"
    }
  }
}
```

---

## 3. Windows 剪貼簿操作

### 決策
使用 `System.Windows.Forms.Clipboard` API 存取剪貼簿（需引用 Windows Forms）。

### 理由
- **.NET 原生支援**：無需額外依賴
- **完整功能**：支援格式檢查與讀寫
- **穩定性**：經過長期驗證的 API

### 考慮的替代方案
| 方案 | 排除原因 |
|------|----------|
| Win32 API (P/Invoke) | 較繁瑣，需手動管理記憶體 |
| TextCopy 套件 | 功能較簡單，不支援格式檢查 |
| WPF Clipboard | 需引用更多依賴 |

### 實作要點
```csharp
// Console 專案需加入 UseWindowsForms
// <UseWindowsForms>true</UseWindowsForms>

public class WindowsClipboardService : IClipboardService
{
    [STAThread]
    public ClipboardContent GetContent()
    {
        if (!Clipboard.ContainsText(TextDataFormat.UnicodeText))
        {
            return ClipboardContent.NonText();
        }
        
        var text = Clipboard.GetText(TextDataFormat.UnicodeText);
        if (string.IsNullOrEmpty(text))
        {
            return ClipboardContent.Empty();
        }
        
        return ClipboardContent.WithText(text);
    }
    
    public void SetText(string text)
    {
        Clipboard.SetText(text, TextDataFormat.UnicodeText);
    }
}
```

### 注意事項
- 剪貼簿操作需在 STA 執行緒執行
- Program.cs 需標記 `[STAThread]`

---

## 4. Spectre.Console 選單

### 決策
使用 `SelectionPrompt<T>` 顯示選單，支援鍵盤選擇。

### 理由
- **美觀**：現代化的 Console UI
- **易用**：高階 API 快速建立互動介面
- **可客製**：支援自訂樣式與顏色

### 考慮的替代方案
| 方案 | 排除原因 |
|------|----------|
| Console.ReadLine | 不支援選單式互動 |
| System.CommandLine | 較適合 CLI 命令解析，非互動選單 |

### 實作要點
```csharp
var selection = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .Title("[green]請選擇功能：[/]")
        .AddChoices(new[] {
            "[1] 翻譯成繁體中文",
            "[2] 翻譯成英文"
        }));

// 或使用數字鍵直接選擇
var key = Console.ReadKey(intercept: true);
switch (key.Key)
{
    case ConsoleKey.D1 or ConsoleKey.NumPad1:
        // 翻譯成繁體中文
        break;
    case ConsoleKey.D2 or ConsoleKey.NumPad2:
        // 翻譯成英文
        break;
    case ConsoleKey.Escape:
        // 取消
        return;
}
```

---

## 5. Serilog 日誌設定

### 決策
使用 File Sink 輸出到 %AppData%\Rivet\logs，按日期滾動。

### 理由
- **結構化日誌**：支援屬性與查詢
- **彈性輸出**：多種 Sink 可選
- **效能**：非同步寫入，不阻塞主執行緒

### 實作要點
```csharp
var appDataPath = Environment.GetFolderPath(
    Environment.SpecialFolder.ApplicationData);
var logPath = Path.Combine(appDataPath, "Rivet", "logs", "rivet-.log");

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()  // 支援 Trace
    .WriteTo.File(
        logPath,
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7)
    .CreateLogger();

// 記錄剪貼簿內容（Trace 層級）
Log.Verbose("剪貼簿內容: {ClipboardContent}", clipboardText);
```

---

## 6. 依賴注入設定

### 決策
使用 `Microsoft.Extensions.DependencyInjection` 管理依賴。

### 理由
- **.NET 原生**：與 .NET 生態系整合良好
- **輕量**：不需額外大型框架
- **測試友善**：便於 Mock 替換

### 實作要點
```csharp
var services = new ServiceCollection();

// 基礎設施層
services.AddSingleton<IClipboardService, WindowsClipboardService>();
services.AddSingleton<ITranslationService, GoogleTranslationService>();
services.AddSingleton<IConfigurationService, JsonConfigurationService>();

// 服務層 - 責任鏈
services.AddTransient<TranslateToChineseHandler>();
services.AddTransient<TranslateToEnglishHandler>();
services.AddSingleton<CommandChain>(sp =>
{
    var chain = new CommandChain();
    chain.AddHandler(sp.GetRequiredService<TranslateToChineseHandler>());
    chain.AddHandler(sp.GetRequiredService<TranslateToEnglishHandler>());
    return chain;
});

var provider = services.BuildServiceProvider();
```

---

## 7. 專案參考關係

### 決策
採用分層依賴：Console → Service ← Infrastructure

### 理由
- **依賴反轉**：Service 層定義介面，Infrastructure 實作
- **可替換性**：前端與基礎設施皆可獨立替換
- **測試隔離**：Service 層可純單元測試

### 專案參考
```
Rivet.Console
├── → Rivet.Service (業務邏輯)
└── → Rivet.Infrastructure (DI 組裝時需要實作類別)

Rivet.Service
└── (無專案參考，僅定義介面)

Rivet.Infrastructure
└── → Rivet.Service (實作介面)
```

---

## 結論

所有技術選型已確認，無遺留的 NEEDS CLARIFICATION 項目。可進入 Phase 1 設計階段。
