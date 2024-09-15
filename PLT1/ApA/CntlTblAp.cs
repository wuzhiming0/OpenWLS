using OpenWLS.Server.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.PLT1.ApA
{
    public class CntlTblAp : InstCntlTbl
    {
        public const ushort offset_tx_gain = 0;
        public const ushort offset_tx_equ = 2;
        public const ushort offset_tx_speed = 4;
        public const ushort offset_rx_gain = 6;
        public const ushort offset_rx_equ = 8;
        public const ushort offset_rx_speed = 10;
        public const ushort offset_depth_sim_speed = 12;
        public const ushort offset_depth_src = 16;
        public static ushort[] GetOffsets()
        {
            return new ushort[] { offset_tx_gain, offset_tx_equ, offset_tx_speed, offset_rx_gain, 
                    offset_rx_equ, offset_rx_speed, offset_depth_sim_speed, offset_depth_src };
        }
        public ushort TxGain { get; set; }
        public ushort TxEqu { get; set; }
        public ushort TxSpeed { get; set; }
        public ushort RxGain { get; set; }
        public ushort RxEqu { get; set; }
        public ushort RxSpeed { get; set; }
        public int DepthSimSpeed { get; set; }      // m per hour 
        public byte DepthSrc { get; set; }


        public override void Restore(byte[] bs)
        {
            DataReader r = new DataReader(bs);

            TxGain = r.ReadUInt16();
            TxEqu = r.ReadUInt16();
            TxSpeed = r.ReadUInt16();
            RxGain = r.ReadUInt16();
            RxEqu = r.ReadUInt16();
            RxSpeed = r.ReadUInt16();

            DepthSimSpeed = r.ReadInt32();
            DepthSrc = r.ReadByte();
        }

        public override byte[] GetTotalBytes()
        {
            DataWriter w = new DataWriter(20);

            w.WriteData(TxGain);
            w.WriteData(TxEqu);
            w.WriteData(TxSpeed);
            w.WriteData(RxGain);
            w.WriteData(RxEqu);
            w.WriteData(RxSpeed);

            w.WriteData(DepthSimSpeed);
            w.WriteData(DepthSrc);
            return w.GetBuffer();
        }

    }
}
