using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;


namespace OpenWLS.Server.Base
{
    public class IntArrayConverter {
        public static byte[]? GetBytes(uint[]? uint_array)
        {
            if (uint_array == null) return null;
            int c = uint_array.Length * sizeof(uint);
            if (c == 0) return null;
            byte[] bs = new byte[c];
            Buffer.BlockCopy(uint_array, 0, bs, 0, c);
            return bs;
        }
        public static byte[]? GetBytes(int[]? int_array)
        {
            if (int_array == null) return null;
            int c = int_array.Length * sizeof(int);
            if (c == 0) return null;
            byte[] bs = new byte[c];
            Buffer.BlockCopy(int_array, 0, bs, 0, c);
            return bs;
        }
        public static uint[] GetUint32Array(byte[] bs)
        {
            int c = bs.Length / sizeof(uint);
        //    if (c == 0) return null;
            uint[] res = new uint[c];
            Buffer.BlockCopy(bs, 0, res, 0, bs.Length);
            return res;
        }

        public static int[] GetInt32Array(byte[] bs)
        {
            int c = bs.Length / sizeof(int);
       //     if (c == 0) return null;
            int[] res = new int[c];
            Buffer.BlockCopy(bs, 0, res, 0, bs.Length);
            return res;
        }
    }
    /*
    public class DimensionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int[] a = (int[])value;
            if (a == null || a.Length == 0)
                return "";
            string str = a[0].ToString();
            for (int i = 1; i < a.Length; i++)
                if(a[i] > 0)
                    str = str + "," + a[i].ToString();
            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        public static int[] GetFromString(string s)
        {
            if (s == null)
                return null;
            s = s.Trim();
            if (s.Length == 0)
                return null;
            string[] ss = s.Trim().Split(',');
            int[] ns = new int[ss.Length];
            for (int i = 0; i < ss.Length; i++)
                ns[i] = System.Convert.ToInt32(ss[i]);
            return ns;
        }
        public static string ToString(int[] ns)
        {
            if (ns == null)
                return null;
            if (ns.Length == 0)
                return null;
            string s = ns[0].ToString();
            for (int i = 1; i < ns.Length; i++)
                s = s + "," + ns[i].ToString();
            return s;
        }

    }*/
}
    