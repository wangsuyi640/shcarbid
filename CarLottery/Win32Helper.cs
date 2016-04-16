using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace CarBidWebClient
{
    /// <summary>
    /// 1. 根据端口号获取进程句柄
    /// 2. kill进程
    /// </summary>
    public static class ProcessPortHelper
    {
        public enum TcpOrUdp
        {
            TcpType,
            UdpType
        }

        [DllImport("ProcessorPort.dll", CallingConvention = CallingConvention.StdCall)]
        private extern static uint GetProcessIdByPort(TcpOrUdp type, uint dwPort);

        [DllImport("ProcessorPort.dll", CallingConvention = CallingConvention.StdCall)]
        private extern static uint GetAllPortByProcessId(TcpOrUdp type, uint dwProcessId, uint[] dwAllPort, uint dwMaxLen);

        public static uint Win32GetProcessIdByPort(TcpOrUdp type, uint dwPort)
        {
            return GetProcessIdByPort(TcpOrUdp.TcpType, dwPort);
        }

        public static string Win32GetProcessNameByPort(TcpOrUdp type, uint dwPort)
        {
            uint processId = GetProcessIdByPort(TcpOrUdp.TcpType, dwPort);
            Process p = Process.GetProcessById(Convert.ToInt32(processId));
            return p.ProcessName;
        }
        public static void Win32KillProcessById(uint processId)
        {
            Process p = Process.GetProcessById(Convert.ToInt32(processId));
            p.Kill();
        }
        public static void Win32KillProcessByPort(TcpOrUdp type, uint dwPort)
        {
            uint processId = GetProcessIdByPort(TcpOrUdp.TcpType, dwPort);
            Win32KillProcessById(processId);
        }
    }

    /// <summary>
    /// 获取当前窗口的位置
    /// </summary>
    public static class Win32WindowHelper
    {
         public struct Rect
        {
            public int Left { get; set; }    //最左坐标
            public int Top { get; set; }     //最上坐标
            public int Right { get; set; }   //最右坐标
            public int Bottom { get; set; }  //最下坐标
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private extern static IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private extern static IntPtr FindWindow(string strClassName, string strWindowName);

        [DllImport("user32.dll")]
        private extern static bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        public static Rect Win32GetWindowPosition()
        {
            //IntPtr awin = GetForegroundWindow();

            IntPtr awin = FindWindow(null, "拍牌小助手");

            Rect rect = new Rect();
            GetWindowRect(awin, ref rect);
            return rect;
        }
    }

    /// <summary>
    /// 1. 键盘自动输入
    /// 2. 鼠标自动点击
    /// </summary>
    public static class Win32InputHelper
    {
        public enum MOUSEEVENTF
        {
            MOUSEEVENTF_MOVE = 0x0001,         // 移动鼠标
            MOUSEEVENTF_LEFTDOWN = 0x0002,     // 模拟鼠标左键按下
            MOUSEEVENTF_LEFTUP = 0x0004,       // 模拟鼠标左键抬起
            MOUSEEVENTF_RIGHTDOWN = 0x0008,    // 模拟鼠标右键按下
            MOUSEEVENTF_RIGHTUP = 0x0010,      // 模拟鼠标右键抬起
            MOUSEEVENTF_MIDDLEDOWN = 0x0020,   // 模拟鼠标中键按下
            MOUSEEVENTF_MIDDLEUP = 0x0040,     // 模拟鼠标中键抬起
            MOUSEEVENTF_ABSOLUTE = 0x8000      // 标示是否采用绝对坐标
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("user32")]
        private static extern int SetCursorPos(int x, int y);

        [DllImport("user32.dll", EntryPoint = "keybd_event")]
        private static extern void keybd_event(

        byte bVk, //虚拟键值

        byte bScan,// 一般为0

        int dwFlags, //这里是整数类型 0 为按下，2为释放

        int dwExtraInfo //这里是整数类型 一般情况下设成为 0
        );

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private extern static IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private extern static bool SetForegroundWindow(IntPtr hWnd);

        public static void Win32MouseClick(int x, int y)
        {
            SetCursorPos(x, y);
            mouse_event(0x0002, 0, 0, 0, 0);
            mouse_event(0x0004, 0, 0, 0, 0);
        }

        public static void Win32ClipboardPaste()
        {
            IntPtr awin = GetForegroundWindow();
            SetForegroundWindow(awin);

            // Ctrl + A 全选并且清空
            keybd_event((byte)System.Windows.Forms.Keys.ControlKey, 0, 0, 0);
            keybd_event((byte)System.Windows.Forms.Keys.A, 0, 0, 0);
            keybd_event((byte)System.Windows.Forms.Keys.ControlKey, 0, 0x2, 0);
            keybd_event((byte)System.Windows.Forms.Keys.A, 0, 0x2, 0);

            keybd_event((byte)System.Windows.Forms.Keys.Delete, 0, 0, 0);
            keybd_event((byte)System.Windows.Forms.Keys.Delete, 0, 0x2, 0);

            // Ctrl + V 粘帖
            keybd_event((byte)System.Windows.Forms.Keys.ControlKey, 0, 0, 0);
            keybd_event((byte)System.Windows.Forms.Keys.V, 0, 0, 0);
            keybd_event((byte)System.Windows.Forms.Keys.ControlKey, 0, 0x2, 0);
            keybd_event((byte)System.Windows.Forms.Keys.V, 0, 0x2, 0);
        }

        public static bool Win32BidPriceInput(string bidPrice)
        {
            if (bidPrice.Length != 5)
            {
                System.Windows.Forms.MessageBox.Show("鸡巴价格 竟然不是5位数！");
                return false;
            }

            IntPtr awin = GetForegroundWindow();
            SetForegroundWindow(awin);

            byte[] priceList = System.Text.Encoding.Default.GetBytes(bidPrice);

            for (int i = 0; i < priceList.Length; i++)
            {
                keybd_event(priceList[i], 0, 0, 0);
                keybd_event(priceList[i], 0, 0x2, 0);
            }

            return true;
        }
    }
}
