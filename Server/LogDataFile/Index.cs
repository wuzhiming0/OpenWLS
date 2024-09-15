using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using OpenLS.Base.UOM;
using OpenWLS.Server.Base;
using OpenWLS.Server.LogDataFile;
using OpenWLS.Server.LogDataFile.Models;

namespace OpenWLS.Server.LogDataFile
{
    public class Index
    {
        public static LogIndexType ConvertToIndexType(string str)
        {
            if (str == null)
                return LogIndexType.Unknown;
            string s = str.Replace('-', '_');
            try 
            {            
                LogIndexType t = (LogIndexType)Enum.Parse(typeof(LogIndexType), s);   
                return t;
            }
            catch (Exception e) { return LogIndexType.Unknown; }
        }

        public static string  IndexTypeTostring(LogIndexType t)
        {
            return t.ToString().Replace('_', '-');
        }

        public static LogIndexType ConvertToIndexType(IndexUnit unit){
            switch(unit){
                case IndexUnit.second:
                    return LogIndexType.TIME;
                case IndexUnit.meter:
                case IndexUnit.ft:
                    return LogIndexType.BOREHOLE_DEPTH;
                default:
                    return LogIndexType.Unknown;
             }
        }

        public static string ToUomString(IndexUnit unit)
        {
            switch (unit)
            {
                case IndexUnit.second:
                    return "s";
                case IndexUnit.meter:
                    return "m";
                case IndexUnit.ft:
                    return "ft";
                default:
                    return "";
            }
        }

        public static string ToStandardUomString(string str)
        {
            str = str.Trim().ToLower();
            if (str == "m" || str == "meter" )
                return "m";
            if (str == "f" || str == "ft" || str == "feet" )
                return "ft";
            if (str == "s"  || str == "second" )
                return "s";
            return "";
        }

        public static IndexUnit ConvertToIndexUnit(string str)
        {
            if(str == null)
                return IndexUnit.unknown;
            str = str.Trim().ToLower();
            if (str == "m" || str == "meter" )
                return IndexUnit.meter;
            if (str == "f" || str == "ft" || str == "feet" )
                return IndexUnit.ft;
            if (str == "s" || str == "second" )
                return IndexUnit.second;
            return IndexUnit.unknown;
        }

        public static LogIndexType FromUnit(string str)
        {
            if( str == null )
                return LogIndexType.NotIndex;
            str = str.Trim().ToLower();
            if ( str == "m" || str == "meter" )
                return LogIndexType.BOREHOLE_DEPTH;
            if ( str == "f" || str == "ft" || str == "feet" )
                return LogIndexType.BOREHOLE_DEPTH;
            if ( str == "s" || str == "second" )
                return LogIndexType.TIME;
            return LogIndexType.NotIndex;
        }
        public static double GetIndexCoeff(string to_u, string from_u, LogIndexType lit)
        {
            double indexCoeff = 1;
            switch (lit)
            {
                case LogIndexType.SAMPLE_NUMBER:                   
                    break;
                case LogIndexType.VERTICAL_DEPTH:
                case LogIndexType.BOREHOLE_DEPTH:
                    indexCoeff = MeasurementUnit.GetDepthConvertMul(to_u, ToStandardUomString(from_u));
                    break;
                case LogIndexType.TIME:
                case LogIndexType.DATE_TIME:
                    indexCoeff = 1;
                    indexCoeff = MeasurementUnit.GetTimeConvertMul(to_u, ToStandardUomString(from_u));
                    break;
                default:
                    indexCoeff = 1;
                    break;
            }
            return indexCoeff;
        }
        public LogIndexType Type { get; set; }
        public string UOM{get; set;}
        public Measurement Measurement { get; set; }
        public double Start{get; set;}
        public double Stop{get; set;}
        public double? Spacing{get; set;}
        public double IndexShift { get; set; }

        public int Samples { get; set; }
        protected double[] values_master;

        protected int[] mids;
        protected double[] offsets;
  //      protected int selected_mid;
 //       protected double[] values_sel;
 //       public double[] ValuesSel { get { return values_sel; } }

