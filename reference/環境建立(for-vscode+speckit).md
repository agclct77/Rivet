
# 開發主機環境
## winget
```ps
# 檢查有無安裝winget(如果有出現版本號，代表安裝過了，建議進行更新；反之請安裝。)
winget --version

# 安裝 winget
# 自行參考網頁下載安裝：https://learn.microsoft.com/zh-tw/windows/package-manager/winget/

# 更新 winget
winget upgrade --id Microsoft.AppInstaller --silent --accept-source-agreements --accept-package-agreements
```
## 安裝 Git（版本控制）
```ps
winget install Git.Git --silent --accept-source-agreements --accept-package-agreements
```
## 安裝 uv（Python 套件管理器）
```ps
winget install astral-sh.uv --silent --accept-source-agreements --accept-package-agreements
```
## 安裝 nvm（Node.js 版本管理器）
```ps
winget install CoreyButler.NVMforWindows --silent --accept-source-agreements --accept-package-agreements
```

- [ ] 關閉PowerShell
# 工具組
## 安裝 Spec Kit 的 Specify CLI 工具
```
uv tool install specify-cli --from git+https://github.com/github/spec-kit.git
```

## 安裝 GitHub Copilot CLI
```
npm install -g @github/copilot
```

# IDE(VS code)
- Extensions: Github Copilot Chat

# 確認
## 檢查 Spec Kit 工具支援狀態
```
specify check
# 必須出現訊息「Specify CLI is ready to Use」
```

**預期結果**
> [!SUMMARY]
> PS C:\Users\user1> specify check
>                                                                               ███████╗██████╗ ███████╗ ██████╗██╗███████╗██╗   ██╗
>                                                                               ██╔════╝██╔══██╗██╔════╝██╔════╝██║██╔════╝╚██╗ ██╔╝
>                                                                               ███████╗██████╔╝█████╗  ██║     ██║█████╗   ╚████╔╝
>                                                                               ╚════██║██╔═══╝ ██╔══╝  ██║     ██║██╔══╝    ╚██╔╝
>                                                                               ███████║██║     ███████╗╚██████╗██║██║        ██║
>                                                                               ╚══════╝╚═╝     ╚══════╝ ╚═════╝╚═╝╚═╝        ╚═╝
> 
>                                                                                 GitHub Spec Kit - Spec-Driven Development Toolkit
> 
> Checking for installed tools...
> 
> Check Available Tools
> ├── ● Git version control (available)
> ├── ○ GitHub Copilot (IDE-based, no CLI check)
> ├── ● Claude Code (available)
> ├── ● Gemini CLI (available)
> ├── ○ Cursor (IDE-based, no CLI check)
> ├── ● Qwen Code (not found)
> ├── ● opencode (not found)
> ├── ● Codex CLI (not found)
> ├── ○ Windsurf (IDE-based, no CLI check)
> ├── ○ Kilo Code (IDE-based, no CLI check)
> ├── ● Auggie CLI (not found)
> ├── ● CodeBuddy (not found)
> ├── ○ Roo Code (IDE-based, no CLI check)
> ├── ● Amazon Q Developer CLI (not found)
> ├── ● Amp (not found)
> ├── ● SHAI (not found)
> ├── ○ IBM Bob (IDE-based, no CLI check)
> ├── ● Visual Studio Code (available)
> └── ● Visual Studio Code Insiders (not found)
> 
> Specify CLI is ready to use!



# 附錄
[Github-speckit-中譯版(保哥)](https://github.com/doggy8088/spec-kit)
[[20251103_安裝紀錄]]