using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

public static class WindowsApi
{
	public static RECT GetScreenSize()
	{
		RECT screen;
		if (!SystemParametersInfo(SPI_GETWORKAREA, 0, out screen, 0))
			throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
		return screen;
	}

	public static IDictionary<IntPtr, string> GetOpenWindows()
	{
		var shellWindow = GetShellWindow();
		var windows = new Dictionary<IntPtr, string>();

		EnumWindows(delegate (IntPtr hWnd, int lParam)
		{
			if (hWnd == shellWindow) return true;
			if (!IsWindowVisible(hWnd)) return true;

			var length = GetWindowTextLength(hWnd);
			if (length == 0) return true;

			var builder = new StringBuilder(length);
			GetWindowText(hWnd, builder, length + 1);

			windows[hWnd] = builder.ToString();
			return true;

		}, 0);

		return windows;
	}

	public static WINDOWPLACEMENT GetPlacement(IntPtr hwnd)
	{
		WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
		placement.length = Marshal.SizeOf(placement);
		GetWindowPlacement(hwnd, ref placement);
		return placement;
	}

	public static void EnableClickthrough()
	{
		SetWindowLong(GetActiveWindow(), -20, WsExLayered | WsExTransparent);	
	}

	private delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

	[DllImport("USER32.DLL")]
	private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

	[DllImport("USER32.DLL")]
	private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

	[DllImport("USER32.DLL")]
	private static extern int GetWindowTextLength(IntPtr hWnd);

	[DllImport("USER32.DLL")]
	private static extern bool IsWindowVisible(IntPtr hWnd);

	[DllImport("USER32.DLL")]
	private static extern IntPtr GetShellWindow();	
	[DllImport("user32.dll")]
	private static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);
	[DllImport("user32.dll")]
	static extern bool SystemParametersInfo(uint uiAction, uint uiParam, out RECT pvParam, uint fWinIni);
	const uint SPI_GETWORKAREA = 0x0030;
	[DllImport("user32.dll")]
	private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

	[DllImport("user32.dll")]
	public static extern IntPtr GetActiveWindow();

	private const uint WsExLayered = 0x00080000;
	private const uint WsExTransparent = 0x00000020;
	[DllImport("user32.dll")]
	private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
	
}

	public enum ShowWindowCommands : int
	{
		Hide = 0,
		Normal = 1,
		Minimized = 2,
		Maximized = 3,
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct WINDOWPLACEMENT
	{
		public int length;
		public int flags;
		public ShowWindowCommands showCmd;
		public System.Drawing.Point ptMinPosition;
		public System.Drawing.Point ptMaxPosition;
		public System.Drawing.Rectangle rcNormalPosition;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct RECT
	{
		public int Left, Top, Right, Bottom;
	}
