using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Hardcodet.Wpf.TaskbarNotification.Interop
{
    /// <summary>
    /// Win32 API imports.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    internal static class WinApi
    {
        /// <summary>
        /// Creates, updates or deletes the taskbar icon.
        /// </summary>
        [DllImport("shell32.Dll", EntryPoint = "Shell_NotifyIconW", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern bool Shell_NotifyIcon(
            [In] NotifyCommand cmd, 
            [In, Out] ref NotifyIconData data
            );


        /// <summary>
        /// Creates the helper window that receives messages from the taskar icon.
        /// </summary>
        [DllImport("user32.dll", EntryPoint = "CreateWindowExW", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern IntPtr CreateWindowEx(
            [In] int dwExStyle,
            [In] string lpClassName,
            [In] string lpWindowName,
            [In] int dwStyle,
            [In] int x,
            [In] int y,
            [In] int nWidth,
            [In] int nHeight, 
            [In] IntPtr hWndParent,
            [In] IntPtr hMenu,
            [In] IntPtr hInstance,
            [In] IntPtr lpParam
            );


        /// <summary>
        /// Processes a default windows procedure.
        /// </summary>
        [DllImport("user32.dll", EntryPoint = "DefWindowProcW", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern IntPtr DefWindowProc(
            [In] IntPtr hWnd,
            [In] uint msg,
            [In, Out] IntPtr wparam,
            [In, Out] IntPtr lparam
            );

        /// <summary>
        /// Registers the helper window class.
        /// </summary>
        [DllImport("user32.dll", EntryPoint = "RegisterClassW", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern short RegisterClass(
            [In, Out] ref WindowClass lpWndClass
            );

        /// <summary>
        /// Registers a listener for a window message.
        /// </summary>
        /// <param name="lpString"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "RegisterWindowMessageW", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern uint RegisterWindowMessage(
            string lpString
            );

        /// <summary>
        /// Used to destroy the hidden helper window that receives messages from the
        /// taskbar icon.
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "DestroyWindow", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern bool DestroyWindow(IntPtr hWnd);


        /// <summary>
        /// Gives focus to a given window.
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow", ExactSpelling = true)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);


        /// <summary>
        /// Gets the maximum number of milliseconds that can elapse between a
        /// first click and a second click for the OS to consider the
        /// mouse action a double-click.
        /// </summary>
        /// <returns>The maximum amount of time, in milliseconds, that can
        /// elapse between a first click and a second click for the OS to
        /// consider the mouse action a double-click.</returns>
        [DllImport("user32.dll", EntryPoint = "GetDoubleClickTime", ExactSpelling = true)]
        public static extern int GetDoubleClickTime();


        /// <summary>
        /// Gets the screen coordinates of the current mouse position.
        /// </summary>
        [DllImport("user32.dll", EntryPoint = "GetPhysicalCursorPos", ExactSpelling = true)]
        public static extern bool GetPhysicalCursorPos(ref Point lpPoint);


        [DllImport("user32.dll", EntryPoint = "GetCursorPos", ExactSpelling = true)]
        public static extern bool GetCursorPos(ref Point lpPoint);
    }
}