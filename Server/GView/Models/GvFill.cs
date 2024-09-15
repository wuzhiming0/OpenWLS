using OpenWLS.Server.Base;
using OpenWLS.Server.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Drawing;


namespace OpenWLS.Server.GView.Models
{
    public class GvFill : GvItem
    {
        public int ImageId { get; set; }
        public int LeftCurve { get; set; }
        public int RightCurve { get; set; }
        public float LeftBorder { get; set; }
        public float RightBorder { get; set; }
        public GvFill()
        {
            EType = GvType.Fill;
        }

        public override byte[]? GetItemExtBytes()
        {
            DataWriter w = new DataWriter(20);
            w.WriteData(ImageId);   
            w.WriteData( LeftCurve );      
            w.WriteData( RightCurve ); 
            w.WriteData( LeftBorder );      
            w.WriteData( RightBorder );        
            return w.GetBuffer();
        }
        protected override void RestoreExt(byte[] bs)
        {
           DataReader r = new DataReader(bs);
           ImageId = r.ReadInt32();
           LeftCurve = r.ReadInt32();
           RightCurve = r.ReadInt32();
           LeftBorder = r.ReadSingle();
           RightBorder = r.ReadSingle();
        }

        public override void OffsetID(int offset)
        {
            base.OffsetID(offset);
            LeftCurve += offset;
            RightCurve += offset;
        }
    }


  }
