using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using System.Drawing;
using System.Drawing.Drawing2D;
using OpenWLS.Server.Base;
using OpenWLS.Server.GView.Models;

namespace OpenWLS.Client.GView.Models
{
    public class GvFillImageC : GvFillImage, IGvItemC
    {
        Image image;

        public Image Image
        {
            get
            {
                return image;
            }
        }

        public void DrawItem(Graphics g, float top, float bot)
        {

        }

        public float DrawScrollbar(Graphics g, float leftMargin, float w, float sh)
        {
            return leftMargin;
        }

        public void Init(float dpiX, float dpiY, GvItemCs items)
        {
            MemoryStream ms = new MemoryStream(fpBmp);
            image = Bitmap.FromStream(ms);

        }
        public void OffsetElementY(float offset, float dpiY)
        {

        }
        public void OffsetControl(double scrollX, double scrollY)
        {

        }
    }   

    public class GvFillC : GvFill,  IGvItemC
    {
        Image image;
        float fl;
        float fr;
        GvItemCs items;
        Brush brush;


        public void DrawItem(Graphics g, float top, float bot)
        {
            if (LeftCurve >= 0)  {             
                switch(RightCurve){
                    case -1: 
                        DrawCurveToBorder(g); break; //curve-border
                    case -2: 
                        DrawCurveToBorder(g); break; //curve overscale
                    case -3: 
                        DrawCurveToBorder(g); break; //curve underscale
                    default: 
                        DrawCurveToCurve(g); break; //curve-curve
                }                   
            }
            else
            {
                //Border to Curve
                if(RightCurve >= 0)
                    DrawBorderToCurve(g);
            }             
        }
        public float DrawScrollbar(Graphics g, float leftMargin, float w, float sh)
        {
            return leftMargin;
        }
        public void  Init(float dpiX, float dpiY, GvItemCs items)
        {
            this.items = items;
            ConvertToView(dpiX, dpiY);
            GvFillImageC fImage = (GvFillImageC)items.GetLastItem(ImageId, GvType.FillingImage);
            if(fImage != null)
                image = fImage.Image;
            brush = new TextureBrush(image);
            fl = (float)(LeftBorder * dpiX);
            fr = (float)(RightBorder * dpiX);
        }


        public void  OffsetControl( double scrollX, double scrollY)
        {

        }

        void DrawUnderScale(Graphics g)
        {
            //under scale
     /*       if (RightCurve == -3)
            {
                var csa = ge_get_f_csa(f.lc);
                var s = csa.css.length;
                for (var i = 0; i < s; i++)
                {
                    var cs = csa.css[i];
                    if (cs.m < 0)
                    {
                        var xs = cs.Xs; var ys = cs.Ys;
                        ct.moveTo(xs[0], ys[0]);
                        for (var j = 1; j < xs.length; j++)
                            ct.lineTo(xs[j], ys[j]);
                        ct.lineTo(f.rp * Doc.Dpi_x, ys[j - 1]);
                        ct.lineTo(f.rp * gvDoc.dpi_x, ys[0]);
                        ct.lineTo(xs[0], ys[0]);
                    }
                }
            }
     */
        }

        void DrawOverScale(Graphics g)
        {
            //over scale
            GvCurveSectionCs cLeft = items.GetCurveSections(LeftCurve);
            cLeft.ResetReadPosition();
            while(!cLeft.EOP)
            {
                PointF p = cLeft.GetNxtPoint();
          //      if (cLeft.Xmod > 0)
                {

                }
            }
       

        }
 
        void DrawBorderToCurve(Graphics g)
        {
            GvCurveSectionCs cRight = items.GetCurveSections(RightCurve);
            if (cRight == null)
                return;
            cRight.ResetReadPosition();
            PointF[] bs = new PointF[2];
            float x = fl + 1;
            bs[0].X = x;
            bs[1].X = x;
           // 

            while (!cRight.EOP)
            {
                if(cRight.CurSection.Xmod == 0){
                    PointF[] bs1 = cRight.CurSection.Points;
                    if (bs1.Length > 2)
                    {
                        for (int i = 0; i < bs1.Length; i++)
                            if(bs1[i].X > x) bs1[i].X--;
                        bs1[0].Y--;
                        bs1[bs1.Length - 1].Y++;
                        bs[0].Y = bs1[0].Y;
                        bs[1].Y = bs1[bs1.Length-1].Y;
                        ge_draw_fp_c_c(g, bs, bs1);
                    }
                }
                cRight.MoveNxtSection();
            }
        }

