using OpenWLS.Server.LogDataFile.Models;
using OpenWLS.Server.LogInstance.Instrument;
//using OpenWLS.Server.LogDataFile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.LogInstance.RtDataFile
{
    public class IndexMeasurement : Instrument.Measurement
    {
     //   protected bool indexIncrease;
        protected double[] index_vals;
        protected int pos_wr;
        public int WritePosition { get { return pos_wr; } }
        public double[] IndexVals { get { return index_vals; } }
        protected override void OnStartLog()
        {
            index_vals = new double[measurement_df.MVWriter.TotalSamples];
            pos_wr = 0;
        }
        public override void AddSample(double v)
        {
            if (pos_wr >= index_vals.Length)
                pos_wr = 0;
            index_vals[pos_wr++] = v;
            base.AddSample(v);
        }
    }

    public class TimeMeasurement : IndexMeasurement
    {
        public TimeMeasurement(int samples, DataFileRt ar, LogInstance api, Frame f, double spacing)
        {
            /*    Frame = f; 
                head.Frame = f.Name;
                sampleBuffer = SampleBuffer.CreateBuffer(samples, head);
                sampleBuffer.Set1DVal(0);
                SetID(ar);
                head.UOM = api.Time.Unit;
                head.Name = "TIME";
                head.IsIndexChannel = true;
                head.IndexType = LogIndexType.TIME;
                head.Spacing = spacing;
                indexIncrease = true;*/
        }

    }
    public class DepthMeasurement : IndexMeasurement
    {
        public DepthMeasurement(int samples, DataFileRt df, LogInstance api, LogDataFile.Models.Frame f, double spacing, bool indexIncrease)
        {
            /*   Frame = f;
               head.Frame = f.Name;

               sampleBuffer = SampleBuffer.CreateBuffer(samples, head);
               sampleBuffer.Set1DVal(api.Depth.Value);
               SetID(ar);
               head.UOM = api.Depth.Unit;
               head.Name = "DEPT";
               head.IsIndexChannel = true;
               head.IndexType = LogIndexType.BOREHOLE_DEPTH;
               head.Spacing = spacing;
               indexIncrease = _indexIncrease;*/
        }
    }



}
