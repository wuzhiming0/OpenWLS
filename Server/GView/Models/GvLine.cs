using OpenWLS.Server.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
//using System.Drawing;


namespace OpenWLS.Server.GView.Models
{
    public class GvLine : GvItem
    {
   //     public static int section_size = 1024;
        public uint Color{ get; set;}        //4
        public byte LineType { get; set; }   // 1 will support arrow   
        public byte LineStyle { get; set; }   //1
        public byte Thickness { get; set; }  //1
 //       protected GvLineSection sectionCur;
        public override byte[]? GetItemExtBytes()
        {
            DataWriter w = new DataWriter(8);
            w.WriteData( Color );       
            w.WriteData( LineType );             
            w.WriteData( LineStyle );       
            w.WriteData( Thickness );   
            return w.GetBuffer();
        }

        protected override void RestoreExt(byte[] bs)
        {
            DataReader r = new DataReader( bs );
            Color = r.ReadUInt32();
            LineType = r.ReadByte();
            LineStyle = r.ReadByte();
            Thickness = r.ReadByte();
        }
        public GvLine()
        {
            EType = GvType.Line;
        }
        public GvLine(uint color, byte thikness, byte style){
            EType = GvType.Line;
            Color = color; 
            LineStyle = style;
            Thickness = thikness;             
        }


        public void AddLine(float x1, float y1, float x2, float y2)
        {
            if (sectionCur == null)
                sectionCur = new GvLineSection(Id, x1, y1, x2, y2); 
            else
            {
                ((GvLineSection)sectionCur).AddLine(x1, y1, x2, y2);
                if (sectionCur.Full)
                {
                    Doc.AddSection(this, sectionCur);
                    sectionCur = null;
                }
            }
        } 
    }
   public class GvLineSection : GvSection {
        public static int buf_size_max = 4096;
        protected List<float> pnts;
        public override bool Full
        {
            get
            {
                return pnts.Count >= buf_size_max;
            }
        }

        public GvLineSection()
        {
            Bot = float.NegativeInfinity;
            Top = float.PositiveInfinity;
            pnts = new List<float>();
        }
        public GvLineSection(int iid)
        {
            IId = iid;
            Bot = float.NegativeInfinity;
            Top = float.PositiveInfinity;
            pnts = new List<float>() ;
        }

        public GvLineSection(int iid, float x1, float y1, float x2, float y2)
        {
            IId = iid;
            Bot = Math.Max(y1, y2);
            Top = Math.Min(y1, y2);
            pnts = new List<float>() { x1, y1, x2, y2 };
        }
        //     public override bool SectionFull { get{ return wr_pos >= wr_size;} }

        public override void ConvertToView(float dpiX, float dpiY)
        {
            base.ConvertToView(dpiX, dpiY);
            for (int i = 0; i < pnts.Count; i = i + 2)
            {
                pnts[i] *= dpiX;
                pnts[i + 1] *= dpiY;
            }
        }
        public void AddLine(float x1, float y1, float x2, float y2)
        {
            pnts.Add(x1); pnts.Add(y1); pnts.Add(x2); pnts.Add(y2);

            if (Bot < y1) Bot = y1;
            if (Bot < y2) Bot = y2;
            if (Top > y1) Top = y1;
            if (Top > y2) Top = y2;

        //    if (Right < x1) Right = x1;
        //    if (Right < x2) Right = x2;
        //    if (Left > x1) Left = x1;
        //    if (Left > x2) Left = x2;
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
        public PointF[] GetPointsScrollbarPts(float left, float x_scale, float y_scale)
        {
            PointF[] ps = new PointF[pnts.Count >> 1];
            int k = 0;
            for (int i = 0; i < ps.Length; i++)
            {
                ps[i].X = (pnts[k++] - left) * x_scale;
                ps[i].Y = pnts[k++] * y_scale;
            }
            return ps;
        }
        public override void RestoreVal(byte[] bs)
        {
            int c = bs.Length >> 2;
            float[] pnts1 = new float[c];
            Buffer.BlockCopy(bs, 0, pnts1, 0, bs.Length);
            pnts = pnts1.ToList();
        }

        public override byte[] GetValBytes()
        {
            int s_float = pnts.Count;
            int s_byte = s_float << 2;
            byte[] bs = new byte[s_byte];
            Buffer.BlockCopy(pnts.ToArray(), 0, bs, 0, s_byte);
            return bs;
        }

    }
}
