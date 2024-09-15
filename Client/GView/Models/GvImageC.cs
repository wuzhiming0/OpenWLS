using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using System.Drawing;
using OpenWLS.Server.Base;
using OpenWLS.Server.GView.Models;

namespace OpenWLS.Client.GView.Models
{
  

    public class GvImageC : GvImage,  IGvItemC
    {

        float fl;
        float fr;

        public void DrawItem(Graphics g, float top, float bot)
        {
            int actualWidth = GetActualWidth(BmpWidth);
            foreach (GvImageSection s in sections)
            {
                byte[] bs = new byte[GvImage.headSize + s.xs.Length];
                int h = s.xs.Length / actualWidth;
                BmpHeight = (ushort)h;   
                Buffer.BlockCopy(bh, 0, bs, 0, GvImage.headSize);
                Buffer.BlockCopy(s.xs, 0, bs, GvImage.headSize, s.xs.Length);
                MemoryStream ms = new MemoryStream(bs); 
                Bitmap bp = new Bitmap(ms);
                System.Drawing.RectangleF src = h == 1 ? new System.Drawing.RectangleF(0, 0, BmpWidth, h) : new System.Drawing.RectangleF(0, 0, BmpWidth, h - 1);
                System.Drawing.RectangleF dst = new System.Drawing.RectangleF(fl, (float)(s.Top - top), fr - fl, s.Height);

                g.DrawImage(bp, dst, src, GraphicsUnit.Pixel);
            }

        }
        public float DrawScrollbar(Graphics g, float leftMargin, float w, float sh)
        {
            if (SBar == null  )
                return leftMargin;          
            foreach (GvImageSection s in sections)
            {
                byte[] bs = new byte[GvImage.headSize + s.xs.Length];
                Buffer.BlockCopy(bh, 0, bs, 0, GvImage.headSize);
                Buffer.BlockCopy(s.xs, 0, bs, GvImage.headSize, s.xs.Length);
                MemoryStream ms = new MemoryStream(bs);
                Bitmap bp = new Bitmap(ms);
            //    System.Drawing.RectangleF src = s.Height == 1 ? new System.Drawing.RectangleF(0, 0, Width, s.Height) : new System.Drawing.RectangleF(0, 0, Width, s.Height - 1);
                System.Drawing.RectangleF src = s.Height == 1 ? new System.Drawing.RectangleF(0, 0, BmpWidth, BmpHeight) : new System.Drawing.RectangleF(0, 0, BmpWidth, BmpHeight - 1);
                System.Drawing.RectangleF dst = new System.Drawing.RectangleF(0, (float)(s.Top * sh), (float)w, (float)(s.Height * sh));

                g.DrawImage(bp, dst, src, GraphicsUnit.Pixel);
            }
            return leftMargin;
        }

        public void  Init(float dpiX, float dpiY, GvItemCs items)
        {
            ConvertToView(dpiX, dpiY);
         //   bmpHead = (GvBmpHeadC)items.GetLastItem(Id, GvType.BmpHead);
         //   fl = (float)(ViewLeft * dpiX);
         //   fr = (float)(ViewRight * dpiX);
            fl = Left;
            fr = Right;
        }

        public void OffsetElementY(float offset, float dpiY)
        {
      //      base.YOffsetElement(offset);
       //     infor.ConvertToViewY(dpiY);
        }

        public void  OffsetControl( double scrollX, double scrollY)
        {

        }

    }

    public class GvImageSectionC : GvImageSection
    {

    }

}
