using OpenWLS.Server.Base.DataType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.DFlash.Plt1
{
    public class ActDb : DataBlock
    {
        AcqItems acqItems;
        public ActDb()
        {
            BlockType = BlockType.AcqItems;
        }

        public override void UpdateValue(DFVersion v)
        {
            byte[] bs = null;
            switch (v)
            {
                case DFVersion.Lv_1:
                case DFVersion.Lv_G1:
                    int s = 0;
                    foreach (AcqItem acq in acqItems)
                        if (acq.Enable)
                            s++;
                    s = 2 + s * 12;
                    bs = new byte[s];
                    DataWriter w = new DataWriter(bs);
                    w.WriteData((byte)BlockType);
                    w.WriteData((byte)acqItems.Count);
                    foreach (AcqItem acq in acqItems)
                    {
                        if (acq.Enable)
                        {
                            w.WriteData((byte)acq.Address);
                            w.WriteData((byte)acq.DataGroup);
                            w.WriteData((ushort)acq.DataSize);
                            w.WriteData((uint)(acq.IntervalTime * 1000));
                            w.WriteData((uint)(acq.IntervalDepth * 1000));
                        }
                    }
                    break;
                default:
                    break;
            }
            Val = bs;
        }


        public override void Restore(DataReader r, DFVersion v, APInstance api)
        {

        }

        public override string GetSummary()
        {
            string s = base.GetSummary();
            foreach (AcqItem acq in acqItems)
                s = s + "  " + acq.ToolName + ": " + acq.DataGroup + "\n";
            return s;
        }
    }

}
