using System.Runtime.InteropServices;

namespace OpenWLS.Server.Base
{
    [StructLayout(LayoutKind.Explicit)]
    public class ByteUnion
    {
        #region Fields to change byte order 
        [FieldOffset(0)] public byte b1;
        [FieldOffset(1)] protected byte b2;
        [FieldOffset(2)] protected byte b3;
        [FieldOffset(3)] protected byte b4;
        [FieldOffset(4)] protected byte b5;
        [FieldOffset(5)] protected byte b6;
        [FieldOffset(6)] protected byte b7;
        [FieldOffset(7)] protected byte b8;
        [FieldOffset(0)] public sbyte sb;
        [FieldOffset(0)] public short s;
        [FieldOffset(0)] public ushort us;
        [FieldOffset(0)] public int i;
        [FieldOffset(0)] public uint ui;
        [FieldOffset(0)] public float f;
        [FieldOffset(0)] public double d;
        [FieldOffset(0)] public long l;
        [FieldOffset(0)] public ulong ul;

        [FieldOffset(12)] protected int offset;
        [FieldOffset(16)] protected byte[] buffer;
        #endregion

        public int Offset { get { return offset; } set { offset = value; } }

        #region functions to decode the data
        public void Read1Byte()
        {
            b1 = buffer[offset++];
        }


        public virtual void Read2Byte()
        {
            b1 = buffer[offset++];
            b2 = buffer[offset++];
        }


        public virtual void Read4Byte()
        {
            b1 = buffer[offset++];
            b2 = buffer[offset++];
            b3 = buffer[offset++];
            b4 = buffer[offset++];
        }
        public virtual void Read8Byte()
        {
            b1 = buffer[offset++];
            b2 = buffer[offset++];
            b3 = buffer[offset++];
            b4 = buffer[offset++];
            b5 = buffer[offset++];
            b6 = buffer[offset++];
            b7 = buffer[offset++];
            b8 = buffer[offset++];
        }
        #endregion

        #region functions to write to the buffer
        public void Write1Byte()
        {
            buffer[offset++] = b1;
        }
        public virtual void Write2Byte()
        {
            buffer[offset++] = b1;
            buffer[offset++] = b2;
        }
        public virtual void Write4Byte()
        {
            buffer[offset++] = b1;
            buffer[offset++] = b2;
            buffer[offset++] = b3;
            buffer[offset++] = b4;
        }
        public virtual void Write8Byte()
        {
            buffer[offset++] = b1;
            buffer[offset++] = b2;
            buffer[offset++] = b3;
            buffer[offset++] = b4;
            buffer[offset++] = b5;
            buffer[offset++] = b6;
            buffer[offset++] = b7;
            buffer[offset++] = b8;
        }
        #endregion

        public ByteUnion(byte[] buf)
        {
            buffer = buf; offset = 0;
        }

    }

    public class ByteUnionR : ByteUnion
    {
        public ByteUnionR(byte[] buf) : base(buf)
        {

        }
        public override void Read2Byte()
        {
            b2 = buffer[offset++];
            b1 = buffer[offset++];
        }

        public override void Read4Byte()
        {
            b4 = buffer[offset++];
            b3 = buffer[offset++];
            b2 = buffer[offset++];
            b1 = buffer[offset++];
        }
        public override void Read8Byte()
        {
            b8 = buffer[offset++];
            b7 = buffer[offset++];
            b6 = buffer[offset++];
            b5 = buffer[offset++];
            b4 = buffer[offset++];
            b3 = buffer[offset++];
            b2 = buffer[offset++];
            b1 = buffer[offset++];
        }

        public override void Write2Byte()
        {
            buffer[offset++] = b2;
            buffer[offset++] = b1;
        }
        public override void Write4Byte()
        {
            buffer[offset++] = b4;
            buffer[offset++] = b3;
            buffer[offset++] = b2;
            buffer[offset++] = b1;
        }
        public override void Write8Byte()
        {
            buffer[offset++] = b8;
            buffer[offset++] = b7;
            buffer[offset++] = b6;
            buffer[offset++] = b5;
            buffer[offset++] = b4;
            buffer[offset++] = b3;
            buffer[offset++] = b2;
            buffer[offset++] = b1; 
        }
    }

}
