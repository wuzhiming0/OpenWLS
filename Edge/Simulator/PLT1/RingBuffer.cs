using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Edge.Simulator.PLT1
{
    public class RingBuffer
    {
        byte[] buffer;
        int rd_pos;
        int wr_pos;
        int rd_size;
        int total_size;

        public RingBuffer(int size)
        {
            buffer = new byte[size];
            total_size = size;
            rd_pos = 0;
            wr_pos = 0;
            rd_size = 0;        
        }

        public bool Write(byte[] dat)
        {
            int s = dat.Length;
            if( (s + rd_size) > total_size)
                return false;
            int s1 = total_size - wr_pos;
            if(s1 >= s)
            {
                Buffer.BlockCopy(dat, 0, buffer, wr_pos, s);
                if (s1 == s) wr_pos = 0;
                else wr_pos += s;
            }
            else
            {
                Buffer.BlockCopy(dat, 0, buffer, wr_pos, s1);
                Buffer.BlockCopy(dat, s1, buffer, 0, s - s1);
                wr_pos = s - s1;
            }
            return true;
        }

        public byte[] Read(byte[] dat)
        {
            int s = rd_size;
            byte[] res = new byte[rd_size];
            int s1 = total_size - rd_pos;
            if( s1 >= s )
            {
                Buffer.BlockCopy(buffer, rd_pos, res, 0, s);
                if(s1 == s) rd_pos = 0;
                else        rd_pos += s;
            }
            else
            {
                Buffer.BlockCopy(buffer, rd_pos, res, 0, s1);
                Buffer.BlockCopy(buffer, 0, res, s1, s-s1);
                rd_pos = s-s1;
            }
            rd_size -= s;
            return res;
        }
    }
}
