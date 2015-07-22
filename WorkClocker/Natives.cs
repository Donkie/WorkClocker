using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace WorkClocker
{
	class Natives
	{
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetWindowText(IntPtr hWnd, [Out] StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

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

		private static string GetActiveProcessFileName(out IntPtr hwnd)
		{
			hwnd = GetForegroundWindow();
			uint pid;
			GetWindowThreadProcessId(hwnd, out pid);
			var p = Process.GetProcessById((int)pid);
			return p.MainModule.FileName;
		}

		public static WindowExe GetFocusWindow()
		{
		    try
		    {
		        var sb = new StringBuilder(256);
		        IntPtr hwnd;
		        var str = GetActiveProcessFileName(out hwnd);
		        GetWindowText(hwnd, sb, 256);
		        var f = new FileInfo(str);
		        return new WindowExe
		        {
		            Title = sb.ToString(),
		            Exe = f.Name.Remove(f.Name.Length - f.Extension.Length, f.Extension.Length)
		        };
		    }
		    catch (AccessViolationException e)
		    {
		        Console.WriteLine(e.ToString());
		    }
		    catch (System.ComponentModel.Win32Exception e)
		    {
                if(e.NativeErrorCode == 299)
                    return new WindowExe { Title = "64-bit Application", Exe = "64-bit App" };
                Console.WriteLine(e.NativeErrorCode);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.ToString());
            }
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
			return new WindowExe { Title = "Unknown", Exe = "Unknown" };
		}
	}

	internal class WindowExe
	{
		public string Exe { get; set; }
		public string Title { get; set; }
        public bool IsAfkExe { get; set; }
	}
}
