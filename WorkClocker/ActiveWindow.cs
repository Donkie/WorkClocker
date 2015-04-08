using System;
using System.Runtime.InteropServices;
using System.Text;

namespace WorkClocker
{
	class ActiveWindow
	{
		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool GetGUIThreadInfo(uint hTreadId, ref Guithreadinfo lpgui);

		[DllImport("user32.dll")]
		public static extern uint GetWindowThreadProcessId(uint hwnd, out uint lpdwProcessId);

		[DllImport("user32", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern int GetWindowText(IntPtr hWnd, [Out] StringBuilder lpString, int nMaxCount);

		[StructLayout(LayoutKind.Sequential)]
		public struct Rect
		{
			public int iLeft;
			public int iTop;
			public int iRight;
			public int iBottom;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct Guithreadinfo
		{
			public int cbSize;
			public int flags;
			public IntPtr hwndActive;
			public IntPtr hwndFocus;
			public IntPtr hwndCapture;
			public IntPtr hwndMenuOwner;
			public IntPtr hwndMoveSize;
			public IntPtr hwndCaret;
			public Rect rectCaret;
		}

		const uint HWND = 0;
		public static bool GetInfo(out Guithreadinfo lpgui)
		{
			uint lpdwProcessId;
			GetWindowThreadProcessId(HWND, out lpdwProcessId);

			lpgui = new Guithreadinfo();
			lpgui.cbSize = Marshal.SizeOf(lpgui);

			return GetGUIThreadInfo(lpdwProcessId, ref lpgui);
		}

		public static string GetFocusWindow()
		{
			try
			{
				Guithreadinfo info;
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
