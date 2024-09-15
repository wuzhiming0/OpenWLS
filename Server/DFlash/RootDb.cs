using OpenWLS.Server.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.DFlash
{
    public class  RootDb : DataBlock
    {
        int chips;
        public DFVersion Version { get; set; }
        public int PageSize { get; set; }      
        public int ChipSize { get; set; }  // in Mb 
        public ulong Asset { get; set; }
        public ulong InstPre { get; set; }
        public ulong InstNxt { get; set; }
        public DateTime Time { get; set; }        //dt LogHead was created
        public int SystemID { get; set; }   
        public int Copies { get; set; }
        public byte[] Chips { get; set; }

        public RootDb()
        {
            BlockType = BlockType.Root;
        }



      /*  public RootDb(LvDFlashFS fs)
        {
            BlockType = BlockType.Root;
     //       Version = fs.Version;
            Copies = fs.Copies;
            Chips = fs.Chips;
            PageSize = fs.PageSize;
            ChipSize = fs.ChipSize;
            UpdateValue(Version);
        }
      */
        public override void UpdateValue(DFVersion v)
        {    
            int s = 13 + 36; 
            if (Chips != null)
                s += Chips.Length;
            byte[]  bs = new byte[s];
            DataWriter w = new DataWriter(bs);
            Version = v;
            w.WriteData((byte)BlockType);           //1
            w.WriteData((ushort)s);                 //2    
            w.WriteData((uint)Version);            //4
            w.WriteData((ushort)ChipSize);          //2
            w.WriteData((ushort)PageSize);         //2

            w.WriteData(Asset);
            w.WriteData(InstPre);
            w.WriteData(InstNxt);
            w.WriteData(Time.Ticks);
            w.WriteData(SystemID);

            w.WriteData((byte)Copies);              //1
            w.WriteData((byte)Chips.Length);        //1
            w.WriteData(Chips);
            Val = bs;
        }

        public int GetNoChipListLength()
        {
            return 13;
        }

        public void RestoreNoChipList(DataReader r)
        {
            Version = (DFVersion)r.ReadUInt32();
            ChipSize = r.ReadUInt16();
            PageSize = r.ReadUInt16();

            Asset = r.ReadULongInt();
            InstPre = r.ReadULongInt();
            InstNxt = r.ReadULongInt();
            Time = new DateTime(r.ReadLongInt());
            SystemID = r.ReadInt32();

            Copies = r.ReadByte();
            chips = r.ReadByte();
        }

        public override void Restore(DataReader r )
        {
            RestoreNoChipList(r);
            Chips = r.ReadByteArray(chips);
        }

        public override string GetSummary()
        {
            string s = base.GetSummary();
            s = s + "  Copies:" + Copies.ToString() + "\n";
            return s;
        }
    }

}
