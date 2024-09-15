using OpenWLS.Server.Base;
using OpenWLS.Server.LogDataFile.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System;
using System.Xml.Linq;
using System.Diagnostics.Metrics;
using System.Xml;
using OpenWLS.Server.DBase;
using OpenWLS.Server.LogDataFile;
using System.Data;
using OpenWLS.Server.GView.ViewDefinition;
using System.Text.Json.Serialization;

namespace OpenWLS.Server.LogDataFile.Models
{
    public partial class Measurement 
    {
        protected MVBlocks mVBlocks;   
        protected DataFile dataFile; 
        protected MVWriter mVWriter;       
        public MHead Head { get; set; }

        public MVBlocks MVBlocks
        {
            get { return mVBlocks; }
            set { mVBlocks = value; }
        }
        [JsonIgnore]
        public int Id
        {
            get  { return Head.Id;  }
            set
            {
                Head.Id = value;
                mVBlocks.MId = value;
            }
        }
        [JsonIgnore]
        public Frame Frame { get; set; }

        [JsonIgnore]
        public DataFile DataFile
        {
            get { return dataFile; }
            set { dataFile = value; }
        }
        
        [JsonIgnore]
        public MVWriter MVWriter { get { return mVWriter; } }     
     
        [JsonIgnore]
        public int Samples {
            get
            {
                if (mVBlocks == null) return 0;
                return mVBlocks.GetTotalSamples();
            }
        }
        [JsonIgnore]
        public bool? NoValueGap
        {
            get
            {
                if (Head.Frame == null)
                    return StartIndex + Head.Spacing * (Samples-1) == StopIndex;
                else
                    return null;
            }
        }
   //     [JsonIgnore]
        public double StartIndex { get; set; }
   //     [JsonIgnore]
        public double StopIndex { get; set; }

        public Measurement()
        {
            Head = new MHead();
            mVBlocks = new MVBlocks();
            Id = -1;
        }

        public void SetPropertyValue(string name, string val)
        {
            if (name == "ValueEmpty")
            {
                Head.VEmpty = Convert.ToDouble(val); return;
            }
            if (name == "DisplayFormat")
            {
                Head.DFormat = val; return;
            }
            if (name == "IndexShift")
            {
                Head.IndexShift = Convert.ToDouble(val); return;
            }

        }

        public string GetStdNumericFormat()
        {
            if (Head.DFormat == null)
                return null;
            string s = Head.DFormat.StartsWith("A") ? Head.DFormat.Substring(1, Head.DFormat.Length - 1) : Head.DFormat;
            int k = s.IndexOf(';');
            if (k > 0)
                s = s.Substring(0, k);
            if (s.Length == 1)
                return null;
            if (s.StartsWith("I")) // integer
            {
                s = s.Substring(1, s.Length - 1);
                string s1 = "0";
                int c = Convert.ToInt32(s1);
                for (int i = 1; i < c; i++)
                    s1 = "#" + s1;
                return "{0," + s + ":" + s1 + "}";
            }
            if (s.StartsWith("F")) // float
            {
                s = s.Substring(1, s.Length - 1);
                k = s.IndexOf('.');
                int t = Convert.ToInt32(s.Substring(0, k));
                int d = Convert.ToInt32(s.Substring(k + 1, s.Length - k - 1));
                string s1 = "0";
                int c = Convert.ToInt32(s1);
                for (int i = 1; i < d; i++)
                    s1 = s1 + "0";
                s1 = "0." + s1;
                for (int i = 1; i < t - d - 1; i++)
                    s1 = "#" + s1;
                return "{0," + t.ToString() + ":" + s1 + "}";
            }
            if (s.StartsWith("E"))  // exponential
            {
                return s.Substring(1, s.Length - 1);
            }

            return null;
        }

        public void ProcessIndex()
        {
            if (Head.Frame != null && Head.Spacing < 0)
            {                
                StartIndex = mVBlocks.Max(b => b.StartIndex);
                StopIndex = mVBlocks.Min(b => b.StopIndex);
                mVBlocks.OrderByDescending(mvb => mvb.StartIndex);
            }
            else
            {
                StartIndex = mVBlocks.Min(b => b.StartIndex);
                StopIndex = mVBlocks.Max(b => b.StopIndex);
                mVBlocks.OrderBy(mvb => mvb.StartIndex);
            }
        }

        public string GetGDisplayChName()
        {
            if (Head.DataAxes == null)
                return "{Head.Name}";
            return "{Head.Name}({Head.Dims})";
        }

        public Measurement(int mid, DataFile df)
        {
            dataFile = df;
            MHead head = new MHead(mid, df);
            mVBlocks = new MVBlocks()
            {
                MId = head.Id,
            };
        }

        public Measurement(MHead head, DataFile df)
        {
            dataFile = df;
            mVBlocks = new MVBlocks()
            {
                MId = head.Id,
            };
            Head = head;
        }

        public void CreateMVWriter(int samples)
        {
            mVWriter = MVWriter.CreateMVWriter(this.Head, samples);
        }
        public void CreateMVWriter(byte[] buffer)
        {
            mVWriter = MVWriter.CreateMVWriter(this.Head, buffer);
        }
        public byte[]? LoadSampleBuffer()
        {
            if (mVBlocks.Count == 1)
                return BinObject.GetVal(dataFile, mVBlocks[0].Id, MVBlock.val_tble_name);
            if (mVBlocks.Count == 0)
                return null;
            int s = 0;
            byte[] bs1 = new byte[GetTotalSampleBytes()];
            foreach (MVBlock mvb in mVBlocks)
            {
                byte[] bs2 = BinObject.GetVal(dataFile, mvb.Id, MVBlock.val_tble_name);
                Buffer.BlockCopy(bs2, 0, bs1, s, bs1.Length);
                s += bs2.Length;
            }
            return bs1;
        }
        public void UpdateMVBlock(byte[] buffer)
        {
            mVBlocks.UpdateMValue(dataFile, buffer);
        }
        public void UpdateMVBlock(byte[] buffer, int offset, int size)
        {

        }

        /*
        public Server.LogDataFile.Index GetDefaultIndex()
        {
            return Frame.Indexes[0];
        }

        public Server.LogDataFile.Index GetIndex(LogIndexType indexType)
        {
            return Frame.GetIndex(indexType);
        }
        */

        public int GetTotalSampleBytes()
        {
            return Samples * Head.SampleElements * DataType.GetDataSize(Head.DType);
        }
 
    }


    public partial class Measurements : List<Measurement>
    {
        public Measurement? GetMeasurement(string? frame, string name)
        {
            if(frame == null)
            return this.Where(m => m.Head.Frame == null && m.Head.Name == name).FirstOrDefault();
            return this.Where(m => m.Head.Frame == frame && m.Head.Name == name).FirstOrDefault();
        }
        /*
        public Measurement GetMeasurement(int id)
        {
            return this.Where(m => m.Id == id).FirstOrDefault();
        }

        public List<Measurement> GetIndexMeasurements()
        {
            return this.Where(m => m.Head.IType != LogIndexType.NotIndex).ToList();

        }
        */

        public string GetMeasurementNames()
        {
            if (Count > 0)
            {
                string str = "";
                str = ((Measurement)this[0]).GetGDisplayChName();
                for (int i = 0; i < Count; i++)
                {
                    if (this[i].Head.IType != LogIndexType.NotIndex) continue;
                    str = str + "," + this[i].GetGDisplayChName();
                }
                return str;
            }
            else
                return "";
        }

    }

}