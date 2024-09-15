using OpenWLS.Server.Base;
using System.Xml.Linq;

namespace OpenWLS.Server.GView.Models
{
    public class GvBOS : GvItem
    {
        public GvBOS() { EType = GvType.BOS;  }

        protected override void RestoreExt(byte[] bs)
        {      
            DataReader r = new DataReader(bs);
            float w = r.ReadSingle();
            float h = r.ReadSingle();
            Doc.Size = new SizeF(w, h);
         
        }

        public override byte[] GetItemExtBytes(){
            DataWriter w = new DataWriter(8);
            w.WriteData(Doc.Size.Width);
            w.WriteData(Doc.Size.Height);
            return w.GetBuffer();
        }
    }

    public class GvEOS : GvItem //End Of Stream
    {
        public GvEOS() {EType = GvType.EOS; }
    }
    public class GvYOffset : GvItem       //offset 
    {
        public GvYOffset() { EType = GvType.YOffset; }
        public float Offset { get; set; }
    }

    public class GvBOA : GvItem  // Begining of Appendant
    {
        public GvBOA() { EType = GvType.BOA; }
    }
    public class GvEOA : GvItem  // End of Appendant
    {
        public GvEOA(){ EType = GvType.EOA; }
    }
    public class GvBOU : GvItem // Begining of Update
    {
        public GvBOU(){ EType = GvType.BOU; }
    }

    public class GvEOU : GvItem // End of Update
    {
        public GvEOU(){ EType = GvType.EOU; }
    }
}

