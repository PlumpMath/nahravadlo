using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Nahravadlo
{
	internal class Utils
	{
		public static void ShowBubble(IWin32Window obj, ToolTipIcon toolTipIcon, String toolTipTitle, String toolTipText)
		{
			const int duration = 5000;

			ShowBubble(obj, toolTipIcon, toolTipTitle, toolTipText, duration);
		}

		public static void ShowBubble(IWin32Window obj, ToolTipIcon toolTipIcon, String toolTipTitle, String toolTipText, int duration)
		{
			var tip = new ToolTip {IsBalloon = true, ToolTipIcon = toolTipIcon, ToolTipTitle = toolTipTitle, Active = false};
		    tip.Show("", obj);
			tip.Active = true;
			tip.Show(toolTipText, obj, duration);
		}

		public static DateTime ParseISO8601DateTime(string dt)
		{
			var parseFormat = new[] {"r", "s", "u", "yyyyMMdd'T'HHmmss", "yyyyMMdd'T'HHmmss%K", "yyyy-MM-dd'T'HH:mm:ss", "yyyy-MM-dd'T'HH:mm:ss%K"};
			return DateTime.ParseExact(dt, parseFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal | DateTimeStyles.AdjustToUniversal).ToLocalTime();
		}

		public static string CorrectFilename(string name)
		{
			var r = new Regex("[/:\\*\\?\"<>\\|]");
			return r.Replace(name, "_");
		}

		public static string RemoveDiacritics(string stIn)
		{
			var stFormD = stIn.Normalize(NormalizationForm.FormD);
			var sb = new StringBuilder();

			for (var ich = 0; ich < stFormD.Length; ich++)
			{
				var uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
				if (uc != UnicodeCategory.NonSpacingMark)
					sb.Append(stFormD[ich]);
			}

			return (sb.ToString().Normalize(NormalizationForm.FormC));
		}
	}
}