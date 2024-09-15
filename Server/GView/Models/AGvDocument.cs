using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.IO;
using OpenWLS.Server.Base;

//using OpenLS.Display.iTextPDF;

namespace OpenWLS.Server.GView.Models
{
    public class AGvDocument : GvDocument
    {
        public static AGvDocument  NewHeader()
        {
            AGvDocument h = new AGvDocument();
        //    h.OpenGEFile(@"..\..\data\server\sys\misc\header1.agf", true);
            h.Type = "Header";
            return h;
        }
        public static AGvDocument  NewBanner()
        {
            AGvDocument h = new AGvDocument();
  //          h.OpenGEFile(@"..\..\data\server\sys\misc\header1.agf", true);
            h.Type = "Banner";
            return h;
        }

        public string Type { get; set; }
        public GvFont GetFont(string family, float size, ushort style)
        {
            foreach(GvItem e in Items)
            {
                if(e.EType == GvType.Font)
                {
                    GvFont f = (GvFont)e;
                    if (f.Size == size && f.Name == family && f.Style == (GvFontStyle)style)
                        return f;
                }
            }
            return null;
        }

        public GvNumber GetFormat(byte format, GvFont valueFont, GvFont rangeFont)
        {
            foreach (GvItem e in Items)
            {
                if (e.EType == GvType.NumberValueFormat)
                {
                    GvNumber f = (GvNumber)e;
                    if (f.ValueDecimals == format && f.ValueFId == valueFont.Id && f.RangeFId == rangeFont.Id)
                        return f;
                }
            }
            return null;
        }    

        public void  AddTextPDF( string text, float x, float y, float w, uint color, string font, float size, ushort style)
        {
            size = size / 96;
            GvFont f = GetFont(font, size, style);
            if (f == null)
                f = AddNewFont(font, size, style);

            AddText(text, x/96, y/96, w/96, color, f, 0);
        }

        public void AddText(string text, float x, float y, float w, uint color, GvFont f, ushort style)
        {
            GvText t = new GvText()
            {
                FId = f.Id,
                Color = color,
                Left = x,
                Width = w,
                Alignment = (GvTextAlignment)style
            };
            t.WriteText(text,  y);
            AddItem(t);
        }

        GvLine GetLine(float thickness, uint color, byte style)
        {
            foreach (GvItem e in Items)
            {
                if (e.EType == GvType.Line)
                {
                    GvLine f = (GvLine)e;
                    if (f.Thickness == thickness && f.Color == color && f.LineStyle == style)
                        return f;
                }
            }
            return null;
        } 

        public void AddLine(float x1,  float y1,float x2, float y2,  byte thickness, uint color, byte style)
        {
            GvLine g = GetLine(thickness, color, style);
            if (g == null)
            {
                g = new GvLine();
                g.LineStyle = style;
                g.Color = color;
                g.Thickness = thickness;
                AddItem(g);
            }
            g.AddLine(x1, y1, x2, y2);
        }

        GvRect GetGvRect( uint lcolor,uint fcolor,DrawMode mode, byte thickness,byte fstyle,byte lstyle)
        {
            foreach (GvItem e in Items)
            {

                if (e.EType == GvType.Rect)
                {
                    GvRect f = (GvRect)e;
                    if  (f.LineColor == lcolor && f.FillColor == fcolor && 
                        f.DrawMode == mode && f.Thickness == thickness &&
                        f.FillStyle == fstyle && f.LineStyle == lstyle)
                        return f;
                }
            }
            return null;
        }

        public void AddRect(float x, float y, float w, float h, uint lcolor, uint fcolor, DrawMode mode, byte thickness, byte fstyle, byte lstyle)
        {
            GvRect g = GetGvRect( lcolor, fcolor, mode,  thickness, fstyle, lstyle);
            if (g == null)
            {
                g = new GvRect()
                {
                    LineColor = lcolor,
                    FillColor = fcolor,
                    DrawMode = mode,
                    Thickness = thickness,
                    FillStyle = fstyle,
                    LineStyle = lstyle
                };
                AddItem(g);
            }
            g.AddRect(x, y, w, h);
        }

        public void SaveValueFile(string fn)
        {
            string str = Type + "\n";

            foreach (GvItem g in Items)
            {
                if (g is GvParameter)
                {
                    foreach(GvParameterSection s in g.Sections)
                    str = str + s.Para.ToString() + "\n";
                }
            }

            FileStream fs = new FileStream(fn, FileMode.OpenOrCreate);
            fs.SetLength(0);
            byte[] bs = StringConverter.ToByteArray(str);
            fs.Write(bs, 0, bs.Length);
            fs.Close();
        }

        public void LoadValueFile(string fn)
        {
            FileStream fs = new FileStream(fn, FileMode.Open);
            byte[] bs = new byte[fs.Length];
            fs.Read(bs, 0, bs.Length);
            fs.Close();
            string str = StringConverter.ToString(bs);
            string[] strs = str.Split(new char[] { '\n' });
            if(Type != strs[0].Trim())
                return;
            char[] cc = new char[]{':'};
            for (int i = 1; i < strs.Length; i++)
            {
                string[] ss = strs[i].Split(cc);
                if(ss.Length != 4)      //name,description,zone,value
                    continue;
                string name = ss[0].Trim();

                foreach (GvItem g in Items)
                {
                    if (g is GvParameter)
                    {
                        GvParameter p = (GvParameter)g;
                        {
                            foreach (GvParameterSection s in g.Sections)
                            {
                                if (s.Para.Name == ss[0].Trim())
                                    s.Para.Val = new string[] { ss[3].Trim() };
                            }
                        }
                    }
                }
            }
        }

/*
        public GvNumberFormat AddValueFormat()
        {
            GvNumberFormat = 
        } 
 */
    }
}
