# 資料模型：剪貼簿翻譯工具

**Branch**: `001-clipboard-translator`  
**Date**: 2024-12-04  
**Status**: 已完成

## 實體總覽

本專案定義以下核心實體，從功能規格的 Key Entities 與需求中提取。

---

## 1. ClipboardContent（剪貼簿內容）

代表從系統剪貼簿讀取的資料。

### 欄位

| 欄位名稱 | 型別 | 必填 | 說明 |
|----------|------|------|------|
| Status | ClipboardStatus | ✅ | 剪貼簿狀態 |
| Text | string? | ❌ | 純文字內容（僅 HasText 狀態有值） |

### 狀態列舉 (ClipboardStatus)

| 值 | 說明 |
|----|------|
| HasText | 包含有效的純文字內容 |
| Empty | CF_UNICODETEXT 為空字串 |
| NonText | 非純文字格式（如圖片） |

### 驗證規則
- 當 Status 為 `HasText` 時，Text 不得為 null 或空白
- 當 Status 為 `Empty` 或 `NonText` 時，Text 應為 null

### 工廠方法
```csharp
public record ClipboardContent
{
    public ClipboardStatus Status { get; init; }
    public string? Text { get; init; }
    
    public static ClipboardContent WithText(string text) 
        => new() { Status = ClipboardStatus.HasText, Text = text };
    
    public static ClipboardContent Empty() 
        => new() { Status = ClipboardStatus.Empty, Text = null };
    
    public static ClipboardContent NonText() 
        => new() { Status = ClipboardStatus.NonText, Text = null };
}
```

---

## 2. TranslationRequest（翻譯請求）

代表使用者發起的翻譯請求。

### 欄位

| 欄位名稱 | 型別 | 必填 | 說明 |
|----------|------|------|------|
| SourceText | string | ✅ | 來源文字 |
| TargetLanguage | TargetLanguage | ✅ | 目標語言 |

### 目標語言列舉 (TargetLanguage)

| 值 | 語言代碼 | 說明 |
|----|----------|------|
| TraditionalChinese | zh-TW | 繁體中文 |
| English | en | 英文 |

### 驗證規則
- SourceText 不得為 null 或空白
- SourceText 長度不得超過 5000 字元
- TargetLanguage 必須為有效的列舉值

```csharp
public record TranslationRequest
{
    public required string SourceText { get; init; }
    public required TargetLanguage TargetLanguage { get; init; }
}

public enum TargetLanguage
{
    TraditionalChinese,
    English
}
```

---

## 3. TranslationResult（翻譯結果）

代表翻譯操作的結果。

### 欄位

| 欄位名稱 | 型別 | 必填 | 說明 |
|----------|------|------|------|
| IsSuccess | bool | ✅ | 是否成功 |
| TranslatedText | string? | ❌ | 翻譯後文字（成功時有值） |
| ErrorMessage | string? | ❌ | 錯誤訊息（失敗時有值） |

### 狀態轉換
- 成功：IsSuccess = true, TranslatedText 有值, ErrorMessage 為 null
- 失敗：IsSuccess = false, TranslatedText 為 null, ErrorMessage 有值

### 工廠方法
```csharp
public record TranslationResult
{
    public bool IsSuccess { get; init; }
    public string? TranslatedText { get; init; }
    public string? ErrorMessage { get; init; }
    
    public static TranslationResult Success(string translatedText) 
        => new() { IsSuccess = true, TranslatedText = translatedText };
    
    public static TranslationResult Failure(string errorMessage) 
        => new() { IsSuccess = false, ErrorMessage = errorMessage };
}
```

---

## 4. CommandContext（指令上下文）

傳遞給責任鏈處理器的上下文資訊。

### 欄位

| 欄位名稱 | 型別 | 必填 | 說明 |
|----------|------|------|------|
| CommandType | CommandType | ✅ | 指令類型 |
| ClipboardContent | ClipboardContent | ✅ | 剪貼簿內容 |

### 指令類型列舉 (CommandType)

