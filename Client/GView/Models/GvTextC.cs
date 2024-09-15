using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using OpenWLS.Server.Base;
using OpenWLS.Server.GView.Models;
using System.Drawing.Drawing2D;
using OpenWLS.Client.GView.GUI;

namespace OpenWLS.Client.GView.Models
{
    public class GvTextC : GvText, IGvItemC
    {
        public GvFontC Font { get; set; }
       // int vx, vw;
        public float DrawScrollbar(Graphics g, float leftMargin, float w, float scale_y)
        {
            if ((SBar == null) || Font == null)
                return leftMargin;
            float x = leftMargin == 0 ? Left : leftMargin;
            Brush b = new SolidBrush(MediaColorConverter.ConvertToColor(Color));
            foreach (GvTextSectionC s in sections)
            {
                float vy = s.Top * scale_y;
                float vw = Width;
                float vx = Left - x;
                System.Drawing.SizeF size = g.MeasureString(s.Text, ((GvFontC)Font).Font);
                System.Drawing.RectangleF r = new System.Drawing.RectangleF(vx, vy, vw, s.Height * scale_y);

                DrawText(g, s.Text, b, vx, vy, vw, size, r);
            }
            return x;
        }
        public void DrawItem(Graphics g, float top, float bot)
        {             
            Brush b = new SolidBrush(MediaColorConverter.ConvertToColor(Color));
            float vw = Width;
            float vx = Left;
            foreach (GvTextSectionC s in sections)
            {
                if (s.Inside(top, bot))
                {
                    float vy = s.Top - top;
                    if (Font == null)
                        return;

                    System.Drawing.SizeF size = g.MeasureString(s.Text, ((GvFontC)Font).Font);
                    System.Drawing.RectangleF r = new System.Drawing.RectangleF(vx, vy, vw, s.Height);

                    DrawText(g, s.Text, b, vx, vy, vw, size, r);
                }
            }
        }

        void DrawText(Graphics g, string text, Brush b, float vx, float vy, float vw, System.Drawing.SizeF s, System.Drawing.RectangleF r )
        {
            
            /* GraphicsState g_state = null;
          if (Rotation != 0)
           {
               g_state = g.Save();
               g.ResetTransform();
               g.RotateTransform(Rotation);
           }*/
            StringFormat drawFormat = new StringFormat();
            if ((GvFontStyle.ClearBG & Font.Style) != 0)        // clear background
            {

                switch (Alignment)
                {
                    case GvTextAlignment.Left:
                        //   if (vw != 0)
                        g.FillRectangle(Brushes.White, vx, vy, s.Width, s.Height);
                        break;
                    case GvTextAlignment.Center:
                        g.FillRectangle(Brushes.White, vx + (vw - s.Width) / 2, vy, s.Width, s.Height);
                        break;
                    case GvTextAlignment.Right:
                        g.FillRectangle(Brushes.White, vx + vw - s.Width, vy, s.Width, s.Height);
                        break;
                }
            }

            switch (Alignment)
            {
                case GvTextAlignment.Left:
                    if (vw == 0)
                        g.DrawString(text, ((GvFontC)Font).Font, b, vx, vy);
                    else
                    {
                        drawFormat.Alignment = StringAlignment.Near;
                        g.DrawString(text, ((GvFontC)Font).Font, b, r, drawFormat);
                    }
                    break;
                case GvTextAlignment.Center:
                    drawFormat.Alignment = StringAlignment.Center;
                    if (Rotation == 90)
                        drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;
                    g.DrawString(text, ((GvFontC)Font).Font, b, r, drawFormat);
                    break;
                case GvTextAlignment.Right:
                    drawFormat.Alignment = StringAlignment.Far;
                    g.DrawString(text, ((GvFontC)Font).Font, b, r, drawFormat);
                    break;
            }
            //   if(g_state != null)
            //       g.Restore(g_state);
        }

        public void Init(float dpiX, float dpiY, GvItemCs items)
        {
            ConvertToView(dpiX, dpiY);
            Font = (GvFontC)items.GetLastItem(FId, GvType.Font);
        }


        public void  OffsetControl( double scrollX, double scrollY)
        {

        }
    }

    public class GvTextSectionC : GvTextSection
    {
        public GvTextSectionC()
        {
        }

    }
}
