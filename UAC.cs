using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows.Forms;

namespace Nahravadlo
{
	public class UAC
	{
		private const int BcmFirst = 0x1600;
		internal const int BcmSetShield = (BcmFirst + 0x000C);

		[DllImport("user32")]
		private static extern UInt32 SendMessage(IntPtr hWnd, UInt32 msg, UInt32 wParam, UInt32 lParam);

		public static bool IsVistaOrHigher()
		{
			return Environment.OSVersion.Version.Major >= 6;
		}

		/// <summary>
		/// Checks if the process is elevated
		/// </summary>
		/// <returns>If is elevated</returns>
		public static bool IsAdmin()
		{
			var id = WindowsIdentity.GetCurrent();
            if (id == null) return false;

			var p = new WindowsPrincipal(id);
			return p.IsInRole(WindowsBuiltInRole.Administrator);
		}

		/// <summary>
		/// Add a shield icon to a button
		/// </summary>
		/// <param name="b">The button</param>
		public static void AddShieldToButton(Button b)
		{
			b.FlatStyle = FlatStyle.System;
			SendMessage(b.Handle, BcmSetShield, 0, 0xFFFFFFFF);
		}

		/// <summary>
		/// Restart the current process with administrator credentials
		/// </summary>
		public static void RestartElevated(bool passArgs)
		{
			var startInfo = new ProcessStartInfo
			                    {
			                        UseShellExecute = true,
			                        WorkingDirectory = Environment.CurrentDirectory,
			                        FileName = Application.ExecutablePath
			                    };
		    if (passArgs)
				startInfo.Arguments = String.Join(" ", Environment.GetCommandLineArgs(), 1, Environment.GetCommandLineArgs().Length - 1);
			startInfo.Verb = "runas";
			try
			{
				Process.Start(startInfo);
			}
			catch {}
		}
	}
}