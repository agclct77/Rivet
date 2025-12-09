# 資料模型：Rivet WPF 手動啟動剪貼簿翻譯

**Branch**: `002-wpf-shell`  
**Date**: 2025-12-10  
**Status**: 設計中

本文件列出 WPF 手動啟動剪貼簿翻譯的核心實體，延續既有 Service/Infrastructure 模型並補充 WPF 狀態需求。

---

## 1. ClipboardContent（剪貼簿內容）
代表從系統剪貼簿讀取的資料，需處理純文字、RTF 降階與不支援格式（圖片）。

| 欄位名稱 | 型別 | 必填 | 說明 |
|----------|------|------|------|
| Status | ClipboardStatus | ✅ | 剪貼簿狀態：HasText / Empty / NonText / Image / Rtf |
| PlainText | string? | ❌ | 純文字內容（HasText 或 Rtf 且可降階時有值） |
| RtfText | string? | ❌ | RTF 來源內容（僅 Status=Rtf 時有值，用於日誌或後續處理） |

> 註：剪貼簿讀取需支援 3–5 次重試；一旦讀取到資料即複製到自有結構避免鎖定。

### 驗證
- Status = HasText 時，PlainText 不得為 null 或空白。
- Status = Empty / NonText / Image 時，PlainText、RtfText 應為 null。
- Status = Rtf 時，PlainText 可為 null（無純文字降階）；RtfText 不得為空。

---

## 2. TranslationRequest（翻譯請求）
代表使用者觸發的翻譯動作。

| 欄位名稱 | 型別 | 必填 | 說明 |
|----------|------|------|------|
| SourceText | string | ✅ | 從剪貼簿取得的來源文字 |
| TargetLanguage | TargetLanguage | ✅ | 目標語言：TraditionalChinese / English |
| OriginalFormat | ClipboardStatus | ✅ | 來源格式（HasText/Rtf），用於記錄與 UI 提示 |

### 驗證
- SourceText 不得為 null/空白，長度 ≤ 20000；超過應拒絕並提示（依規格 edge case）。
- TargetLanguage 必須為有效列舉值。
- OriginalFormat 需與剪貼簿讀取結果對應，Rtf 轉純文字時保留「已降階」標記供 UI 使用。

---

## 3. TranslationResult（翻譯結果）
代表翻譯操作的結果（沿用既有定義）。

| 欄位名稱 | 型別 | 必填 | 說明 |
|----------|------|------|------|
| IsSuccess | bool | ✅ | 是否成功 |
| TranslatedText | string? | ❌ | 翻譯後文字（成功時有值） |
| ErrorMessage | string? | ❌ | 錯誤訊息（失敗時有值） |
| ErrorCode | TranslationErrorCode? | ❌ | 失敗類型：EMPTY_CLIPBOARD / NON_TEXT / RTF_UNSUPPORTED / IMAGE_UNSUPPORTED / TRANSLATION_UNAVAILABLE / CLIPBOARD_WRITE_FAILED |

### 狀態轉換
- 成功：IsSuccess = true，TranslatedText 有值，ErrorMessage、ErrorCode 為 null。
- 失敗：IsSuccess = false，TranslatedText 為 null，ErrorMessage、ErrorCode 需對應實際原因。

---

## 4. CommandContext（指令上下文）
責任鏈傳遞資訊，WPF 也沿用以觸發翻譯。

| 欄位名稱 | 型別 | 必填 | 說明 |
|----------|------|------|------|
| CommandType | CommandType | ✅ | 指令類型：TranslateToChinese / TranslateToEnglish / Cancel |
| ClipboardContent | ClipboardContent | ✅ | 剪貼簿內容（從 UI/服務端注入） |

### 驗證
- CommandType 必須為有效值；ClipboardContent 不得為 null。

---

## 5. AppConfiguration（應用程式設定）
沿用現有 config.json 結構，提供 Google Translation API 金鑰位置。

| 欄位名稱 | 型別 | 必填 | 說明 |
|----------|------|------|------|
| Services | ServiceConfiguration | ✅ | 服務設定 |
| Services.GoogleTranslation | GoogleTranslationConfig | ✅ | Google 翻譯金鑰設定 |
| Services.GoogleTranslation.KeyFilePath | string | ✅ | 金鑰檔案路徑 |

### 驗證
- KeyFilePath 不得為空且檔案需存在。

---

## 6. WpfTranslationViewState（WPF 視圖狀態）
描述 WPF UI 需綁定的狀態，用於顯示翻譯結果與錯誤。

| 欄位名稱 | 型別 | 必填 | 說明 |
|----------|------|------|------|
| Direction | CommandType | ✅ | 目前選擇的翻譯方向 |
| SourceText | string | ✅ | 從剪貼簿抓取的文字（供顯示/除錯，若為 Rtf 則顯示純文字版本） |
| TranslatedText | string | ❌ | 翻譯結果（成功時有值） |
| ClipboardStatus | ClipboardStatus | ✅ | 剪貼簿狀態（HasText/Empty/NonText/Image/Rtf） |
| IsBusy | bool | ✅ | UI 是否正在執行翻譯（顯示進度/禁用按鈕） |
| ErrorMessage | string? | ❌ | 最近一次錯誤提示（空白代表無錯誤） |
| ErrorCode | TranslationErrorCode? | ❌ | 失敗類型（對應 TranslationResult.ErrorCode，供 UI 顯示） |
| IsClipboardWriteBackSucceeded | bool? | ❌ | 回寫剪貼簿是否成功（null 代表尚未嘗試） |
| LastUpdatedUtc | DateTime | ✅ | 狀態最後更新時間（for 日誌/偵錯） |

### 狀態轉換（摘要）
1. Idle → Translating：點擊翻譯按鈕後 `IsBusy=true`，清空 ErrorMessage。
2. Translating → Success：取得 TranslatedText，`IsBusy=false`，回寫剪貼簿並更新 LastUpdatedUtc，`IsClipboardWriteBackSucceeded=true`。
3. Translating → Error：設定 ErrorMessage/ErrorCode，`IsBusy=false`，`IsClipboardWriteBackSucceeded=false`（若寫回失敗則保留結果供複製）。
4. Error/Success → Idle：使用者再點擊翻譯按鈕或更換剪貼簿。

---

## 實體關係圖
```
ClipboardContent ──► TranslationRequest ──► TranslationResult
        │                   │                     │
        │                   ▼                     ▼
        └────► CommandContext (含方向)     WpfTranslationViewState
                                     └─ 綁定 UI 顯示結果/錯誤/忙碌狀態
```

---

## 設計說明
- 沿用既有不可變 record 型別，確保狀態清晰且易於測試。
- WpfTranslationViewState 專注於 UI 綁定，避免與 Service/Infrastructure 耦合。
- 仍採 Result Pattern（TranslationResult/CommandResult），方便在 UI 顯示錯誤並記錄日誌。
- 剪貼簿操作與翻譯請求輸入保持分離，便於在 UI 上呈現「原文」與「譯文」並允許錯誤後重試。
