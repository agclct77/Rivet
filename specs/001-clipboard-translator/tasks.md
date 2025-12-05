# Tasks: 剪貼簿翻譯工具

**Input**: Design documents from `/specs/001-clipboard-translator/`
**Prerequisites**: plan.md ✅, spec.md ✅, research.md ✅, data-model.md ✅, contracts/interfaces.md ✅, quickstart.md ✅

## Format: `[ID] [P?] [Story?] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

```text
src/
├── Rivet.Console/           # 前端專案（Console UI）
├── Rivet.Service/           # 業務邏輯專案
└── Rivet.Infrastructure/    # 基礎設施專案
tests/
├── Rivet.Service.Tests/     # Service 層單元測試
├── Rivet.Infrastructure.Tests/  # Infrastructure 層整合測試
└── Rivet.Console.Tests/     # Console 層測試
```

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: 專案初始化與基本結構

- [x] T001 建立方案檔 src/Rivet.sln
- [x] T002 [P] 建立 Console 專案 src/Rivet.Console/Rivet.Console.csproj（.NET 8, UseWindowsForms, [STAThread]）
- [x] T003 [P] 建立 Service 專案 src/Rivet.Service/Rivet.Service.csproj（.NET 8 類別庫）
- [x] T004 [P] 建立 Infrastructure 專案 src/Rivet.Infrastructure/Rivet.Infrastructure.csproj（.NET 8 類別庫）
- [x] T005 設定專案參考：Console → Service, Console → Infrastructure, Infrastructure → Service
- [x] T006 [P] 安裝 NuGet 套件至 Rivet.Console：Spectre.Console, Microsoft.Extensions.DependencyInjection, Serilog, Serilog.Sinks.File
- [x] T007 [P] 安裝 NuGet 套件至 Rivet.Infrastructure：Google.Cloud.Translation.V2, Microsoft.Extensions.DependencyInjection.Abstractions

---

## Phase 1.5: Test Infrastructure

**Purpose**: 建立測試專案結構，符合 Constitution II Testing Standards

- [x] T007a [P] 建立 Service 測試專案 tests/Rivet.Service.Tests/Rivet.Service.Tests.csproj（NUnit, NSubstitute）
- [x] T007b [P] 建立 Infrastructure 測試專案 tests/Rivet.Infrastructure.Tests/Rivet.Infrastructure.Tests.csproj（NUnit）
- [x] T007c 設定測試專案參考：Service.Tests → Service, Infrastructure.Tests → Infrastructure, Service
- [x] T007d [P] 安裝 NuGet 套件至測試專案：NUnit, NUnit3TestAdapter, NSubstitute, Microsoft.NET.Test.Sdk, coverlet.collector

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: 所有 User Story 共用的核心基礎設施

**⚠️ CRITICAL**: 此階段必須完成後，才能開始任何 User Story 的實作

