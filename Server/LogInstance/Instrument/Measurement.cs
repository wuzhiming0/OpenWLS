using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Data;
using System.Xml;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using OpenLS.Base.UOM;
using System.Data.SQLite;


using OpenWLS.Server.LogInstance;
using OpenWLS.Server.LogInstance.RtDataFile;
using OpenWLS.Server.Base;
using OpenWLS.Server.LogDataFile.Models;
using OpenWLS.Server.DBase.Repositories.InstrumentM;
using System.Security.Policy;
using System.Diagnostics.Eventing.Reader;
using System.Xml.Linq;
using OpenWLS.Server.DBase.Models.GlobalDb;
using OpenWLS.Server.LogInstance.OperationDocument;

namespace OpenWLS.Server.LogInstance.Instrument
{  
    public class Measurement 
    {
        protected LogDataFile.Models.Measurement measurement_df;
        protected LogInstanceS logInstance;

        protected MeasurementUnit mu;

        protected int elementsPerSamples;
        protected int bufSamples;
        protected double[] values_cur;
        protected Instrument inst;
//        protected OperationDocument.InstSub? inst_sub;

        protected bool depthAsIndex;

        protected int? sub_id;
        protected string? sub_model;
        protected FrameRt frame_rt;

        public int Id { get { return measurement_df.Head.Id; } set { measurement_df.Head.Id = value; } }
        public string Name { get { return measurement_df.Head.Name; } set { measurement_df.Head.Name = value; } }
        public string? Desc { get { return measurement_df.Head.Desc; } set { measurement_df.Head.Desc = value; } }           //Description
        public string? DataAxes { get { return measurement_df.Head.DataAxes; } set { measurement_df.Head.DataAxes = value; } }
        //    [JsonIgnore]
        //    public DataAxes? DataAxes { get { return measurement_df.Head.DataAxes; } set { measurement_df.Head.DataAxes = value; } }            //Dimensions
        public SparkPlugDataType DType { get { return measurement_df.Head.DType; } set { measurement_df.Head.DType = value; } } //DataType

        public LogIndexType? IType { get { return measurement_df.Head.IType; } set { measurement_df.Head.IType = value; } }
        public bool? IndexM { get { return measurement_df.Head.IndexM; } set { measurement_df.Head.IndexM = value; } }            // index measurement
    //    public string? UOM { get { return measurement_df.Head.UOM; } set { measurement_df.Head.UOM = value; } }
        public string? UOI { get { return measurement_df.Head.UOI; } set { measurement_df.Head.UOI = value; } }   //unit of index
        public string? Frame { get { return measurement_df.Head.Frame; } set { measurement_df.Head.Frame = value; } }
        public string? DFormat { get { return measurement_df.Head.DFormat; } set { measurement_df.Head.DFormat = value; } }
        public double? Spacing { get { return measurement_df.Head.Spacing; } set { measurement_df.Head.Spacing = value; } }

        public string? UOM
        {
            get
            {
                if (mu != null)
                    return mu.Name;
                return measurement_df.Head.UOM;
            }
            set
            {
                if (mu != null)
                    mu.ChangeUnit(value, TOM);
                else
                    measurement_df.Head.UOM = value;
            }
        }
        
        public string? TOM { get; set; }
  //      public bool? NuDisp { get; set; }            // numeric display
        public bool? Record { get; set; }

        //  SampleBuffer sampleBuffer;
        LinearFunction eng_f;
        IFunction cal_f;

        public double MPoint { get; set; }

        [JsonIgnore]
        public MGroup MGroup { get; set; }

        [JsonIgnore]
        public MeasurementUnit MUnit { get { return mu; } }

        [JsonIgnore]
        public double[] ValuesCur { get { return values_cur; } }
        [JsonIgnore]
        public LogDataFile.Models.Measurement MeasurementDf { get { return measurement_df;  } }
        [JsonIgnore]
        public bool NameDup { get; set; } //duplicated
        [JsonIgnore]
        public int BufSamples { get { return bufSamples; } set { bufSamples = value; } }
        [JsonIgnore]
        public  FrameRt FrameRt { set { frame_rt = value; } }
        public static Measurement CreateMeasurement(MeasurementOd m_od)
        {
            Measurement m = new Measurement();
            m.measurement_df.Head.GetHead1().CopyFrom(m_od);
            m.MPoint = m_od.MPoint;
            m.Id = m_od.Id;
            return m;
        }

        public Measurement()
        {
            bufSamples = 1024;
            measurement_df = new LogDataFile.Models.Measurement();
        }

        protected virtual void OnStartLog()
        {

        }
        public void StartLog(bool depthIndex)
        {
            depthAsIndex = depthIndex;
            measurement_df.CreateMVWriter(bufSamples);
            measurement_df.MVWriter.StartIndex = 0;
            OnStartLog();
        }


