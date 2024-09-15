using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using OpenWLS.Server.Base;
using OpenWLS.Server.LogDataFile;
using OpenWLS.Server.GView.Models;
using Index = OpenWLS.Server.LogDataFile.Index;
using OpenWLS.Server.LogDataFile.Models;
using OpenWLS.Server.LogInstance.RtDataFile;

namespace OpenWLS.Server.GView.ViewDefinition
{
    public class VdCurve : VdItem, IScaleContainer//, IChDataConsumer
    {

        GvCurve gCurve;
        GvLine headLine;

        public GvCurve GCurve   {    get {  return gCurve;  }  }
        int scaleID;

        public uint Color { get; set; }
        public byte LineThickness { get; set; }
        public byte LineStyle { get; set; }
        public bool LineWrap { get; set; }
        public bool ShowMark { get; set; }

        public float left, right;
        //width;
        // public double spacing;
        public bool Filling { get; set; }

        protected int[] rtGeIDs;
        protected int[] rtMods;
        // List<double> dcValues;

        GvSymbol gMark;
        public VdCurve()
        {
            Name = "Curve";
            Type = LogViewItemType.Curve;
            leftPos = new VdxPosition();
            rightPos = new VdxPosition();
            leftPos.Position = 0;
            leftPos.TrackID = 0;
            rightPos.Position = 100;
            rightPos.TrackID = 0;
            scale = new Base.Scale();
            Filling = false;
            Color = 0xff000000;
            Measurement = new VdMeasurement();

            rtMods = new int[1] { 0 };
        }

        public override int InitItem()
        {
            scale = doc.Scales.GetScale(scaleID);
            leftPos.InitTrack(doc.Tracks);
            rightPos.InitTrack(doc.Tracks);
            CalcHeadXPosition();
            return 0;
        }


        override public void Restore(DataRow dr)
        {
            base.Restore(dr);

            scaleID = Convert.ToInt32(dr["Scale"]);
            leftPos.TrackID = Convert.ToInt32(dr["LeftTrack"]);
            rightPos.TrackID = Convert.ToInt32(dr["RightTrack"]);
            leftPos.Position = Convert.ToDouble(dr["LeftPos"]);
            rightPos.Position = Convert.ToDouble(dr["RightPos"]);

            Color = dr["LineColor"] == DBNull.Value ? 0xff000000 : Convert.ToUInt32(dr["LineColor"]);
            LineThickness = Convert.ToByte(dr["LineThickness"]);
            LineStyle = Convert.ToByte(dr["LineStyle"]);
            LineWrap = Convert.ToByte(dr["LineWrap"]) != 0;
            if (dr.Table.Columns.Contains("ShowMark") && dr["ShowMark"] != DBNull.Value)
                ShowMark = true;
            Filling = false;
        }

        override public void Save(DataRow dr)
        {
            base.Save(dr);

            dr["Scale"] = scale != null ? scale.ID : 0;
            dr["LeftTrack"] = leftPos.TrackID;
            dr["RightTrack"] = rightPos.TrackID;
            dr["LeftPos"] = leftPos.Position;
            dr["RightPos"] = rightPos.Position;

            dr["LineColor"] = Convert.ToUInt32(Color);
            dr["LineThickness"] = LineThickness;
            dr["LineStyle"] = LineStyle;
            dr["LineWrap"] = LineWrap ? 1 : 0;
            if (ShowMark)
                dr["ShowMark"] = 1;
        }

        void CreateScaleText(GvDocument gvDoc, float yOffset)
        {
          //  if(!Show)
            float y = HeadRect.Y + (HeadRect.Height / 2) + yOffset;
            headLeftScaleText = new()
            {
                GId = Id,
                FId = doc.ItemNameFont.Id,
                Alignment = GvTextAlignment.Left,
                Left = (float)HeadRect.X,
                Right = (float)HeadRect.Right,
            };

            headRightScaleText = new GvText()
            {
                FId = doc.ItemNameFont.Id,
                Alignment = GvTextAlignment.Right,
                Left = (float)HeadRect.X,
                Right = (float)HeadRect.Right,
            };
 
            gvDoc.AddItem(headLeftScaleText);
            gvDoc.AddItem(headRightScaleText);
            
            headLeftScaleText.WriteText(scale.From.ToString(), y );
            headRightScaleText.WriteText(scale.To.ToString(), y);
        }

        override public void DrawItemHead(GvDocument gvDoc, bool firstSection, float yOffset)
        {
            if (!ShowHead) return;
            float y = HeadRect.Y + (HeadRect.Height / 2) + yOffset;
            CreateNameText(gvDoc, yOffset, Color);
            CreateScaleText(gvDoc, yOffset);

            headLine = new (Color, LineThickness, LineStyle);
            gvDoc.AddItem(headLine);
            headLine.AddLine(HeadRect.X + (float)0.02, y, HeadRect.Right - (float)0.02, y);
            headLine.FlushSection();
        }



