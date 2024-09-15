using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

using OpenWLS.Server.GView.Models;
using OpenWLS.Server.Base;


namespace OpenWLS.Client.GView.Models
{
    public class GvNumberC : GvNumber,   IGvItemC
    {
        public GvFontC ValueFont { get; set; }
        public GvFontC RangeFont { get; set; }

        //   public int LeftView { get { return vx; } }
        //     public int WidthView { get { return vw; } }
        //       public int WidthView { get { return (int)infor.WidthView; } }

        public Brush GetValueBrush(GvNumberSection s)
        {

            ValueLimitStatus status = s.GetStatus();
            switch (status)
            {
                case ValueLimitStatus.Inside:
                    return Brushes.Green;
                case ValueLimitStatus.Outside:
                    return Brushes.Red;
                default:
                    return Brushes.Black;
            }
        }
 
        public void DrawItem(Graphics g, float top, float bot)
        {
            System.Drawing.Rectangle rect;

            int vh = (int)(((GvFontC)ValueFont).Size * 1.5);
            int rh = (int)(((GvFontC)RangeFont).Size * 1.5);
            int vw = (int)Width;
            int vx = (int)Left;
            foreach (GvNumberSectionC s in sections)
            {
                if (s.LowLimit != s.HighLimit)
                {
                    bool b = false;
                    int t = vh + (int)s.Top;
                    int hw = (vw >> 1) - 2;
                    if (!float.IsNaN(s.LowLimit))
                    {
                        rect = new System.Drawing.Rectangle(vx, t + 1, hw, rh);
                        g.DrawString(s.LowLimit.ToString(), ((GvFontC)ValueFont).Font, Brushes.Gray, rect);
                        b = true;
                    }
                    if (!float.IsNaN(s.HighLimit))
                    {
                        rect = new System.Drawing.Rectangle(vx + hw + 4, t + 1, hw, rh);
                        StringFormat drawFormat = new StringFormat();
                        drawFormat.Alignment = StringAlignment.Far;
                        g.DrawString(s.HighLimit.ToString(), ((GvFontC)ValueFont).Font, Brushes.Gray, rect, drawFormat);
                        b = true;
                    }
                    if (b)
                    {
                        rect = new System.Drawing.Rectangle(vx, t + 1, vw, rh);
                        g.DrawRectangle(Pens.Gray, rect);
                    }

                }


                Brush br = GetValueBrush(s);
                rect = new System.Drawing.Rectangle(vx, (int)s.Top, vw, vh + 1);
                //  g.DrawRectangle(Pens.Gray, rect);
                StringFormat df1 = new StringFormat();
                df1.Alignment = StringAlignment.Center;
                g.DrawString(s.Value.ToString(), ((GvFontC)ValueFont).Font, br, rect, df1);
            }

        }
        public float DrawScrollbar(Graphics g, float leftMargin, float w, float sh)
        {
            return leftMargin;
        }
        public void Init(float dpiX, float dpiY, GvItemCs items)
        {
            ConvertToView(dpiX, dpiY);
       //     vx = (int)(dpiX * X);
        //    vw = (int)(dpiX * Width);
   //         Format = (GvNumberValueFormatC)items.GetLastItem(formatId, GvType.NumberValueFormat);
        }

        public void OffsetElementY(float offset, float dpiY)
        {

        }
        public void  OffsetControl( double scrollX, double scrollY)
        {

        }
    }

    public class GvNumberSectionC : GvNumberSection
    {
        


    }
}
