
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.DFlash.Plt1
{
    public class InstDb : DataBlock
    {
        string summary;
        public Instruments Insts { get; set; }
        public InstDb()
        {
            BlockType = BlockType.Instruments;
            Insts = new Instruments();
        }

        public InstDb(Instruments insts, DFVersion v)
        {
            BlockType = BlockType.Instruments;
            Insts = insts;
            UpdateValue(v);
        }

        public override void UpdateValue(DFVersion v)
        {
            byte[] bs = null;
            switch (v)
            {
                case DFVersion.Lv_1:
                case DFVersion.Lv_G1:
                    int s = 4 + Insts.Count * 14;        //2+4+8 = 14
                    bs = new byte[s];
                    DataWriter w = new DataWriter(bs);
                    w.WriteData((byte)BlockType);
                    w.WriteData((ushort)s);
                    w.WriteData((byte)Insts.Count);
                    foreach (Instrument.Instrument inst in Insts)
                    {
                        w.WriteData(inst.Address);                    //2
                        w.WriteData(inst.MID);                              //4
                        w.WriteData(Convert.ToUInt64(inst.AssetNumber, 16));    //8    
                    }
                    break;
                default:
                    break;
            }
            Val = bs;
        }

        public override void Restore(DataReader r, DFVersion v, APInstance api)
        {
            int c = r.ReadByte();
            summary = c.ToString() + " instruments\n";
            for (int i = 0; i < c; i++)
            {
                ushort s = r.ReadUInt16();
                ushort addr = r.ReadUInt16();
                int mid = r.ReadInt32();
                ulong asset = r.ReadULongInt();
                if (api != null)
                {
                    Instrument.Instrument inst = api.Insts.GetInstrument(addr);
                    Insts.Add(inst);
                }
                else
                    summary = summary + "  " + Instrument.Instrument.GetName(mid) + ": " + asset.ToString("x") + "\n";
            }
        }

        public override string GetSummary()
        {
            string s = base.GetSummary();
            foreach (Instrument.Instrument inst in Insts)
                if (inst == null)
                    s = s + "?\n";
                else
                    s = s + "  " + inst.Name + ": " + inst.AssetNumber + "\n";
            return s;
        }
    }

}
