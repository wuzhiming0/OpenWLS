using OpenWLS.Server.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OpenWLS.Server.GView.Models
{
    public class GvParameter : GvItem
    {
    //    public Parameter Para{get; set;}
    //    public float Left { get; set; }
    //    public float Right { get; set; }
        public int FId { get; set; }
        public uint Color { get; set; }
        public GvTextAlignment Alignment {  get; set; }



        public GvParameter()
        {
            EType = GvType.Parameter;
         //   Para = new Parameter();
        }

        protected override void RestoreExt(byte[] bs)
        {
            DataReader r = new DataReader(bs);
            FId = r.ReadInt32();
            Color = r.ReadUInt32();
            Alignment = (GvTextAlignment)r.ReadByte();
            r.Seek(3, SeekOrigin.Current);
        }
        public override byte[]? GetItemExtBytes()
        {
            DataWriter w = new DataWriter(12);
            w.WriteData(FId);
            w.WriteData(Color);
            w.WriteData(Alignment);
            return w.GetBuffer();
        }


        public void AddParameter(Parameter p, float x, float y)
        {

        }

            public GvText ToGvText()
        {
            GvText t = new GvText()
            {
                Color = this.Color,
                //       t.Color = 0xff000000;
                Id = this.Id,
                FId = this.FId,
                Left = this.Left,
            };
          //  foreach(GvTextSection s in )
          //  t.WriteText(Para.Val.ToString(), Top);
            return t;
        }

        public override void OffsetID(int offset)
        {
            base.OffsetID(offset);
                FId += offset;
        }
        /*
        public override uint GetBodySize()
        {
            if (Para == null)
                return 18;
            return (uint)( 16 + Para.Getsize() ) ;
        }

        public GvText PrintGvText()
        {
            return new GvText(Para.Val.ToString(), Left, infor.Top, Font, Color);
        }
        
        public override void WriteElementBody(DataWriter w)
        {
        //   w.WriteData(Left);
        //   w.WriteData(Right);
           w.WriteData(Color);
           if (Font == null)
               w.WriteData(fontEID);
           else
               w.WriteData(Font.Infor.EID);
           Para.Save(w);
        }

        public override void RestoreElementBody(DataReader r)
        {
       //     Left=r.ReadSingle();
       //     Right=r.ReadSingle();
            Color=r.ReadUInt32();
            fontEID = r.ReadInt32();
            Para.Restore(r);
        }
*/
   
    
    
    
    }

    public class GvParameterSection : GvSection
    {
        public Parameter Para { get; set; }
    }
}
