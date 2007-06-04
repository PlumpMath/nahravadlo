using System;
using System.Windows.Forms;

namespace Nahravadlo
{
	class Utils
	{
		public static void ShowBubble(IWin32Window obj, ToolTipIcon toolTipIcon, String toolTipTitle, String toolTipText)
		{
			const int duration = 5000;
			
			ShowBubble(obj, toolTipIcon, toolTipTitle, toolTipText, duration);
		}
		
		public static void ShowBubble(IWin32Window obj, ToolTipIcon toolTipIcon, String toolTipTitle, String toolTipText, int duration)
		{
			ToolTip tip = new ToolTip();
			tip.IsBalloon = true;
			tip.ToolTipIcon = toolTipIcon;
			tip.ToolTipTitle = toolTipTitle;
			tip.Active = false;
			tip.Show("", obj);
			tip.Active = true;
			tip.Show(toolTipText, obj, duration);
		}
	}
}
