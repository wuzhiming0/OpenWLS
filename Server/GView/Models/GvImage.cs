using Microsoft.EntityFrameworkCore.Storage;
//using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.GView.Models
{
    public enum ColorMode { Earth = 0, VDL = 1, Hue = 2, Custom = 3 };
    public class GvImage : GvItem
    {
        //                                  0     1    2   3  4  5  6  7  8  9  10   11   12 13  14 15 16 17 18 19 20 21 22 23 24 25 
        static byte[] bmpHD = new byte[] { 0x42, 0x4d, 14, 0, 0, 0, 0, 0, 0, 0, 0x1a, 0x3, 0, 0, 12, 0, 0, 0, 0, 0, 0, 0, 1, 0, 8, 0 };
        public static int headSize = 14 + 12 + 256 * 3;  //0x31a = 794
        int colors;


        protected byte[] bh;
        float y_pre;
       // protected ushort bmpWidth;
        public ushort BmpWidth
        {
            set
            {
         //       bmpWidth = value;
       //         actualWidth = GetActualWidth(width);
                bh[18] = (byte)value;
                bh[19] = (byte)(value >> 8);
            }
            get {
                return (ushort)(bh[18] + (bh[19] << 8)) ;
              }
        }

        public ushort BmpHeight
        {
            set
            {
                bh[20] = (byte)value;
                bh[21] = (byte)(value >> 8);
            }
            get
            {
                return (ushort)(bh[20] + (bh[21] << 8));
            }
        }

        public static int GetActualWidth(int w)
        {
            int i = w % 4;
            return w + 4 - i;
        }

        public GvImage()
        {
            colors = 64;
            EType = GvType.Image;
            bh = new byte[headSize];
            Buffer.BlockCopy(bmpHD, 0, bh, 0, bmpHD.Length);
        }

        public override byte[]? GetItemExtBytes()
        {
            return bh;
        }

        protected override void RestoreExt(byte[] bs)
        {
            bh = bs;
        }

        public void AddLine(byte[] ps, float y)
        {
            if (sectionCur == null)
                sectionCur = new GvImageSection(Id, y, BmpWidth, BmpHeight);
           
            if (((GvImageSection)sectionCur).Full)
            {
                ClosePixels(y);
                sectionCur = new GvImageSection(Id, y, BmpWidth, BmpHeight);
            }
            ((GvImageSection)sectionCur).AddLine(ps);           
            y_pre = y;
        }
        public override void FlushSection()
        {
            if(sectionCur != null )
                ClosePixels(y_pre);
        }
        public void ClosePixels(float y)
        {
            if (sectionCur == null || ((GvImageSection)sectionCur).Empty)
                return;
            sectionCur.Bot = y;
            Doc.AddSection(this, sectionCur);
            sectionCur = null;
        }

        public void CreateBmpHead(int mode, uint colorLow, uint colorHigh)
        {
            ColorMode cm = (ColorMode)mode;
            switch (cm)
            {
                case ColorMode.Earth:
                    SetEarthPal(); break;
                case ColorMode.Hue:
                    SetHuePal(); break;
                //      case ColorMode.VDL:
                //          SetAmpPal(); break;
                default:
                    SetCustomPal(colorLow, colorHigh);
                    break;
            }
        }

        void SetEarthPal()
        {
            // unsigned char* cs = &bh[26];
            int i, k = 26;
            for (i = 0; i < 15; i++)
            {
                bh[k++] = 0; bh[k++] = 0; bh[k++] = (byte)(255 * i / 15);
            }
            int s1 = 15; int s2 = 49;
            int lr = 255; int hr = 210;
            int lg = 0; int hg = 210;
            int lb = 0; int hb = 0;
            k = SetColors(s1, s2, lr, lg, lb, hr, hg, hb, k);

            hr = 255; lr = 210;
            hg = 255; lg = 210;
            hb = 200; lb = 0;
            s1 = 49; s2 = colors;
            k = SetColors(s1, s2, lr, lg, lb, hr, hg, hb, k);

            for (i = colors; i < 256; i++)
            {
                bh[k++] = 255; bh[k++] = 255; bh[k++] = 255;
            }
        }
        public void SetCustomPal(uint colorLow, uint colorHigh)
        {
            //   unsigned char* cs = &bh[26];
            int hb = (int)(colorHigh & 0xff); int lb = (int)(colorHigh & 0xff); int db = hb - lb;
            int hg = (int)(colorHigh >> 8) & 0xff; int lg = (int)(colorLow >> 8) & 0xff; int dg = hg - lg;
            int hr = (int)(colorHigh >> 16) & 0xff; int lr = (int)(colorHigh >> 16) & 0xff; int dr = hr - lr;
            int i, j = 26;
            for (i = 0; i < colors; i++)
            {
                bh[j++] = (byte)(lb + db * i / 64);
                bh[j++] = (byte)(lg + dg * i / 64);
                bh[j++] = (byte)(lr + dr * i / 64);
            }
            for (i = colors; i < 256; i++)
            {
                bh[j++] = 255; bh[j++] = 255; bh[j++] = 255;
            }
        }
        int SetColors(int s1, int s2, int lr, int lg, int lb, int hr, int hg, int hb, int k)
        {
            int dr = hr - lr;
            int dg = hg - lg;
            int db = hb - lb;
            int s3 = s2 - s1;
            for (int i = s1; i < s2; i++)
            {
                bh[k++] = (byte)(lb + db * (i - s1) / s3);
                bh[k++] = (byte)(lg + dg * (i - s1) / s3);
                bh[k++] = (byte)(lr + dr * (i - s1) / s3);
            }
            return k;
        }
        void SetHuePal()
        {
            int s1 = 0;
            int s2 = 8;
            int k = 26;
            //b-bg
            int lr = 0; int hr = 0;
            int lg = 0; int hg = 0;
            int lb = 128; int hb = 255;

            k = SetColors(s1, s2, lr, lg, lb, hr, hg, hb, k);

            //b-bg
            lr = 0; hr = 0;
            lg = 0; hg = 255;
            lb = 255; hb = 255;
            s1 = 8; s2 = 20;
            k = SetColors(s1, s2, lr, lg, lb, hr, hg, hb, k);

            //bg-g
            lr = 0; hr = 0;
            lg = 255; hg = 255;
            lb = 255; hb = 0;
            s1 = 20; s2 = 32;
            k = SetColors(s1, s2, lr, lg, lb, hr, hg, hb, k);

            //g-gr
            lr = 0; hr = 255;
            lg = 255; hg = 255;
            lb = 0; hb = 0;
            s1 = 32; s2 = 44;
            k = SetColors(s1, s2, lr, lg, lb, hr, hg, hb, k);

            //gr-r
            lr = 255; hr = 255;
            lg = 255; hg = 0;
            lb = 0; hb = 0;
            s1 = 44; s2 = 56;
            k = SetColors(s1, s2, lr, lg, lb, hr, hg, hb, k);

            //r
            lr = 255; hr = 128;
            lg = 0; hg = 0;
            lb = 0; hb = 0;
            s1 = 56; s2 = colors;
            k = SetColors(s1, s2, lr, lg, lb, hr, hg, hb, k);
            for (int i = colors; i < 256; i++)
            {
                bh[k++] = 255; bh[k++] = 255; bh[k++] = 255;
            }
            bh[26] = 255;
            bh[27] = 255;
            bh[28] = 255;
        }
    }

    public class GvImageSection : GvSection
    {
        int line_wr;
        int w;
        int h;
        int actualWidth;
        public bool Full
        {
            get { return (line_wr < 0); }
        }
        public bool Empty
        {
            get
            {
                return xs == null || line_wr == (h - 1);
            }
        }
        public byte[] xs;

        public GvImageSection()
        {
            xs = null;
        }
        public void AddLine(byte[] ps)
        {
            if ( line_wr >=0 && line_wr < h )
           //     Buffer.BlockCopy(ps, 0, xs, line_wr * w, ps.Length);
                  Buffer.BlockCopy(ps, 0, xs, line_wr * actualWidth, ps.Length);
            if (line_wr >= 0)
                line_wr--;
        }
        public  GvImageSection(int iid, float y, ushort width, ushort height)
        {
            IId = iid;
            Top = y;
            w = width;
            actualWidth = GvImage.GetActualWidth(w);
            h = height;
            line_wr = h - 1;
            if (xs == null)              
                xs = new byte[(actualWidth * height)];
        }
        public override byte[] GetValBytes()
        {
            if(Empty) return null;
            if(line_wr == -1)
                return xs;
            else
            {
                int s = (h - line_wr - 1) * actualWidth;
                byte[] bs = new byte[s];
                Buffer.BlockCopy(xs, (line_wr + 1) * actualWidth, bs, 0, s);
                return bs;
            }
        }

        public override void RestoreVal(byte[] bs) {   
            xs = bs; 
        } 

      

    }
}
