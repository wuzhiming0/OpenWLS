using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using OpenWLS.Server.LogDataFile;
using static OpenWLS.Server.Base.VSNumericAxis;
using System.Data.SQLite;
using OpenWLS.Server.LogDataFile.Models;

namespace OpenWLS.Server.Base
{
    public class DataAxis 
    {
        protected static char seperator = ',';
        public enum AxisType { Simple = 0, ConstantSpace, VariableSpace, Texture, Measurement}

        protected AxisType type;

        public virtual int Dimension { get; set; }
        public DataAxis()
        {
        }

        public static DataAxis CreateNew(string str)
        {
            string[] ss = str.Split(seperator);
            if (ss.Length == 1)
            {
                DataAxis da = new SimpleAxis();
                da.Restore(str);
                return da;
            }
            else
            {
                AxisType t = (AxisType)Convert.ToInt32(ss[0]);
                DataAxis da = null;
                switch (t)
                {
                    case AxisType.Simple:
                        da = new SimpleAxis(); break;
                    case AxisType.ConstantSpace:
                        da = new ConstantSpaceAxis(); break;
                    case AxisType.VariableSpace:
                        da = new VSNumericAxis(); break;
                    case AxisType.Texture:
                        da = new TextureAxis(); break;
                    case AxisType.Measurement:
                        da = new MeasurementAxis(); break;
                }
                if (da != null)
                    da.Restore(ss[1]);
                return da;
            }
        }


        public virtual void Restore(string str)
        {

        }


    }
    public class SimpleAxis : DataAxis
    {
        public SimpleAxis() { Dimension = 1; }
        public SimpleAxis(int dim) { Dimension = dim; }
        public override string ToString()
        {
            return $"{Dimension}";
        }

        public override void Restore(string str)
        {
            Dimension = Convert.ToInt32(str);
        }
    }
    public class ConstantSpaceAxis : DataAxis
    {
        public float Spacing { get; set; }
                
        public override string ToString()
        {
            return $"{(int)type}, {Dimension}, {(int)Spacing}";
        }

        public override void Restore(string str)
        {
            string[] ss = str.Split(seperator);
            if(ss.Length == 2)
            {
                Dimension = Convert.ToInt32(ss[0]);
                Spacing = Convert.ToSingle(ss[1]);
            }
        }
    }

    public class VSNumericAxis : DataAxis
    {
        public float[] Coordinates { get; set; }
        public override int Dimension
        {
            get
            {
                if (Coordinates != null) return Coordinates.Length;
                return 0;
            }
            set
            {
                if (value <= 0) Coordinates = null;
                else Coordinates = new float[Coordinates.Length];
            }
        }

        public override string ToString()
        {
            if (Coordinates == null) return null;
            return $"{(int)type}, {string.Join(",", Coordinates)}";
        }

        public override void Restore(string str)
        {
            string s = str.Trim();
            if (string.IsNullOrEmpty(s)) Coordinates = null;
            else
            {
                string[] ss = s.Split(seperator);
                Coordinates = new float[ss.Length];
                for (int i = 0; i < ss.Length; i++)
                    Coordinates[i] = Convert.ToSingle(ss[i]);

            }

        }
    }
    public class TextureAxis : DataAxis
    {
        public string[] Coordinates { get; set; }
        public override int Dimension
        {
            get
            {
                if (Coordinates != null) return Coordinates.Length;
                return 0;
            }
            set
            {
                if (value <= 0) Coordinates = null;
                else Coordinates = new string[Coordinates.Length];
            }
        }

        public override string ToString()
        {
            if (Coordinates == null) return null;
            return $"{(int)type}, {string.Join(",", Coordinates)}";
        }

        public override void Restore(string str)
        {
            string s = str.Trim();
            if (string.IsNullOrEmpty(s)) Coordinates = null;
            else
                Coordinates = s.Split(seperator);
        }
    }

    public class MeasurementAxis : DataAxis
    {
        public string mName { get; set; }
        public Measurement Measurement { get; set; }
        public float[] Coordinates { get; set; }

        public override string ToString()
        {
            if (Measurement != null)
                return $"{(int)type}, {Measurement.Head.Name}";
            return mName;
        }

        public override void Restore(string str)
        {
            mName = str;
        }
    }

    public class DataAxes : List<DataAxis>
    {
        static char seperator = '|';
        public static DataAxes CreateDataAxes(string? str)
        {
            if (string.IsNullOrEmpty(str))return new DataAxes { new SimpleAxis()};
            else {
                DataAxes das = new DataAxes();
                string[] ss = str.Split(seperator);
                foreach (string s in ss)
                    das.Add(DataAxis.CreateNew(s));
                return das;
            }            
        }
        public static DataAxes CreateDataAxes(int[]? ds)
        {
            if (ds == null || ds.Length <= 0) return new DataAxes { new SimpleAxis() }; 
            else
            {
                DataAxes das = new DataAxes();
                for (int i = 0; i < ds.Length; i++)
                    das.Add(new SimpleAxis(ds[i]));
                return das;
            }
        }

        public static int GetTotalElements(string? dataAxes)
        {
            if (dataAxes == null) return 1;
            else return Base.DataAxes.CreateDataAxes(dataAxes).GetTotalElements();
        }
        public DataAxes()
        {
        }
        public int[] GetDimensions()
        {
            if (Count == 0) return new int[] { 1 };
            else
            {
                int[] ds = new int[Count];
                for(int i = 0; i < Count; i++)  ds[i] = this[i].Dimension;
                return ds;
            }
        }
        public int GetTotalElements()
        {
            int n = 1;
            for (int i = 0; i < Count; i++) n *= this[i].Dimension;
            return n;
        }
        public override string ToString()
        {
            return string.Join(seperator, this);
        }
    }
}
