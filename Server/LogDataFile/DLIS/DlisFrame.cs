using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWLS.Server.Base;



using System.IO;
using OpenWLS.Server.LogDataFile.Models;
using OpenWLS.Server.LogDataFile.DLIS.V1;
using OpenWLS.Server.LogDataFile.DLIS.V2;
using OpenWLS.Server.LogDataFile.Models.NMRecord;

namespace OpenWLS.Server.LogDataFile.DLIS
{
    public class DlisFrame : Frame
    {
        public double? LevelSpacing { get; set; }
        public double? IndexMin { get; set; }
        public double? IndexMax { get; set; }
        public string UOI { get; set; }

        public LogIndexType IndexType { get; set; }

        protected bool? indexDecreasing;
        protected int ch_nu;
    //    int samples;
    //    public int Samples { get { return samples; } }


        public bool? IndexDecreasing
        {
            get
            {
                return indexDecreasing;
            }
        }

        public void ReadFrameData(DataReader r, int bufferDataLength)
        {
            try
            {
                while (r.Position < bufferDataLength)
                {
                    if (ch_nu >= ms.Count)
                    {
                        ch_nu = 0;
                        if (r.Position + 1 == bufferDataLength)  //to get around boundry bug
                            break;
                    }
                    if (((DlisChannel)ms[ch_nu]).ReadCurveData(r,  bufferDataLength))
                        ch_nu++;
                }
            }
            catch (Exception e)
            {

            }

        }

        public static DlisFrame CreateFrame(bool v1, ObjectComponent oc, AttributeComponents template, SetComponent channelSet, SetComponent axisSet)
        {
            DlisFrame newFrame = v1?  new DlisFrameV1() : new DlisFrameV2();
            if (newFrame.InitFrame(oc, template, channelSet, axisSet))
                return newFrame;
            else
                return null;
        }

        public virtual bool InitFrame(ObjectComponent oc, AttributeComponents template, SetComponent channelSet, SetComponent axisSet)
        {
            return false;
        }


    }
    public class DlisFrames : List<DlisFrame>
    {
        public DlisFrame GetFrame(string name)
        {
            foreach (DlisFrame df in this)
                if (df.Name == name)
                    return df;
            return null;
        }

        public void UpdateChannelHeads()
        {
            foreach (DlisFrame df in this)
            {
                foreach (Measurement m1 in df.Measurements)
                {
                    MVWriter w = ((DlisChannel)m1).MVWriter;
                    m1.Head.VFirst = w.FirstVal;
                    m1.Head.VLast = w.LastVal;
              //      m1.Head.Samples = w.TotalSamples;
                }
            }
        }
        public void Init(bool v1, SetComponents sets)
        {
            SetComponent channelSet = sets.GetSetComponent("CHANNEL");
            SetComponent frameSet = sets.GetSetComponent("FRAME");
            SetComponent axisSet = sets.GetSetComponent("AXIS");
            int mid = 0;
            foreach (ObjectComponent o in frameSet.Objects)
            {
                DlisFrame frame = DlisFrame.CreateFrame(v1, o, frameSet.Template, channelSet, axisSet);
                if (frame != null)
                {
                    foreach(Measurement m in frame.Measurements)
                        m.Id = mid++;   
                    Add(frame);

                }
            }

        }

  
        /*
        public void SetIndexRange(FileVersion hd)
        {
            if (Count == 0)
                return;
            double d_min = double.MaxValue;
            double d_max = double.MinValue;
            double d_s = 0;
            string ud = null;
            double t_min = double.MaxValue;
            double t_max = double.MinValue;
            double t_s = 0;
            string ut = null;
            for(int i = 1; i < Count; i++)
            {
                DlisFrame df = this[i];

                if (df.IndexType == LogIndexType.BOREHOLE_DEPTH)
                {
                    d_min = Math.Min(df.IndexMin, d_min);
                    d_max = Math.Min(df.IndexMax, d_max);
                    ud = df.Measurements[0].Head.UOI;
                    d_s = (double)df.Measurements[0].Head.Spacing;
                }
              
                if (df.IndexType == LogIndexType.TIME)
                {
                    t_min = Math.Min(df.IndexMin, t_min);
                    t_max = Math.Min(df.IndexMax, t_max);
                    ut = df.Measurements[0].Head.UOI;
                    t_s = (double)df.Measurements[0].Head.Spacing;
                }
            }

            if (double.MinValue == d_min)
            {
                hd.StartDepth = double.NaN;
                hd.StopDepth = double.NaN;
            }
            else
            {
                if(d_s > 0 )
                    hd.SetDepthRange(Index.ConvertToIndexUnit(ud), d_min, d_max);
                else
                    hd.SetDepthRange(Index.ConvertToIndexUnit(ud), d_max, d_min );
            }

            
            if (double.MinValue == t_min)
            {
                hd.StartTime = 0;
                hd.StopTime = 0;
            }
          /*  else
            {
                if (t_s > 0)
                    hd.SetIndexRange(Index.ConvertToIndexUnit(ud), t_min, t_max);
                else
                    hd.SetIndexRange(Index.ConvertToIndexUnit(ud), t_max, d_min );
            }
    */

//        }

    }
}