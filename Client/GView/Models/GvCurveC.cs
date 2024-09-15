using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using OpenWLS.Server.Base;
using OpenWLS.Server.GView.Models;
using System.Runtime.InteropServices;
using OpenWLS.Client.GView.GUI;
using System.Windows.Media.Media3D;

namespace OpenWLS.Client.GView.Models
{
    public class GvCurveEsC : GvCurveES, IGvItemC
    {
        Pen pen;
        float dy;
        GvItemCs items;

        public void Init(float dpiX, float dpiY, GvItemCs _items)
        {
            items = _items;
            ConvertToView(dpiX, dpiY);
            dy = Spacing;
            pen = CreatPen(this);
        }


        public void DrawItem(Graphics g, float top, float bot)
        {
            if (pen == null)
                return;
            foreach (GvCurveSection s in sections)
            {
                if (s.Inside(top, bot))
                {
                    PointF[] ps = s.GetPoints(top, dy);
                    g.DrawLines(pen, ps);
                    if (Fill != 0)
                        items.CurveSections.Add(new GvCurveSectionC(Id, s.Xmod, ps));
                }
            }

        }

        public float DrawScrollbar(Graphics g, float leftMargin, float w, float scale_y)
        {
            if (SBar == null || pen == null)
                return leftMargin;

            foreach (GvCurveSection s in sections)
            {
                PointF[] ps = s.GetPointsScrollbarPts(Spacing, leftMargin, w / Width, scale_y);
                g.DrawLines(pen, ps);
            }
            return leftMargin;
        }

        static public Pen CreatPen(GvCurve c)
        {
            Pen pen = new Pen(MediaColorConverter.ConvertToColor(c.Color), c.Thickness);
            switch (c.Style)
            {
                case (byte)1:
                    pen.DashPattern = new float[] { (float)1.5, (float)1.5 };
                    break;
                case (byte)2:
                    pen.DashPattern = new float[] { 5, 3 };
                    break;
                case (byte)3:
                    pen.DashPattern = new float[] { 10, 3 };
                    break;
            }

            return pen;
        }


        public void OffsetControl(double scrollX, double scrollY)
        {

        }

    }

    public class GvCurveVsC : GvCurveVS, IGvItemC
    {
        GvItemCs items;
        PointF[] ps;
        Pen pen;

        public void Init(float dpiX, float dpiY, GvItemCs _items)
        {
            items = _items;
            ConvertToView(dpiX, dpiY);
            pen = GvCurveEsC.CreatPen(this);
        }
        public PointF[] GetPoints(double top)
        {
            int c = ps.Length;
            PointF[] ps1 = new PointF[c];
            float t = (float)top;
            for (int i = 0; i < c; i++)
            {
                ps1[i].X = ps[i].X;
                ps1[i].Y = ps[i].Y - t;
            }
            return ps1;
        }

        public void DrawItem(Graphics g, float top, float bot)
        {
            if (pen == null)
                return;
            foreach (GvCurveVsSection s in sections)
            {
                if (s.Inside(top, bot))
                {
                    PointF[] ps = s.GetPoints(top);
                    g.DrawLines(pen, ps);
                    if (Fill != 0)
                        items.CurveSections.Add(new GvCurveSectionC(Id, s.Xmod, ps));
                }
            }

        }

        public float DrawScrollbar(Graphics g, float leftMargin, float w, float scale_y)
        {
            if (SBar == null || pen == null)
                return leftMargin;

            foreach (GvCurveSection s in sections)
            {
                PointF[] ps = s.GetPointsScrollbarPts(Spacing, leftMargin, w / Width, scale_y);
                g.DrawLines(pen, ps);
            }
            return leftMargin;
        }



        public void OffsetControl(double scrollX, double scrollY)
        {

        }
    }

    public class GvCurveSectionC
    {
        public int IId { get; set; }
        public sbyte Xmod { get; set; }
        public PointF[] Points { get; set; }
        public GvCurveSectionC(int iid, sbyte xm, PointF[] ps)
        {
            IId = iid; Xmod = xm; Points = ps;
        }

    }
    public class GvCurveSectionCs : List<GvCurveSectionC>
    {
        int section_id;
        int point_id;
        public bool EOP { get { return section_id >= Count; } }
        public GvCurveSectionC CurSection { get {return this[section_id];} }

        public void ResetReadPosition()
        {
            section_id = 0;
            point_id = 0;
        }

        public void MoveNxtSection()
        {
            section_id++;
            point_id = 0;
        }

        public PointF GetNxtPoint()
        {
            if (EOP) return PointF.Empty;
            PointF p = this[section_id].Points[point_id];
            point_id++;
            if(point_id >= this[section_id].Points.Length)
            {
                point_id = 0;
                section_id++;
            }
            return p;
        }

     /*   public PointF[] GetCurSectionPnts()
        {
            return this[section_id].Points;
        }*/
    }


}