        void DrawCurveToBorder(Graphics g)
        {
            GvCurveSectionCs cLeft = items.GetCurveSections(LeftCurve);
            if (cLeft == null )
                return;
            cLeft.ResetReadPosition();
        }

        void DrawCurveToCurve(Graphics g)
        {
           
            List<PointF> lps = new List<PointF>();
            List<PointF> rps = new List<PointF>();
            bool fill = false; 
            bool filPre = false;
            bool intersect = false;
            GvCurveSectionCs cLeft = items.GetCurveSections(LeftCurve);
            GvCurveSectionCs cRight = items.GetCurveSections(RightCurve); 

            if(cLeft == null || cRight == null)
                return;
            cLeft.ResetReadPosition();
            cRight.ResetReadPosition();

            PointF ptl1 = cLeft.GetNxtPoint();
            PointF ptl2 = ptl1;
            PointF ptr1 = cRight.GetNxtPoint();
            PointF ptr2 = ptr1;

            bool b = true; float x = 0; float y = 0;

            do
            {
                while (ptl2.Y < ptr1.Y)
                {      
                    if (cLeft.EOP)
                    { b = false; break; }
                    else
                    {
                        if (fill) { ptl2.X++;  lps.Add(ptl2); ptl2.X--; }
                        ptl1 = ptl2; ptl2 = cLeft.GetNxtPoint();
                    }
                }
                while (ptr2.Y < ptl1.Y)
                {
                    if (cRight.EOP)
                    { b = false; break; }
                    else
                    {
                        if (fill) { ptr2.X--; rps.Add(ptr2); ptr2.X++; }
                        ptr1 = ptr2; ptr2 = cRight.GetNxtPoint();
                    }
                } 
                   
                if ( (ptl1.X < ptr1.X) && (ptl1.X < ptr2.X) && (ptl2.X < ptr1.X) && (ptl2.X < ptr2.X) )     // all left < all right
                { fill = true; intersect = false; }
                else
                {
                    if ( (ptl1.X > ptr1.X) && (ptl1.X > ptr2.X) && (ptl2.X > ptr1.X) && (ptl2.X > ptr2.X) )  // all right < all left 
                    { fill = false; intersect = false; }
                    else
                    {
                        float dxl = ptl2.X - ptl1.X; float dxr = ptr2.X - ptr1.X;
                        if (dxl == 0)                                                                                                        
                        {
                            if (dxr == 0)                                                           // both left and right are straight line; left == right 
                            {
                                x = ptl1.X; y = ptl1.Y;
                                intersect = (ptl1.X == ptr2.X);
                              //  fill = true;
                                fill = false;
                            }
                            else                                                                    //left is straight line
                            {
                                float kr = (ptr2.Y - ptr1.Y) / dxr;
                                x = ptl1.X; y = kr * (x - ptr1.X ) + ptr1.Y;
                                intersect = intersectX(ptl1.X, ptl2.X, ptr1.X, ptr2.X, x) && intersectY(ptl1.Y, ptl2.Y, y);
                                fill = (ptr2.X >= x);
                            }
                        }
                        else
                        {
                            if (dxr == 0)                                                           //right is straight line                
                            {
                                // x = ptr1.X; 
                                x = ptr1.X;  var kl = (ptl2.Y - ptl1.Y) / dxl; 
                                y = kl * (x - ptl1.X) + ptl1.Y;
                                intersect = intersectX(ptl1.X, ptl2.X, ptr1.X, ptr2.X, x) && intersectY(ptr1.Y, ptr2.Y, y);
                               // fill = (ptr2.X <= x);
                               fill = ptr2.X > x;
                            }
                            else
                            {
                                var kl = (ptl2.Y - ptl1.Y) / dxl; var kr = (ptr2.Y - ptr1.Y) / dxr;
                                if (kl == kr)                                                       //right and straight parellel  
                                {
                                    x = ptl1.X; y = ptl1.Y;
                                    //         intersect = (intersectY(ptl1.Y, ptl2.Y, ptr1.Y) || intersectY(ptl1.Y, ptl2.Y, ptr2.Y)) && (ptl1.X == ptr2.X);
                                   // intersect = (intersectY(ptl1.Y, ptl2.Y, ptr1.Y) || intersectY(ptl1.Y, ptl2.Y, ptr2.Y)) ;
                                    //fill = true;
                                }
                                else
                                {
                                    float cl = ptl1.Y - kl * ptl1.X;
                                    float cr = ptr1.Y - kl * ptr1.X;

                                    x = (cr - cl) / (kl - kr); 
                                    y = kr * (x - ptr1.X) + ptr1.Y;
                                    intersect = intersectX(ptl1.X, ptl2.X, ptr1.X, ptr2.X, x);
                                    if(intersect)
                                    {
                                        if (kl > 0) 
                                            fill = (kr > 0) ? kl > kr : false;
                                        else 
                                            fill = (kr > 0) ? true : kl > kr;
                                    }
                                }
                            }
                        }
                    }
                }

                if ( intersect && ( fill || filPre ) )
                    lps.Add(new PointF(x, y));

                if (cRight.EOP)
                    break;

                if (filPre && (!fill))
                    ge_draw_fp_c_c(g, lps, rps); 

                if (ptl2.Y < ptr2.Y)
                {
                    if (cLeft.EOP)
                    { b = false; break; }
                    else
                    {
                        if (fill) 
                        { ptl2.X++; lps.Add(ptl2); ptl2.X--; }
                        ptl1 = ptl2;  ptl2 = cLeft.GetNxtPoint();
                    }
                }
                else
                {
                    if (cRight.EOP)
                    { b = false; break; }
                    else
                    {
                        if (fill) 
                        { ptr2.X--; rps.Add(ptr2); ptr2.X++; }
                        ptr1 = ptr2; ptr2 = cRight.GetNxtPoint(); 
                    }

                }
                filPre = fill;
            } while (b);

      //      if (fill)
                ge_draw_fp_c_c(g, lps, rps);       

        }

