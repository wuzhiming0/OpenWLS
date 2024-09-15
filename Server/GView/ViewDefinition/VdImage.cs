using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.CompilerServices;
using OpenWLS.Server.LogDataFile;
using OpenWLS.Server.GView.Models;
using OpenWLS.Server.Base;
using Index = OpenWLS.Server.LogDataFile.Index;
using OpenWLS.Server.LogDataFile.Models;

namespace OpenWLS.Server.GView.ViewDefinition
{

    public class VdImage : VdItem, IScaleContainer
    {

   //     MVReadByIndex? chReader;
   //     public int Element { get; set; }
        int scaleID;
  
        public uint ColorH { get; set; }
        public uint ColorL { get; set; }
        public ColorMode ColorMode { get; set; }
                
        public double CutoffL { get; set; }
        public double CutoffH { get; set;  } 
                
        public double   XMapStart { get; set; }
        public double   XMapInterval { get; set; }





        public int imageWidth, imageHeight;

        public GvImage image;



        public VdImage()
        {
            Name = "Image";
            Type = LogViewItemType.Image;
            leftPos = new VdxPosition();
            rightPos = new VdxPosition();
            scale = new Scale();
            Measurement = new VdMeasurement();
//            Measurement.MType = PdMType.N;
            emptyValue = double.NaN;
        }



        public override int InitItem()
        {
            scale = doc.Scales.GetScale(scaleID);
            leftPos.InitTrack(doc.Tracks);
            rightPos.InitTrack(doc.Tracks);
            CalcHeadXPosition();
         //   bmpHead.Infor.Bottom = (float)doc.ViewHeight;

            //     bmpHead.realTime = bmpHead;
            return 0;
        }

        public override void Restore(System.Data.DataRow dr)
        {
            base.Restore(dr);
  //          Measurement.Restore(dr);
        //    Measurement.Name = dr["ChName"] == DBNull.Value ? "" : (string)dr["ChName"];      
            //Channel.Name = (string)dr["ChName"];
          //  Element = Convert.ToInt32(dr["Element"]);

            scaleID = Convert.ToInt32(dr["Scale"]);
            leftPos.TrackID = Convert.ToInt32(dr["LeftTrack"]);
            rightPos.TrackID = Convert.ToInt32(dr["RightTrack"]);
            leftPos.Position = Convert.ToDouble(dr["LeftPos"]);
            rightPos.Position = Convert.ToDouble(dr["RightPos"]);

            ColorH = dr["ColorH"] == DBNull.Value ? 0xff000000 : Convert.ToUInt32(dr["ColorH"]);
            ColorL = Convert.ToUInt32(dr["ColorL"]);
            ColorMode = (ColorMode)Convert.ToInt32(dr["ColorM"]); ;

            CutoffL = Convert.ToDouble(dr["CutoffL"]);
            CutoffH = Convert.ToDouble(dr["CutoffH"]);

            XMapStart = Convert.ToDouble(dr["XMapStart"]);
            XMapInterval = Convert.ToDouble(dr["XMapInterval"]);
        }

        public override void Save(System.Data.DataRow dr)
        {
            base.Save(dr);
   //         Measurement.Save(dr);

    //        dr["ChName"] = Measurement.Name;
      //      dr["Element"] = Element;

            dr["Scale"] = scale != null ? scale.ID : 0;
            dr["LeftTrack"] = leftPos.TrackID;
            dr["RightTrack"] = rightPos.TrackID;
            dr["LeftPos"] = leftPos.Position;
            dr["RightPos"] = rightPos.Position;

            dr["ColorH"] = ColorH;
            dr["ColorL"] = ColorL;
            dr["ColorM"] = (int) ColorMode;

            dr["CutoffL"] = CutoffL;
            dr["CutoffH"] = CutoffH;

            dr["XMapStart"] = XMapStart;
            dr["XMapInterval"] = XMapInterval;

        }
        public override void DrawItemHead(GvDocument gvDoc, bool firstSection, float yOffset)
        {
            //  CalcScalePosition();
            if (!ShowHead) return;

            if (ShowName) CreateNameText(gvDoc, yOffset, 0xff000000);

            image = new GvImage()
            {
                BmpWidth = 64,
                BmpHeight = 1,
                Left = HeadRect.Left + (float)0.02,
                Right = HeadRect.Right - (float)0.02
            };
            image.CreateBmpHead((int)ColorMode, ColorL, ColorH);
            gvDoc.AddItem(image);

            byte[] bs = new byte[64];
            for (int i = 0; i < 64; i++)
                bs[i] = (byte)i;

            float y = HeadRect.Y + yOffset;
            image.AddLine(bs, y + HeadRect.Height * 3 / 8);
            image.ClosePixels(y + HeadRect.Height * 5 / 8); //set bot

        }

