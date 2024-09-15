using System.Text.Json.Serialization;
using OpenWLS.Server.LogDataFile;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading;
using Index = OpenWLS.Server.LogDataFile.Index;

namespace  OpenWLS.Server.LogDataFile.Models
{
    public partial class Frame
    {
        public string Name { get; set; }       //20 
        public int Samples { get; set; }       //4 
        public DataFile DataFile { get; set; }
        protected Measurements ms;
        public Measurements Measurements { get { return ms; } }

        protected Indexes indexes;
        public Indexes Indexes { get { return indexes; } }



 /*     public Index DefaultIndex
        {
            get
            {
                if (indexes.Count == 1)
                    return indexes[0];
                else
                    return indexes[1];
            }
        }
 
        public object Tag { get; set; }  // LAS convert
*/


        public Frame()
        {
            ms = new Measurements();
            indexes = new Indexes();
        }

        public Frame(string name, int samples)
        {
            ms = new Measurements();
            Name = name;
            Index ai = new IndexEsWithoutGap(samples);
            indexes = new Indexes();
            Indexes.Add(ai);
        }

        public Frame(Measurement m)
        {
            ms = new Measurements();
            Server.LogDataFile.Index ai = new IndexEsWithoutGap(m.Samples);
            indexes = new Indexes();
            Indexes.Add(ai);
            if (m.Head.UOI != null)
            {
                ai = (bool)m.NoValueGap? new IndexEsWithoutGap(m) : new IndexEsWithGap(m);
                Indexes.Add(ai);
            }
        }


        public Index GetIndex(LogIndexType indexType)
        {

            foreach (Index ai in indexes)
            {
                if (ai.Type == indexType)
                    return ai;
            }
            return null;
        }



        public void AddMeasurement(Measurement m)
        {
            Measurements.Add(m);
            if (m.Head.Frame == null)
                return;
            if (m.Head.IndexM != null)
            {
                Index ai = new IndexVsM(m);
                indexes.Add(ai);
            }
        }
    }
    public class Frames : List<Frame>
    {

        public Frame GetFrame(string frame)
        {
            foreach (Frame f in this)
                if (f.Name == frame)
                    return f;
            return null;
        }

        public Frame GetFrameNF(Measurement m)
        {
            if ((bool)m.NoValueGap)
            {
                foreach (Frame f in this)
                {
                    if (f.Indexes.ContainIndex(m))
                        return f;
                }
            }
            Frame f1 = new Frame(m);
            Add(f1);
            return f1;
        }

        public Frame GetFrame(Measurement m)
        {
            string fn = m.Head.Frame;
            foreach (Frame f in this)
            {
                if (f.Name == fn)
                    return f;
            }

            Frame f1 = new Frame(fn, m.Samples);
            Add(f1);
            return f1;
        }

        public void Init(Measurements ms)
        {
            foreach (Measurement m in ms)
            {
                if (string.IsNullOrEmpty(m.Head.Frame))
                    m.Frame = GetFrameNF(m);
                else
                    m.Frame = GetFrame(m);
                m.Frame.AddMeasurement(m);
            }
        }

    }

}
