using System.Runtime.InteropServices;
using System.Text;
using Rivet.Service.Clipboard;

namespace Rivet.Infrastructure.Clipboard;

/// <summary>
/// Windows 剪貼簿服務實作
/// 使用 Windows API 與系統剪貼簿互動
/// </summary>
public class WindowsClipboardService : IClipboardService
{
    // Windows API 常數
    private const int CF_UNICODETEXT = 13;

    // Windows API 宣告
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GlobalLock(IntPtr hMem);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GlobalUnlock(IntPtr hMem);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GlobalSize(IntPtr hMem);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool OpenClipboard(IntPtr hWndNewOwner);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool CloseClipboard();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr GetClipboardData(uint format);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetClipboardData(uint format, IntPtr hMem);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GlobalAlloc(uint flags, UIntPtr size);

    private const uint GMEM_MOVEABLE = 0x0002;

    /// <inheritdoc />
    public ClipboardContent GetContent()
    {
        try
        {
            if (!OpenClipboard(IntPtr.Zero))
            {
                return ClipboardContent.NonText();
            }

            try
            {
                IntPtr hClipboardData = GetClipboardData(CF_UNICODETEXT);
                if (hClipboardData == IntPtr.Zero)
                {
                    // 剪貼簿中沒有 Unicode 文字
                    return ClipboardContent.Empty();
                }

                IntPtr pLockedBuffer = GlobalLock(hClipboardData);
                if (pLockedBuffer == IntPtr.Zero)
                {
                    return ClipboardContent.NonText();
                }

                try
                {
                    // 取得緩衝區大小
                    IntPtr size = GlobalSize(hClipboardData);
                    int byteCount = (int)size.ToInt64();

                    if (byteCount <= 0)
                    {
                        return ClipboardContent.Empty();
                    }

                    // 讀取 Unicode 字串
                    byte[] buffer = new byte[byteCount];
                    Marshal.Copy(pLockedBuffer, buffer, 0, byteCount);

                    // 轉換為字串（去除末尾的 null 終止符）
                    string text = Encoding.Unicode.GetString(buffer).TrimEnd('\0');

                    if (string.IsNullOrEmpty(text))
                    {
                        return ClipboardContent.Empty();
                    }

                    return ClipboardContent.WithText(text);
                }
                finally
                {
                    GlobalUnlock(hClipboardData);
                }
            }
            finally
            {
                CloseClipboard();
            }
        }
        catch (Exception)
        {
            // 任何存取剪貼簿的錯誤都視為非文字內容
            return ClipboardContent.NonText();
        }
    }

    /// <inheritdoc />
    public void SetText(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (!OpenClipboard(IntPtr.Zero))
        {
            throw new InvalidOperationException("無法開啟剪貼簿");
        }

        try
        {
            // 轉換文字為 Unicode
            byte[] data = Encoding.Unicode.GetBytes(text);

            // 分配全域記憶體
            UIntPtr byteCount = new((ulong)data.Length + 2); // +2 for null terminator
            IntPtr hGlobalMemory = GlobalAlloc(GMEM_MOVEABLE, byteCount);

            if (hGlobalMemory == IntPtr.Zero)
            {
                throw new OutOfMemoryException("無法分配剪貼簿記憶體");
            }

            // 鎖定記憶體並複製資料
            IntPtr pLockedBuffer = GlobalLock(hGlobalMemory);
            if (pLockedBuffer == IntPtr.Zero)
            {
                throw new InvalidOperationException("無法鎖定剪貼簿記憶體");
            }

            try
            {
                // 複製資料（包括 null 終止符）
                Marshal.Copy(data, 0, pLockedBuffer, data.Length);
                // 寫入 null 終止符
                Marshal.WriteInt16(pLockedBuffer, data.Length, 0);
            }
            finally
            {
                GlobalUnlock(hGlobalMemory);
            }

            // 設定剪貼簿內容
            if (!SetClipboardData(CF_UNICODETEXT, hGlobalMemory))
            {
                throw new InvalidOperationException("無法設定剪貼簿內容");
            }
        }
        finally
        {
            CloseClipboard();
        }
    }
}