        void ComputeImageES(double top, double bottom, GvDocument geDoc, float yOffset)
        {
            if (chReader == null)
                return;

            double spacing = 1 / doc.YScale / 96;
            chReader.LoadXdData(top, bottom, spacing );
            double index = chReader.From;
            double y = ((index - top) * doc.YScale) + yOffset;
            double dy = (chReader.Spacing * doc.YScale);
            InitImage(geDoc, (float)y);
            double[] ds;
            byte[] xs = new byte[dim0];
            double dr = 63 / (CutoffH - CutoffL);
            bool emptyValueNotNaN = emptyValue != null;
            while (!chReader.EOR)
            {
                ds = chReader.ReadDoubles(out index);

                if (ds == null || double.IsNaN(ds[0]) || emptyValueNotNaN && (ds[0] == emptyValue)) 
                {
                    for (int i = 0; i < dim0; i++)
                        xs[i] = 255;
                } 
                else
                {
                    for (int i = 0; i < dim0; i++)
                    {
                        double di = ds[i];
                        double x = (di > CutoffH) ? 64 : (di < CutoffL) ? 0 : (di - CutoffL) * dr;
                        xs[i] = (byte)x;
                    }
                }
                y = ((index - top) * doc.YScale) + yOffset;
                image.AddLine(xs, (float)y);

                // y += dy;
                //   index = chReader.Index;
            }
            image.ClosePixels((float)y);

        }

        void ComputeImageVS(double top, double bottom, GvDocument geDoc, float yOffset)
        {
            if (chReader == null)
                return;
            double spacing = 1 / doc.YScale / 96;
            chReader.LoadXdData(top, bottom, spacing);
            double index = chReader.From;

            double y = (index - top) * doc.YScale + yOffset;
            double dy = chReader.Spacing * doc.YScale;
            InitImage(geDoc, (float)y);
            byte[] xs = new byte[dim0];
            double dr = 63 / (CutoffH - CutoffL);
     //       chReader.MoveTo(top);
            while (index < bottom && (!chReader.EOR) )
            {
                double[] ds = chReader.ReadDoubles(out index);
                if (ds != null)
                {
                    for (int i = 0; i < dim0; i++)
                    {
                        double di = ds[i];
                        double x = (di > CutoffH) ? 64 : (di < CutoffL) ? 0 : (di - CutoffL) * dr;
                        xs[i] = (byte)x;
                    }
                }
                else
                {
                    for (int i = 0; i < dim0; i++)
                        xs[i] = 255;
                }
              
                image.AddLine(xs, (float)y);     
                y += dy;
            }
            image.ClosePixels((float)y);

        }

        public override int DrawItem(double top, double bottom, GvDocument gvDoc, bool firstSection, float yOffset)
        {
            if(chReader == null || image == null)
                return -1;

            image.CreateBmpHead((int)ColorMode, ColorL, ColorH);

            //   int s=chReader->GetDataChannel()->dataAxes[0]->GetDimension();  
           imageWidth = GvImage.GetActualWidth(dim0);
           imageHeight = 20480 / imageWidth;

            image.BmpWidth = (ushort)(imageWidth);
            image.BmpHeight = (ushort)(imageHeight);
            gvDoc.AddItem(image);  

            if (chReader.EqualSpacing)
                ComputeImageES(top, bottom, gvDoc, yOffset);
            else
                ComputeImageVS(top, bottom, gvDoc, yOffset);

            return 0;
        }



        public void InitImage(GvDocument gvDoc, float y)
        {
            image = new()
            {
                BmpWidth = (ushort)dim0,
                BmpHeight = (ushort)imageHeight,
                Left = HeadRect.Left + (float)0.02,
                Right = HeadRect.Right - (float)0.02,
                SBar = InScrollbar
            };
            image.CreateBmpHead((int)ColorMode, ColorL, ColorH);
            gvDoc.AddItem(image);
        }


/*
   
        override public int ProcRtData(DataReader r)
        {
            return 0;
        }
     override public int InitItemRt(double index, GvDocument geDoc)
        {
       //     DataChannel dc = ((ArchiveRt)doc.DFiles[0].ar).RtChannels.GetDataChannel(Channel.Name);
       //     dc.CdConsumers.Add(this);
            IndexRt = index;

            return 0;
        }
        */
    }
}