        protected bool indexDecreasing;
        [JsonIgnore]
        public bool IndexDecrease
        {
            get {  return indexDecreasing;   }
        }


        [JsonIgnore]
        public double Top
        {
            get {  return Math.Min(Start, Stop);     }
        }

        [JsonIgnore]
        public double Bottom
        {
            get {  return Math.Max(Start, Stop);        }
        }

        [JsonIgnore]
        public bool EqualSpacing { get { return this is IndexEsWithoutGap;} }

        public Index() { }
 


        protected  void Init (int samples)
        {
            Start = 1;
            Stop = samples;
            Samples = samples;
            Spacing = 1;
            Type = LogIndexType.SAMPLE_NUMBER;
            UOM = null;
            IndexShift = 0;
            indexDecreasing = false;
        }

        protected void Init(Measurement m)
        {
            Start = (double)m.StartIndex;
            Stop = (double)m.StopIndex;
            Spacing = m.Head.Spacing;
            UOM = m.Head.UOI.ToLower();
            IndexShift = m.Head.IndexShift == null? 0 : (double)m.Head.IndexShift;
        //    Samples = head.Samples;

            if (m.Head.IType == null || m.Head.IType == LogIndexType.NotIndex)
                Type = FromUnit(UOM);
            else
                Type = (LogIndexType)m.Head.IType;
            indexDecreasing = Start > Stop;
        }

        public bool SameIndex(Measurement m)
        {
            return Start == m.StartIndex
            && Stop == m.StopIndex
            && Spacing == m.Head.Spacing
            && (Type == m.Head.IType || m.Head.IType == null)
            && UOM == m.Head.UOI.ToLower();
        }

        public int SetIndexType(string strType)
        {
            strType.Replace('-', '_');
            try
            {
                Type = (LogIndexType)Enum.Parse(typeof(LogIndexType), strType);
            }
            catch (Exception e)
            {
                return -1;
            }
            return 0;
        }
        
        public virtual void LoadIndexBuffer()
        {

        }
                
        public virtual double  GetSpacingInInches(){
            if (Spacing == null) return 0;
            if( UOM=="m" ) return 39.3701 * (double)Spacing;
            else return 12 * (double)Spacing;            
        }

        public string ToShortString()
        {
            if (Type == LogIndexType.SAMPLE_NUMBER)
                return $"{Type} ( {Stop.ToString("g")} )";
            else
            {
                if(Spacing == null)
                    return $"{Type} ( {StringConverter.GetStringAndTrimEnd(Start, "f2")}, {StringConverter.GetStringAndTrimEnd(Stop, "f2")} ) {UOM}";
                else
                    return $"{Type} ( {StringConverter.GetStringAndTrimEnd(Start, "f2")}, {StringConverter.GetStringAndTrimEnd(Stop, "f2")}," +
                        $"  {StringConverter.GetStringAndTrimEnd((double)Spacing, "f3")} ) {UOM}";
            }
        }

        public virtual double GetSampleIndex(int position)
        {
            return double.NaN;
        }
        public virtual int GetSampleOffset(double index)
        {
            return 0;
        }

        public  double[]  GetMeasurmentIndexes(int mid)
        {
            for (int k = 0; k < mids.Length; k++)
            {
                if (mid == mids[k])
                {
                    double[] values_sel = new double[values_master.Length];
                    for (int i = 0; i < values_master.Length; i++)
                        values_sel[i] = values_master[i] + offsets[k];
                    return values_sel;
                }
            }
            return values_master;
        }
    }

    public class IndexEsWithoutGap : Index
    {
        public IndexEsWithoutGap(int samples)
        {
            Init(samples);
            IndexShift = 0;
        }
        public IndexEsWithoutGap(Measurement m) {
            Init(m);
        }
        public override double GetSampleIndex(int position)
        {
            return Top + (double)Spacing * position;
        }
        public override int GetSampleOffset(double index)
        {
            return (int)((index - Top) / Spacing);
        }

