using OpenWLS.Server.Base.DataType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.DFlash.Plt1
{
    public enum TrigSource { None = 0, Samples = 1, Time = 2, Depth = 3, InstStatus = 4 };
    public enum TrigMode { Disable = 0, Single = 1, Repeat = 2 };

    public class InstPara
    {
        public byte InstID { get; set; }
        public byte ID { get; set; }
        public TrigSource TSource { get; set; }
        public TrigMode TMode { get; set; }
        public uint TVal { get; set; }              // Value of Samples
                                                    // Time, in second
                                                    // Depth
                                                    // InstAtatus
        public float Depth { get; set; }

        public virtual int GetSize()
        {
            return 10;
        }
        public virtual byte[] GetVals(DFVersion v)
        {
            return null;
        }

        public virtual void Restore(DataReader r, DFVersion v)
        {

        }

        public void Save(DataWriter w, DFVersion v)
        {
            byte[] vals = GetVals(v);
            w.WriteData((ushort)(vals.Length + 10));    //2
            w.WriteData(ID);                            //1        
            w.WriteData(InstID);                        //1

            w.WriteData((byte)TSource);                 //1        
            w.WriteData((byte)TMode);                   //1          
            if (TSource == TrigSource.Depth)            //4
                w.WriteData(Depth);
            else
                w.WriteData(TVal);
            w.WriteData(vals);
        }

    }

    public class ParaDb : DataBlock
    {
        public List<InstPara> Paras { get; set; }
        public ParaDb()
        {
            BlockType = BlockType.Parameters;
            Paras = new List<InstPara>();
        }

        public override void UpdateValue(DFVersion v)
        {
            byte[] bs = null;
            switch (v)
            {
                case DFVersion.Lv_1:
                case DFVersion.Lv_G1:
                    int s = 4;
                    foreach (InstPara p in Paras)
                        s += p.GetSize();
                    bs = new byte[s];
                    DataWriter w = new DataWriter(bs);
                    w.WriteData((byte)BlockType);
                    w.WriteData((ushort)s);
                    w.WriteData((byte)Paras.Count);
                    foreach (InstPara p in Paras)
                        p.Save(w, v);
                    break;
                default: break;
            }
            Val = bs;
        }


        public override void Restore(DataReader r, DFVersion v, APInstance api)
        {
            int c = r.ReadByte();
            for (int i = 0; i < c; i++)
            {
                ushort s = r.ReadUInt16();
                byte pid = r.ReadByte();
                byte tid = r.ReadByte();
                TrigSource t = (TrigSource)r.ReadByte();
                TrigMode m = (TrigMode)r.ReadByte();
                Instrument.Instrument inst = api.Insts.GetInstrument(tid);
                InstPara ip = inst.CreateInstPara(pid);
                if (t == TrigSource.Depth)
                    ip.Depth = r.ReadSingle();
                else
                    ip.TVal = r.ReadUInt32();
                ip.Restore(r, v);
                Paras.Add(ip);
            }
        }

        public override string GetSummary()
        {
            string s = base.GetSummary();
            foreach (InstPara p in Paras)
                s = s + "  " + p.TSource + "@ " + p.TVal + ":" + p.InstID + "-" + p.ID + "\n";
            return s;
        }
    }

    public class ParaChangeDb : DataBlock
    {
        public InstPara Para { get; set; }
        public ParaChangeDb()
        {
            BlockType = BlockType.Parameters;
        }

        public override void UpdateValue(DFVersion v)
        {
            byte[] bs = null;
            switch (v)
            {
                case DFVersion.Lv_1:
                case DFVersion.Lv_G1:
                    int s = 3 + Para.GetSize();
                    bs = new byte[s];
                    DataWriter w = new DataWriter(bs);
                    w.WriteData((byte)BlockType);
                    w.WriteData((ushort)s);
                    Para.Save(w, v);
                    break;
                default: break;
            }
            Val = bs;
        }


        public override void Restore(DataReader r, DFVersion v, APInstance api)
        {
            ushort s = r.ReadUInt16();
            byte pid = r.ReadByte();
            byte tid = r.ReadByte();
            TrigSource t = (TrigSource)r.ReadByte();
            TrigMode m = (TrigMode)r.ReadByte();
            Instrument.Instrument inst = api.Insts.GetInstrument(tid);
            Para = inst.CreateInstPara(pid);
            if (t == TrigSource.Depth)
                Para.Depth = r.ReadSingle();
            else
                Para.TVal = r.ReadUInt32();
            Para.Restore(r, v);

        }

        public override string GetSummary()
        {
            string s = base.GetSummary();
            s = s + "  " + Para.TSource + "@ " + Para.TVal + ":" + Para.InstID + "-" + Para.ID + "\n";
            return s;
        }
    }
}
