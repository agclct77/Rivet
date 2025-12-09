# 熱鍵與 WPF 方案文件總覽（Doc Map）

## 導覽
- **決策/ADR**
  - `reference/adr/ADR-002-register-hotkey-wpf.md`：採用 WPF 常駐 + RegisterHotKey (Alt+.) 的正式決策、替代方案、影響與後續工作。
- **行為與架構路線圖**
  - `reference/002-hotkeys/20251209-roadmap.md`：高階行為與流程設計（常駐、單實例、主窗隱藏、譯文視窗並行、剪貼簿策略）。
- **執行里程碑**
  - `reference/002-hotkeys/milestones.md`：分階段落地計畫（M1~M5：移植→UI轉型→熱鍵常駐→並行策略→設定化）。
- **方案差異說明**
  - `reference/002-hotkeys/diff-wpf-vs-console.md`：WPF 方案 vs 既有 Console 差異（架構、熱鍵、UI/UX、剪貼簿策略、風險）。
- **技術研究**
  - `reference/002-hotkeys/20251209-chat-windows-hotkey-tech.md`：Windows 熱鍵階層、Hook/Hotkey 比較、案例與剪貼簿/文本取得技術研究。

## 使用建議
- 想快速了解決策依據 → 先讀 ADR-002，再看 roadmap 確認行為設計。
- 排程執行 → 按 `milestones.md` 推進；遇到 Console/WPF 差異可查 `diff-wpf-vs-console.md`。
- 熱鍵實作細節 → 參考 roadmap + 技術研究檔（此處不再列 prompt-spec，後續里程碑會另行撰寫具體需求）。

## 後續調整
- 若文件更新，請同步在本 README 中維護清單與用途，確保入口一致。
