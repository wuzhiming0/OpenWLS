using OpenWLS.Server.LogInstance;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using OpenWLS.Server.Base;
using OpenWLS.Server.LogInstance.Instrument;

namespace OpenWLS.PLT1.Edge
{
    /**********************************Frame Structure***************************/
    /*********Dst Address|Src Address|Msg Blocks|Checksum(Optional)|Bus Token******/
    /*                               |          |
      -------------------------------            ----------------------------------
    |********************************Block Structure*******************************|
     *********Block Size|Block Type|Block Body|Padding Bytes(Optional)**************/

    public class Block
    {
        public byte Type { get; set; }
        public byte[]? Body { get; set; }
        public static Block? ReadBlock(DataReader r)
        {
            if(r.END) return null;
            Block blk = new Block();
            int s = r.ReadUInt16();     //include all items in this message: size, type, body 
            if (s == 0) return null;
            blk.Type = r.ReadByte();
            int body_size = s - 3;
            if (body_size > 0)
                blk.Body = r.ReadByteArray(body_size);
            return blk;
        }

        public int GetSize()
        {
            int s = 3;
            if(Body != null)
                s += Body.Length;
            return s;
        }

        public byte[] GetBytes()
        {
            int c = GetSize();
            DataWriter w = new DataWriter(c);
            w.WriteData((ushort)c);
            w.WriteData(Type);
            if (Body != null)
                w.WriteData(Body);
            return w.GetBuffer();
        }
        public void WriteBlock(DataWriter w)
        {
            w.WriteData((ushort)GetSize());
            w.WriteData(Type);
            if(Body != null)
                w.WriteData(Body);
        }
    }

    public class Blocks : List<Block>
    {
        public static Blocks ReadBlocks(DataReader r)
        {
            Blocks ms = new Blocks();
            Block? m = Block.ReadBlock(r);
            while(m != null)
            {
                ms.Add(m);
                m = Block.ReadBlock(r);
            }
            return ms;
        }
    }

    public class Frame
    {
        public static readonly byte SRC_ADDRESS_OFFSET = 1;
        public ushort Size { get; set; }
        public byte DstAddress { get; set; }
        public byte SrcAddress { get; set; }
        public Blocks Blocks {  get; set; }
        public bool Checksum { get; set; }
        public bool? BusToken { get; set; }
        public static Frame ReadFrame(DataReader r)
        {
            Frame p = new Frame();
            int pos1 = r.Position;

            //read addresses and blocks
            p.DstAddress = r.ReadByte();
            p.SrcAddress = r.ReadByte();
            p.Blocks = Blocks.ReadBlocks(r);

            //calc checksum
            byte cs = 0;
            byte[] bs = r.GetBuffer();
            for (int i = pos1; i < r.Position; i++)
                cs += bs[i];
            p.Checksum = r.ReadByte() == cs;

            //bus token
            byte bt = r.ReadByte();
            p.BusToken = bt == IBProtocol.EOF_BT_ADDR ? true : bt == IBProtocol.EOF_NBT_ADDR ? false : null;
            return p;
        }

        public static byte[] GetFrameBytes(byte addr_dst, byte addr_src, byte m_type, bool bt)
        {
            return GetFrameBytes(addr_dst, addr_src, m_type, null, bt);
        }

        public static byte[] GetFrameBytes(byte addr_dst, byte addr_src, byte m_type, byte[]? body, bool bt)
        {
            Frame p = new Frame()
            {
                DstAddress = addr_dst,
                SrcAddress = addr_src,
                BusToken = bt
            };
            Block b = new Block()
            {
                Type = m_type,
                Body = body,
            };
            p.Blocks.Add(b);
            return p.GetBytes();
        }
 

        public Frame()
        {
            Blocks = new Blocks();
        }

        int GetSize()
        {
            int c = 6;                  //dst, src, dummy block(2 bytes)  cs, bt 
            foreach (Block b in Blocks)
                c += b.GetSize();
            return c;
        }

        public byte[] GetBytes()
        {
            int c = GetSize();
            DataWriter w = new DataWriter(c);
            w.WriteData(DstAddress);
            w.WriteData(SrcAddress);
            foreach (Block b in Blocks)
                b.WriteBlock(w);
            w.WriteData((ushort)0);
            byte cs = 0;
            byte[] bs = w.GetBuffer();
            for (int i = 0; i < c - 2; i++)
                cs += bs[i];
            bs[c - 2] = cs;
            bs[c - 1] = BusToken != null? IBProtocol.EOF_BT_ADDR : IBProtocol.EOF_NBT_ADDR;
            return w.GetBuffer();
        }

    }

   
}
