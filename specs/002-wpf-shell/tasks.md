---

description: "Tasks for Rivet WPF 手動啟動剪貼簿翻譯"
---

# Tasks: Rivet WPF 手動啟動剪貼簿翻譯

**Input**: Design documents from `/specs/002-wpf-shell/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/
> 本文件須以繁體中文撰寫（依憲章語言規範）。

**Tests**: 憲章要求新功能覆蓋率 ≥80%，需涵蓋剪貼簿/翻譯管線、錯誤處理與建置/發行流程；WPF UI 自動化採 FlaUI(UIA3)+NUnit，測試以 STA 並行度 None 執行。

**Organization**: Tasks 依使用者故事分組，確保每一故事可獨立實作與驗證。

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: 建立 WPF 專案與測試骨架，納入現有解決方案。

- [x] T001 建立 WPF 專案骨架 `src/Rivet.WPF/Rivet.WPF.csproj`（含 `App.xaml`、`MainWindow.xaml` 初始檔）
- [x] T002 將 `src/Rivet.WPF/Rivet.WPF.csproj` 加入 `Rivet.sln` 並參考 `Rivet.Service`、`Rivet.Infrastructure`
- [x] T003 [P] 建立 WPF UI 測試專案 `tests/Rivet.WPF.Tests/Rivet.WPF.Tests.csproj`（新增 NUnit、FlaUI.UIA3 相依並參考 WPF 專案）
- [x] T004 [P] 初始化 WPF 專案資源/設定目錄 `src/Rivet.WPF/Properties/`（含 `AssemblyInfo.cs`、`Resources.resx`、`app.manifest` 佈局）

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: 建立可重用的組合根、DI、設定與基礎 ViewModel/版面，解鎖後續故事。

- [ ] T005 建立 Host 組合根 `src/Rivet.WPF/Hosting/RivetWpfHostBuilder.cs`，載入 `%AppData%/Rivet/config.json`、配置 Serilog、呼叫現有 Service/Infrastructure 註冊
- [ ] T006 [P] 建立 DI 擴充 `src/Rivet.WPF/Extensions/ServiceCollectionExtensions.cs` 將翻譯/剪貼簿/設定服務與 WPF 專屬型別註冊入容器
- [ ] T007 [P] 串接應用進入點 `src/Rivet.WPF/App.xaml.cs`：啟動 Host、以 DI 建立 `MainWindow`、`OnExit` 釋放資源
- [ ] T008 建立 WPF 視圖狀態與基底 ViewModel `src/Rivet.WPF/ViewModels/WpfTranslationViewState.cs`、`src/Rivet.WPF/ViewModels/MainWindowViewModel.cs`（含方向/忙碌/錯誤欄位）
- [ ] T009 [P] 佈局主視窗基礎 XAML `src/Rivet.WPF/MainWindow.xaml`（綁定來源/譯文/錯誤區塊與按鈕佈局，暫用樣式）

---

## Phase 3: User Story 1 - 手動啟動並翻譯剪貼簿 (Priority: P1) 🎯 MVP

**Goal**: 使用者點擊翻譯按鈕即可將剪貼簿文字翻譯並回寫剪貼簿，同步在 UI 顯示結果。

**Independent Test**: 啟動 WPF、剪貼簿填入中/英文文字、分別點擊「英→中」「中→英」，確認 UI 顯示譯文且剪貼簿被替換。

### Tests for User Story 1

- [ ] T010 [P] [US1] 新增翻譯成功情境測試（英→中）於 `tests/Rivet.WPF.Tests/TranslationHappyPathTests.cs`（FlaUI 驅動 UI，驗證剪貼簿被覆寫）
- [ ] T011 [P] [US1] 新增翻譯成功情境測試（中→英）於 `tests/Rivet.WPF.Tests/TranslationHappyPathTests.cs`（STA 單執行緒，驗證 UI 顯示譯文）

### Implementation for User Story 1

- [ ] T012 [P] [US1] 實作翻譯命令邏輯 `src/Rivet.WPF/Commands/TranslateCommand.cs`（讀剪貼簿、建構 `TranslationRequest`、呼叫翻譯服務）
- [ ] T013 [US1] 完成 `MainWindowViewModel` 翻譯流程 `src/Rivet.WPF/ViewModels/MainWindowViewModel.cs`（async/await、狀態更新、UI 執行緒寫回剪貼簿）
- [ ] T014 [US1] 綁定主視窗按鈕與狀態 `src/Rivet.WPF/MainWindow.xaml`（顯示來源/譯文、忙碌指示、剪貼簿回寫結果）
- [ ] T015 [US1] 加入翻譯流程日誌/遙測 `src/Rivet.WPF/Logging/TranslationLoggingExtensions.cs`（記錄方向、延遲、結果碼，避免落地敏感字串）

**Checkpoint**: User Story 1 可獨立操作並驗收。

---

## Phase 4: User Story 2 - 友善錯誤提示與不中斷 (Priority: P2)

**Goal**: 剪貼簿空白/非文字/翻譯失敗時提供清楚提示且應用不中斷，可重試。

**Independent Test**: 模擬空白、圖片、翻譯服務故障，確認 UI 顯示 zh-TW 錯誤訊息且可再次點擊按鈕，剪貼簿內容未被破壞。

### Tests for User Story 2

- [ ] T016 [P] [US2] 新增剪貼簿空白/非文字情境測試 `tests/Rivet.WPF.Tests/ClipboardErrorTests.cs`（FlaUI 驗證錯誤提示、未改寫剪貼簿）
- [ ] T017 [P] [US2] 新增翻譯服務失敗情境測試 `tests/Rivet.WPF.Tests/TranslationFailureTests.cs`（模擬服務例外，驗證 UI 可重試）
- [ ] T030 [P] [US2] 新增 RTF/HTML 降階提示測試 `tests/Rivet.WPF.Tests/ClipboardFormatDowngradeTests.cs`（驗證僅取純文字且提示，剪貼簿未被破壞）

### Implementation for User Story 2

- [ ] T018 [US2] 補齊剪貼簿驗證/長文字拒絕邏輯 `src/Rivet.WPF/ViewModels/MainWindowViewModel.cs`（狀態=Empty/NonText/Image/Rtf/TooLong 時回傳錯誤）
- [ ] T019 [US2] 實作 WPF 專用通知器 `src/Rivet.WPF/Notification/WpfUserNotifier.cs`（顯示 zh-TW 訊息區/對話框，取代 ConsoleNotifier）
- [ ] T020 [US2] 建立錯誤訊息對應與 UI 呈現 `src/Rivet.WPF/MainWindow.xaml`、`src/Rivet.WPF/Resources/ErrorMessages.resx`（含剪貼簿鎖定、回寫失敗提示）
- [ ] T021 [US2] 加入剪貼簿讀寫重試與防凍結處理 `src/Rivet.WPF/Clipboard/ClipboardAdapter.cs`（UI 執行緒寫回、背景讀取，含退避重試）
- [ ] T031 [US2] 強化 RTF/HTML 降階處理與提示 `src/Rivet.WPF/ViewModels/MainWindowViewModel.cs`、`src/Rivet.WPF/MainWindow.xaml`（保留原剪貼簿、UI 顯示降階訊息）
- [ ] T032 [US2] 完成 UI 可及性基線（鍵盤操作、焦點樣式、對比度與字級）`src/Rivet.WPF/MainWindow.xaml`、`src/Rivet.WPF/Resources/`（符合 WCAG 2.1 AA 基線）

**Checkpoint**: User Story 2 可獨立驗收（錯誤情境全覆蓋）。

---

## Phase 5: User Story 3 - 可建置與發行 (Priority: P3)

**Goal**: 使用標準命令可建置/發行 WPF，產生 self-contained 單檔可執行檔。

**Independent Test**: 執行 `dotnet build`、`dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true`，取得可啟動的 `Rivet.WPF.exe` 並通過主流程。

### Tests for User Story 3

- [ ] T022 [P] [US3] 建置/發行驗證腳本與測試 `tests/Rivet.WPF.Tests/BuildPublishVerification.cs`（呼叫 build/publish，檢查輸出與啟動返回碼）

### Implementation for User Story 3

- [ ] T023 [US3] 新增發行設定 `src/Rivet.WPF/Properties/PublishProfiles/win-x64-singlefile.pubxml`（self-contained、SingleFile、win-x64）
- [ ] T024 [P] [US3] 撰寫本地發行腳本 `scripts/publish-wpf.ps1`（包裝 quickstart 命令，輸出至 `publish/wpf`）
- [ ] T025 [US3] 更新 `quickstart.md` WPF 區段與 `README.md` 建置/發行指引（含設定檔需求與日誌路徑）

**Checkpoint**: User Story 3 可獨立驗收並交付執行檔。

---

## Final Phase: Polish & Cross-Cutting Concerns

**Purpose**: 加強文件、品質與性能，覆蓋跨故事事項。

- [ ] T026 [P] 補充性能/延遲度量記錄與日誌樣板 `src/Rivet.WPF/Logging/PerformanceMetrics.cs`（量測啟動與翻譯延遲、記憶體峰值）
- [ ] T027 清理重複程式與可讀性調整 `src/Rivet.WPF/`（VM/命令/通知器重構、移除未使用相依）
- [ ] T028 [P] 擴充文件與驗收檢核 `specs/002-wpf-shell/quickstart.md`、`specs/002-wpf-shell/checklists/requirements.md`（核對 FR/SC 與實作）
- [ ] T029 執行 quickstart 驗證與最終冒煙測試（依 quickstart.md 流程）
- [ ] T033 [P] 建立性能驗證腳本/測試 `tests/Rivet.WPF.Tests/PerformanceVerification.cs`（檢查啟動與翻譯延遲 p95、記憶體峰值 <150MB，含閾值斷言）
- [ ] T034 建立覆蓋率報告流程並設定 80% 驗收 gate（整合 `dotnet test /p:CollectCoverage=true` 或等效，於 CI/本地輸出報告並在驗收清單記錄）
- [ ] T035 完成可及性檢核清單/測試（對比、鍵盤/焦點操作、最小字級，必要時採 FlaUI 驗證並記錄例外）

---

## Dependencies & Execution Order

- Setup (Phase 1) → Foundational (Phase 2) → User Story 1 (Phase 3) → User Story 2 (Phase 4) → User Story 3 (Phase 5) → Polish (Final Phase)
- User story順序：US1 (P1) 優先完成，US2 (P2) 可在完成 Foundational 後與 US1 並行但須保證獨立驗收，US3 (P3) 需 Foundational 完成且可與 US2 並行。
- Story 內順序：先測試，再模型/命令，再 ViewModel，再 XAML/通知/重試，再日誌/性能。

## Parallel Execution Examples

- US1 可並行：T010 與 T011 (測試)；T012 與 T015 (命令 vs 日誌) 可並行，完成後再做 T013/T014。
- US2 可並行：T016 與 T017 (測試)；T019 與 T021 (通知器 vs 讀寫重試) 可並行，整合於 T018/T020 後驗收。
- US3 可並行：T022 (驗證) 與 T024 (腳本) 可並行，T023 需先完成後 T022 才能跑完整流程。

## Implementation Strategy

1. MVP：完成 Phase 1+2 後先交付 US1（T010–T015），確保翻譯與剪貼簿回寫可演示。
2. Incremental：依序加入 US2（錯誤提示）、US3（建置/發行），每階段皆可單獨驗收與 demo。
3. Parallel：多人時 Phase 2 完成後可分工 US1/US2/US3，避免同檔衝突（命令/VM/通知/腳本 分檔）。
