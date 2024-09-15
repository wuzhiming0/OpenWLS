using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using OpenWLS.Server.Base;
using Microsoft.AspNetCore.Http.HttpResults;
using static System.Net.WebRequestMethods;
using System.Reflection;

namespace OpenWLS.Server.GView.Models
{
    public class GvFillImage : GvItem
    {
       // public static string fpNamePath = @"../../data/server/sys/fill-patterns";
        /*    public static string[] GetNames()
            {
                string[] fileEntries = Directory.GetFiles(fpNamePath);
                int c = fpNamePath.Length + 1;
                for (int i = 0; i < fileEntries.Length; i++)
                {
                    fileEntries[i] = fileEntries[i].Remove(0, c);
                    fileEntries[i] = fileEntries[i].Remove(fileEntries[i].Length - 4, 4);
                }
                return fileEntries;
            }
        */
        public static string[] GetNames()
        {
            List<string> ns = new List<string>();
            foreach (string resourceName in Assembly.GetExecutingAssembly().GetManifestResourceNames())
            {
                int k = resourceName.IndexOf("fill_patterns");
                if(k > 0)
                {
                    string n = resourceName.Substring(k + 14, resourceName.Length - k - 18);
                    ns.Add(n);
                }
            }
            return ns.ToArray();
        }

        protected byte[] fpBmp;
        public GvFillImage()
        {
            EType = GvType.FillingImage;
        }
        
        public void CreateFillImage(string fn, uint fcolor, uint bcolor)
        {
            Assembly a = Assembly.GetExecutingAssembly();
            string str = $"OpenWLS.Server.GView.fill_patterns.{fn}.bmp";
            using (Stream fs = a.GetManifestResourceStream(str))
            {
                int c = (int)fs.Length;
                fpBmp = new byte[c];
                fs.Read(fpBmp, 0, c);
                fs.Close();

                DataReader reader = new DataReader( fpBmp);
                reader.SetByteOrder(false);
                if (fpBmp[0x1c] == 8)
                {
                    reader.Seek(54, SeekOrigin.Begin);

                    while (reader.Position < 950)
                    {
                        if (reader.ReadInt32() == 0)
                            break;
                    }

                    if (reader.Position < 950)
                    {
                        int i = reader.Position;
                        fpBmp[i++] = 0; fpBmp[i++] = 0; fpBmp[i++] = 0;
                    }

                }
                else
                {
                    if (fpBmp[0x1c] == 1)
                    {
                        int k = 0x33;
                        fpBmp[k++] =  (byte)(bcolor >> 0);
                        fpBmp[k++] = (byte)(bcolor >> 8);
                        fpBmp[k++] = (byte)(bcolor >> 16);

                        fpBmp[k++] =  (byte)(fcolor >> 0);
                        fpBmp[k++] = (byte)(fcolor >> 8);
                        fpBmp[k++] = (byte)(fcolor >> 16);
                        k++;
                    }
                    else
                    {
                        int r = (byte)(fcolor >> 16);
                        int g = (byte)(fcolor >> 8);
                        int b = (byte)(fcolor >> 0);
                        int k = 0x36;
                        fpBmp[k++] = (byte)b;
                        fpBmp[k++] = (byte)g;
                        fpBmp[k++] = (byte)r;
                    }
                }
            }
        }
  
        protected override void RestoreExt(byte[] bs)
        {
            fpBmp = bs;
        }

        public override byte[]? GetItemExtBytes()
        {
            return fpBmp;
             
        }

    }
}