- [x] T008 建立 ClipboardStatus 列舉，位於 src/Rivet.Service/Clipboard/ClipboardStatus.cs
- [x] T009 [P] 建立 ClipboardContent record in src/Rivet.Service/Clipboard/ClipboardContent.cs
- [x] T010 [P] 建立 TargetLanguage 列舉 in src/Rivet.Service/Translation/TargetLanguage.cs
- [x] T011 [P] 建立 TranslationRequest record in src/Rivet.Service/Translation/TranslationRequest.cs
- [x] T012 [P] 建立 TranslationResult record in src/Rivet.Service/Translation/TranslationResult.cs
- [x] T013 [P] 建立 CommandType 列舉 in src/Rivet.Service/Commands/CommandType.cs
- [x] T014 [P] 建立 CommandContext record in src/Rivet.Service/Commands/CommandContext.cs
- [x] T015 [P] 建立 CommandResult record in src/Rivet.Service/Commands/CommandResult.cs
- [x] T016 [P] 建立 AppConfiguration 相關 records in src/Rivet.Service/Configuration/AppConfiguration.cs
- [x] T017 [P] 建立 ConfigurationResult record in src/Rivet.Service/Configuration/ConfigurationResult.cs
- [x] T018 建立 IClipboardService 介面 in src/Rivet.Service/Clipboard/IClipboardService.cs
- [x] T019 [P] 建立 ITranslationService 介面 in src/Rivet.Service/Translation/ITranslationService.cs
- [x] T020 [P] 建立 IConfigurationService 介面 in src/Rivet.Service/Configuration/IConfigurationService.cs
- [x] T021 [P] 建立 IUserNotifier 介面 in src/Rivet.Service/Notification/IUserNotifier.cs
- [x] T022 [P] 建立 ICommandHandler 介面 in src/Rivet.Service/Commands/ICommandHandler.cs
- [x] T023 建立 CommandHandlerBase 抽象類別 in src/Rivet.Service/Commands/CommandHandlerBase.cs
- [x] T024 實作 WindowsClipboardService in src/Rivet.Infrastructure/Clipboard/WindowsClipboardService.cs
- [x] T025 [P] 實作 JsonConfigurationService in src/Rivet.Infrastructure/Configuration/JsonConfigurationService.cs
- [x] T026 [P] 實作 GoogleTranslationService in src/Rivet.Infrastructure/Translation/GoogleTranslationService.cs
- [x] T027 [P] 設定 Serilog 日誌 in src/Rivet.Infrastructure/Logging/SerilogSetup.cs（含 Trace 層級設定，用於記錄剪貼簿內容）
- [x] T028 [P] 實作 ConsoleUserNotifier in src/Rivet.Console/Notification/ConsoleUserNotifier.cs
- [x] T028a [P] 撰寫 ClipboardContent 單元測試 in tests/Rivet.Service.Tests/Clipboard/ClipboardContentTests.cs
- [x] T028b [P] 撰寫 TranslationResult 單元測試 in tests/Rivet.Service.Tests/Translation/TranslationResultTests.cs

**Checkpoint**: Foundation ready - 可以開始實作各 User Story

---

## Phase 3: User Story 3 - 透過快捷鍵開啟功能選單 (Priority: P1) 🎯 MVP

**Goal**: 使用者可透過 Ctrl+Alt+. 開啟 Rivet 並看到功能選單，可按數字鍵選擇功能或 Esc 取消

**Independent Test**: 執行程式，驗證顯示功能選單，按 Esc 可正常關閉程式

### Implementation for User Story 3

- [x] T029 [US3] 建立 MainMenu 選單顯示類別 in src/Rivet.Console/Menu/MainMenu.cs（使用 Spectre.Console 顯示選單，支援數字鍵 1/2 選擇與 Esc 取消）
- [x] T030 [US3] 建立 CancelCommandHandler 處理器 in src/Rivet.Service/Commands/CancelCommandHandler.cs（處理 Esc 取消操作）
- [x] T031 [US3] 建立 CommandChain 責任鏈管理器 in src/Rivet.Service/Commands/CommandChain.cs
- [x] T032 [US3] 實作 Program.cs 進入點 in src/Rivet.Console/Program.cs（DI 設定、選單流程、0.5 秒延遲結束）
- [x] T032a [US3] 撰寫 CommandChain 單元測試 in tests/Rivet.Service.Tests/Commands/CommandChainTests.cs

**Checkpoint**: User Story 3 完成 - 可執行程式並看到選單、按 Esc 可取消關閉

---

## Phase 4: User Story 1 - 將剪貼簿英文內容翻譯成繁體中文 (Priority: P1)

**Goal**: 使用者可將剪貼簿中的英文內容翻譯成繁體中文

**Independent Test**: 複製一段英文文字，執行程式按 1，驗證剪貼簿已更新為繁體中文翻譯結果

### Implementation for User Story 1

- [x] T033 [US1] 建立 TranslateToChineseHandler 處理器 in src/Rivet.Service/Commands/TranslateToChineseHandler.cs（整合 IClipboardService, ITranslationService, IUserNotifier）
- [x] T034 [US1] 在 Program.cs 註冊 TranslateToChineseHandler 到責任鏈 in src/Rivet.Console/Program.cs
- [x] T035 [US1] 驗證完整流程：讀取剪貼簿 → 翻譯成繁中 → 寫入剪貼簿 → 顯示成功訊息
- [x] T035a [US1] 撰寫 TranslateToChineseHandler 單元測試 in tests/Rivet.Service.Tests/Commands/TranslateToChineseHandlerTests.cs

