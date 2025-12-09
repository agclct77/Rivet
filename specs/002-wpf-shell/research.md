# Research Findings

## DI 共用註冊與可重用性
- Decision: 抽出 `IServiceCollection` 擴充方法（預計命名 `AddRivetCoreServices`）封裝 Serilog 初始化、組態服務、翻譯服務與剪貼簿服務的註冊，由 Console 與 WPF 共用；UI 端僅需額外註冊對應的 `IUserNotifier` 與 UI 入口。
- Rationale: 可避免複製 `Program.cs` 的 DI 腳本，確保金鑰讀取、日誌路徑與翻譯服務配置一致，降低維護成本並符合憲章的一致性要求。
- Alternatives considered: (1) WPF 專案直接複製 Console 的 ConfigureServices，易出現設定漂移；(2) 引入 Generic Host 重構整體啟動流程，對現有 Console 影響大且非 M1 必要。

## WPF 執行緒與剪貼簿/翻譯流程
- Decision: 剪貼簿讀寫保持在 UI STA 執行緒；翻譯呼叫與網路等待以 `async/await` 包裝並在背景工作（例如 `Task.Run` 包裹翻譯呼叫）執行，完畢後透過 Dispatcher 回到 UI 執行緒更新結果與狀態。錯誤提示與剪貼簿回寫也在 UI 執行緒完成以避免 STA 衝突。
- Rationale: Win32 剪貼簿 API 需要 STA，直接在 ThreadPool 呼叫可能拋出 COM/剪貼簿開啟失敗；翻譯過程可能阻塞，需移出 UI 執行緒以維持視窗可回應；Dispatcher 確保 UI 控制項與剪貼簿設定同步且不中斷。
- Alternatives considered: (1) 全流程留在 UI 執行緒，當翻譯延遲時 UI 會卡頓；(2) 全流程丟到 ThreadPool，會破壞 STA 要求並導致剪貼簿操作失敗。

## WPF 通知策略
- Decision: 為 WPF 實作 `IUserNotifier`（例如狀態區塊 + MessageBox/對話框），呈現成功/錯誤訊息並避免使用 ConsoleNotifier；UI 應可顯示持續訊息與錯誤重試提示。
- Rationale: 憲章要求一致的使用者回饋與可重試性，ConsoleNotifier 不適用於 WPF；專屬 notifier 可集中處理剪貼簿空白、非文字、翻譯失敗等提示並避免阻塞主流程。
- Alternatives considered: (1) 只用 MessageBox，同步阻塞且易干擾流程；(2) 直接在視窗標題顯示訊息，訊息密度不足且不易被注意。
