using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Drawing;
using OpenWLS.Server.Base;
using OpenWLS.Server.GView.Models;

namespace OpenWLS.Client.GView.Models
{
    public class GvFontC : GvFont,  IGvItemC
    {
        public float SizeDpi{get; set;}
        public Font Font{get; set;}

        public void DrawItem(Graphics g, float top, float bot)
        {

        }
        public float DrawScrollbar(Graphics g, float leftMargin, float w, float sh)
        {
            return leftMargin;
        }
        public void Init(float dpiX, float dpiY, GvItemCs items)
        {
            SizeDpi = (float)(dpiX * Size);
            Font = new Font(Name, SizeDpi, (FontStyle)((ushort)Style & 0xff) );
        }

        public void OffsetElementY(float offset, float dpiY)
        {

        }
        public void  OffsetControl( double scrollX, double scrollY)
        {

        }

    }
}
