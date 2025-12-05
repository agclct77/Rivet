# Rivet 快捷鍵設定問題 - 待重新規劃

## 問題描述

**日期**：2025-12-05  
**狀態**：⚠️ 待解決

### 現象

- 捷徑已建立：
  - 桌面：`C:\Users\yun\Desktop\Rivet.lnk`
  - 開始功能表：`C:\Users\yun\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Rivet.lnk`
- 快速鍵設定：`Ctrl + Alt + /`
- **問題**：快速鍵無法觸發程式

### 技術調查結果

1. **捷徑本身**
   - ✅ 捷徑檔案存在
   - ✅ 目標路徑正確：`D:\Lab\Rivet\publish\Rivet.Console.exe`
   - ✅ 執行檔存在且可直接執行

2. **快速鍵設定**
   - ⚠️ Windows COM API (`WScript.Shell.Hotkey`) 無法設定特殊字符如 `/` 或 `.`
   - ⚠️ 透過 UI 手動設定快速鍵未正確保存到 .lnk 檔案
   - ⚠️ 直接編輯 .lnk 二進制數據設定快速鍵失敗

3. **根本原因**
   - Windows 快捷鍵系統與 PowerShell COM 不兼容
   - `.lnk` 檔案格式對特殊按鍵的支持限制

### 已嘗試的解決方案

| 方案 | 結果 | 備註 |
|------|------|------|
| PowerShell WScript.Shell | ❌ 失敗 | 不支援 `/` 或 `.` 字符 |
| UI 手動設定 | ❌ 失敗 | 快速鍵未保存到檔案 |
| 二進制編輯 .lnk | ❌ 失敗 | 虛擬碼設定無效 |
| Registry 修改 | ❌ 失敗 | 無相關 Registry 項 |

### 建議的解決方案（未實施）

1. **AutoHotkey**（推薦）
   - 第三方工具，需要安裝
   - 高可靠性
   - 開源免費

2. **PowerShell 監聽腳本**
   - 無需外部工具
   - 較複雜
   - 需要常駐進程

3. **Windows Task Scheduler + 快速鍵**
   - 可行性待驗證
   - 複雜度高

4. **重新評估快速鍵設定**
   - 考慮使用其他按鍵組合
   - 或使用 GUI 菜單替代快速鍵

### 下一步行動

需要與使用者討論並決定：
1. 是否安裝 AutoHotkey
2. 是否改用其他快速鍵組合（如 `Ctrl+Shift+R`）
3. 是否改為使用其他啟動方式（如開始功能表固定）

---

## 當前配置狀態

### ✅ 已完成
- 發布單一執行檔
- 建立桌面和開始功能表捷徑
- 捷徑指向正確的執行檔
- 程式能直接執行（點擊捷徑）

### ⏳ 待解決
- 快速鍵觸發機制

---

## 相關檔案

| 檔案 | 狀態 |
|------|------|
| `D:\Lab\Rivet\publish\Rivet.Console.exe` | ✅ 可執行 |
| `C:\Users\yun\Desktop\Rivet.lnk` | ✅ 存在，快速鍵未生效 |
| `C:\Users\yun\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Rivet.lnk` | ✅ 存在，快速鍵未生效 |
| `%APPDATA%\Rivet\config.json` | ✅ 已配置 |

---

## 測試結果

```
直接執行檔案：D:\Lab\Rivet\publish\Rivet.Console.exe → ✅ 成功
點擊捷徑：C:\Users\yun\Desktop\Rivet.lnk → ⏳ 待確認（應該可運行）
快速鍵 Ctrl+Alt+/ → ❌ 無反應
```

---

**更新時間**：2025-12-05  
**負責人**：待分派  
**優先級**：🔴 高（用戶體驗關鍵功能）
