using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OpenWLS.Server.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Drawing;
 

namespace OpenWLS.Server.GView.Models
{
    [Flags]
    public enum GvFontStyle   {
        Bold = 1, Italic = 2,  Underline = 4, Strikeout = 8, ClearBG = 0x8000
    };

    public class GvFont : GvItem
    {                        
        public float  Size {get; set;}         //4                                                       
        public GvFontStyle Style {get; set;}           //6
        public string Name {get; set;} 

        public static GvFont CreateFont(string strFont)
        {
            string[] strs = strFont.Split(new char[]{':'});
            float size = (float)10.0 / 96;
            if (strs.Length > 1)
                size = Convert.ToSingle(strs[1]);
            return new GvFont(strs[0], size);
        }
        public GvFont()
        {
            EType = GvType.Font;
            Size = (float)0.125;
            Name = "Arial";
            Style = 0;
        }
        public GvFont(string name, float size, GvFontStyle style)
        {
            Size = size;
            Name = name;
            EType = GvType.Font;
            Style = style;           
        }

        public GvFont(string name, float size)
        {
            Size = size;
            Name = name;
            EType = GvType.Font;
            Style = 0;
        }

        public override byte[]? GetItemExtBytes()
        {
            byte[] bs = Encoding.UTF8.GetBytes(Name);
            DataWriter w = new DataWriter(bs.Length + 6);
            w.WriteData(Size);
            w.WriteData((ushort)Style);
            w.WriteData(bs);
            return w.GetBuffer();
        }

        protected override void RestoreExt(byte[] bs)
        {
            DataReader r = new DataReader(bs);
            Size = r.ReadSingle();
            Style = (GvFontStyle)r.ReadUInt16();
            byte[] bs1 = r.ReadByteArray(bs.Length-6);
            Name = UTF8Encoding.UTF8.GetString(bs1);
        }

    }
}
