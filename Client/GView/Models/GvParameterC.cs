using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;

using OpenWLS.Server.GView.Models;
using OpenWLS.Server.Base;

using System.Drawing;

namespace OpenWLS.Client.GView.Models
{
  
    public class GvParameterCs : List<GvParameterC>
    {
        public string GetParaNameValString()
        {
            if (Count == 0)
                return "";

            string str = this[0].ToString();
            for(int i = 1; i < Count; i++)
                str = str + "|" + this[i].ToString();

            return str;

        }
    }

    public class GvParameterC : GvParameter,  IGvItemC
    {
        public GvFontC Font { get; set; }



        public static GvParameterC FromGEVNumberValue(GvNumberC val, GvItemCs items)
        {
            GvParameterC p = new GvParameterC()
            {
                Left = val.Left,
                Font = val.ValueFont,
                Width = val.Width,
               // Name = val.Name
            };
            foreach(GvNumberSectionC s in val.Sections)
            {
            /*    
                 p.AddParameter(s.Top);
            p.Infor.Top = val.Infor.Top;
            p.Infor.Bottom = (p.Infor.Top + val.Infor.Bottom)/2;

            p.Para.Value = new Base.Archive.ArParaValue(iLogDataType.Single, val.Value);
            */
            }


            return p;
        }
        public void Init(float dpiX, float dpiY, GvItemCs items)
        {

            Font = (GvFontC)items.GetLastItem(FId, GvType.Font);
            ConvertToView(dpiX, dpiY);


        }

     
        public void OffsetElementY(float offset, float dpiY)
        {

        }

        public void OffsetControl(double sx, double sy)
        {
            foreach (GvParameterSectionC s in sections)
                s.OffsetControl(sx, sy);
        }

        public void DrawItem(Graphics g, float top, float bot)
        {

        }

        public float DrawScrollbar(Graphics g, float leftMargin, float w, float sh)
        {
            return leftMargin;
        }



    }

    public class GvParameterSectionC : GvParameterSection
    {
        System.Windows.Forms.Control cntl;
        Point location;
        bool selected;
        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;
                if (cntl != null)
                    cntl.BackColor = selected ? System.Drawing.Color.FromName("LightBlue") : System.Drawing.Color.FromName("WhiteSmoke");
            }
        }

        public bool Enable
        {
            get
            {
                if (cntl == null)
                    return false;
                return cntl.Enabled;
            }
            set
            {
                if (cntl != null)
                    cntl.Enabled = value;
            }
        }

        public System.Windows.Forms.Control Cntl
        {
            get { return cntl; }
        }



        public void OffsetControl(double sx, double sy)
        {
            if (cntl != null)
            {
                Point loc = location;
                loc.Offset(new Point((int)sx, (int)sy));
                cntl.Location = loc;
            }
        }

        /*
        public override string ToString()
        {
            if (cntl == null)
                return Para.ToShortString();
            else
                return Para.Name + ":" + cntl.Text;                
        }
        public void Init()
        {
            selected = false;
            location = new Point((int)(Left), (int)(Top));
            CreateCntl(items);
        }
        */

        void CreateCntl(GvItemCs items)
        {
            /*
            switch (Para.Value.DataType)
            {
                case iLogDataType.Enum:
                    cntl = new ComboBox();
                    break;
                case iLogDataType.Bool:
                    cntl = new CheckBox();
                    break;
                default:
                    TextBox tb = new TextBox();
                    //    tb.AcceptsReturn = true;
                    tb.Multiline = true;
                    tb.Text = Para.Value.ToString();
                    tb.BorderStyle = BorderStyle.None;
                    tb.BackColor = System.Drawing.Color.FromName("WhiteSmoke");
                    cntl = tb;
                    break;
            }
            if (cntl != null)
            {
                cntl.Location = new Point((int)infor.LeftView, (int)infor.TopView);
                cntl.Height = (int)infor.HeightView;
                cntl.Width = (int)infor.WidthView;
                cntl.Tag = this;
            }
            */
        }
    }

}
