using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using OpenWLS.Server.GView.Models;

namespace OpenWLS.Server.GView.ViewDefinition
{
    public class VdFill : VdItem
    {
        static public string[] PatternNames { get; set; }

 //       GeFill fp;
 //       GvText headText;
        public uint ColorB { get; set; }
        public uint ColorF { get; set; }
        public string FPName { get; set; }
        public string LeftCurve { get; set; }
        public string RightCurve { get; set; }

        GvFillImage fp;


        public VdFill()
        {
            Type = LogViewItemType.Fill;

        }

        public override int InitItem()
        {
            SetHeadXPosition();
            return 0;
        }
       /* */
        override public void Restore(DataRow dr)
        {
            base.Restore(dr);

            FPName = (string)dr["FPName"];
            ColorF = dr["ColorF"] == DBNull.Value ? 0xff000000 : Convert.ToUInt32(dr["ColorF"]);
            ColorB = Convert.ToUInt32(dr["ColorB"]) ;

            LeftCurve = (string)(dr["LeftCurve"]);
            RightCurve =  (string)(dr["RightCurve"]);   
        }

        override public void Save(DataRow dr)
        {
            base.Save(dr);

            dr["FPName"] = FPName;

            dr["ColorB"] = ColorB;
            dr["ColorF"] = ColorF;

            dr["LeftCurve"] = LeftCurve;
            dr["RightCurve"] = RightCurve;
        }

        void SetHeadXPosition()
        {
            double left = 0;
            double right = 0;
            VdCurve cLeft = (VdCurve)doc.Items.GetItem(LogViewItemType.Curve, LeftCurve);
            VdCurve cRight = (VdCurve)doc.Items.GetItem(LogViewItemType.Curve, RightCurve);
            if (cLeft != null)
                left = cLeft.LeftPos.Track.GetX(0);
            else
            {
                if (cRight != null)
                    left = cRight.LeftPos.Track.GetX(0);
            }

            if (cLeft != null)
                cLeft.Filling = true;
            if (cRight != null)
                cRight.Filling = true;

            if (cRight != null)
                right = cRight.RightPos.Track.GetX(100);
            else
            {
                if (cLeft != null)
                    right = cLeft.RightPos.Track.GetX(100);
            }

            HeadRect.X = (float)left;
            HeadRect.Width = (float)(right - left);
        }

        void CreateFillingPattern(GvDocument gvDoc)
        {
            fp = new GvFillImage();
            fp.CreateFillImage(FPName, ColorF, ColorB);
            gvDoc.AddItem( fp);
        }

        public override void DrawItemHead(GvDocument gvDoc, bool firstSection, float yOffset)
        {
            if (!ShowHead) return;
            float y1 = HeadRect.Y + yOffset;
            float y = y1 + (HeadRect.Height / 4);
            if (firstSection)
                CreateFillingPattern(gvDoc);
            float d = HeadRect.Height / 4;
            
            GvRect r = new GvRect()
            {
                FillStyle = (byte)FillMode.Pattern,
                DrawMode = DrawMode.Fill,
                ImageId = fp.Id
            };
            gvDoc.AddItem(r);
            r.AddRect(HeadRect.X, y1, HeadRect.Width, HeadRect.Height);

            /*
            r = new GvRect()
            {
                FillStyle = 0,
                DrawMode = DrawMode.Fill,
                FillColor = 0xffffffff
                //ImageId = fp.Id
            };
            gvDoc.AddItem(r);
            r.AddRect(HeadRect.X, y , HeadRect.Width, HeadRect.Height - d - d);
            */
            CreateNameText(gvDoc, yOffset + d, ColorF);
        }

        public override int DrawItem(double top, double bottom, GvDocument gvDoc, bool firstSection, float yOffset)
        {
            if (firstSection)
            {
                CreateFillingPattern(gvDoc);
            }


            GvFill f = new GvFill()
            {
                ImageId = fp.Id,
                GId = Id
            };

            VdCurve lc = (VdCurve)doc.Items.FindItem(LeftCurve, LogViewItemType.Curve);
            if (lc == null)
            {
                f.LeftCurve = -1;
            }
            else
            {
                if (lc.GCurve != null)
                {
                    f.LeftCurve = lc.GCurve.Id;
                    f.LeftBorder = (float)lc.LeftPos.Track.GetX(0);
                }
                else
                    return -2;
            }


            VdCurve rc = (VdCurve)doc.Items.FindItem(RightCurve, LogViewItemType.Curve);
            if (rc == null)
            {
                f.RightCurve = -1;
                if(RightCurve == "OverScale")
                    f.RightCurve = -2;
                if (RightCurve == "UnderScale")
                    f.RightCurve = -3;
                f.RightBorder = (float)lc.LeftPos.Track.GetX(100);
            }
            else
            {
                if (rc.GCurve != null)
                {
                    f.RightCurve = rc.GCurve.Id;
                    f.RightBorder = (float)rc.LeftPos.Track.GetX(100);
                    if (lc == null)
                        f.LeftBorder = (float)rc.LeftPos.Track.GetX(0);
                }
                else
                    return -3;
            }
            gvDoc.AddItem(f);
            return 0;
        }


        /*

        public  void InitFillingPattern(GvDocument geDoc)
        {

            fp.DocGe = geDoc; //fp.docWs = doc;
            fp.CreateFillingPattern(FPName);
  //          fp.RealTime = doc.RealTime;
            if (doc.RealTime)
                fp.OutputGElement();
        }  
        */
    }
}
