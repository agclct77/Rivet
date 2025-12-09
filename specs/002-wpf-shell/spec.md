# Feature Specification: Rivet WPF 手動啟動剪貼簿翻譯

> 本文件須以繁體中文撰寫（依憲章語言規範）。

**Feature Branch**: `002-wpf-shell`  
**Created**: 2025-12-10  
**Status**: Draft  
**Input**: User description: "建立 WPF 手動啟動版，讀剪貼簿→翻譯（中↔英）→顯示結果，沿用 Service/Infrastructure/DI，保持回寫剪貼簿，可 build/publish"

## User Scenarios & Testing *(mandatory)*

<!--
  IMPORTANT: User stories should be PRIORITIZED as user journeys ordered by importance.
  Each user story/journey must be INDEPENDENTLY TESTABLE - meaning if you implement just ONE of them,
  you should still have a viable MVP (Minimum Viable Product) that delivers value.
  
  Assign priorities (P1, P2, P3, etc.) to each story, where P1 is the most critical.
  Think of each story as a standalone slice of functionality that can be:
  - Developed independently
  - Tested independently
  - Deployed independently
  - Demonstrated to users independently
-->

### User Story 1 - 手動啟動並翻譯剪貼簿 (Priority: P1)

使用者啟動 WPF 應用程式，點擊翻譯動作，即可將剪貼簿文字於介面顯示譯文並同步回寫剪貼簿。

**Why this priority**: 此為最小可用流程，直接提供翻譯價值並驗證 WPF 殼與既有服務的整合。

**Independent Test**: 僅需啟動 WPF、準備剪貼簿文字並觸發翻譯即可完成驗收，無需倚賴熱鍵或常駐行為。

**Acceptance Scenarios**:

1. **Given** 剪貼簿含英文文字且已啟動 WPF 主視窗，**When** 使用者點擊「英→中」翻譯，**Then** 介面顯示中文譯文且剪貼簿內容被替換為譯文。
2. **Given** 剪貼簿含中文文字且已啟動 WPF 主視窗，**When** 使用者點擊「中→英」翻譯，**Then** 介面顯示英文譯文且剪貼簿內容被替換為譯文。

---

### User Story 2 - 友善錯誤提示與不中斷 (Priority: P2)

當剪貼簿為空或非文字、或翻譯失敗時，使用者可收到清楚提示且應用程式保持可用。

**Why this priority**: 防止崩潰並確保手動流程可重試，是可用性與信任的關鍵。

**Independent Test**: 模擬空白剪貼簿、圖片剪貼簿、網路失敗，驗證介面提示與應用程式不中斷即可獨立驗收。

**Acceptance Scenarios**:

1. **Given** 剪貼簿為空，**When** 使用者點擊任一翻譯，**Then** 介面顯示「剪貼簿無文字」類提示且程式保持可操作。
2. **Given** 剪貼簿為圖片，**When** 使用者點擊翻譯，**Then** 介面顯示「僅支援文字」類提示且剪貼簿不被修改。
3. **Given** 翻譯服務暫時不可達，**When** 使用者點擊翻譯，**Then** 介面顯示「翻譯失敗，請稍後重試」並保留原剪貼簿內容。
4. **Given** 剪貼簿為 RTF/HTML 文字，**When** 使用者點擊翻譯，**Then** 系統僅取純文字、於 UI 顯示已降階提示，且原剪貼簿內容不被破壞。

---

### User Story 3 - 可建置與發行 (Priority: P3)

維運人員可使用標準命令建置或發行 WPF 版本，取得可執行檔供手動啟動。

**Why this priority**: 確保成果可交付且便於部署，滿足驗收需求。

**Independent Test**: 單獨執行 `dotnet build` 與 `dotnet publish` 產出檔案並啟動驗證即可驗收，無需其他功能依賴。

**Acceptance Scenarios**:

1. **Given** 開發機具備 .NET 8 環境，**When** 執行建置/發行命令，**Then** 生成可啟動的 WPF 執行檔且與現有設定檔協同運作。

---

[Add more user stories as needed, each with an assigned priority]

### Edge Cases

- 剪貼簿含超長文字（如 >20,000 字元）時，需提示可能的延遲並避免介面凍結。
- 剪貼簿含格式化文字（RTF/HTML）時，系統應僅取純文字並提示已降階處理。
- 設定檔缺失或金鑰無效時，應提示設定問題並不中斷應用程式；日誌記錄細節。
- 翻譯成功但寫回剪貼簿失敗（被鎖定）時，應提示失敗並至少保留介面結果可複製。

## Requirements *(mandatory)*

<!--
  ACTION REQUIRED: The content in this section represents placeholders.
  Fill them out with the right functional requirements.
-->

### Functional Requirements

