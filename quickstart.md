# 快速入門：Rivet 剪貼簿翻譯工具

**Branch**: `001-clipboard-translator`  
**Date**: 2024-12-04

## 系統需求

- Windows 10/11
- .NET 8 Runtime
- 網際網路連線（用於翻譯服務）
- Google Cloud Translation API 金鑰

---

## 安裝步驟

### 1. 取得 Google Cloud Translation API 金鑰

1. 前往 [Google Cloud Console](https://console.cloud.google.com/)
2. 建立或選擇專案
3. 啟用 Cloud Translation API
4. 建立服務帳戶並下載 JSON 金鑰檔
5. 將金鑰檔儲存到安全位置（例如：`C:\Keys\google-translation-key.json`）

### 2. 建立設定檔

在 `%AppData%\Rivet\` 目錄下建立 `config.json`：

```powershell
# 建立目錄
New-Item -ItemType Directory -Force -Path "$env:APPDATA\Rivet"

# 建立設定檔（請修改 keyFilePath 為您的金鑰檔路徑）
@"
{
  "services": {
    "googleTranslation": {
      "keyFilePath": "C:\\Keys\\google-translation-key.json"
    }
  }
}
"@ | Out-File -FilePath "$env:APPDATA\Rivet\config.json" -Encoding UTF8
```

### 3. 建置專案

```powershell
# 進入專案目錄（請替換為您的實際路徑）
cd <your-project-path>

# 還原 NuGet 套件
dotnet restore

# 建置專案
dotnet build -c Release

# 發布為單一執行檔
dotnet publish src/Rivet.Console/Rivet.Console.csproj -c Release -o ./publish
```

### 4. 設定快捷鍵

1. 在桌面或任意位置建立 `Rivet.exe` 的捷徑
2. 右鍵點選捷徑 → 內容
3. 在「快速鍵」欄位按下 `Ctrl + Alt + .`
4. 點選「確定」儲存

**注意**：捷徑必須放在桌面或「開始」功能表中，快捷鍵才會生效。

---

## 使用方式

### 基本操作

1. **複製文字**：在任何應用程式中選取文字並複製（Ctrl+C）
2. **按下快捷鍵**：按 `Ctrl + Alt + .` 開啟 Rivet
3. **選擇功能**：
   - 按 `1`：翻譯成繁體中文
   - 按 `2`：翻譯成英文
   - 按 `Esc`：取消並關閉
4. **貼上結果**：翻譯完成後，按 Ctrl+V 即可貼上翻譯結果

### 範例流程

```
[使用者操作]
1. 在網頁上選取英文段落
2. 按 Ctrl+C 複製
3. 按 Ctrl+Alt+. 開啟 Rivet

[Rivet 顯示]
┌────────────────────────────┐
│  請選擇功能：              │
│  [1] 翻譯成繁體中文        │
│  [2] 翻譯成英文            │
│  按 Esc 取消               │
└────────────────────────────┘

[使用者操作]
4. 按數字鍵 1

[Rivet 顯示]
翻譯完成，結果已複製到剪貼簿
[程式自動結束]

[使用者操作]
5. 在目標位置按 Ctrl+V 貼上翻譯結果
```

---

## 錯誤處理

| 訊息 | 原因 | 解決方式 |
|------|------|----------|
| 剪貼簿內容為空無法翻譯 | 剪貼簿沒有文字內容 | 先複製文字再執行翻譯 |
| 無法翻譯非純文字 | 剪貼簿包含圖片等非文字內容 | 確認複製的是純文字 |
| 找不到設定檔 | config.json 不存在 | 依照安裝步驟建立設定檔 |
| 找不到翻譯服務金鑰檔 | 金鑰檔路徑錯誤或檔案不存在 | 確認 config.json 中的路徑正確 |
| 翻譯服務認證失敗 | 金鑰無效或已過期 | 重新產生並更換金鑰檔 |
| 無法連線到翻譯服務 | 網路問題 | 檢查網路連線 |

---

## 日誌檔案位置

日誌檔案位於：`%AppData%\Rivet\logs\`

```powershell
# 檢視日誌
Get-Content "$env:APPDATA\Rivet\logs\rivet-*.log" -Tail 50
```

---

## 開發者快速參考

### 專案結構

```
src/
├── Rivet.Console/       # 進入點與 UI
├── Rivet.Service/       # 業務邏輯（介面定義）
└── Rivet.Infrastructure/ # 具體實作
```

### 執行測試

```powershell
dotnet test
```

### 新增指令處理器

1. 在 `Rivet.Service/Commands/` 建立新的 Handler 類別
2. 繼承 `CommandHandlerBase`
3. 實作 `CanHandle` 和 `ExecuteAsync`
4. 在 DI 容器註冊並加入責任鏈

---

## 常見問題

### Q: 快捷鍵沒有反應？

A: 確認：
1. 捷徑放在桌面或開始功能表
2. 沒有其他程式佔用相同快捷鍵
3. 試著將捷徑以系統管理員身分執行

### Q: 翻譯結果不準確？

A: 本工具使用 Google Cloud Translation API，翻譯品質取決於 Google 服務。若需更高品質翻譯，可考慮：
1. 檢查來源文字是否有誤
2. 分段翻譯較長的文字

### Q: 如何查看詳細的除錯資訊？

A: 日誌檔案中包含 Trace 層級的詳細資訊，包括剪貼簿內容。位置：`%AppData%\Rivet\logs\`