        public IndexMeasurement? GetIndexMeasurement(LogIndexType indexType)
        {
            foreach(Measurement m in frame_rt.Measurements)
                if(m is IndexMeasurement)
                {
                    if (m.measurement_df.Head.IType == indexType)
                        return (IndexMeasurement)m;
                }
            return null;
        }

        public virtual int InputData(double[] data, int offset)
        {
            for (int i = 0; i < elementsPerSamples; i++)
                values_cur[i] = data[offset + 1];

            //engineering domain
            if (eng_f != null)
            {
                for (int i = 0; i < elementsPerSamples; i++)
                    eng_f.F(values_cur, values_cur);
            }
            return 0;
        }

        public virtual void AddSample(double val)
        {
            values_cur[0] = val;
            //   if (measurement_df != null)
            measurement_df.MVWriter.WriteSample(val);
        }

        public void AddSample(double[] val)
        {
        //    if (measurement_df != null)
                measurement_df.MVWriter.WriteSample(val);
        }
        public int ProcessData( double[] data, int offset)
        {
            double index = depthAsIndex ? logInstance.Depth.Value - MPoint : logInstance.Depth.Value;
           // if (spacing_type == SpacingType.Index)
           //     values_cur[0] = index;
           // else
                InputData(data, offset);

            //calibration
            if (cal_f != null)
            {
                for (int i = 0; i < elementsPerSamples; i++)
                    cal_f.F(values_cur, values_cur);
            }
            //unit convertion
            if (mu != null)
            {
                if (mu.Func != null)
                {
                    for (int i = 0; i < elementsPerSamples; i++)
                        mu.Func.F(values_cur, values_cur);
                }
            }
                
            // write to buffer          
         //   if(spacing_type == SpacingType.ConstantSpacing)
         //       ((RtCsBufferCS)sampleBuffer).WriteSample(values_cur, 0, index);               
         //   else
         //       sampleBuffer.WriteSample(values_cur);

            return 0;
        }

       /*
        public void SetMP(bool metric)
        {
            double d = metric ? mp_sub : mp_sub * 3.28084;
            mp_string = inst_sub.Bottom + d;
        }
        
        public string GetGDisplayChName()
        {
            return Name + "(" + elementsPerSamples.ToString() + ")";
        }
       */

    }

    public class Measurements : List<Measurement>
    {
        public static Measurements CreateMeasurements(MeasurementOds ms_od)
        {
            Measurements res = new Measurements();
            foreach(MeasurementOd m_od in ms_od)
            {
                Measurement m = Measurement.CreateMeasurement(m_od);
                res.Add(m);
            }
            return res;
        }

        public Measurement TimeChannel { get; set; }
        public Measurement DepthChannel { get; set; }
        public void AddChannel(Measurement dc)
        {
            foreach (Measurement dc1 in this)
                if (dc == dc1)
                    return;
            Add(dc);
        }

        public Measurement? GetMeasurement(string? frame, string name)
        {
            return Find(a => a.Frame == frame && a.Name == name );
        }


        public void SetMP(OperationDocument.InstSubs subs)
        {
            foreach (Measurement dc in this)
                SetMP(subs);
        }
        /*
        public string GetMeasurementNames()
        {
            if (Count > 0)
            {
                string str = "";
                str = this[0].GetGDisplayChName();
                for (int i = 1; i < Count; i++)
                    str = str + "," + this[i].GetGDisplayChName();
                return str;
            }
            else
                return "";
        }
        */
        public void CheckDupNames()
        {
            foreach (Measurement dc in this)
                dc.NameDup = false;

            for(int i = 1; i < Count; i++)
            {
                Measurement di = this[i];
                for(int j = 0; j < i; j++)
                {
                    Measurement dj = this[j];
                    if(di.Name == dj.Name)
                    {
                        dj.NameDup = true;
                        break;
                    }
                }
            }
        }

        public void ChanegUnitToSelected()
        {
            foreach (Measurement dc in this)
                if (dc.MUnit != null)
                    dc.MUnit.ChanegUnitToSelected();
        }
/*
        public int[] GetN1DMeasurementIds()
        {
            List<Measurement> ms = this.Where( m => m.NuDisp != null && m.MeasurementDf.Head.SampleElements == 1 ).ToList();
            int[] result = new int[ms.Count];
            for (int i = 0; i < ms.Count; i++)
                result[i] = ms[i].Id;
            return result;
        }


        public void AddN1D(string name, int ele)
        {
            Measurement dc = GetMeasurement(name);
            if (dc != null)
                dc.RemoveN1D(ele);
        }

        public void RemoveN1D(string name, int ele)
        {
            Measurement dc = GetMeasurement(name);
            if (dc != null)
                dc.RemoveN1D(ele);
        }
*/
     
    }

    public class Measurement1DVal
    {
        public int Id { get; set; }
        public string Val { get; set; }
    }
}
