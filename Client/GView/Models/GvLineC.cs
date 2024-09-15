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
    public class GvLineC : GvLine, IGvItemC
    {
        int dx;
        float xMin;
        Pen pen;
     //   protected PointF[] ptns1;

        public void DrawItem(Graphics g, float top, float bot)
        {
            if (pen == null)
                return;
            foreach (GvLineSection s in sections)
            {
                if (s.Inside(top, bot))
                {
                    PointF[] ps = s.GetPoints(top);
                    for(int i = 0; i < ps.Length; i = i+2)
                        g.DrawLine(pen, ps[i], ps[i+1]);
                }
            }

        }

        public float DrawScrollbar(Graphics g, float leftMargin, float w, float sh)
        {
            if (SBar == null || pen == null )
                return leftMargin; 
            float x = leftMargin == 0? xMin : leftMargin;
            foreach (GvLineSectionC s in sections)
            {

                PointF[] ps = s.GetPoints(0);
                for (int i = 0; i < ps.Length; i = i + 2)
                {
                    ps[i].X -= x; ps[i].Y *= sh;
                    ps[i+1].X -= x; ps[i+1].Y *= sh;
                    g.DrawLine(pen, ps[i], ps[i + 1]);
                }
            }
            return x;
        }

        public void Init(float dpiX, float dpiY, GvItemCs items)
        {
            ConvertToView(dpiX, dpiY);
            pen = new Pen(MediaColorConverter.ConvertToColor(Color), Thickness);
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

        }

        
    }

    public class GvLineSectionC : GvLineSection
    {

    }
}
