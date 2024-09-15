using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OpenWLS.Server.Base;
using System.IO;
using System.Reflection;

namespace OpenWLS.PLT1
{
    public abstract class InstCntlTbl
    {
        public static byte[] GetItemBodyBytes(ushort offset, byte[] bs)
        {
            int size = 2 + bs.Length;
            byte[] bs_res = new byte[size];
            bs_res[0] = (byte)offset; bs_res[1] = (byte)(offset >> 8);
            //      bs_res[2] = (byte)size; bs_res[3] = (byte)(size >> 8);
            Buffer.BlockCopy(bs, 0, bs_res, 2, bs.Length);
            return bs_res;
        }
        public abstract void Restore(byte[] bs);
        public abstract byte[] GetTotalBytes();
        public object? GetItemValue(string itemName)
        {
            PropertyInfo? pi = GetType().GetProperty(itemName);
            if (pi == null) return null;
            return pi.GetValue(this, null);
        }
        public object? GetItemValue(int index)
        {
            var v = GetType().GetProperties();
            if (index < v.Length)
                return v[index].GetValue(this, null);
            else
                return null;
        }

        public byte[] GetItemBytes(int offset, int size)
        {
            byte[] bs_all = GetTotalBytes();
            byte[] bs_out = new byte[size];
            Buffer.BlockCopy(bs_all, 0, bs_out, 0, size);
            return bs_out;
        }
        public void SetItemBytes(int offset, byte[] bs)
        {
            byte[] bs_all = GetTotalBytes();
            DataWriter w = new DataWriter(bs_all);
            w.Seek(offset, SeekOrigin.Begin);
            w.WriteData(bs);
            Restore(bs_all);
        }


        public byte[] GetItemBodyBytes(  ushort offset, ushort size)
        {
            byte[] bs = new byte[2 + size];
            bs[0] = (byte)offset; bs[1] = (byte)(offset >> 8);
       //     bs[2] = (byte)size; bs[3] = (byte)(size >> 8);
            byte[] bs_tbl = GetTotalBytes();
            Buffer.BlockCopy(bs_tbl, offset, bs, 2, size);
            return bs; 
        }

    }
}