- **FR-001**: WPF 應用程式須提供主視窗與「中→英」「英→中」翻譯觸發動作，啟動後可由使用者手動執行翻譯。
- **FR-002**: 翻譯操作必須讀取現有剪貼簿文字，若偵測到非文字或空內容則不應嘗試送出翻譯請求。
- **FR-003**: 翻譯成功時，介面需顯示譯文並回寫剪貼簿；若寫回失敗需提示並保留介面結果供複製。
- **FR-004**: 翻譯方向選擇須可在介面上明確操作並反映於結果，與 Console 版語言對應一致。
- **FR-005**: 應用程式須沿用既有設定與服務註冊流程（翻譯服務、剪貼簿服務、設定、日誌），避免重複業務邏輯。
- **FR-006**: 設定檔缺失、金鑰無效或翻譯服務不可達時，須於 UI 顯示明確錯誤訊息，並在日誌中記錄詳細原因。
- **FR-007**: 翻譯流程產生的錯誤（包括剪貼簿鎖定、網路錯誤）不得導致應用程式崩潰，使用者可重試。
- **FR-008**: 應用程式須在 Windows 10/11 .NET 8 目標環境下可執行，並可透過既定建置與發行流程產出可執行檔以供驗收。
- **FR-009**: 日誌輸出路徑與格式須與現有實作一致，並覆蓋成功與失敗情境；需避免在日誌中落地敏感資訊。
- **FR-010**: UI 需提供基本可讀性與可及性，至少：主要文字字級 ≥14px、互動元件對比度 ≥4.5:1、Tab 順序可達且焦點樣式可見、按鈕/錯誤訊息均為繁中，並提供可複製譯文區域。
- **FR-011**: 可及性需符合 WCAG 2.1 AA 相關對比與鍵盤操作基線；如因 WPF 主題限制需例外，必須文件化並提供替代方案。

- **性能**：啟動至主視窗可操作時間 ≤2 秒 (p95)；介面觸發翻譯至結果呈現 ≤1.5 秒 (p95) 於翻譯服務可達時；單次翻譯流程記憶體峰值 <150MB，並釋放資源避免長時間佔用；處理過程不在日誌落地敏感內容。
- **測試要求**：新功能測試覆蓋率 ≥80%，需產出覆蓋率報告做為驗收 gate；需提供剪貼簿讀取/驗證、翻譯服務呼叫、錯誤提示、依賴注入接線的單元或整合測試，以及發行腳本/命令的驗證步驟。

#### Dependencies & Assumptions

- 現有翻譯服務、剪貼簿服務、設定與日誌元件已在解決方案中可用並維持既有行為。
- 目標環境已具備可用的翻譯服務金鑰與網路連線，且允許讀寫剪貼簿。
- 單一使用者情境下啟動 WPF 應用程式（不處理多實例互斥）。

### Key Entities *(include if feature involves data)*

- **ClipboardContent**: 代表當前剪貼簿純文字內容與來源格式，用於判斷是否可翻譯並保留原文字供錯誤提示使用。
- **TranslationRequest**: 包含來源語言、目標語言、原始文字與觸發方向，用於送往翻譯服務並追蹤請求狀態。
- **TranslationResult**: 包含譯文文字、方向、完成時間戳與錯誤狀態，用於 UI 顯示、剪貼簿回寫與日誌記錄。

## Success Criteria *(mandatory)*

<!--
  ACTION REQUIRED: Define measurable success criteria.
  These must be technology-agnostic and measurable.
-->

### Measurable Outcomes

- **SC-001**: 在目標環境啟動應用程式至主視窗可操作時間 p95 ≤2 秒。
- **SC-002**: 剪貼簿文字觸發翻譯至結果呈現 p95 ≤1.5 秒（翻譯服務可達且網路正常）。
- **SC-003**: 單次翻譯流程記憶體峰值 <150MB，連續執行 20 次翻譯後未出現持續性記憶體上升。
- **SC-004**: 空白/非文字剪貼簿與翻譯失敗情境，100% 顯示友善錯誤訊息且程式不中斷，可立即重試。
- **SC-005**: 標準建置與發行流程在 Windows 10/11 上均可成功產出可執行檔並啟動完成主流程。

## Clarifications (2025-12-10)

- **日誌隱私**：採用選項 A，日誌僅記錄操作狀態與錯誤碼/訊息，不落地剪貼簿或譯文字串。
- **UI 介面範式**：採用選項 A，單窗格，來源文字只讀 + 兩個按鈕（英→中、中→英）+ 譯文區塊。
- **非文字/長文字處理**：採用選項 A，僅取純文字並提示「已降階為純文字」，若超過 20,000 字元則直接拒絕並提示。
- **發行輸出形態**：採用選項 A，`dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true`。
- **錯誤訊息語言**：採用選項 A，UI 提示固定繁中，使用預設友善文案。
- **熱鍵範圍**：本里程碑不涵蓋熱鍵或常駐行為；憲章中熱鍵相關性能門檻於此交付不適用，僅保留翻譯與剪貼簿流程性能要求。
