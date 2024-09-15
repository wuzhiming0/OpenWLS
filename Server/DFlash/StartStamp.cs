using OpenWLS.Server.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json.Serialization;

namespace OpenWLS.Server.DFlash
{

    public class StartStamp : DataBlock
    {

        public DateTime Time { get; set; }        //dt LogHead was created
        public double Depth { get; set; }

        [JsonIgnore]
        public byte Chip { get; set; }
        [JsonIgnore]
        public uint Address { get; set; }

        [JsonIgnore]
        public long fileOffset { get; set; }

        public StartStamp()
        {
            BlockType = BlockType.StartStamp;
        }

        public StartStamp(DFVersion v, double depth)
        {
            BlockType = BlockType.StartStamp;
            Time = DateTime.Now;
            Depth = depth;
            UpdateValue(v);
        }
        public override void UpdateValue(DFVersion v)
        {
            ushort s = 3 + 16;
            DataWriter w = new DataWriter(s);
            w.WriteData((byte)BlockType);
            w.WriteData(s);
            w.WriteData(Time.Ticks);       //8
            w.WriteData(Depth);                //8
            Val = w.GetBuffer();
        }

        public override void Restore(DataReader r)
        {
            //       Downloaded = r.ReadByte() == 0;       
            Time = new DateTime(r.ReadLongInt());       //8     
            Depth = r.ReadDouble();                     //8           

            // LBlockSize = r.ReadUInt16();              
            // Items.Restore(r, Version);
        }

        public override string GetSummary()
        {
            string s = base.GetSummary();
            s += "  Time: " + Time.ToString();
            s += "\n  Depth: " + Depth.ToString();
            return s;
        }

    }

    public class StopStamp : DataBlock
    {
        public DateTime Time { get; set; }
        public double Depth { get; set; }
        public uint Samples { get; set; }
        public uint TimeTicks { get; set; } // in 100mS

        [JsonIgnore]
        public byte Chip { get; set; }
        [JsonIgnore]
        public uint Address { get; set; }
        [JsonIgnore]
        public long fileOffset { get; set; }

        public StopStamp()
        {
            BlockType = BlockType.StopStamp;
        }

        public StopStamp(DFVersion v, double depth)
        {
            BlockType = BlockType.StopStamp;
            Time = DateTime.Now;
            Depth = depth;
            UpdateValue(v);
        }

        public override void UpdateValue(DFVersion v)
        {
            ushort s = 3 + 20;
            DataWriter w = new DataWriter(s);
            w.WriteData((byte)BlockType);
            w.WriteData(s);
            w.WriteData(Time.Ticks);       //8
            w.WriteData(Depth);                //8
            w.WriteData(Samples); 
            Val = w.GetBuffer();
        }

        public override void Restore(DataReader r)
        {
            Time = new DateTime(r.ReadLongInt());           //8     
            Depth = r.ReadDouble();                         //8   
            Samples = r.ReadUInt32();                       //4
        }
        public override string GetSummary()
        {
            string s = base.GetSummary();
            s += "  Time: " + Time.ToString();
            s += "\n  Depth: " + Depth.ToString();
            s += "\n  Samples: " + Samples.ToString();
            return s;
        }
    }

    public class LogEnd : DataBlock
    {
        public DateTime BeginTime { get; set; }
        public double Depth { get; set; }

        public LogEnd(DFVersion v, double depth)
        {
            BlockType = BlockType.LogEnd;
            BeginTime = DateTime.Now;
            Depth = depth;
            UpdateValue(v);
        }

        public override void UpdateValue(DFVersion v)
        {
            ushort s = 3 + 16;
            DataWriter w = new DataWriter(s);
            w.WriteData((byte)BlockType);
            w.WriteData(s);
            w.WriteData(BeginTime.Ticks);       //8
            w.WriteData(Depth);                 //8

            Val = w.GetBuffer();
        }

        public override void Restore(DataReader r)
        {
            BeginTime = new DateTime(r.ReadLongInt());     //8     
            Depth = r.ReadDouble();                        //8           
        }

    }

}