**Checkpoint**: User Story 1 完成 - 可將英文翻譯成繁體中文

---

## Phase 5: User Story 2 - 將剪貼簿繁體中文內容翻譯成英文 (Priority: P1)

**Goal**: 使用者可將剪貼簿中的繁體中文內容翻譯成英文

**Independent Test**: 複製一段繁體中文文字，執行程式按 2，驗證剪貼簿已更新為英文翻譯結果

### Implementation for User Story 2

- [x] T036 [US2] 建立 TranslateToEnglishHandler 處理器 in src/Rivet.Service/Commands/TranslateToEnglishHandler.cs（整合 IClipboardService, ITranslationService, IUserNotifier）
- [x] T037 [US2] 在 Program.cs 註冊 TranslateToEnglishHandler 到責任鏈 in src/Rivet.Console/Program.cs
- [x] T038 [US2] 驗證完整流程：讀取剪貼簿 → 翻譯成英文 → 寫入剪貼簿 → 顯示成功訊息
- [x] T038a [US2] 撰寫 TranslateToEnglishHandler 單元測試 in tests/Rivet.Service.Tests/Commands/TranslateToEnglishHandlerTests.cs

**Checkpoint**: User Story 2 完成 - 可將繁體中文翻譯成英文

---

## Phase 6: User Story 4 - 處理非純文字剪貼簿內容 (Priority: P2) ✅

**Goal**: 當剪貼簿包含非純文字內容時，顯示清楚的錯誤訊息

**Note**: 考慮將 US4/US5 的驗證邏輯提取至 `CommandHandlerBase` 或共用方法，避免重複程式碼

**Independent Test**: 複製一張圖片，執行程式選擇翻譯，驗證顯示「無法翻譯非純文字」訊息

### Implementation for User Story 4

- [x] T039 [US4] 在 TranslateToChineseHandler 加入非純文字檢查邏輯 in src/Rivet.Service/Commands/TranslateToChineseHandler.cs
- [x] T040 [US4] 在 TranslateToEnglishHandler 加入非純文字檢查邏輯 in src/Rivet.Service/Commands/TranslateToEnglishHandler.cs

**Checkpoint**: User Story 4 完成 - 非純文字內容顯示正確錯誤訊息 ✅

---

## Phase 7: User Story 5 - 處理空白剪貼簿 (Priority: P2)

**Goal**: 當剪貼簿為空時，顯示清楚的錯誤訊息

**Independent Test**: 清空剪貼簿，執行程式選擇翻譯，驗證顯示「剪貼簿內容為空無法翻譯」訊息

### Implementation for User Story 5

- [x] T041 [US5] 在 TranslateToChineseHandler 加入空白檢查邏輯 in src/Rivet.Service/Commands/TranslateToChineseHandler.cs
- [x] T042 [US5] 在 TranslateToEnglishHandler 加入空白檢查邏輯 in src/Rivet.Service/Commands/TranslateToEnglishHandler.cs

**Checkpoint**: User Story 5 完成 - 空白剪貼簿顯示正確錯誤訊息 ✅

---

## Phase 8: Polish & Cross-Cutting Concerns

**Purpose**: 改善整體品質與完成度

