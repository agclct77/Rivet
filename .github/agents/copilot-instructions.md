# Rivet Development Guidelines

Auto-generated from all feature plans. Last updated: 2025-12-04

## Active Technologies
- C# / .NET 8 (WPF) + Microsoft.Extensions.DependencyInjection、Serilog、Google Cloud Translation API、Windows 剪貼簿服務 (Infrastructure) (002-wpf-shell)
- N/A（僅讀寫剪貼簿與設定檔） (002-wpf-shell)
- C# 12 / .NET 8 / WPF (Windows 桌面，STA UI 執行緒) + WPF（視窗/按鈕/Binding）、Microsoft.Extensions.DependencyInjection（DI 重用）、Serilog（既有設定）、Google Cloud Translation API（既有）、Windows 剪貼簿服務（既有 Infrastructure）、NUnit + NSubstitute（既有測試框架） (002-wpf-shell)
- 無持久化需求；沿用 `%AppData%/Rivet/config.json` 與日誌檔案 (002-wpf-shell)

- C# / .NET 8 + Spectre.Console (CLI UI)、Google.Cloud.Translation.V2 (翻譯服務)、Serilog (日誌)、Microsoft.Extensions.DependencyInjection (DI) (001-clipboard-translator)

## Project Structure

```text
src/
tests/
```

## Commands

# Add commands for C# / .NET 8

## Code Style

C# / .NET 8: Follow standard conventions

## Recent Changes
- 002-wpf-shell: Added C# 12 / .NET 8 / WPF (Windows 桌面，STA UI 執行緒) + WPF（視窗/按鈕/Binding）、Microsoft.Extensions.DependencyInjection（DI 重用）、Serilog（既有設定）、Google Cloud Translation API（既有）、Windows 剪貼簿服務（既有 Infrastructure）、NUnit + NSubstitute（既有測試框架）
- 002-wpf-shell: Added C# / .NET 8 (WPF) + Microsoft.Extensions.DependencyInjection、Serilog、Google Cloud Translation API、Windows 剪貼簿服務 (Infrastructure)

- 001-clipboard-translator: Added C# / .NET 8 + Spectre.Console (CLI UI)、Google.Cloud.Translation.V2 (翻譯服務)、Serilog (日誌)、Microsoft.Extensions.DependencyInjection (DI)

<!-- MANUAL ADDITIONS START -->
<!-- MANUAL ADDITIONS END -->
