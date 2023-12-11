using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMS.Helpers
{
	public static class Common
    {
		public static Windows.UI.Color GetColor(string hex)
		{
			hex = hex.Replace("#", string.Empty);
			byte a = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
			byte r = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
			byte g = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
			byte b = (byte)(Convert.ToUInt32(hex.Substring(6, 2), 16));
			return Windows.UI.Color.FromArgb(a, r, g, b);
		}
	}
}