- [x] T045 [P] 確認設定檔不存在時顯示正確錯誤訊息（含預期路徑）
- [x] T046 [P] 確認金鑰檔不存在時顯示正確錯誤訊息（含路徑）
- [x] T047 處理翻譯服務連線錯誤，保留原始剪貼簿內容
- [x] T048 [P] 程式碼清理與重構
- [x] T048a 驗證責任鏈擴充性：確認新增指令處理器只需新增檔案，不需修改既有程式碼
- [x] T049 執行 quickstart.md 驗證流程
- [x] T050 ~~建立 Windows 捷徑說明文件~~ (已知問題：Windows 快速鍵設定複雜，待重新規劃解決方案)
- [x] T051 [P] 撰寫 README.md in src/README.md（專案說明、安裝步驟、使用方式、專案結構）

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: 無依賴 - 可立即開始
- **Foundational (Phase 2)**: 依賴 Setup 完成 - **阻擋所有 User Stories**
- **User Story 3 (Phase 3)**: 依賴 Foundational 完成 - 選單基礎（MVP 核心）
- **User Story 1 (Phase 4)**: 依賴 Phase 3 完成（選單必須先能顯示）
- **User Story 2 (Phase 5)**: 依賴 Phase 3 完成，可與 Phase 4 平行
- **User Story 4 (Phase 6)**: 依賴 Phase 4 & 5 完成（需要已有翻譯功能）
- **User Story 5 (Phase 7)**: 依賴 Phase 4 & 5 完成，可與 Phase 6 平行
- **Polish (Phase 8)**: 依賴所有 User Stories 完成

### User Story Dependencies

```text
Phase 1 (Setup)
    │
    ▼
Phase 2 (Foundational) ─── BLOCKS ALL USER STORIES
    │
    ▼
Phase 3 (US3: 選單) ◄─── MVP 起點
    │
    ├──────────────┐
    ▼              ▼
Phase 4 (US1)   Phase 5 (US2) ◄─── 可平行
    │              │
    └──────┬───────┘
           │
    ┌──────┴───────┐
    ▼              ▼
Phase 6 (US4)   Phase 7 (US5) ◄─── 可平行
    │              │
    └──────┬───────┘
           │
           ▼
     Phase 8 (Polish)
```

### Parallel Opportunities

**Phase 1 (Setup)**:

```bash
# 可平行：T002, T003, T004
Task: "建立 Console 專案"
Task: "建立 Service 專案"
Task: "建立 Infrastructure 專案"

# 可平行：T006, T007
Task: "安裝 NuGet 套件至 Console"
Task: "安裝 NuGet 套件至 Infrastructure"
```

**Phase 2 (Foundational)**:

```bash
# 可平行：T009-T017 (所有 models 和 records)
Task: "建立 ClipboardContent record"
Task: "建立 TargetLanguage 列舉"
Task: "建立 TranslationRequest record"
Task: "建立 TranslationResult record"
...

# 可平行：T018-T022 (所有介面)
Task: "建立 IClipboardService 介面"
Task: "建立 ITranslationService 介面"
Task: "建立 IConfigurationService 介面"
...

# 可平行：T024-T028 (基礎設施實作)
Task: "實作 WindowsClipboardService"
Task: "實作 JsonConfigurationService"
Task: "實作 GoogleTranslationService"
Task: "設定 Serilog 日誌"
Task: "實作 ConsoleUserNotifier"
```

**User Stories 4 & 5 可平行**:

```bash
Task: "US4 非純文字檢查"
Task: "US5 空白檢查"
```

---

## Implementation Strategy

### MVP First (Phase 1-3 + Phase 4)

1. Complete Phase 1: Setup（專案建立）
2. Complete Phase 2: Foundational（核心基礎設施）
3. Complete Phase 3: User Story 3（選單顯示）
4. Complete Phase 4: User Story 1（英翻中）
5. **STOP and VALIDATE**: 測試基本翻譯流程
6. 可部署/展示 MVP

### Incremental Delivery

1. Setup + Foundational → Foundation ready
2. Add User Story 3 (選單) → 可執行程式、顯示選單
3. Add User Story 1 (英翻中) → **MVP Ready!** 可部署/展示
4. Add User Story 2 (中翻英) → 完整翻譯功能
5. Add User Story 4 & 5 (錯誤處理) → 完善使用者體驗
6. Polish → 最終版本

---

## Notes

- [P] tasks = 不同檔案，無依賴關係
- [Story] label 將 task 對應到特定 User Story 以便追蹤
- 每個 User Story 應可獨立完成並測試
- 每個 task 或邏輯群組完成後 commit
- 在任何 Checkpoint 可停下來獨立驗證該 Story
- 避免：模糊的 task、同檔案衝突、破壞獨立性的跨 Story 依賴
