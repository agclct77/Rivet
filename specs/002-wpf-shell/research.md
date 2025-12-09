# Research Findings

## WPF UI 自動化測試選型
- Decision: 採用 FlaUI(UIA3) 搭配 NUnit，在 `tests/Rivet.WPF.Tests` 建立端到端 UI 測試；測試類別以 `[Apartment(ApartmentState.STA)]`、`Parallelizable(ParallelScope.None)` 確保單執行緒並與現有測試框架一致。
- Rationale: FlaUI 直接基於 UI Automation，對 WPF 控制項支援成熟、相容 .NET 8，無需額外驅動服務，與既有 NUnit 生態整合簡單且在 CI 上可重現。
- Alternatives considered: WinAppDriver/Appium（需額外服務且已停維護，易 flake）、Playwright+WinAppDriver（間接依賴，維護成本高）、XamlTest/MSTest 耦合較深、TestStack.White 已過時。

## Generic Host 與 DI 共用（僅供 WPF，Console 不再相容）
- Decision: 建立 WPF 專用的組合根（例如 `RivetHostBuilder`）使用 `Host.CreateDefaultBuilder`/`UseSerilog`，載入 `%AppData%/Rivet/config.json`，統一註冊 Service/Infrastructure/Logging；`App.xaml.cs` 在 `Main` 建立/啟動 Host，`OnExit` 停止並釋放。未來 Console 作廢，不再要求共用或相容。
- Rationale: 單一組合根避免多份設定漂移；Serilog/設定一次配置即被 WPF 使用；Host 生命週期管理與 Options/Logging 整合優於手工容器，且可移除 Console 遺留耦合，專注 WPF。
- Alternatives considered: (1) 只抽 `IServiceCollection` 擴充方法但不使用 Host，需手動管理 logging/設定生命週期；(2) 保留 Console 相容的雙路註冊，會增加維護成本且與「Console 作廢」目標相違；(3) HostApplicationBuilder 語法簡潔但與 Host.CreateDefaultBuilder 差異不大，可視實作偏好擇一。

## 剪貼簿與翻譯流程（STA 安全）
- Decision: UI 按鈕 `async/await` 觸發；背景任務處理剪貼簿讀取（含 3–5 次重試）與翻譯呼叫，完成後透過 Dispatcher 回 UI 更新狀態並在 UI 執行緒寫回剪貼簿（同樣重試）。空白/非文字/圖片情境直接回傳友善訊息，不崩潰。
- Rationale: 剪貼簿要求 STA，寫回需在 UI 執行緒；翻譯/網路 I/O 需移出 UI 以避免凍結；重試可減少剪貼簿被鎖定時的失敗；流程符合憲章的可回應性與穩定性。
- Alternatives considered: (1) 全流程在 UI 執行緒導致延遲卡頓；(2) 全流程 ThreadPool 破壞 STA 導致剪貼簿失敗；(3) 不做重試會在剪貼簿佔用時大量失敗。

## WPF 通知策略
- Decision: 為 WPF 實作 `IUserNotifier`（例如狀態區塊 + MessageBox/對話框），呈現成功/錯誤訊息並避免使用 ConsoleNotifier；UI 應可顯示持續訊息與錯誤重試提示。
- Rationale: 憲章要求一致的使用者回饋與可重試性，ConsoleNotifier 不適用於 WPF；專屬 notifier 可集中處理剪貼簿空白、非文字、翻譯失敗等提示並避免阻塞主流程。
- Alternatives considered: (1) 只用 MessageBox，同步阻塞且易干擾流程；(2) 直接在視窗標題顯示訊息，訊息密度不足且不易被注意。
