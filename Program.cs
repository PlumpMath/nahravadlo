using System;
using System.Windows.Forms;
using Nahravadlo.Schedule.VLC;

namespace Nahravadlo
{
	public static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		public static void Main(String[] args)
		{
		    Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			if (args.Length > 0)
				Application.Run(new formMain(args));
			else
				Application.Run(new formMain());
		}
	}
}