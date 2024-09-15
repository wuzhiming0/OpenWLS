using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using OpenWLS.Server.Base;
using OpenWLS.Server.GView.Models;
using OpenWLS.Client.GView.GUI;
using System.Windows.Media;
using Brush = System.Drawing.Brush;

namespace OpenWLS.Client.GView.Models
{
    public class GvSymbolC : GvSymbol, IGvItemC
    {
        Brush bush;
        string symbolChar;
        protected PointF[] ptns1;

        public float DrawScrollbar(Graphics g, float leftMargin, float w, float sh)
        {
            return leftMargin;
        }
        public void DrawItem(Graphics g, float top, float bot)
        {
            Font f = new Font("Arial", Size, FontStyle.Bold);

            foreach (GvSymbolSection s in sections)
            {
                if (s.Inside(top, bot))
                {
                    PointF[] ps = s.GetPoints(top + (Size >> 1));
                    for (int i = 0; i < ps.Length; i = i + 2)
                        g.DrawString(symbolChar, f, bush, ps[i].X, ps[i].Y );
                }
            }

                
        }

        public void Init(float dpiX, float dpiY, GvItemCs items)
        {
            ConvertToView(dpiX, dpiY);
            bush = new SolidBrush(MediaColorConverter.ConvertToColor(Color));
            symbolChar = "•";
            // Solid = 0, Dash = 1,  Dot = 2,   DashDot = 3,       DashDotDot = 4,  Custom = 5,
            switch (Symbol)
            {
                case  Symbol.Dot:
                    symbolChar = "•";
                    break;
                case  Symbol.Cross:
                     symbolChar = "X";                   
                    break;
                case Symbol.Star:
                    symbolChar = "*";                        
                    break;
                case Symbol.Squar:
                    symbolChar = "□";                        
                    break;
            }
        }


        public void  OffsetControl( double scrollX, double scrollY)
        {

        }
    }

    public class GvSymbolSectionC : GvSymbolSection
    {

    }
}
