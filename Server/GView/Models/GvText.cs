using OpenWLS.Server.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OpenWLS.Server.GView.Models
{
    public enum GvTextAlignment{Left = 0, Center = 1, Right = 2};
   // [Flags]
   // public enum GvTextFlags { ClearBG = 1 };

    public class GvText : GvItem
    {
        public int FId { get; set; }            // Front Id
        public uint Color { get; set; }
        public float Rotation { get; set; }
        //       public string Text { get; set; }
        //      protected int fontEID;
        public GvTextAlignment Alignment { get; set; }
    //    public GvTextFlags Flags { get; set; }
        public GvText()
        {
            EType = GvType.Text;
            Color = 0xff000000;
            Alignment = GvTextAlignment.Left;
            Rotation = 0;
    //        Flags = 0;
        }
        public GvText(GvFont font, uint color)
        {
            EType = GvType.Text;
            FId = font.Id;
            Color = color;
            Alignment = GvTextAlignment.Left;
            Rotation = 0;
       //     Flags = 0;
        }

        public GvText(GvFont font, uint color, GvTextAlignment align, float x, float w)
        {
            EType = GvType.Text;
            FId = font.Id;
            Color = color;
            Alignment = align;
            Rotation = 0;

            Left = x;
            Right = x + w;
        }

        public override byte[]? GetItemExtBytes()
        {
            DataWriter w = new DataWriter(16);
            w.WriteData(FId);
            w.WriteData(Color);
            w.WriteData(Rotation);
            w.WriteData((byte)Alignment);
    //        w.WriteData((byte)Flags);
            //      w.WriteData(StringConverter.ToByteArray(Text));
            return w.GetBuffer();
        }

        protected override void RestoreExt(byte[] bs)
        {
            DataReader r = new DataReader(bs);
            FId = r.ReadInt32();
            Color = r.ReadUInt32();
            Rotation = r.ReadSingle();
            Alignment = (GvTextAlignment)r.ReadByte();
         //   Flags = (GvTextFlags)r.ReadByte();
            //    byte[] bs1 = r.ReadByteArray(bs.Length - 13);
            //      Text = StringConverter.ToString(bs1);
        }


        public override void OffsetID(int offset)
        {
            base.OffsetID(offset);
            FId += offset;
        }


        public void WriteText(string t, float y)
        {
            GvTextSection ts = new GvTextSection(t, y) { IId = Id };
            Doc.AddSection(this, ts);

        }
        public void WriteText(string t, float top, float bot)
        {
            GvTextSection ts = new GvTextSection(t, top, bot) { IId = Id };
            Doc.AddSection(this, ts);
        }
    }


 
    public class GvTextSection : GvSection
    {
        public string Text{ get; set; }

        public override byte[] GetValBytes()
        {
            return Encoding.UTF8.GetBytes(Text); 
        } 
        public override void RestoreVal(byte[] bs) {   
            Text = UTF8Encoding.UTF8.GetString(bs);
        } 
    
        public GvTextSection()
        {

        }
        public GvTextSection(string t,  float y)
        {
            Text = t;
            Top = Bot = y;
        }
        public GvTextSection(string t, float top, float bot)
        {
            Text = t;
            Top =  top;
            Bot = bot;
        }
    }
}
