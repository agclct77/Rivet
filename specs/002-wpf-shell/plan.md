# Implementation Plan: Rivet WPF 手動啟動剪貼簿翻譯

> 本文件須以繁體中文撰寫（依憲章語言規範）。

**Branch**: `002-wpf-shell` | **Date**: 2025-12-10 | **Spec**: `specs/002-wpf-shell/spec.md`
**Input**: Feature specification from `/specs/002-wpf-shell/spec.md`

**Note**: 本計畫依 `/speckit.plan` 流程產出，後續 Phase 2 請使用 `/speckit.tasks`。

## Summary

建立 `Rivet.WPF` 手動啟動殼，沿用現有 Service/Infrastructure/DI（不再要求 Console 相容），提供主視窗按鈕觸發中↔英翻譯：讀剪貼簿→呼叫既有翻譯→顯示譯文並回寫剪貼簿。需保留錯誤提示與 Serilog 紀錄，符合 WCAG 2.1 AA 基線（對比/鍵盤焦點/最小字級），並支援 `dotnet build/publish` 產生可執行檔。

## Technical Context

**Language/Version**: C# 12 / .NET 8 / WPF (Windows 桌面，STA UI 執行緒)  
**Primary Dependencies**: WPF（視窗/按鈕/Binding）、Microsoft.Extensions.DependencyInjection（DI 重用）、Serilog（既有設定）、Google Cloud Translation API（既有）、Windows 剪貼簿服務（既有 Infrastructure）、NUnit + NSubstitute（既有測試框架）  
**Storage**: 無持久化需求；沿用 `%AppData%/Rivet/config.json` 與日誌檔案  
**Testing**: 單元/整合測試使用 NUnit + NSubstitute；翻譯與剪貼簿管線沿用既有測試；WPF UI 自動化採 FlaUI(UIA3) + NUnit，測試類別以 STA/非平行執行；性能量測需涵蓋啟動與翻譯延遲、記憶體峰值；覆蓋率報告需達 80% 並作為驗收 gate  
**Target Platform**: Windows 10/11 x64 桌面  
**Project Type**: 單一桌面應用 + 共用服務層；新增加 WPF UI 專案（Console 後續將淘汰，不列為相容目標）  
**Performance Goals**: 啟動至主視窗可操作 p95 ≤2 秒；剪貼簿文字至結果呈現 p95 ≤1500ms（服務可達）；單次翻譯記憶體峰值 <150MB；資源需釋放避免長時佔用  
**Constraints**: 無熱鍵/常駐/系統匣（本里程碑排除，憲章之熱鍵性能門檻不適用於此交付）；需維持剪貼簿回寫；必須沿用既有設定/日誌路徑；UI 單窗格，固定繁中提示；必須符合 WCAG 2.1 AA 對比與鍵盤操作基線；發行需支援 `-r win-x64 --self-contained true /p:PublishSingleFile=true`  
**Scale/Scope**: 單使用者單視窗；僅提供兩種方向按鈕；後續里程碑再加入熱鍵與 UX 改善

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- 語言：本計畫與產出文件均以繁中撰寫，符合憲章 V。✅
- 測試：新功能須 ≥80% 覆蓋；剪貼簿+翻譯整合測試與公開介面合約測試需覆蓋，WPF UI 自動化工具已決定為 FlaUI(UIA3)+NUnit（STA 單序執行）。✅
- 性能：沿用 p95 ≤1500ms 翻譯、記憶體 <150MB；啟動 p95 ≤2 秒；資源釋放需檢核。✅（需在設計與驗收計畫中落實測試方式）。
- 分支與提交：分支 `002-wpf-shell` 符合 `<ticket-id>-<feature-name>`；提交需遵循 Conventional Commits 並以繁中撰寫。✅

## Project Structure

### Documentation (this feature)

```text
specs/002-wpf-shell/
├── plan.md              # 本檔
├── research.md          # Phase 0 產出
├── data-model.md        # Phase 1 產出
├── quickstart.md        # Phase 1 產出（WPF 版啟動/發行指南）
├── contracts/           # Phase 1 API/UI/介面合約
└── tasks.md             # Phase 2（由 /speckit.tasks 產生）
```

### Source Code (repository root)

```text
src/
├── Rivet.Service/          # 業務與抽象介面（翻譯、剪貼簿、設定、通知）
├── Rivet.Infrastructure/   # 實作層（Google 翻譯、剪貼簿、設定、Serilog）
├── Rivet.WPF/              # 新增 WPF 專案（App.xaml、MainWindow、DI 啟動；主入口）
├── Rivet.Console/          # 既有 CLI（後續淘汰，不再作為相容目標）
└── Rivet.sln

tests/
├── Rivet.Service.Tests/        # 業務單元/整合
├── Rivet.Infrastructure.Tests/ # 基礎設施整合
├── Rivet.Console.Tests/        # 既有 CLI 測試（存量，非相容目標）
└── Rivet.WPF.Tests/            # 新增 WPF UI/整合測試（FlaUI+NUnit）
```

**Structure Decision**: 採單一解決方案多專案；新建 `Rivet.WPF` 作為 UI 層，復用既有 Service/Infrastructure；測試將視選型新增對應 WPF 自動化專案（名稱/工具於 Phase 0 確認）。

## Complexity Tracking

目前無需憲章例外，暫無複雜度豁免紀錄。
