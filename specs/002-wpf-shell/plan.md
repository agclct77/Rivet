# Implementation Plan: Rivet WPF 手動啟動剪貼簿翻譯

> 本文件須以繁體中文撰寫（依憲章語言規範）。

**Branch**: `002-wpf-shell` | **Date**: 2025-12-10 | **Spec**: `./spec.md`
**Input**: Feature specification from `/specs/002-wpf-shell/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/commands/plan.md` for the execution workflow.

## Summary

建立 WPF 殼以手動啟動剪貼簿翻譯流程，重用現有 Service/Infrastructure 的剪貼簿與翻譯服務、設定與日誌組態，提供中↔英翻譯按鈕，顯示譯文並回寫剪貼簿，同時保留錯誤提示與可建置/發行的 WPF 專案。

## Technical Context

<!--
  ACTION REQUIRED: Replace the content in this section with the technical details
  for the project. The structure here is presented in advisory capacity to guide
  the iteration process.
-->

**Language/Version**: C# / .NET 8 (WPF)  
**Primary Dependencies**: Microsoft.Extensions.DependencyInjection、Serilog、Google Cloud Translation API、Windows 剪貼簿服務 (Infrastructure)  
**Storage**: N/A（僅讀寫剪貼簿與設定檔）  
**Testing**: NUnit + NSubstitute（需新增 WPF/DI/剪貼簿流程測試）  
**Target Platform**: Windows 10/11 桌面（WPF，x64）  
**Project Type**: 桌面單一應用（WPF 前端 + 既有 Service/Infrastructure）  
**Performance Goals**: 翻譯觸發至結果呈現 ≤1.5s (p95)；啟動至主視窗可操作 ≤2s (p95)  
**Constraints**: 單次翻譯流程記憶體 <150MB；需沿用 Serilog 日誌路徑與設定；翻譯需可在網路失敗時給出提示；剪貼簿鎖定需處理  
**Scale/Scope**: 單使用者桌面應用（MVP 無熱鍵/常駐）；未引入額外服務

**已澄清事項**：
- DI/設定註冊將抽出為 `IServiceCollection` 擴充（例如 `AddRivetCoreServices`），Console 與 WPF 共用，UI 端另行註冊對應 notifier。
- WPF UI 保持剪貼簿操作在 UI STA 執行緒，翻譯呼叫在背景執行並以 Dispatcher 回到 UI 更新結果與錯誤提示。

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- 語言：本計畫與後續產出均以繁體中文撰寫，符合憲章。
- 測試：需新增/調整測試以達 ≥80% 覆蓋率，涵蓋 WPF 觸發流程、剪貼簿驗證、翻譯管線與 DI 接線；公開介面變更需合約測試。
- 性能：此里程碑無熱鍵，但仍需確保剪貼簿至輸出 ≤1500ms（服務可達），啟動至主視窗可操作 ≤2s，單次翻譯後記憶體 <150MB 並釋放資源。
- 分支與提交：已在分支 `002-wpf-shell`；提交須遵循 Conventional Commits 並以繁體中文含 Ticket-ID。

**設計後複核**：Phase 1 輸出（data-model、contracts、quickstart）均以繁體中文撰寫，性能/測試要求沿用並需在 WPF 實作時落地；仍需於開發階段補齊 WPF/DI/流程測試以滿足覆蓋率與合約測試要求。

## Project Structure

### Documentation (this feature)

```text
specs/[###-feature]/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)
<!--
  ACTION REQUIRED: Replace the placeholder tree below with the concrete layout
  for this feature. Delete unused options and expand the chosen structure with
  real paths (e.g., apps/admin, packages/something). The delivered plan must
  not include Option labels.
-->

```text
src/
├── Rivet.Console/            # 既有 Console UI（提供 DI 參考）
├── Rivet.Service/            # 業務邏輯與指令/翻譯/剪貼簿介面
├── Rivet.Infrastructure/     # 剪貼簿、翻譯、設定、日誌實作
└── Rivet.WPF/                # 本次新增 WPF 殼（主視窗、App、DI 引導）

tests/
├── Rivet.Console.Tests/
├── Rivet.Service.Tests/
├── Rivet.Infrastructure.Tests/
└── [TBD] Rivet.WPF.Tests/    # 需新增/調整以涵蓋 WPF/DI/流程測試
```

**Structure Decision**: 採單一解決方案下的多專案結構，新增 `src/Rivet.WPF` 與對應測試專案（或於既有測試增補 WPF 入口與 DI/流程測試）。

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| [e.g., 4th project] | [current need] | [why 3 projects insufficient] |
| [e.g., Repository pattern] | [specific problem] | [why direct DB access insufficient] |
