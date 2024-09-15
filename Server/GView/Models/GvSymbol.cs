using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using System.Drawing;
using OpenWLS.Server.Base;

namespace OpenWLS.Server.GView.Models
{
     [Flags]
    public enum Symbol { Dot = 0, Cross = 1, Star = 2, Squar = 3 }
    public class GvSymbol : GvItem
    {
        public uint Color  {get; set;}     //4  line color
        protected Symbol Symbol{get; set;}         //5
        protected byte Size{get; set;}                  //6
       
        public GvSymbol()
        {
            EType = GvType.Symbol;
            Color = 0;
            Symbol = Symbol.Dot;
            Size = 12;
        }

        protected override void RestoreExt(byte[] bs)
        {
            DataReader r = new DataReader(bs);
            Color = r.ReadUInt32();       //4
            Symbol = (Symbol)r.ReadByte();         //5
            Size = r.ReadByte();     //6  
        }

        public override byte[]? GetItemExtBytes()
        {
            DataWriter w = new DataWriter(12);
            w.WriteData(Color);
            w.WriteData((byte)Symbol);
            w.WriteData(Size);
            return w.GetBuffer();
        }
        public void AddSymbol(float x, float y)
        {
            if(sectionCur == null) 
                sectionCur = new GvSymbolSection(Id);
            else
            {
                ((GvSymbolSection)sectionCur).AddSymbol(x, y);
                if (sectionCur.Full)
                {
                    Doc.AddSection(this, sectionCur);
                    sectionCur = null;
                }
            }
        }  
    }

    public class GvSymbolSection : GvSection
    {
        public static int buf_size_max = 4096;
        protected List<float> pnts;
        public override bool Full
        {
            get
            {
                return pnts.Count >= buf_size_max;
            }
        }
        public GvSymbolSection(int iid) { IId = iid;  pnts = new List<float>();  }
        public GvSymbolSection() { pnts = new List<float>();  }
        public void AddSymbol(float x, float y)
        {
            if (pnts.Count == 0)
                Top = Bot = y;

            pnts.Add(x); pnts.Add(y);

            if (Bot < y) Bot = y;
            if (Top > y) Top = y;
        }

        public override void ConvertToView(float dpiX, float dpiY)
        {
            base.ConvertToView(dpiX, dpiY);
            for (int i = 0; i < pnts.Count; i = i + 2)
            {
                pnts[i] *= dpiX;
                pnts[i + 1] *= dpiY;
            }
        }

        public override void YOffsetSection(float offset)
        {
            base.YOffsetSection(offset);
            for (int i = 1; i < pnts.Count; i = i + 2)
                pnts[i] = pnts[i] + offset;
        }
        public PointF[] GetPoints(float top)
        {

            PointF[] ps = new PointF[pnts.Count >> 1];
            int k = 0;
            for (int i = 0; i < ps.Length; i++)
            {
                ps[i].X = pnts[k++];
                ps[i].Y = pnts[k++] - top;
            }
            return ps;
        }
        public override void RestoreVal(byte[] bs)
        {
            int c = bs.Length >> 2;
            float[] ptns1 = new float[c];
            Buffer.BlockCopy(bs, 0, ptns1, 0, bs.Length);
            pnts = ptns1.ToList();
        }
        public override byte[] GetValBytes()
        {
            int c = pnts.Count << 2;
            byte[] bs = new byte[c];
            Buffer.BlockCopy(pnts.ToArray(), 0, bs, 0, c);
            return bs;
        }


    }
}
