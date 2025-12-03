# Implementation Plan: 剪貼簿翻譯工具

**Branch**: `001-clipboard-translator` | **Date**: 2024-12-04 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/001-clipboard-translator/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/commands/plan.md` for the execution workflow.

## Summary

實作一個 Windows 工具箱應用程式，透過快捷鍵（Ctrl+Alt+.）開啟 Console 選單，讓使用者快速將剪貼簿內容翻譯成繁體中文或英文。採用分層架構（Console/Service/Infrastructure）確保前端與翻譯服務的可替換性，並使用責任鏈模式處理指令以支援未來功能擴充。

## Technical Context

**Language/Version**: C# / .NET 8  
**Primary Dependencies**: Spectre.Console (CLI UI)、Google.Cloud.Translation.V2 (翻譯服務)、Serilog (日誌)、Microsoft.Extensions.DependencyInjection (DI)  
**Storage**: %AppData%\Rivet\config.json (設定檔)、%AppData%\Rivet\logs (日誌檔)  
**Testing**: NUnit + NSubstitute  
**Target Platform**: Windows (因使用 CF_UNICODETEXT 剪貼簿格式)
**Project Type**: 多專案方案（Rivet.Console、Rivet.Service、Rivet.Infrastructure）  
**Performance Goals**: 選單顯示 <500ms、翻譯操作 <5s (500 字元以內)、整體執行 <10s  
**Constraints**: 每次執行為獨立實例、翻譯完成後 0.5 秒自動結束  
**Scale/Scope**: 單一使用者桌面工具、2 項翻譯功能、3 個專案結構

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| 原則 | 要求 | 本計劃合規狀態 |
|------|------|----------------|
| I. Code Quality | 可讀性、可維護性、一致性、錯誤處理、Code Review | ✅ 預計：分層架構確保單一職責、所有錯誤路徑明確處理 |
| II. Testing Standards | 80% 覆蓋率、Unit/Integration/Contract 測試 | ✅ 預計：NUnit + NSubstitute 覆蓋所有業務邏輯與介面 |
| III. User Experience Consistency | 一致的互動模式、適當的回饋、響應式 | ✅ 預計：統一的 Console UI、明確的成功/錯誤訊息、0.5s 延遲回饋 |
| IV. Performance Requirements | API 200ms p95、無記憶體洩漏、資源正確釋放 | ✅ 預計：短生命週期實例、using 確保資源釋放、效能目標已定義 |
| V. Language Policy | 技術文件使用繁體中文 | ✅ 所有 spec/plan/tasks 使用繁體中文 |
| VI. Git Commit Convention | Conventional Commits、繁體中文描述 | ✅ 預計遵循：`feat(001-clipboard-translator): 描述` |

**Pre-Phase 0 Gate 評估**: ✅ 通過 — 無違規需要辯護

## Project Structure

### Documentation (this feature)

```text
specs/001-clipboard-translator/
├── spec.md              # 功能規格（已完成）
├── plan.md              # 本文件（/speckit.plan 輸出）
├── research.md          # Phase 0 輸出
├── data-model.md        # Phase 1 輸出
├── quickstart.md        # Phase 1 輸出
├── contracts/           # Phase 1 輸出
│   └── interfaces.md    # 介面合約定義
├── checklists/
│   └── requirements.md  # 需求檢核表（已存在）
└── tasks.md             # Phase 2 輸出（/speckit.tasks 產生）
```

### Source Code (repository root)

```text
src/
├── Rivet.Console/           # 前端專案（Console UI）
│   ├── Program.cs           # 進入點
│   ├── Menu/                # 選單顯示
│   │   └── MainMenu.cs
│   └── Rivet.Console.csproj
│
├── Rivet.Service/           # 業務邏輯專案
│   ├── Commands/            # 責任鏈指令處理
│   │   ├── ICommand.cs
│   │   ├── ICommandHandler.cs
│   │   ├── CommandChain.cs
│   │   ├── TranslateToChineseHandler.cs
│   │   └── TranslateToEnglishHandler.cs
│   ├── Translation/         # 翻譯服務抽象
│   │   ├── ITranslationService.cs
│   │   ├── TranslationRequest.cs
│   │   └── TranslationResult.cs
│   ├── Clipboard/           # 剪貼簿服務抽象
│   │   ├── IClipboardService.cs
│   │   └── ClipboardContent.cs
│   ├── Configuration/       # 設定服務抽象
│   │   └── IConfigurationService.cs
│   └── Rivet.Service.csproj
│
├── Rivet.Infrastructure/    # 基礎設施專案
│   ├── Translation/         # 翻譯實作
│   │   └── GoogleTranslationService.cs
│   ├── Clipboard/           # 剪貼簿實作
│   │   └── WindowsClipboardService.cs
│   ├── Configuration/       # 設定實作
│   │   └── JsonConfigurationService.cs
│   ├── Logging/             # 日誌設定
│   │   └── SerilogSetup.cs
│   └── Rivet.Infrastructure.csproj
│
└── Rivet.sln                # 方案檔

tests/
├── Rivet.Service.Tests/     # Service 層單元測試
│   ├── Commands/
│   │   └── CommandChainTests.cs
│   ├── Translation/
│   │   └── TranslationServiceTests.cs
│   └── Rivet.Service.Tests.csproj
│
├── Rivet.Infrastructure.Tests/  # Infrastructure 層整合測試
│   ├── Translation/
│   │   └── GoogleTranslationServiceTests.cs
│   └── Rivet.Infrastructure.Tests.csproj
│
└── Rivet.Console.Tests/     # Console 層測試（如需要）
    └── Rivet.Console.Tests.csproj
```

**Structure Decision**: 採用三層專案結構（Console/Service/Infrastructure），符合 spec 中的結構要求。這確保：
- **Rivet.Console**：可被 WPF 或其他 UI 框架替換
- **Rivet.Service**：純業務邏輯，不依賴具體實作
- **Rivet.Infrastructure**：具體實作，可替換翻譯服務提供者

## Complexity Tracking

> Constitution Check 無違規，此區塊保留以備未來需要。

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| N/A | N/A | N/A |

---

## Post-Design Constitution Check

*Re-evaluated after Phase 1 design completion.*

| 原則 | 設計驗證結果 |
|------|--------------|
| I. Code Quality | ✅ 通過：責任鏈模式確保單一職責、Record 類型確保不可變性、介面定義清晰的合約 |
| II. Testing Standards | ✅ 通過：data-model.md 定義可測試的實體、contracts/interfaces.md 定義可 Mock 的介面 |
| III. User Experience Consistency | ✅ 通過：quickstart.md 定義標準化錯誤訊息、統一的使用者互動流程 |
| IV. Performance Requirements | ✅ 通過：效能目標已定義於 Technical Context、短生命週期設計避免資源洩漏 |
| V. Language Policy | ✅ 通過：所有文件皆以繁體中文撰寫 |
| VI. Git Commit Convention | ✅ 通過：Branch 名稱符合規範 (`001-clipboard-translator`) |

**Post-Design Gate 評估**: ✅ 通過 — 設計符合所有規範要求

---

## 產出清單

| 階段 | 檔案 | 狀態 |
|------|------|------|
| Phase 0 | research.md | ✅ 已完成 |
| Phase 1 | data-model.md | ✅ 已完成 |
| Phase 1 | contracts/interfaces.md | ✅ 已完成 |
| Phase 1 | quickstart.md | ✅ 已完成 |
| Phase 1 | agent context 更新 | ✅ 已完成 |
| Phase 2 | tasks.md | 🔜 待執行 /speckit.tasks |
