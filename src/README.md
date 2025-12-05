# Rivet 剪貼簿翻譯工具

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=.net)](https://dotnet.microsoft.com/)
[![Code Quality: High](https://img.shields.io/badge/Code%20Quality-High-brightgreen)]()
[![Tests: 64 Passed](https://img.shields.io/badge/Tests-64%20Passed-brightgreen)]()

Rivet 是一個輕量化的 Windows 桌面工具，透過快捷鍵 `Ctrl+Alt+.` 快速將剪貼簿內容翻譯成繁體中文或英文。支援 Google Cloud Translation API，提供快速、準確的翻譯服務。

## 功能特性

✨ **快速啟動**
- 單一快捷鍵 `Ctrl+Alt+.` 開啟翻譯功能
- 無需開啟額外的應用程式視窗
- 翻譯完成後自動結束，不占用系統資源

🌐 **雙向翻譯**
- 英文 ↔ 繁體中文
- 基於 Google Cloud Translation API，翻譯品質穩定

🛡️ **完善的錯誤處理**
- 清晰的使用者提示訊息
- 支援空白剪貼簿檢查
- 支援非純文字內容檢查
- 網路錯誤時保留原始剪貼簿內容

📊 **詳細日誌**
- 記錄所有操作，包括翻譯內容
- 位置：`%AppData%\Rivet\logs\`
- 支援 Trace 層級調試資訊

🏗️ **分層架構**
- Console：使用者介面層
- Service：業務邏輯層
- Infrastructure：資料存取與外部服務層
- 易於擴充和測試

---

## 系統需求

| 需求 | 版本 |
|------|------|
| **操作系統** | Windows 10/11 |
| **.NET Runtime** | 8.0 或更高版本 |
| **網路連線** | 必須（用於翻譯服務） |
| **Google Cloud** | 有效的 Translation API 金鑰 |

## 快速開始

### 1. 安裝 .NET Runtime

確保已安裝 .NET 8：

```powershell
dotnet --version
```

若未安裝，前往 [.NET 官方網站](https://dotnet.microsoft.com/download) 下載。

### 2. 準備 Google Cloud 金鑰

1. 前往 [Google Cloud Console](https://console.cloud.google.com/)
2. 建立或選擇專案
3. 啟用 **Cloud Translation API**
4. 建立服務帳戶並下載 JSON 金鑰檔
5. 將金鑰檔保存到安全位置（例如：`C:\Keys\google-key.json`）

### 3. 建立配置檔

在 `%AppData%\Rivet\` 建立 `config.json`：

```powershell
New-Item -ItemType Directory -Force -Path "$env:APPDATA\Rivet"

@"
{
  "services": {
    "googleTranslation": {
      "keyFilePath": "C:\\Keys\\google-key.json"
    }
  }
}
"@ | Out-File -FilePath "$env:APPDATA\Rivet\config.json" -Encoding UTF8
```

### 4. 執行程式

發布完成後，直接執行可執行檔：

```powershell
# 執行發布的程式
./publish/Rivet.Console.exe
```

或建立桌面捷徑以便快速訪問：

```powershell
# 建立桌面捷徑
$publishPath = "D:\Lab\Rivet\publish\Rivet.Console.exe"
$desktopPath = [Environment]::GetFolderPath('Desktop')
$shortcutPath = Join-Path $desktopPath "Rivet.lnk"

$WshShell = New-Object -ComObject WScript.Shell
$shortcut = $WshShell.CreateShortcut($shortcutPath)
$shortcut.TargetPath = $publishPath
$shortcut.WorkingDirectory = Split-Path -Parent $publishPath
$shortcut.Save()

Write-Host "✓ 桌面捷徑已建立"
```

### 5. 執行程式

直接執行已發布的可執行檔：

```powershell
# 方式 1：直接執行發布的檔案
D:\Lab\Rivet\publish\Rivet.Console.exe

# 方式 2：建立桌面捷徑後點擊運行
# 將發布的 Rivet.Console.exe 建立捷徑放到桌面
# 直接點擊捷徑運行程式
```

**注意**：
- 目前快捷鍵 `Ctrl+Alt+/` 設定存在技術限制（詳見 [`KNOWN_ISSUES.md`](../../KNOWN_ISSUES.md)）
- 建議暫時使用直接執行或桌面捷徑的方式

---

## 使用方式

### 基本流程

```
1. 建置並發布程式
    ↓
2. 執行 Rivet.Console.exe（直接執行或點擊桌面捷徑）
    ↓
3. 複製文字到剪貼簿 (Ctrl+C)
    ↓
4. 程式自動開啟選單
    ↓
5. 選擇功能 (按 1 或 2)
    ↓
6. 翻譯完成 (自動結束)
    ↓
7. 貼上結果 (Ctrl+V)
```

### 使用範例

#### 英文翻譯成繁體中文

```
輸入: "The quick brown fox jumps over the lazy dog."
輸出: "敏捷的棕色狐狸跳過懶狗。"
```

#### 繁體中文翻譯成英文

```
輸入: "今天天氣很好，適合去戶外活動。"
輸出: "The weather is nice today, it's a good day to go outdoors."
```

---

## 專案結構

```
Rivet/
├── src/
│   ├── Rivet.Console/
│   │   ├── Program.cs              # 進入點與 DI 設定
│   │   ├── Menu/
│   │   │   └── MainMenu.cs         # UI 選單顯示
│   │   ├── Notification/
│   │   │   └── ConsoleUserNotifier.cs  # 使用者通知
│   │   └── Rivet.Console.csproj
│   │
│   ├── Rivet.Service/
│   │   ├── Commands/               # 指令處理（責任鏈模式）
│   │   │   ├── CommandChain.cs
│   │   │   ├── CommandHandlerBase.cs
│   │   │   ├── TranslateToChineseHandler.cs
│   │   │   └── TranslateToEnglishHandler.cs
│   │   ├── Translation/            # 翻譯服務介面
│   │   ├── Clipboard/              # 剪貼簿服務介面
│   │   ├── Configuration/          # 配置服務介面
│   │   └── Rivet.Service.csproj
│   │
│   ├── Rivet.Infrastructure/
│   │   ├── Translation/            # Google 翻譯實作
│   │   │   └── GoogleTranslationService.cs
│   │   ├── Clipboard/              # Windows 剪貼簿實作
│   │   │   └── WindowsClipboardService.cs
│   │   ├── Configuration/          # JSON 設定實作
│   │   │   └── JsonConfigurationService.cs
│   │   ├── Logging/                # Serilog 設定
│   │   │   └── SerilogSetup.cs
│   │   └── Rivet.Infrastructure.csproj
│   │
│   └── Rivet.sln
│
├── tests/
│   ├── Rivet.Service.Tests/        # 業務邏輯單元測試
│   ├── Rivet.Infrastructure.Tests/ # 基礎設施整合測試
│   └── Rivet.Console.Tests/        # UI 層測試
│
├── docs/
│   ├── setup-shortcut.md           # Windows 快捷鍵設定
│   └── README.md                   # 本文件
│
└── specs/
    └── 001-clipboard-translator/
        ├── spec.md                 # 功能規格
        ├── plan.md                 # 技術計劃
        ├── research.md             # 技術研究
        ├── data-model.md           # 資料模型
        ├── quickstart.md           # 快速入門
        ├── tasks.md                # 任務清單
        └── contracts/
            └── interfaces.md       # API 合約
```

---

## 技術棧

| 元件 | 技術 | 版本 |
|------|------|------|
| **語言** | C# | .NET 8.0 |
| **UI 框架** | Spectre.Console | 最新 |
| **翻譯服務** | Google Cloud Translation API | V2 |
| **日誌框架** | Serilog | 最新 |
| **依賴注入** | Microsoft.Extensions.DependencyInjection | 最新 |
| **測試框架** | NUnit + NSubstitute | 最新 |

---

## 架構設計

### 分層架構

```
┌─────────────────────────────────┐
│   Rivet.Console               │  ← UI Layer (Spectre.Console)
├─────────────────────────────────┤
│   Rivet.Service               │  ← Business Logic (Interfaces)
├─────────────────────────────────┤
│   Rivet.Infrastructure        │  ← Data/External (Implementations)
├─────────────────────────────────┤
│   External Services           │  ← Google Cloud Translation API
└─────────────────────────────────┘
```

### 設計模式

1. **責任鏈模式**（Chain of Responsibility）
   - 用於指令處理管道
   - 便於新增翻譯功能

2. **依賴注入模式**（Dependency Injection）
   - 使用 Microsoft.Extensions.DependencyInjection
   - 提高測試能力和可維護性

3. **記錄模式**（Record Types）
   - 使用 C# 9+ Record 表示不可變資料
   - 提高代碼清晰度和安全性

---

## 運行測試

### 執行所有測試

```powershell
dotnet test
```

### 執行特定測試專案

```powershell
# Service 層測試（53 個測試）
dotnet test tests/Rivet.Service.Tests/Rivet.Service.Tests.csproj

# Infrastructure 層測試（1 個測試）
dotnet test tests/Rivet.Infrastructure.Tests/Rivet.Infrastructure.Tests.csproj

# Console 層測試（10 個測試）
dotnet test tests/Rivet.Console.Tests/Rivet.Console.Tests.csproj
```

### 測試覆蓋率

```powershell
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

**當前狀態**：64 個測試，全部通過 ✅

---

## 配置

### 配置檔位置

```
%AppData%\Rivet\config.json
```

### 配置檔格式

```json
{
  "services": {
    "googleTranslation": {
      "keyFilePath": "C:\\Path\\To\\google-key.json"
    }
  }
}
```

### 環境變數

Rivet 自動設定以下環境變數：
- `GOOGLE_APPLICATION_CREDENTIALS`：指向金鑰檔路徑

---

## 日誌

### 日誌位置

```
%AppData%\Rivet\logs\
```

### 日誌檔案格式

```
rivet-20241204.log
```

### 檢視日誌

```powershell
# 顯示最後 50 行
Get-Content "$env:APPDATA\Rivet\logs\rivet-*.log" -Tail 50

# 實時監控
Get-Content "$env:APPDATA\Rivet\logs\rivet-*.log" -Tail 10 -Wait
```

### 日誌層級

- **Information**：正常操作
- **Warning**：可能的問題
- **Error**：錯誤情況
- **Trace**：詳細調試信息（包括剪貼簿內容）

---

## 常見問題

### Q: 如何執行程式？

A: 直接執行發布的可執行檔：
```powershell
D:\Lab\Rivet\publish\Rivet.Console.exe
```

或建立桌面捷徑後點擊運行（參考「快速開始」步驟 4）。

### Q: 快捷鍵 Ctrl+Alt+/ 為什麼沒有反應？

A: 目前快捷鍵設定存在技術限制。詳見 [`KNOWN_ISSUES.md`](../../KNOWN_ISSUES.md)。建議暫時使用直接執行或桌面捷徑的方式。

### Q: 翻譯結果不準確？

A: 本工具使用 Google Cloud Translation API，翻譯品質取決於 Google 服務。若需更高品質翻譯，可考慮分段翻譯或檢查來源文字是否有誤。

### Q: 如何新增其他翻譯語言？

A: 
1. 在 `Rivet.Service/Translation/TargetLanguage.cs` 新增語言列舉
2. 在 `Rivet.Infrastructure/Translation/GoogleTranslationService.cs` 更新語言代碼對應
3. 建立新的 Handler 類別（例如 `TranslateToJapaneseHandler.cs`）
4. 在 `Program.cs` 註冊新 Handler

### Q: 如何自訂翻譯快捷鍵？

A: 目前快捷鍵設定存在技術複雜性。可以：
1. 使用 AutoHotkey 工具（第三方）
2. 建立桌面捷徑後直接點擊運行
3. 追蹤 [`KNOWN_ISSUES.md`](../../KNOWN_ISSUES.md) 中的解決方案進展

### Q: 可以離線使用嗎？

A: 不行，Rivet 需要網際網路連線才能使用 Google Cloud Translation API。

---

## 擴充性

Rivet 設計為易於擴充的架構。新增功能步驟：

### 新增翻譯語言

1. 在 `TargetLanguage.cs` 新增列舉值
2. 在 `GoogleTranslationService.cs` 新增語言代碼
3. 建立新的 Handler 類別
4. 在 `Program.cs` 註冊

### 新增指令處理器

1. 建立繼承 `CommandHandlerBase` 的新類別
2. 實作 `CanHandle()` 和 `ExecuteAsync()`
3. 在 `Program.cs` 透過 `commandChain.AddHandler()` 註冊

**注意**：新增 Handler 無需修改既有代碼，符合開放-閉合原則。

---

## 開發指南

### 本地開發設定

```powershell
# 克隆倉庫
git clone <repository-url>
cd Rivet

# 還原 NuGet 套件
dotnet restore

# 建置解決方案
dotnet build -c Debug

# 執行測試
dotnet test --verbosity normal

# 發布
dotnet publish src/Rivet.Console/Rivet.Console.csproj -c Release -o ./publish
```

### 編碼規範

- 遵循 C# 命名規範（PascalCase、camelCase）
- 為公開 API 提供 XML 文件註解
- 使用 Record 類型表示不可變資料
- 儘早驗證參數（Guard Clauses）

### 提交規範

遵循 Conventional Commits：

```
feat(001-clipboard-translator): 新增日本語翻譯支持
fix(cli): 修正空白剪貼簿檢查
docs: 更新快速入門指南
test: 新增翻譯服務單元測試
```

---

## 許可證

MIT License - 詳見 [`LICENSE`](LICENSE) 文件

---

## 貢獻指南

歡迎提交 Issue 和 Pull Request！

### 報告 Bug

1. 確認 Bug 可以穩定重現
2. 收集日誌檔案（`%AppData%\Rivet\logs\`）
3. 提交 Issue 並附上日誌內容

### 功能建議

1. 描述期望的功能
2. 解釋使用場景
3. 如果可能，提供實現建議

---

## 相關資源

- [Google Cloud Translation API 文件](https://cloud.google.com/translate/docs)
- [Spectre.Console 文件](https://spectreconsole.net/)
- [Serilog 文件](https://serilog.net/)
- [C# 官方文件](https://docs.microsoft.com/en-us/dotnet/csharp/)

---

## 反饋

如有任何問題或建議，歡迎透過以下方式聯繫：

- 📧 Email: [your-email@example.com]
- 🐛 Issue: [GitHub Issues](link-to-issues)
- 💬 Discussion: [GitHub Discussions](link-to-discussions)

---

**最後更新**：2024-12-05  
**版本**：1.0.0  
**狀態**：✅ MVP 完成