        void CreateGCurveMark(bool es, GvDocument gvDoc)
        {
            gCurve =  new GvCurveES()
            {   GId = Id,
                SBar = InScrollbar,
                Left = (float)scale.PosFrom,
                Right = (float)scale.PosTo,
                Style = LineStyle,
                Thickness = LineThickness,
                Color = Color,
                Fill = Filling ? (byte)1 : (byte)0
            };          
            if (gCurve is GvCurveES)
                ((GvCurveES)gCurve).Spacing = (float)(chReader.Spacing * doc.YScale);
            gvDoc.AddItem(gCurve);

            if (ShowMark)
            {
                gMark = new GvSymbol()
                {
                    GId = Id,
                    Color = Color,
                    Left = (float)scale.PosFrom,
                    Right = (float)scale.PosTo,
                }; 
                gvDoc.AddItem(gMark);

            }
            else
                gMark = null;


        }
        void ComputeCurve(bool es, double top, double bottom, GvDocument gvDoc, float yOffset)
        {
            if (chReader == null)
                return;
            chReader.Load1dData(top, bottom);
            double index = top > chReader.From ? top : chReader.From;
            double x, d = 0;
            float y = (float)((index - top) * doc.YScale) + yOffset;
            sbyte m = 0;
            CreateGCurveMark(es, gvDoc);

//            InitgCurve(m, y);
            bool emptyValueNotNaN = emptyValue == null;
            while (index < bottom && (!chReader.EOR))
            {
                d = chReader.ReadData(out index);
                if ( double.IsNaN(d) || emptyValueNotNaN && (d == emptyValue))
                   gCurve.FlushSection();

                else
                {
                    x = scale.GetDataPosition(d, ref m);
                    if(double.IsNaN(x))
                        gCurve.FlushSection();
                    else
                        AddPoint((sbyte)m, (float)x, (float)y);   
                }
                y = (float)((index - top) * doc.YScale) + yOffset;
              //  index = chReader.Index;
            }
            gCurve.FlushSection();
            if (gMark != null)
                gMark.FlushSection();
        }
        sbyte modPre;
        float preY;
        override public int DrawItem(double top, double bottom, GvDocument gvDoc, bool firstSection, float yOffset)
        {
            if (chReader == null)
                return -1;
            preY = yOffset;            
            modPre = 0;
            CalcScaleXPosition(scaleID);
            ComputeCurve(chReader.EqualSpacing, top, bottom, gvDoc, yOffset);

            return 0;
        }
   /*
        void InitgCurve(  float y)
        {
            gCurve.Style = LineStyle;
            gCurve.Thickness = LineThickness;
            gCurve.Color = Color;            
            if (gCurve is GvCurveES)
                ((GvCurveES)gCurve).Spacing = (float)(chReader.Spacing * doc.YScale);
            gCurve.Fill = Filling ? (byte)1 : (byte)0;
         //   gCurve.InitLineSection((sbyte)mod, y);
        }
   */
        void AddPoint(sbyte m, float x, float y)
        {
            float y1 = (y + preY) / 2;
            if (m > modPre)
            {
                gCurve.AddPoint(right, y1, modPre);
                gCurve.AddPoint(left, y1, m);
            }
            else
            {
                if (m < modPre)
                {
                    gCurve.AddPoint(left, y1, modPre); 
                    gCurve.AddPoint(right, y1, m);
                }    
            }
            gCurve.AddPoint(x, y, m);
            if (gMark != null)
                gMark.AddSymbol(x, y);

            modPre = m;
            preY = y;
        }

 /*
        public override void PLotNewSamples(double top, GvDocument geDoc, float yOffset)
        {
            int start = preDataOffsetRt;
            if (rtValSb == null)
                return;
            int size = rtValSb.SamplesWr - start;
            if (size < 2)
                return;
        //    if(size < 0)
         //       size += rtValSb.Samples;
            preDataOffsetRt += size;
            if ( preDataOffsetRt >= rtValSb.Samples )
                preDataOffsetRt -= rtValSb.Samples;
        //    size++;
            DrawItemRt(top, geDoc, yOffset, start, size);           
        }

        override public int DrawItemRt(double top, GvDocument geDoc, float yOffset)
        {
            int start;
            int size;
            SetRtIndexRange(out start, out size);
            if (size == 0)
                return -1;
            DrawItemRt(top,  geDoc,  yOffset, start, size);
            return 0;
        }
 */
        public override void StartLog()
        {
            gCurve = null;
        }
/*
        void DrawItemRt(double top, GvDocument geDoc, float yOffset, int index_start, int index_size)
        { 
            short m = (short)rtMods[0];
            float x;
            float y = yOffset;
            if (gCurve == null)
            {
                CreateGCureveMark(false, geDoc);
                InitgCurve(m, y);
            }
            else
                gCurve.ClearPtns();
                
            if(gMark != null)
                gMark.ClearPts();

            left = (float)scale.PosFrom;
            right = (float)scale.PosTo;
            int k = index_start;
            int n = rtIndexSb.TotalSamples;
            for (int i = 0; i < index_size; i++)
            {
                double index = rtIndexSb.Buffer1D[k];
                double d = rtValSb.Buffer1D[k];
                y = (float)((index - top) * doc.YScale) + yOffset;
                if (double.IsNaN(d))
                {
                    if (pntCnt > 0)
                    {
                        gCurve.OutputGElement();
                        pntCnt = 0;
                    }
                }
                else
                {
                    x = (float)scale.GetDataPosition(d, ref m);
                    if( !double.IsNaN(x) )
                        AddPoint((sbyte)m, (float)x, (float)y);
                }
                k++;
                if (k >= n)
                    k = 0;
            }
            k--;
            if (k < 0)
                k += n;
            preDataOffsetRt = k;
            gCurve.OutputGElement();
            if(ShowMark)
                gMark.OutputGElement();
            rtMods[0] = m;
        }
*/
        override public void DrawRepeat(double top, double bottom, GvDocument geDoc, VdDFile df)
        {
            rtGeIDs = new int[doc.DFiles.Count-1];
            rtMods = new int[doc.DFiles.Count];
        }

        override public void DrawRepeat(double top, double bottom, VdDFile df)
        {

        }
      
        override public int ProcRtData(DataReader r)
        {
            return 0;
        }

    }


}
