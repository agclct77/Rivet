# 快速開始：Rivet WPF 手動啟動剪貼簿翻譯

**Branch**: `002-wpf-shell`  
**Date**: 2025-12-10

## 前置條件
- Windows 10/11，.NET 8 SDK（發行為 self-contained 可不另裝 Runtime）。
- 已準備 Google Cloud Translation API 金鑰，並在 `%AppData%/Rivet/config.json` 設定 `services.googleTranslation.keyFilePath`。
- 具備網路連線，可呼叫 Google Translation API。

建立設定檔範例：
```powershell
New-Item -ItemType Directory -Force -Path "$env:APPDATA/Rivet" | Out-Null
@"
{
	"services": {
		"googleTranslation": {
			"keyFilePath": "C:\\Keys\\google-translation-key.json"
		}
	}
}
"@ | Out-File -FilePath "$env:APPDATA/Rivet/config.json" -Encoding UTF8
```

## 建置
```powershell
# 還原套件
cd d:/Lab/Rivet
 dotnet restore

# 建置 WPF 專案（Debug）
dotnet build src/Rivet.WPF/Rivet.WPF.csproj -c Debug

# 執行既有測試（Service/Infrastructure/Console）；未來 WPF UI 測試加入後同步執行
dotnet test tests/Rivet.Service.Tests/Rivet.Service.Tests.csproj
dotnet test tests/Rivet.Infrastructure.Tests/Rivet.Infrastructure.Tests.csproj
dotnet test tests/Rivet.Console.Tests/Rivet.Console.Tests.csproj
```

## 發行
```powershell
# 發行 x64 自含單檔可執行檔（Release）
dotnet publish src/Rivet.WPF/Rivet.WPF.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true -o ./publish/wpf
```
輸出範例：`publish/wpf/Rivet.WPF.exe`（單一檔案，可攜）

## 執行
```powershell
# 從建置輸出啟動
dotnet run --project src/Rivet.WPF/Rivet.WPF.csproj -c Debug
# 或使用發行輸出
./publish/wpf/Rivet.WPF.exe
```

## 使用流程
1. 複製欲翻譯的文字至剪貼簿。
2. 啟動 `Rivet.WPF`，主視窗顯示目前剪貼簿狀態。
3. 點擊「英→中」或「中→英」按鈕觸發翻譯。
4. 翻譯完成後，譯文會顯示在結果區域並回寫剪貼簿，可直接貼上使用。
5. 若剪貼簿為空/非文字或翻譯失敗，UI 會顯示錯誤提示並允許重試。

## 日誌
- 路徑：`%AppData%/Rivet/logs/`
- 格式與 Console 版一致，建議在除錯時檢視最新檔案 `rivet-*.log`。

## 驗收檢查
- 剪貼簿含英文/中文文字時，對應按鈕可正常翻譯並顯示譯文、回寫剪貼簿。
- 剪貼簿空白、圖片或超過 20,000 字元時，顯示友善錯誤且程式不中斷，並保留 UI 可重試。
- `dotnet build` 與 `dotnet publish` 成功產出可執行檔並可啟動流程；日誌與設定路徑與 Console 版一致。
- 日誌不應落地剪貼簿/譯文字串（僅記錄狀態碼與錯誤）。

