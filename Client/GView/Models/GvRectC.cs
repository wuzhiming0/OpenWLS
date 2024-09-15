using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using OpenWLS.Server.Base;
using OpenWLS.Server.GView.Models;
using OpenWLS.Client.GView.GUI;

namespace OpenWLS.Client.GView.Models
{
    public class GvRectC : GvRect, IGvItemC
    {
        int dx;
        Pen pen;
        Brush bush;
        Image image;
    //    protected Rectangle[] rects;

        public float DrawScrollbar(Graphics g, float leftMargin, float w, float sh)
        {
            return leftMargin;
        }

        public void DrawItem(Graphics g, float top, float bot)
        {
            int t = (int)top;
            List<System.Drawing.RectangleF> rs = new ();
            foreach(GvRectSection s in sections)
            {
                if (s.Inside(top, bot))
                    rs.Add(s.GetRectF());
            }
            System.Drawing.RectangleF[] rs1 = rs.ToArray(); 

            if ((DrawMode & DrawMode.Line) != 0)
                g.DrawRectangles(pen, rs1);

            if ((DrawMode & DrawMode.Fill) != 0)
            {
                switch(FillStyle)
                {
                    case (byte)FillMode.Color:
                        g.FillRectangles(bush, rs1);
                        break;
                    case (byte)FillMode.Pattern:
                           if( image != null)
                       g.FillRectangles(bush, rs1);
                        break;
                }                    
            }
        }

        public void Init(float dpiX, float dpiY, GvItemCs items)
        {
            ConvertToView(dpiX, dpiY);
            if (FillStyle == (byte)FillMode.Pattern)
            {
                GvFillImageC fImage = (GvFillImageC)items.GetLastItem(ImageId, GvType.FillingImage);
                if (fImage != null)
                    image = fImage.Image;
            }
       //     float fx = (float)dpiX;
       //     float fy = (float)dpiY;
      /*
            int c = ptns.Count >> 2;
            rects = new  Rectangle[c];
            int k = 0;
            for (int i = 0; i < c; i++)
            {
                rects[i].X = (int)(ptns[k++] * fx);
                rects[i].Y = (int)(ptns[k++] * fy);
                rects[i].Width = (int)(ptns[k++] * fx);
                rects[i].Height = (int)(ptns[k++] * fy);
            }
    */
            pen = new Pen(MediaColorConverter.ConvertToColor(LineColor), 1);

            bush = new SolidBrush(MediaColorConverter.ConvertToColor(FillColor));
            if ((FillStyle == (byte)FillMode.Pattern) && image != null)
                bush = new TextureBrush(image); 

            // Solid = 0, Dash = 1,  Dot = 2,   DashDot = 3,       DashDotDot = 4,  Custom = 5,
            switch (LineStyle)
            {
                case (byte)1:
                    pen.DashPattern = new float[] { 10, 3 };
                    break;
                case (byte)2:
                    pen.DashPattern = new float[] { (float)1.5, (float)1.5 };
                    break;
                case (byte)3:
                    pen.DashPattern = new float[] { 5, 3 };
                    break;
            }

        }


        public void  OffsetControl( double scrollX, double scrollY)
        {
          //  Rectangle[] Get
        }
    }

    public class GvRectSectionC : GvRectSection
    {

    }
}