| 值 | 對應選單 | 說明 |
|----|----------|------|
| TranslateToChinese | [1] 翻譯成繁體中文 | 將內容翻譯成繁體中文 |
| TranslateToEnglish | [2] 翻譯成英文 | 將內容翻譯成英文 |
| Cancel | Esc | 取消操作 |

```csharp
public record CommandContext
{
    public required CommandType CommandType { get; init; }
    public required ClipboardContent ClipboardContent { get; init; }
}

public enum CommandType
{
    TranslateToChinese,
    TranslateToEnglish,
    Cancel
}
```

---

## 5. AppConfiguration（應用程式設定）

從 config.json 讀取的設定資訊。

### 欄位

| 欄位名稱 | 型別 | 必填 | 說明 |
|----------|------|------|------|
| Services | ServiceConfiguration | ✅ | 服務設定 |

### ServiceConfiguration

| 欄位名稱 | 型別 | 必填 | 說明 |
|----------|------|------|------|
| GoogleTranslation | GoogleTranslationConfig | ✅ | Google 翻譯服務設定 |

### GoogleTranslationConfig

| 欄位名稱 | 型別 | 必填 | 說明 |
|----------|------|------|------|
| KeyFilePath | string | ✅ | 金鑰檔案路徑 |

### JSON 結構
```json
{
  "services": {
    "googleTranslation": {
      "keyFilePath": "C:\\path\\to\\service-account-key.json"
    }
  }
}
```

### 驗證規則
- KeyFilePath 不得為 null 或空白
- 指定的檔案必須存在

```csharp
public record AppConfiguration
{
    public required ServiceConfiguration Services { get; init; }
}

public record ServiceConfiguration
{
    public required GoogleTranslationConfig GoogleTranslation { get; init; }
}

public record GoogleTranslationConfig
{
    public required string KeyFilePath { get; init; }
}
```

---

## 6. ConfigurationResult（設定結果）

代表設定讀取操作的結果。

### 欄位

| 欄位名稱 | 型別 | 必填 | 說明 |
|----------|------|------|------|
| IsSuccess | bool | ✅ | 是否成功 |
| Configuration | AppConfiguration? | ❌ | 設定內容（成功時有值） |
| ErrorMessage | string? | ❌ | 錯誤訊息（失敗時有值） |

### 狀態轉換
- 成功：IsSuccess = true, Configuration 有值, ErrorMessage 為 null
- 失敗：IsSuccess = false, Configuration 為 null, ErrorMessage 有值

### 工廠方法
```csharp
public record ConfigurationResult
{
    public bool IsSuccess { get; init; }
    public AppConfiguration? Configuration { get; init; }
    public string? ErrorMessage { get; init; }
    
    public static ConfigurationResult Success(AppConfiguration config)
        => new() { IsSuccess = true, Configuration = config };
    
    public static ConfigurationResult Failure(string errorMessage)
        => new() { IsSuccess = false, ErrorMessage = errorMessage };
}
```

---

## 實體關係圖

```
┌─────────────────┐
│  CommandContext │
├─────────────────┤
│  CommandType    │───────┐
│  ClipboardContent ──────┼──→  ClipboardContent
└─────────────────┘       │     ├─ Status
                          │     └─ Text?
                          │
                          ▼
              ┌───────────────────────┐
              │  TranslationRequest   │
              ├───────────────────────┤
              │  SourceText           │
              │  TargetLanguage       │
              └───────────┬───────────┘
                          │
                          ▼
              ┌───────────────────────┐
              │  TranslationResult    │
              ├───────────────────────┤
              │  IsSuccess            │
              │  TranslatedText?      │
              │  ErrorMessage?        │
              └───────────────────────┘
```

---

## 設計說明

1. **不可變性（Immutability）**：所有實體使用 C# record 型別，確保不可變性與值語意。

2. **工廠方法**：使用靜態工廠方法建立實體，確保有效狀態建構。

3. **明確狀態**：使用列舉明確表達狀態，避免 null 檢查的模糊語意。

4. **Result Pattern**：TranslationResult 採用 Result Pattern，明確區分成功與失敗。
