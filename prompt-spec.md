# 產品名稱：
Rivet 

# 目的
我的工具箱

# 功能需求：
## 主訴
按下快捷鍵(Alt+.)會出現選單
選單內容：[1]翻譯成繁體中文 [2]翻譯成英文
選擇1之後將剪貼簿中的資料翻譯成指定語言
知會用戶已經將譯文放到剪貼簿中
## 其他案例
- 如果剪貼簿內容非CF_UNICODETEXT，予以回饋： 無法翻譯非純文字
- 如果剪貼簿內容CF_UNICODETEXT為空，予以回饋： 剪貼簿內容為空無法翻譯


# 結構要求：
*至少會有三個*
- Rivet.Console：前端，之後有可能會換成WPF或其他
- Rivet.Service：邏輯處理
- Rivet.Infrastructure：基礎設施

# 框架技術堆棧
- NET 8
- Spectre.Console
- Google Cloud Translation API
- Serilog
- Nunit+Nsubstitute

# 非功能性要求

## 擴充性/變動性
- 保留可能會替換前端技術
- 保留可能會替換翻譯技術
- 功能選單會增加，以責任鏈模式匹配指令，未來功能增加詞，不改動既有代碼。

## 其他
- 剪貼簿內容需要輸出trace層級方便debug。
- log輸出到 %AppData% 下
