using OpenWLS.Server.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
//using System.Drawing;


namespace OpenWLS.Server.GView.Models
{
    public class GvCurve : GvItem
    {
      //  public static int section_size = 1024;
        public uint Color { get; set; }     //4
        public byte Style { get; set; }     //1
        public byte Thickness { get; set; } //1    
        public byte Fill { get; set; }      //1
        public float Spacing { get; set; }  //4    



        protected override void RestoreExt(byte[] bs)
        {
            DataReader r = new DataReader(bs);
            Color = r.ReadUInt32();
            Style = r.ReadByte();
            Thickness = r.ReadByte();
            Fill = r.ReadByte(); 
            r.ReadSByte();
            Spacing = r.ReadSingle(); 
        }
        public override byte[]? GetItemExtBytes()
        {
            DataWriter w = new DataWriter(12);
            w.WriteData(Color);
            w.WriteData(Style);
            w.WriteData(Thickness);
            w.WriteData(Fill);
            w.Seek(1, SeekOrigin.Current);
            w.WriteData(Spacing);
            return w.GetBuffer();
        }


        public void AddPoint(float x, float y, sbyte x_mod)
        {
            if (sectionCur == null)
                sectionCur = this is GvCurveES ? new GvCurveSection(Id, x_mod, x, y) : new GvCurveVsSection(Id, x_mod, x, y);
            else
            {
                ((GvCurveSection)sectionCur).AddPoint(x, y);
                if(sectionCur.Full)
                {
                    Doc.AddSection(this, sectionCur);
                    sectionCur = this is GvCurveES ? new GvCurveSection(Id, x_mod, x, y) : new GvCurveVsSection(Id, x_mod, x, y);
                }
            }
        }
    }

    public class GvCurveES : GvCurve
    {
        public GvCurveES() { EType = GvType.CurveES; }
        public virtual void ConvertToView(float dpiX, float dpiY)
        {
            base.ConvertToView(dpiX, dpiY);
            Spacing = (float)(dpiY * Spacing) ;
        }
    }
    public class GvCurveVS : GvCurve
    {
        public GvCurveVS() { EType = GvType.CurveVS; }

    }
    

    public class GvCurveSection : GvSection
    {
        public static int buf_size_max = 4096;
        public sbyte Xmod { get; set; }     //1
        protected List<float> pnts;
        public override bool Full
        {
            get
            {
                return pnts.Count >= buf_size_max;
            }
        }
        public GvCurveSection()
        {
            pnts = new ();
        }

        public GvCurveSection(int iid, sbyte mods, float x, float y)
        {
            IId = iid;
            Xmod = mods;
            Top = Bot = y;
            pnts = new ();
            pnts.Add(x);

        }

        public virtual void AddPoint(float x, float y)
        {  
            if (y > Bot) Bot = y;
            if (y < Top) Top = y;
            pnts.Add(x);             
        }

        public override void ConvertToView(float dpiX, float dpiY )
        {
            base.ConvertToView (dpiX, dpiY);    
            for(int i = 0; i < pnts.Count; i++ )
                pnts[i] *= dpiX;
        }
        public override void RestoreVal(byte[] bs)
        {
            int c = bs.Length >> 2;
            float[] pnts1 = new float[c];
            Buffer.BlockCopy(bs, 0, pnts1, 0, bs.Length);
            Xmod = (sbyte)pnts1[c - 1];
            pnts = pnts1.ToList();
            pnts.RemoveAt(c - 1);
        }

        public override byte[] GetValBytes()
        {
            pnts.Add(Xmod);
            int s_float = pnts.Count;
            int s_byte = s_float << 2;
            byte[] bs = new byte[s_byte];
            Buffer.BlockCopy(pnts.ToArray(), 0, bs, 0, s_byte);
            pnts.RemoveAt(s_float-1);
            return bs;
        } 

   /*     public override void YOffsetSection(float offset)
        {
            base.YOffsetSection(offset);
            for (int i = 1; i < pnts.Count; i++)
                pnts[i] = pnts[i] + offset;
        }
   */
        public virtual PointF[] GetPoints(float top, float dy)
        {
            float y = Top - top;
            PointF[] ps = new PointF[pnts.Count];
            for (int i = 0; i < pnts.Count; i++)
            {
                ps[i].X = pnts[i];
                ps[i].Y = y;
                y += dy;
            }
            return ps;
        }
        public virtual PointF[]  GetPointsScrollbarPts( float dy, float left,  float x_scale, float y_scale)
        {
            float y = (float)(Top * y_scale);
            float dy1 = (float)(dy * y_scale);
            PointF[] ps = new PointF[pnts.Count];

            for (int i = 0; i < pnts.Count; i++)
            {
                ps[i].X = (pnts[i] - left) * x_scale;
                ps[i].Y = y;
                y += dy1;
            }
            return ps;
        }
    }

    public class GvCurveVsSection : GvCurveSection
    {
        public GvCurveVsSection()
        {

        }
        public GvCurveVsSection(int eid, sbyte mods, float x, float y) : base(eid, mods, x, y)
        {
            pnts.Add(y);
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
        public override void  AddPoint(float x, float y)
        {
            base.AddPoint(x, y);
            pnts.Add(y);
        }

        public override void YOffsetSection(float offset)
        {
            base.YOffsetSection(offset);
            for (int i = 1; i < pnts.Count; i = i + 2)
                pnts[i] = pnts[i] + offset;
        }
        public  PointF[] GetPoints(float top)
        {

            PointF[] ps = new PointF[pnts.Count >> 1];
            for (int i = 0; i < pnts.Count; i = i+2)
            {
                ps[i].X = pnts[i];
                ps[i].Y = pnts[i+1]-top; 
            }
            return ps;
        }
        public  PointF[] GetPointsScrollbarPts( float left, float x_scale, float y_scale)
        {
            PointF[] ps = new PointF[pnts.Count >> 1];
            int k = 0;
            for (int i = 0; i < pnts.Count; i = i + 2)
            {
                ps[k].X = (pnts[i] - left) * x_scale;
                ps[k].Y = pnts[i+1] * y_scale;
                k++;
            }
            return ps;
        }
    }

}

