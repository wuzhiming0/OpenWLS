using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using OpenWLS.PLT1.Edge;
using OpenWLS.Server.Base;
using OpenWLS.Server.LogInstance.OperationDocument;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.PLT1
{
    public class PLT1Sub
    {
        public uint Asset { get; set; }
        public ushort ModelNu { get; set; }
        public byte ModelType { get; set; }
        public byte ModelVersion { get; set; }

        public string Model
        {
            get
            {
                return $"{ModelNu}{(char)ModelType}{(char)(ModelVersion+'A')}";
            }
            set
            {
                int c = value.Length;
                if (c < 3) return;

                ModelNu = Convert.ToUInt16(value.Substring(0, c-2));
                ModelType = (byte)value[c - 2];
                ModelVersion = (byte)(value[c - 1] - 'A');               
            }
        }

        public PLT1Sub()
        {

        }
        public PLT1Sub(InstSub sub)
        {
            Asset = Convert.ToUInt32(sub.Asset);
            Model = sub.Model;
        }

        public void WriteGenInofr(DataWriter w)
        {
            w.WriteData(Asset);
            w.WriteData(ModelNu);
            w.WriteData(ModelType);
            w.WriteData(ModelVersion);
        }
        public void Restore(DataReader r)
        {
            Asset = r.ReadUInt32();
            ModelNu = r.ReadUInt16();
            ModelType = r.ReadByte();
            ModelVersion = r.ReadByte();
        }
    }
    public class PLT1Subs : List<PLT1Sub>
    {

    }
    public class PLT1InstGenInfor
    {
        public byte Addr { get; set; }
        public byte HVersion { get; set; }  //hardware
        public byte FVersion { get; set; }  //firmware
        public PLT1Subs Subs { get; set; }
        public int Id { get; set; }

        public static byte[] GetBytes(InstrumentOd inst)
        {
            PLT1InstGenInfor igi = new PLT1InstGenInfor(inst);
            return igi.GetBytes();
        }

        public PLT1InstGenInfor()
        {

        }
        public PLT1InstGenInfor(InstrumentOd inst)
        {
            Id = inst.Id;
            Addr = inst.Address == null ? (byte)IBProtocol.RESERVED_ADDR : (byte)inst.Address;
            Subs = new PLT1Subs();
            foreach (InstSub sub in inst.Subs)
                Subs.Add(new PLT1Sub(sub));
        }

        public byte[] GetBytes()
        {
            int c = Subs.Count;
            DataWriter w = new DataWriter(8 + c * 8);

            w.WriteData(Addr);
            w.WriteData(HVersion); 
            w.WriteData(FVersion);
            w.WriteData((byte)Subs.Count);
           
            w.WriteData(Id);            
            
            foreach (PLT1Sub sub in Subs)
                sub.WriteGenInofr(w);
            return w.GetBuffer();
        }

        public void Restore(byte[] bs)
        {
            DataReader r = new DataReader(bs);

            Addr = r.ReadByte();   
            HVersion = r.ReadByte();
            FVersion = r.ReadByte();  
            int c = r.ReadByte();
            Id = r.ReadInt32();
            
            Subs = new PLT1Subs();
            for (int i = 0; i < c; i++)
            {
                PLT1Sub sub = new PLT1Sub();
                sub.Restore(r);
                Subs.Add(sub);
            }
        }

        public void UpdateAssetOfSubs(InstSubs subs)
        {
            foreach(PLT1Sub s in Subs)
            {
                InstSub? sub = subs.Find(a => a.Model == s.Model);
                if(sub != null)
                    sub.Asset = s.Asset.ToString();
            }
        }
    }

    public class PLT1InstGenInfors : List<PLT1InstGenInfor>
    {

        public void Restore(byte[] bs)
        {
            DataReader r = new DataReader(bs);
            int c = r.ReadUInt16();
            for (int i = 0; i < c; i++)
            {
                int s = r.ReadInt16();
                PLT1InstGenInfor igi = new PLT1InstGenInfor();
                igi.Restore(r.ReadByteArray(s));
                Add(igi);
            }
        }



        public byte[] GetBytes()
        {
            List<byte[]> bss = new List<byte[]>();
            int c = 2;
            foreach(PLT1InstGenInfor igi in this)
            {
                byte[] bs = igi.GetBytes();
                bss.Add(bs);
                c += 2 + bs.Length;
            }
            DataWriter w = new DataWriter(c);
            w.WriteData((ushort)c);
            foreach (byte[] bs in bss)
            {
                w.WriteData((ushort)bs.Length);
                w.WriteData((byte[]) bs);
            }
            return w.GetBuffer();
        }

    }
}
/*
typedef struct 
{
   uint8_t dst_addr;
   uint8_t src_addr;
   uint16_t b_size;

   uint16_t t_size;
   uint8_t cs;
   uint8_t bt; 
}SGenInforFrame;
 */