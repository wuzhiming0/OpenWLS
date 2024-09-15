using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace OpenWLS.Client.GView.GUI
{
    public class MediaColorConverter : IValueConverter
    {
        static public uint ConvertColorToUint(System.Windows.Media.Color c)
        {
            return (uint)(c.A << 24) + (uint)(c.R << 16) + (uint)(c.G << 8) + (uint)(c.B);
        }

        static public System.Drawing.Color ConvertToColor(uint c)
        {
            byte a = (byte)(c >> 24);
            byte r = (byte)(c >> 16);
            byte g = (byte)(c >> 8);
            byte b = (byte)(c >> 0);
            return System.Drawing.Color.FromArgb(a, r, g, b);
        }
        static public System.Windows.Media.Color ConvertToMediaColor(uint c)
        {
            byte a = (byte)(c >> 24);
            byte r = (byte)(c >> 16);
            byte g = (byte)(c >> 8);
            byte b = (byte)(c >> 0);
            return System.Windows.Media.Color.FromArgb(a, r, g, b);
        }
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is uint)
              return ConvertToMediaColor((uint)value);
            return ConvertToColor(0);           
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is System.Windows.Media.Color)            
                return ConvertColorToUint((System.Windows.Media.Color)value);
            return 0;
        }
    }
}
