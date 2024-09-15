using OpenWLS.Server.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Text.Json.Serialization;

namespace OpenWLS.Server.DFlash
{
    public enum DFlashState { Unkonwn = 0, Unconfigured = 1, Unerased = 2, Blank = 3, Programed = 4, Recording = 5, Stopped = 6, Uploaded = 7 }

   /* public interface ILvDFlashFS
    {
        LvDFlashFS GetLvDFlashFS();
    }
   */
    public class LvDFlashFS : RootDb
    {
        /*
        public DFVersion Version { get; set; }
        public int PageSize { get; set; }
        public int ChipSize { get; set; }  // in Mb
        public int Copies { get; set; }
        public byte[] Chips { get; set; }
        */
        public int ReadCopy { get; set; }
        public uint TotalSize { get; set; }
        public uint UnusedSize { get; set; }

        public DFlashState State { get; set; }

        /**********Root******************/
        public int NxtInst { get; set; }
        //    public bool Started { get; set; }
        public bool Full { get; set; }
        public bool Empty { get; set; }
        //  public bool Downloaded { get; set; }
        public bool HasAE { get; set; }
        /******************************/

        public LvDFlashFS()
        {
            // Started = false;
            Version = DFVersion.Lv_G1;
            Full = false;
           // Downloaded = false;
        }

       

        public void ReadInfor(DataReader r)
        {
            State = (DFlashState)r.ReadByte();
            NxtInst = r.ReadByte();
            int chips = r.ReadByte();
            Copies = r.ReadByte();
            ChipSize = r.ReadUInt16();
            PageSize = r.ReadUInt16();
            int chip_wr = r.ReadByte();
            UInt32 addre_wr = r.ReadUInt32();

            Chips = r.ReadByteArray(chips);
            TotalSize = (uint)chips / (1 + (uint)Copies) * (uint)ChipSize;
            UnusedSize = TotalSize - (uint)(ChipSize * chip_wr + (addre_wr >> 20));
            Empty = (addre_wr == 0) && (chip_wr == 0);
        }


  
    }
}
