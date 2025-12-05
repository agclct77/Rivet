**下次改善**

- 不要使用最上層陳述式 / 使用Program.Main樣式程式(考慮：加到 constitution 或 plan 裡)
- .sln 要配置所有專案含測試(考慮：加到 constitution 或 plan 裡)
- 要在一開始就加上 .gitignore(考慮：與 copilot-instructions.md 一起動作)
- commit訊息不正規(現在已經有規則範例，但AI產出不穩定，考慮：給他模板套用。)
- commit訊息會用英文寫(考慮：加到 constitution 裡)

**修復BUG時**

- 僅修改編碼，卻未確認建置通過。(考慮：加入 copilot-instructions.md)
- 當有問題時，AI的做法是先檢視代碼，但他應該先重現問題。

**其他**

- AI看不見UI設計與互動，需以log為媒介讓他知道進展到哪裡，或者運行之後提更截圖討論。
- log是基礎建設(考慮：加到 constitution 應確保應用程式都應該有日誌監控)
- 要求BDD測試
- 當AI說實作完成時或許就可以考慮提交了，整合測試時解的BUG其實有時候也值得額外提交紀錄。練習專案的話。
- task.md 原先使用 x 標記，後改成 X 標記，導致 commit 有 大規模異動。
