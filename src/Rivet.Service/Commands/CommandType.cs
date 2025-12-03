namespace Rivet.Service.Commands;

/// <summary>
/// 指令類型列舉
/// </summary>
public enum CommandType
{
    /// <summary>
    /// 翻譯成繁體中文 ([1] 選單)
    /// </summary>
    TranslateToChinese = 1,

    /// <summary>
    /// 翻譯成英文 ([2] 選單)
    /// </summary>
    TranslateToEnglish = 2,

    /// <summary>
    /// 取消操作 (Esc)
    /// </summary>
    Cancel = 0
}