        public override void LoadIndexBuffer()
        {
            if (values_master == null)
            {
                values_master = new double[Samples];
                double d = Top;
                double space = (double)Spacing;
                for (int i = 0; i < Samples; i++)
                {
                    values_master[i] = d;
                    d += space;
                }
            //    values_sel = values_master;
            }
        }
    }

    public class IndexEsWithGap : Index
    {
        public IndexEsWithGap(Measurement m) 
        {
            Init(m);
            Measurement = m;
            Samples = m.Samples;
        }
        public override double GetSampleIndex(int position)
        {
            return Top + (double)Spacing * position;
        }

        public override void LoadIndexBuffer()
        {
            if (values_master == null)
            {
                values_master = new double[Samples];

                int k = 0;
                double space = (double)Spacing;
                foreach (MVBlock b in Measurement.MVBlocks)
                {
                    double d = (double)b.StartIndex;
                    for (int i = 0; i < b.Samples; i++)
                    {
                        values_master[k++] = d;
                        d += space;
                    }
                }
               // values_sel = values_master;
            }
        }
    }

    public class IndexVs : Index
    {
        public IndexVs()
        {

        }
        public override double GetSampleIndex(int position)
        {
            int c = values_master.Length;
            if (position < 0)
                return double.NegativeInfinity;
            if (position >= c)
                return double.PositiveInfinity;

            return values_master[position];
        }

        public override int GetSampleOffset(double index)
        {
            return Math.Abs(Array.BinarySearch(values_master, index - IndexShift));
        }



    }

    public class IndexVsM : IndexVs
    {
        public Measurement Measurement { get; set; }

        public IndexVsM(Measurement m)
        {
            Start = (double)m.Head.VFirst;
            Stop = (double)m.Head.VLast;
    //        Spacing = (double)m.Head.Spacing;
            Type = (LogIndexType)m.Head.IType;
            Samples = m.Samples;
            UOM = m.Head.UOM;
            IndexShift = m.Head.IndexShift == null ? 0 : (double)m.Head.IndexShift;
            Measurement = m;
            indexDecreasing = Start > Stop;   
            if(m.Head.IOffsets != null)
            {
                string[] ss = m.Head.IOffsets.Split(',');
                mids = new int[ss.Length];
                offsets = new double[ss.Length];
                for(int i = 0; i < ss.Length; i++)
                {
                    string[] ss1 = ss[i].Split(':');
                    if(ss1.Length==2)
                    {
                        mids[i] = Convert.ToInt32(ss1[0]);
                        offsets[i] = Convert.ToDouble(ss1[1]);
                    }
                }
            }
        }
        public override double GetSampleIndex(int position)
        {
            int c = values_master.Length;
            if (position < 0)
                return double.NegativeInfinity;
            if (position >= c)
                return double.PositiveInfinity;

            return values_master[position];
        }
        public override int GetSampleOffset(double index)
        {
            return Math.Abs(Array.BinarySearch(values_master, index - IndexShift));
        }
        public override void LoadIndexBuffer()
        {
            if (values_master == null)
            {
                MVReader r = new MVReader((Measurement)Measurement);
                values_master = r.ReadAllDoubles();
                if (IndexShift != 0)
                {
                    for (int i = 0; i < values_master.Length; i++)
                        values_master[i] += IndexShift;
                }
                //indexDecreasing
            }
        }
    }

    public class Indexes : List<Index>
    {
        public Index GetChannelIndex(LogIndexType indexType)
        {
            foreach (Index di in this)
                if (di.Type == indexType)
                    return di;
            return null;
        }

        public bool ContainIndex(Measurement m)
        {
            if (Count == 1)
                return this[0].SameIndex(m);
            if(Count > 1)
            {
                for (int i = 1; i < Count; i++)
                    if (this[i].SameIndex(m))
                        return true;
            }
            return false;
        }

        public string Summary {
            get {
                if (Count == 0)
                    return null;
                string str = "";
                foreach (Index ai in this)
                {

                    str = str + "\n" + ai.ToShortString();
                }
                return str.Remove(0, 1);
            }
        }

    }

}
