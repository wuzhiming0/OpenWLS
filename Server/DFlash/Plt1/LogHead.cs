using OpenLS.Base.API;
using OpenWLS.Server.Base.DataType;
using OpenLS.Base.Instrument;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.DFlash.Plt1
{

    public class LogHead : DataBlock
    {
        string insts_summary;
        public Instruments Insts { get; set; }

        public int SystemID { get; set; }
        public int JobID { get; set; }
        public DateTime BeginTime { get; set; }        //dt LogHead was created
        public double Depth { get; set; }

        public LogHead()
        {
            BlockType = BlockType.LogHead;
            Insts = new Instruments();
        }

        public LogHead(DFVersion v, APInstance api)
        {
            BlockType = BlockType.LogHead;
            Insts = api.Insts;
            SystemID = APInstance.system_id;
            JobID = api.SelectedJob.ID;
            BeginTime = DateTime.Now;
            Depth = api.Depth.Value;
            UpdateValue(v);
        }
        public override void UpdateValue(DFVersion v)
        {
            ushort s = (ushort)(3 + 24 + 1 + Insts.Count * 30);
            DataWriter w = new DataWriter(s);
            w.WriteData((byte)BlockType);
            w.WriteData(s);
            w.WriteData(BeginTime.Ticks);       //8
            w.WriteData(Depth);                //8
            w.WriteData(SystemID);              //4    
            w.WriteData(JobID);                 //4

            w.WriteData((byte)Insts.Count);
            foreach (Instrument.Instrument inst in Insts)
            {
                w.WriteData(inst.Address);                              //2
                w.WriteData(inst.MID);                                  //4
                w.WriteData(Convert.ToUInt64(inst.AssetNumber, 16));    //8    
                w.WriteString(inst.Name, 16);                            //16 
            }
            Val = w.GetBuffer();
        }

        public override void Restore(DataReader r, DFVersion v, APInstance api)
        {
            //       Downloaded = r.ReadByte() == 0;       
            BeginTime = new DateTime(r.ReadLongInt());     //8     
            Depth = r.ReadDouble();                        //8           
            SystemID = r.ReadInt32();                       //4
            JobID = r.ReadInt32();                         //4     

            int c = r.ReadByte();
            insts_summary = null;
            for (int i = 0; i < c; i++)
            {
                //  ushort s = r.ReadUInt16();
                ushort addr = r.ReadUInt16();
                int mid = r.ReadInt32();
                ulong asset = r.ReadULongInt();
                string name = r.ReadString1(16).Trim();
                if (api != null)
                {
                    Instrument.Instrument inst = api.Insts.GetInstrument(addr);
                    Insts.Add(inst);
                }
                else
                    insts_summary = insts_summary + "  " + name + ": " + asset.ToString("x") + "\n";
            }
        }

        public override string GetSummary()
        {
            string s = base.GetSummary();
            s += "  Time:  " + BeginTime.ToString();
            s += "\n  Depth: " + Depth.ToString();
            s += "\n  Sys:   " + SystemID.ToString();
            s += "\n  Job:   " + JobID.ToString();
            s += "\n  Instruments\n";
            if (insts_summary == null)
            {
                foreach (Instrument.Instrument inst in Insts)
                    if (inst == null)
                        s = s + "    ?\n";
                    else
                        s = s + "   " + inst.Name + ": " + inst.AssetNumber + "\n";
            }
            else
                s += insts_summary;
            return s;
        }

    }



}
