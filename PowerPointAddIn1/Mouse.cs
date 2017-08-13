using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace PowerPointAddIn1
{
	class Win32API
	{
		#region DLL导入
		/// <summary>
		/// 用于设置窗口
		/// </summary>
		/// <param name="hWnd"></param>
		/// <param name="hWndInsertAfter"></param>
		/// <param name="X"></param>
		/// <param name="Y"></param>
		/// <param name="cx"></param>
		/// <param name="cy"></param>
		/// <param name="uFlags"></param>
		/// <returns></returns>
		[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
		public static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

		/// <summary>
		/// 安装钩子
		/// </summary>
		/// <param name="idHook">钩子类型,鼠标\键盘\巨集等10几种</param>
		/// <param name="lpfn">挂钩的函数,用来处理拦截消息的函数,全局函数</param>
		/// <param name="hInstance">当前进程的句柄,
		/// 为NULL:当前进程创建的一个线程,子程位于当前进程；
		/// 为0(IntPtr.Zero):钩子子程与所有的线程关联，即为全局钩子</param>
		/// <param name="threadId">设置要挂接的线程ID.为NULL则为全局钩子</param>
		/// <returns></returns>
		[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
		public static extern IntPtr SetWindowsHookEx(IntPtr idHook, HookProc lpfn, IntPtr pInstance, uint threadId);

		/// <summary>
		/// 卸载钩子
		/// </summary>
		/// <param name="idHook"></param>
		/// <returns></returns>
		[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
		public static extern bool UnhookWindowsHookEx(IntPtr pHookHandle);

		/// <summary>
		/// 传递钩子
		/// 用于把拦截的消息继续传递下去，不然其他程序可能会得不到相应的消息
		/// </summary>
		/// <param name="idHook"></param>
		/// <param name="nCode"></param>
		/// <param name="wParam"></param>
		/// <param name="lParam"></param>
		/// <returns></returns>
		[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
		public static extern int CallNextHookEx(IntPtr pHookHandle, int nCode, IntPtr wParam, IntPtr lParam);

		/// <summary>
		/// 转换当前按键信息
		/// </summary>
		/// <param name="uVirtKey"></param>
		/// <param name="uScanCode"></param>
		/// <param name="lpbKeyState"></param>
		/// <param name="lpwTransKey"></param>
		/// <param name="fuState"></param>
		/// <returns></returns>
		[DllImport("user32.dll")]
		public static extern int ToAscii(UInt32 uVirtKey, UInt32 uScanCode, byte[] lpbKeyState, byte[] lpwTransKey, UInt32 fuState);

		/// <summary>
		/// 获取按键状态
		/// </summary>
		/// <param name="pbKeyState"></param>
		/// <returns>非0表示成功</returns>
		[DllImport("user32.dll")]
		public static extern int GetKeyboardState(byte[] pbKeyState);

		[DllImport("user32.dll")]
		public static extern short GetKeyStates(int vKey);

		/// <summary>
		/// 获取当前线程Id
		/// </summary>
		/// <returns></returns>
		[DllImport("kernel32.dll")]
		public static extern uint GetCurrentThreadId();

		/// <summary>
		/// 截屏位置
		/// </summary>
		/// <param name="hdcDest">目标设备的句柄 </param>
		/// <param name="nXDest">目标对象的左上角的X坐标</param>
		/// <param name="nYDest">目标对象的左上角的Y坐标</param>
		/// <param name="nWidth">目标对象的矩形的宽度</param>
		/// <param name="nHeight">目标对象的矩形的高度 </param>
		/// <param name="hdcSrc">源设备的句柄 </param>
		/// <param name="nXSrc">源对象的左上角的X坐标 </param>
		/// <param name="nYSrc">源对象的左上角的Y坐标 </param>
		/// <param name="dwRop">光栅的操作值 </param>
		/// <returns></returns>
		[DllImportAttribute("gdi32.dll")]
		private static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight,
			IntPtr hdcSrc, int nXSrc, int nYSrc, System.Int32 dwRop);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lpszDriver">驱动名称</param>
		/// <param name="lpszDevice">设备名称</param>
		/// <param name="lpszOutput">无用，可以设定为"NULL"</param>
		/// <param name="lpInitData">任意的打印机数据</param>
		/// <returns></returns>
		[DllImportAttribute("gdi32.dll")]
		private static extern IntPtr CreateDC(string lpszDriver, string lpszDevice, string lpszOutput, IntPtr lpInitData);
		#endregion DLL导入

		/// <summary>
		/// 钩子委托声明
		/// </summary>
		/// <param name="nCode"></param>
		/// <param name="wParam"></param>
		/// <param name="lParam"></param>
		/// <returns></returns>
		public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);
	}
	public class HookHelper
	{
		#region 枚举定义
		/// <summary>
		/// 操作类型
		/// </summary>
		public enum HookType
		{
			KeyOperation,//键盘操作
			MouseOperation//鼠标操作
		}

		/// <summary>
		/// 底层钩子标识
		/// </summary>
		public enum WH_Codes : int
		{
			///底层键盘钩子
			WH_KEYBOARD_LL = 2,//2:监听键盘消息并且是线程钩子；13：全局鼠标钩子监听

			///底层鼠标钩子
			WH_MOUSE_LL = 7, //7：监听鼠标钩子；14:全局键盘钩子监听鼠标信息

			///nCode为0
			HC_ACTION = 0
		}

		/// <summary>
		/// 鼠标按钮标识
		/// </summary>
		public enum WM_MOUSE : int
		{
			/// <summary>
			/// 鼠标开始
			/// </summary>
			WM_MOUSEFIRST = 0x200,

			/// <summary>
			/// 鼠标移动
			/// </summary>
			WM_MOUSEMOVE = 0x200,

			/// <summary>
			/// 左键按下
			/// </summary>
			WM_LBUTTONDOWN = 0x201,

			/// <summary>
			/// 左键释放
			/// </summary>
			WM_LBUTTONUP = 0x202,

			/// <summary>
			/// 左键双击
			/// </summary>
			WM_LBUTTONDBLCLK = 0x203,

			/// <summary>
			/// 右键按下
			/// </summary>
			WM_RBUTTONDOWN = 0x204,

			/// <summary>
			/// 右键释放
			/// </summary>
			WM_RBUTTONUP = 0x205,

			/// <summary>
			/// 右键双击
			/// </summary>
			WM_RBUTTONDBLCLK = 0x206,

			/// <summary>
			/// 中键按下
			/// </summary>
			WM_MBUTTONDOWN = 0x207,

			/// <summary>
			/// 中键释放
			/// </summary>
			WM_MBUTTONUP = 0x208,

			/// <summary>
			/// 中键双击
			/// </summary>
			WM_MBUTTONDBLCLK = 0x209,

			/// <summary>
			/// 滚轮滚动
			/// </summary>
			/// <remarks>WINNT4.0以上才支持此消息</remarks>
			WM_MOUSEWHEEL = 0x020A
		}

		/// <summary>
		/// 键盘按键标识
		/// </summary>
		public enum WM_KEYBOARD : int
		{
			/// <summary>
			/// 非系统按键按下
			/// </summary>
			WM_KEYDOWN = 0x100,

			/// <summary>
			/// 非系统按键释放
			/// </summary>
			WM_KEYUP = 0x101,

			/// <summary>
			/// 系统按键按下
			/// </summary>
			WM_SYSKEYDOWN = 0x104,

			/// <summary>
			/// 系统按键释放
			/// </summary>
			WM_SYSKEYUP = 0x105
		}

		/// <summary>
		/// SetWindowPos标志位枚举
		/// </summary>
		/// <remarks>详细说明,请参见MSDN中关于SetWindowPos函数的描述</remarks>
		public enum SetWindowPosFlags : int
		{
			/// <summary>
			/// 
			/// </summary>
			SWP_NOSIZE = 0x0001,

			/// <summary>
			/// 
			/// </summary>
			SWP_NOMOVE = 0x0002,

			/// <summary>
			/// 
			/// </summary>
			SWP_NOZORDER = 0x0004,

			/// <summary>
			/// 
			/// </summary>
			SWP_NOREDRAW = 0x0008,

			/// <summary>
			/// 
			/// </summary>
			SWP_NOACTIVATE = 0x0010,

			/// <summary>
			/// 
			/// </summary>
			SWP_FRAMECHANGED = 0x0020,

			/// <summary>
			/// 
			/// </summary>
			SWP_SHOWWINDOW = 0x0040,

			/// <summary>
			/// 
			/// </summary>
			SWP_HIDEWINDOW = 0x0080,

			/// <summary>
			/// 
			/// </summary>
			SWP_NOCOPYBITS = 0x0100,

			/// <summary>
			/// 
			/// </summary>
			SWP_NOOWNERZORDER = 0x0200,

			/// <summary>
			/// 
			/// </summary>
			SWP_NOSENDCHANGING = 0x0400,

			/// <summary>
			/// 
			/// </summary>
			SWP_DRAWFRAME = 0x0020,

			/// <summary>
			/// 
			/// </summary>
			SWP_NOREPOSITION = 0x0200,

			/// <summary>
			/// 
			/// </summary>
			SWP_DEFERERASE = 0x2000,

			/// <summary>
			/// 
			/// </summary>
			SWP_ASYNCWINDOWPOS = 0x4000

		}

		#endregion 枚举定义

		#region 结构定义

		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int X;
			public int Y;
		}

		/// <summary>
		/// 鼠标钩子事件结构定义
		/// </summary>
		/// <remarks>详细说明请参考MSDN中关于 MSLLHOOKSTRUCT 的说明</remarks>
		[StructLayout(LayoutKind.Sequential)]
		public struct MouseHookStruct
		{
			/// <summary>
			/// Specifies a POINT structure that contains the x- and y-coordinates of the cursor, in screen coordinates.
			/// </summary>
			public POINT Point;

			public UInt32 MouseData;
			public UInt32 Flags;
			public UInt32 Time;
			public UInt32 ExtraInfo;
		}

		/// <summary>
		/// 键盘钩子事件结构定义
		/// </summary>
		/// <remarks>详细说明请参考MSDN中关于 KBDLLHOOKSTRUCT 的说明</remarks>
		[StructLayout(LayoutKind.Sequential)]
		public struct KeyboardHookStruct
		{
			/// <summary>
			/// Specifies a virtual-key code. The code must be a value in the range 1 to 254. 
			/// </summary>
			public UInt32 VKCode;

			/// <summary>
			/// Specifies a hardware scan code for the key.
			/// </summary>
			public UInt32 ScanCode;

			/// <summary>
			/// Specifies the extended-key flag, event-injected flag, context code, 
			/// and transition-state flag. This member is specified as follows. 
			/// An application can use the following values to test the keystroke flags. 
			/// </summary>
			public UInt32 Flags;

			/// <summary>
			/// Specifies the time stamp for this message. 
			/// </summary>
			public UInt32 Time;

			/// <summary>
			/// Specifies extra information associated with the message. 
			/// </summary>
			public UInt32 ExtraInfo;
		}

		#endregion 结构定义
	}
	public class Hook
	{
		#region 定义变量
		//定义鼠标钩子处理函数
		private Win32API.HookProc MouseHookProcedure = null;
		//定义键盘钩子处理函数
		private Win32API.HookProc KeyboardProcDelegate = null;

		//定义键盘钩子句柄
		private IntPtr khook;
		//定义鼠标钩子句柄
		private IntPtr hHook = IntPtr.Zero;

		//定义鼠标事件
		public event MouseEventHandler OnMouseActivity;
		#endregion

		/// <summary>
		/// 安装钩子
		/// </summary>
		public void InstallHook(HookHelper.HookType hookType)
		{
			if (hookType == HookHelper.HookType.KeyOperation)
			{
				if (khook == IntPtr.Zero)//键盘钩子
				{
					uint id = Win32API.GetCurrentThreadId();
					this.KeyboardProcDelegate = new Win32API.HookProc(this.KeyboardProc);
					khook = Win32API.SetWindowsHookEx((IntPtr)HookHelper.WH_Codes.WH_KEYBOARD_LL, this.KeyboardProcDelegate, IntPtr.Zero, id);
				}
			}
			else
			{
				if (hHook == IntPtr.Zero)//鼠标钩子
				{
					uint id = Win32API.GetCurrentThreadId();
					this.MouseHookProcedure = new Win32API.HookProc(this.MouseHookProc);
					//这里挂节钩子
					hHook = Win32API.SetWindowsHookEx((IntPtr)HookHelper.WH_Codes.WH_MOUSE_LL, MouseHookProcedure, IntPtr.Zero, id);
				}
			}
		}

		/// <summary>
		/// 卸载鼠标钩子
		/// </summary>
		public void UnInstallHook(HookHelper.HookType hookType)
		{
			bool isSuccess = false;
			if (hookType == HookHelper.HookType.KeyOperation)//键盘钩子
			{
				if (khook != IntPtr.Zero)
				{
					isSuccess = Win32API.UnhookWindowsHookEx(khook);
					this.khook = IntPtr.Zero;
				}
			}
			else
			{
				if (this.hHook != IntPtr.Zero)//鼠标钩子
				{
					isSuccess = Win32API.UnhookWindowsHookEx(hHook);
					this.hHook = IntPtr.Zero;
				}
			}
			if (isSuccess)
			{
				MessageBox.Show("卸载成功！");
			}
			else
			{
				MessageBox.Show("卸载失败！");
			}
		}

		/// <summary>
		/// 鼠标钩子处理函数
		/// </summary>
		/// <param name="nCode"></param>
		/// <param name="wParam"></param>
		/// <param name="lParam"></param>
		/// <returns></returns>
		private int MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
		{
			if (nCode < (int)HookHelper.WH_Codes.HC_ACTION)
			{
				return Win32API.CallNextHookEx(hHook, nCode, wParam, lParam);
			}

			if (OnMouseActivity != null)
			{
				//Marshall the data from callback.
				HookHelper.MouseHookStruct mouseHookStruct = (HookHelper.MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(HookHelper.MouseHookStruct));
				MouseButtons button = MouseButtons.None;
				short mouseDelta = 0;
				switch ((int)wParam)
				{
					case (int)HookHelper.WM_MOUSE.WM_LBUTTONDOWN:
						//case WM_LBUTTONUP: 
						//case WM_LBUTTONDBLCLK:
						button = MouseButtons.Left;
						break;
					case (int)HookHelper.WM_MOUSE.WM_RBUTTONDOWN:
						//case WM_RBUTTONUP: 
						//case WM_RBUTTONDBLCLK: 
						button = MouseButtons.Right;
						break;
					case (int)HookHelper.WM_MOUSE.WM_MOUSEWHEEL:
						//button = MouseButtons.Middle;//滚动轮
						//(value >> 16) & 0xffff; retrieves the high-order word from the given 32-bit value
						mouseDelta = (short)((mouseHookStruct.MouseData >> 16) & 0xffff);
						break;
				}

				int clickCount = 0;//点击数
				if (button != MouseButtons.None)
				{
					if ((int)wParam == (int)HookHelper.WM_MOUSE.WM_LBUTTONDBLCLK || (int)wParam == (int)HookHelper.WM_MOUSE.WM_RBUTTONDBLCLK)
					{
						clickCount = 2;//双击
					}
					else
					{
						clickCount = 1;//单击
					}
				}

				//鼠标事件传递数据
				MouseEventArgs e = new MouseEventArgs(button, clickCount, mouseHookStruct.Point.X, mouseHookStruct.Point.Y, mouseDelta);

				//重写事件
				OnMouseActivity(this, e);
			}

			return Win32API.CallNextHookEx(hHook, nCode, wParam, lParam);
		}

		/// <summary>
		/// 键盘钩子处理函数
		/// </summary>
		/// <param name="code"></param>
		/// <param name="wParam"></param>
		/// <param name="lParam"></param>
		/// <returns></returns>
		private int KeyboardProc(int nCode, IntPtr wParam, IntPtr lParam)
		{
			try
			{
				if (nCode < (int)HookHelper.WH_Codes.HC_ACTION)
				{
					return Win32API.CallNextHookEx((IntPtr)khook, nCode, wParam, lParam);
				}

				HookHelper.KeyboardHookStruct keyHookStruct = (HookHelper.KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(HookHelper.KeyboardHookStruct));

				#region
				//if ((int)wParam == (int)Keys.C && ((int)lParam & (int)Keys.ControlKey) != 0 ||//Ctrl+C
				//    (int)wParam == (int)Keys.X && ((int)lParam & (int)Keys.ControlKey) != 0)//Ctrl+V
				//{
				//    MessageBox.Show("C||V");
				//}

				//if (lParam.ToInt32() > 0)//捕获键盘按下
				//{
				//    Keys keys = (Keys)wParam;
				//    MessageBox.Show("键盘按下");
				//}
				//if (lParam.ToInt32() < 0)//捕获键盘抬起
				//{
				//    MessageBox.Show("键盘抬起");
				//}
				/**************** 
                 //全局键盘钩子判断是否按下键 
                 wParam = = 0x100 // 键盘按下 
                 wParam = = 0x101 // 键盘抬起 
                ****************/
				//return 0;//如果返回1,则结束消息,消息截止,不再传递。如果返回0或调用CallNextHookEx函数,消息出这个钩子继续往下传递。
				#endregion

			}
			catch
			{

			}

			return Win32API.CallNextHookEx(khook, nCode, wParam, lParam);
		}
	}
	class KeyHook
	{
		//定义钩子处理函数
		private Win32API.HookProc KeyboardProcDelegate = null;
		//定义钩子句柄
		private IntPtr khook;
		//定义键盘事件
		public event KeyEventHandler OnKeyDown;

		/// <summary>
		/// 安装钩子
		/// </summary>
		public void InstallHook()
		{
			if (khook == IntPtr.Zero)
			{
				uint id = Win32API.GetCurrentThreadId();
				this.KeyboardProcDelegate = new Win32API.HookProc(this.KeyboardProc);
				khook = Win32API.SetWindowsHookEx((IntPtr)HookHelper.WH_Codes.WH_KEYBOARD_LL, this.KeyboardProcDelegate, IntPtr.Zero, id);
			}
		}

		/// <summary>
		/// 卸载钩子
		/// </summary>
		public void UnInstallHook()
		{
			bool isSuccess = false;
			if (khook != IntPtr.Zero)
			{
				isSuccess = Win32API.UnhookWindowsHookEx(khook);
				this.khook = IntPtr.Zero;
			}
			if (isSuccess)
			{
				MessageBox.Show("卸载成功！");
			}
			else
			{
				MessageBox.Show("卸载失败！");
			}
		}

		/// <summary>
		/// 键盘钩子处理函数
		/// </summary>
		/// <param name="code"></param>
		/// <param name="wParam"></param>
		/// <param name="lParam"></param>
		/// <returns></returns>
		private int KeyboardProc(int nCode, IntPtr wParam, IntPtr lParam)
		{
			try
			{
				if (nCode < (int)HookHelper.WH_Codes.HC_ACTION)
				{
					return Win32API.CallNextHookEx((IntPtr)khook, nCode, wParam, lParam);
				}

				HookHelper.KeyboardHookStruct keyHookStruct = (HookHelper.KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(HookHelper.KeyboardHookStruct));

				#region
				//if ((int)wParam == (int)Keys.C && ((int)lParam & (int)Keys.ControlKey) != 0 ||//Ctrl+C
				//    (int)wParam == (int)Keys.X && ((int)lParam & (int)Keys.ControlKey) != 0)//Ctrl+V
				//{
				//    MessageBox.Show("C||V");
				//}

				//if (lParam.ToInt32() > 0)//捕获键盘按下
				//{
				//    Keys keys = (Keys)wParam;
				//    MessageBox.Show("键盘按下");
				//}
				//if (lParam.ToInt32() < 0)//捕获键盘抬起
				//{
				//    MessageBox.Show("键盘抬起");
				//}
				/**************** 
                 //全局键盘钩子判断是否按下键 
                 wParam = = 0x100 // 键盘按下 
                 wParam = = 0x101 // 键盘抬起 
                ****************/
				//return 0;//如果返回1,则结束消息,消息截止,不再传递。如果返回0或调用CallNextHookEx函数,消息出这个钩子继续往下传递。
				#endregion
				bool handled = false;
				if (this.OnKeyDown != null && (wParam == (IntPtr)HookHelper.WM_KEYBOARD.WM_KEYDOWN || wParam == (IntPtr)HookHelper.WM_KEYBOARD.WM_SYSKEYDOWN))
				{
					Keys keyData = (Keys)keyHookStruct.VKCode;
					KeyEventArgs e = new KeyEventArgs(keyData);
					OnKeyDown.Invoke(this, e);
					handled = e.Handled;
				}

				if (handled)
					return -1;

				return Win32API.CallNextHookEx(khook, nCode, wParam, lParam);
			}
			catch
			{

			}

			return Win32API.CallNextHookEx(khook, nCode, wParam, lParam);
		}
	}
	public class MouseHook
	{
		#region 定义变量
		//定义钩子处理函数
		private Win32API.HookProc MouseHookProcedure;
		//定义钩子句柄
		private IntPtr hHook = IntPtr.Zero;
		//定义鼠标事件
		public event MouseEventHandler OnMouseActivity;
		#endregion

		/// <summary>
		/// 安装鼠标钩子
		/// </summary>
		public void InstallHook()
		{
			if (hHook == IntPtr.Zero)
			{
				uint id = Win32API.GetCurrentThreadId();
				this.MouseHookProcedure = new Win32API.HookProc(this.MouseHookProc);
				//这里挂节钩子
				hHook = Win32API.SetWindowsHookEx((IntPtr)HookHelper.WH_Codes.WH_MOUSE_LL, MouseHookProcedure, IntPtr.Zero, id);
			}
		}

		/// <summary>
		/// 卸载鼠标钩子
		/// </summary>
		public void UnInstallHook()
		{
			bool isSuccess = false;
			if (this.hHook != IntPtr.Zero)
			{
				isSuccess = Win32API.UnhookWindowsHookEx(hHook);
				this.hHook = IntPtr.Zero;
			}
			if (isSuccess)
			{
				MessageBox.Show("卸载成功！");
			}
			else
			{
				MessageBox.Show("卸载失败！");
			}
		}

		/// <summary>
		/// 鼠标钩子处理函数
		/// </summary>
		/// <param name="nCode"></param>
		/// <param name="wParam"></param>
		/// <param name="lParam"></param>
		/// <returns></returns>
		private int MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
		{
			if (nCode < (int)HookHelper.WH_Codes.HC_ACTION)
			{
				return Win32API.CallNextHookEx(hHook, nCode, wParam, lParam);
			}

			if (OnMouseActivity != null)
			{
				//Marshall the data from callback.
				HookHelper.MouseHookStruct mouseHookStruct = (HookHelper.MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(HookHelper.MouseHookStruct));
				MouseButtons button = MouseButtons.None;
				short mouseDelta = 0;
				switch ((int)wParam)
				{
					case (int)HookHelper.WM_MOUSE.WM_LBUTTONDOWN:
						//case WM_LBUTTONUP: 
						//case WM_LBUTTONDBLCLK:
						button = MouseButtons.Left;
						break;
					case (int)HookHelper.WM_MOUSE.WM_RBUTTONDOWN:
						//case WM_RBUTTONUP: 
						//case WM_RBUTTONDBLCLK: 
						button = MouseButtons.Right;
						break;
					case (int)HookHelper.WM_MOUSE.WM_MOUSEWHEEL:
						//button = MouseButtons.Middle;//滚动轮
						//(value >> 16) & 0xffff; retrieves the high-order word from the given 32-bit value
						mouseDelta = (short)((mouseHookStruct.MouseData >> 16) & 0xffff);
						break;
				}

				int clickCount = 0;//点击数
				if (button != MouseButtons.None)
				{
					if (wParam == (IntPtr)HookHelper.WM_MOUSE.WM_LBUTTONDBLCLK || wParam == (IntPtr)HookHelper.WM_MOUSE.WM_RBUTTONDBLCLK)
					{
						clickCount = 2;//双击
					}
					else
					{
						clickCount = 1;//单击
					}
				}

				//鼠标事件传递数据
				MouseEventArgs e = new MouseEventArgs(button, clickCount, mouseHookStruct.Point.X, mouseHookStruct.Point.Y, mouseDelta);

				//重写事件
				OnMouseActivity(this, e);
			}

			return Win32API.CallNextHookEx(hHook, nCode, wParam, lParam);
		}
	}

	//This is test form
	//public partial class Form1 : Form
	//{
	//	public Form1()
	//	{
	//		InitializeComponent();
	//	}
	//	private Hook hook = new Hook();//总钩子控制
	//	private KeyHook keyHook = new KeyHook();//键盘钩子        
	//	private MouseHook mouseHook = new MouseHook();//鼠标钩子        

	//	private void Form1_Load(object sender, EventArgs e)
	//	{
	//		hook.OnMouseActivity += new MouseEventHandler(hook_OnMouseActivity);
	//		mouseHook.OnMouseActivity += new MouseEventHandler(mouseHook_OnMouseActivity);
	//		keyHook.OnKeyDown += new KeyEventHandler(keyHook_OnKeyDown);
	//	}

	//	void keyHook_OnKeyDown(object sender, KeyEventArgs e)
	//	{
	//		WriteLog("KeyDown     - " + e.KeyData.ToString());
	//	}

	//	void hook_OnMouseActivity(object sender, MouseEventArgs e)
	//	{
	//		this.lblMousePosition.Text = string.Format("X:{0} Y={1} Wheel={2}", e.X, e.Y, e.Delta);
	//		if (e.Clicks > 0)
	//		{
	//			WriteLog("MouseButton   -" + e.Button.ToString());
	//		}
	//	}

	//	private void btnMouseHook_Click(object sender, EventArgs e)
	//	{
	//		//mouseHook.InstallHook();
	//		hook.InstallHook(HookHelper.HookType.MouseOperation);
	//	}

	//	private void btnKeyHook_Click(object sender, EventArgs e)
	//	{
	//		keyHook.InstallHook();
	//		//hook.InstallHook(HookHelper.HookType.KeyOperation);
	//	}

	//	private void btnUnIntallHook_Click(object sender, EventArgs e)
	//	{
	//		//mouseHook.UnInstallHook();
	//		//keyHook.UnInstallHook();

	//		hook.UnInstallHook(HookHelper.HookType.KeyOperation);//卸载键盘钩子
	//		hook.UnInstallHook(HookHelper.HookType.MouseOperation);//卸载鼠标钩子
	//	}

	//	private void WriteLog(string message)
	//	{
	//		if (string.IsNullOrWhiteSpace(this.richMessage.Text))
	//		{
	//			this.richMessage.Text = "监控鼠标和键盘操作记录：" + Environment.NewLine;
	//		}
	//		this.richMessage.AppendText(message + Environment.NewLine);
	//		this.richMessage.SelectionStart = this.richMessage.Text.Length;
	//	}

	//	void mouseHook_OnMouseActivity(object sender, MouseEventArgs e)
	//	{
	//		this.lblMousePosition.Text = string.Format("X:{0} Y={1} Wheel={2}", e.X, e.Y, e.Delta);
	//		if (e.Clicks > 0)
	//		{
	//			WriteLog("MouseButton   -" + e.Button.ToString());
	//		}
	//	}
	//	private void Form1_FormClosed(object sender, FormClosedEventArgs e)
	//	{
	//		hook.UnInstallHook(HookHelper.HookType.KeyOperation);//卸载键盘钩子
	//		hook.UnInstallHook(HookHelper.HookType.MouseOperation);//卸载鼠标钩子
	//	}
	//}

}