        void ge_draw_fp_c_c(  Graphics g, List<PointF> lps, List<PointF> rps)
        {
            if (lps.Count > 1 || rps.Count > 0)
            {
                int s = lps.Count + rps.Count;

                PointF[] ps = new PointF[s];
                int k = 0;
                for (int i = 0; i < lps.Count; i++)
                    ps[k++] = lps[i];
                for (int i = rps.Count - 1; i >= 0; i--)
                    ps[k++] = rps[i];
                if (ps.Length == 3)
                {

                }
                //     ps[k++] = lps[0];
                GraphicsPath ct = new GraphicsPath();
                //         ct.AddClosedCurve(ps);
                ct.AddLines(ps);
                g.FillPath(brush, ct);
            }
            lps.Clear(); rps.Clear();
        }

        void ge_draw_fp_c_c(Graphics g, PointF[] lps, PointF[] rps)
        {
            int s = lps.Length + rps.Length;
            if (s <= 2)
                return;
            PointF[] ps = new PointF[s];
            int k = 0;
            for (int i = 0; i < lps.Length; i++)
                ps[k++] = lps[i];
            for (int i = rps.Length - 1; i >= 0; i--)
                ps[k++] = rps[i];
       //     ps[k++] = lps[0];
            GraphicsPath ct = new GraphicsPath();
            ct.AddLines(ps);
            g.FillPath(brush, ct);
        }

        bool intersectX(float x1, float x2, float x3, float x4, float xx)
        {
            if ((x1 > xx) && (x2 > xx)) return false;
            if ((x1 < xx) && (x2 < xx)) return false;
            if ((x3 > xx) && (x4 > xx)) return false;
            if ((x3 < xx) && (x4 < xx)) return false;
            return true;
        }
        bool intersectY(float y1, float y2, float yy)
        {
            if ((y1 > yy) && (y2 > yy)) return false;
            if ((y1 < yy) && (y2 < yy)) return false;
            return true;
        }

    }

}
