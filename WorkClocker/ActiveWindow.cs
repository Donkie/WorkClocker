using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WorkClocker
{
    class ActiveWindow
    {
        [DllImport("user32.dll", SetLastError = true)]
// ReSharper disable once InconsistentNaming
        public static extern bool GetGUIThreadInfo(uint hTreadID, ref GUITHREADINFO lpgui);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(uint hwnd, out uint lpdwProcessId);

        [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetWindowText(IntPtr hWnd, [Out] StringBuilder lpString, int nMaxCount);

        [StructLayout(LayoutKind.Sequential)]
// ReSharper disable once InconsistentNaming
        public struct RECT
        {
            public int iLeft;
            public int iTop;
            public int iRight;
            public int iBottom;
        }

        [StructLayout(LayoutKind.Sequential)]
// ReSharper disable once InconsistentNaming
        public struct GUITHREADINFO
        {
            public int cbSize;
            public int flags;
            public IntPtr hwndActive;
            public IntPtr hwndFocus;
            public IntPtr hwndCapture;
            public IntPtr hwndMenuOwner;
            public IntPtr hwndMoveSize;
            public IntPtr hwndCaret;
            public RECT rectCaret;
        }

        const uint Hwnd = 0;
        public static bool GetInfo(out GUITHREADINFO lpgui)
        {
            uint lpdwProcessId;
            GetWindowThreadProcessId(Hwnd, out lpdwProcessId);

            lpgui = new GUITHREADINFO();
            lpgui.cbSize = Marshal.SizeOf(lpgui);

            return GetGUIThreadInfo(lpdwProcessId, ref lpgui);
        }

        public static string GetFocusWindow()
        {
            try
            {
                GUITHREADINFO info;
                GetInfo(out info);

                var sb = new StringBuilder(256);
                GetWindowText(info.hwndActive, sb, 256);

                return sb.ToString();
            }
            catch (AccessViolationException e)
            {
                Console.WriteLine(e.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return null;
        }
    }
}
