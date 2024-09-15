using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
*/
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;

namespace OpenWLS.Server.Base
{
    public class StringConverter
    {
        static public string ArrayToString(string[] strs, char sep)
        {
            if (strs == null || strs.Length == 0)
                return "";
            string str = strs[0];
            for (int i = 1; i < strs.Length; i++)
                str = str + sep + strs[i];
                return str;
        }
        static public string ArrayToString(int[] ns, char sep)
        {
            if (ns == null || ns.Length == 0)
                return "";
            string str = ns[0].ToString();
            for (int i = 1; i < ns.Length; i++)
                str = str + sep + ns[i];
                return str;
        }
        static public int[] ToIntArray(object str, char sep)
        {
            if (str == DBNull.Value)
                return  null;
            string[] strs = ((string)str).Split(new char[] { sep });
            int[] ds = new int[strs.Length];
            for (int i = 0; i < strs.Length; i++)
                ds[i] = Convert.ToInt32(strs[i]);
            
            return ds;
        }

        static public float[] ToFloatArray(object str, char sep)
        {
            if (str == DBNull.Value)
                return null;
            string[] strs = ((string)str).Split(new char[] { sep });
            float[] ds = new float[strs.Length];
            for (int i = 0; i < strs.Length; i++)
                ds[i] = Convert.ToSingle(strs[i]);

            return ds;
        }

        public static byte[] ToByteArrayWithSizeByte(string? str)
        {
            if(str == null)return new byte[1] { 0 };
            byte[] bs = Encoding.UTF8.GetBytes(str);
            int s = bs.Length;
            if (s > 255) s = 255;
            byte[] res = new byte[s+1];
            res[0] = (byte)s;
            Buffer.BlockCopy(bs, 0, res, 1, s);
            return res;
        }

        public static byte[] ToByteArray( string str)
        {
            return System.Text.Encoding.UTF8.GetBytes(str);
        }

        public static byte[] ToByteArray( string str, int size, byte filling)
        {
            byte[] bs1 =  System.Text.Encoding.UTF8.GetBytes(str);
            byte[] bs2 = new byte[size];
            int s = Math.Min(bs1.Length, bs2.Length);
            Buffer.BlockCopy(bs1, 0, bs2, 0, s );

            for (int i = s; i < bs2.Length; i++)
                bs2[i] = filling;

            return bs2;
        }

        public static byte[] ToByteArray(string str, int size)
        {
            return ToByteArray(str, size, (byte)' ');
        }

        public static string ToString( byte[] bs)
        {
            return System.Text.UTF8Encoding.UTF8.GetString(bs);
        }


        public static string ToString(byte[] bs, int offset, int size )
        {
            byte[] bs1 = new byte[size];
            Buffer.BlockCopy(bs, offset, bs1, 0, size);
            return System.Text.UTF8Encoding.UTF8.GetString(bs);
        }


        public static string GetEmbeddedString(string res)
        {
            Assembly _assembly = Assembly.GetExecutingAssembly();
            using (StreamReader sr = new StreamReader(_assembly.GetManifestResourceStream(res)))
            {
                return sr.ReadToEnd();
            }
        }
        public static string GetNxtFileName(string fn, string ext)
        {
            string str_path = Path.GetDirectoryName(fn);
            string str_fn = Path.GetFileName(fn);
            if (str_fn.ToLower().EndsWith(ext))
                str_fn = str_fn.Substring(0, str_fn.Length - 4);
            int k = str_fn.LastIndexOf('_');
            if (k > 0)
            {
                if (!int.TryParse(fn.Substring(k + 1, str_fn.Length - k - 1), out k))
                    k = 1;
            }
            else
                k = 1;
            string res = null;
            while (true)
            {
                res = $"{str_path}/{str_fn}_{k}{ext}";
                if (File.Exists(res))
                    k++;
                else break;
            }
            return res;
        }

   
    /*
        public static string GetNxtFileName(string path, string name, string ext )
        {
            string[] ss1 = Directory.GetFiles(path, "*" + ext);
            if (ss1.Length == 0)
                return Path.GetFullPath(path) + name + "_0" + ext;

            string[] ss2 = new string[ss1.Length];
            for(int k = 0; k < ss1.Length; k++)
                ss2[k] = Path.GetFileNameWithoutExtension(ss1[k]);

            bool b = true;
            int n = 0;
            while(b)
            {
                b = false;
                string s1 = name + "_" + n.ToString(); 
                foreach(string s2 in ss2)
                {
                    if(s2 == s1)
                    {
                        b = true;
                        n++;
                        break;
                    }
                }
            }
            return Path.GetFullPath(path) + name + "_" + n.ToString() + ext; 
        }
        */
        public static string[] GetStringArray(double[] ds, string f)
        {
            string[] ss = new string[ds.Length];
            if (f == null)
            {
                for (int i = 0; i < ds.Length; i++)
                    ss[i] = ds[i].ToString(".###");
            }
            else
            {
                if (f[0] == '{')
                {
                    for (int i = 0; i < ds.Length; i++)
                        ss[i] = string.Format(f, ds[i]);
                }
                else
                {
                    for (int i = 0; i < ds.Length; i++)
                        ss[i] = ds[i].ToString(f);
                }
            }
            return ss;
        }
        public static string[] GetStringArray(double[][] ds, string f)
        {
            string[] ss = new string[ds.Length];
            if (f == null)
            {
                for (int i = 0; i < ds.Length; i++)
                {
                    double[] ds1 = ds[i];
                    string s = ds1[0].ToString(".###");
                    for (int j = 0; j < ds1.Length; j++)
                        s = s + "|" + ds1[j].ToString();
                    ss[i] = s;
                }    
            }
            else
            {
                if (f[0] == '{')
                {
                    for (int i = 0; i < ds.Length; i++)
                    {
                        double[] ds1 = ds[i];
                        string s = string.Format(f, ds[0]);
                        for (int j = 0; j < ds1.Length; j++)
                            s = s + "|" + string.Format(f, ds1[j]);
                        ss[i] = s;
                    }
                }
                else
                {
                    for (int i = 0; i < ds.Length; i++)
                    {
                        double[] ds1 = ds[i];
                        string s = ds1[0].ToString(f);
                        for (int j = 0; j < ds1.Length; j++)
                            s = s + "|" + ds1[j].ToString(f);
                        ss[i] = s;
                    }
                }
            }
            return ss;
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static string FirstCharToUpperLinq(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            return $"{input.FirstOrDefault().ToString().ToUpper()}{input.Substring(1)}";
        }

        public static string GetStringAndTrimEnd(double v, string format)
        {
            string str = v.ToString(format);
            int k = str.Length - 1;
            if (str[k] == '0')
            {
                str = str.Remove(k, 1);
                if (str[k - 1] == '0')
                {
                    str = str.Remove(k - 1, 1);
                    if (str[k - 2] == '.')
                        str = str.Remove(k - 2, 1);
                }
            }
            return str;
        }
    }
}
