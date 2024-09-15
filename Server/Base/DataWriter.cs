using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace OpenWLS.Server.Base
{

	/// <summary>
	/// write the log data into its buffer
	/// wirte the data from the buffer to a filestream
	/// </summary>
	public class DataWriter
	{
        public delegate void WriteDoubleData(double d);

        int writer_size;
        int writer_begin;
        byte[] buffer;
        ByteUnion bu;

        #region Properties
        public int Position
        {
            get
            {
                return bu.Offset;
            }
        }

        /// <summary>
        ///  Is the end of the buffer?
        /// </summary>
        public bool END
        {
            get
            { return (bu.Offset >= writer_size); }
        }


        public int Length
        {
            get
            {
                return writer_size;
            }
        }
        #endregion

        /// <summary>
        /// create a writer with a new buffer
        /// </summary>
        /// <param name="littleEndian"></param>
        /// <param name="iSize"></param>
        /*	public DataWriter( bool littleEndian, int iSize)
            {
                normalOrder = littleEndian;
                buffer = new byte[iSize];
            }
            /// <summary>
            /// create a writer from a byte array
            /// </summary>
            /// <param name="littleEndian"></param>
            /// <param name="buf"></param>
            public DataWriter( bool littleEndian, byte[] buf)
            {
                buffer = buf;
                normalOrder = littleEndian;
                offset = 0;
            }
    */
        public DataWriter(uint iSize)
		{
			writer_begin = 0;
            writer_size = (int)iSize;
            buffer = new byte[writer_size];
            bu = new ByteUnion(buffer);
        }
		public DataWriter(int iSize)
		{
			writer_begin = 0;
            writer_size = iSize;
            buffer = new byte[writer_size];
            bu = new ByteUnion(buffer);
        }
		/// <summary>
		/// create a writer from a byte array
		/// </summary>
		/// <param name="littleEndian"></param>
		/// <param name="buf"></param>
		public DataWriter(byte[] buf)
		{
			writer_begin = 0;
            writer_size = buf.Length;
            buffer = buf;
            bu = new ByteUnion(buffer);
        }

        public DataWriter(byte[] buf, int offset, int size)
        {
            writer_begin = offset;
            writer_size = size;
            buffer = buf;
            bu = new ByteUnion(buffer);
            bu.Offset = writer_begin;
        }

        public void SetByteOrder( bool littleEndian)
		{
            bu = littleEndian ? new ByteUnion(buffer) : new ByteUnionR(buffer);
        }

        public byte[] GetUsedBuffer()
        {
			int s = bu.Offset - writer_begin;
            byte[] res = new byte[s];
			Buffer.BlockCopy(buffer, writer_begin, res, 0, s);
			return res;

        }

        public byte[] GetBuffer()
		{
			return buffer;
		}

		public void InitBuffer( object defaultValue)
		{
			if(defaultValue == null)
				return;
			int l = buffer.Length;
			if(defaultValue is float)
			{
				float f = (float)defaultValue;
				l -= 4;
				while(bu.Offset < l)
					this.WriteData(f);
			}

			if(defaultValue is double)
			{
				double f = (double)defaultValue;
				l -= 8;
				while(bu.Offset < l)
					this.WriteData(f);
			}

			if(defaultValue is int)
			{
				int f = (int)defaultValue;
				l -= 4;
				while(bu.Offset < l)
					this.WriteData(f);
			}

			if(defaultValue is short)
			{
				short f = (short)defaultValue;
				l -= 2;
				while(bu.Offset < l)
					this.WriteData(f);
			}
			if(defaultValue is byte)
			{
				byte f = (byte)defaultValue;
				l -= 1;
				int offset = 0;
				while( offset < l)
					buffer[offset++] = f;
			}
            bu.Offset = 0;

		}
		/// <summary>
		/// create a writer from a  double array 
		/// </summary>
		/// <param name="littleEndian"></param>
		/// <param name="buf"></param>
		public DataWriter( bool littleEndian, double[] buf)
		{
			int l = buf.Length;
			buffer = new byte[l * 8];
            bu =  littleEndian? new ByteUnion(buffer) : new ByteUnionR(buffer);

			for(int i = 0; i < l; i++)
				WriteData(buf[i]);
		}


		/// <summary>
		/// create a writer from a from a float array 
		/// </summary>
		/// <param name="littleEndian"></param>
		/// <param name="buf"></param>
		public DataWriter( bool littleEndian, float[] buf)
		{
			int l = buf.Length;
			buffer = new byte[l * 4];
            bu =  littleEndian? new ByteUnion(buffer) : new ByteUnionR(buffer);
            for (int i = 0; i < l; i++)
				WriteData(buf[i]);
		}
		/// <summary>
		/// create a writer from a from a int array 
		/// </summary>
		/// <param name="littleEndian"></param>
		/// <param name="buf"></param>
		public DataWriter( bool littleEndian, int[] buf)
		{
			int l = buf.Length;
			buffer = new byte[l * 4];
            bu =  littleEndian? new ByteUnion(buffer) : new ByteUnionR(buffer);
            for (int i = 0; i < l; i++)
				WriteData(buf[i]);
		}

		/// <summary>
		/// create a writer from a short array
		/// </summary>
		/// <param name="littleEndian"></param>
		/// <param name="buf"></param>
		public DataWriter( bool littleEndian, short[] buf)
		{
			int l = buf.Length;
			buffer = new byte[l * 2];
            bu =  littleEndian? new ByteUnion(buffer) : new ByteUnionR(buffer);
            for (int i = 0; i < l; i++)
				WriteData(buf[i]);
		}


		/// <summary>
		/// create a writer from a short array
		/// </summary>
		/// <param name="littleEndian"></param>
		/// <param name="buf"></param>
		public DataWriter( bool littleEndian, ushort[] buf)
		{
			int l = buf.Length;
			buffer = new byte[l * 2];
            bu = littleEndian ? new ByteUnion(buffer) : new ByteUnionR(buffer);
            for (int i = 0; i < l; i++)
				WriteData(buf[i]);
		}


		/// <summary>
		/// write the buffer to file stream
		/// </summary>
		/// <param name="fStream">file stream</param>
		/// <param name="fOffset">offset from the file begining</param>
		/// 
		void WriteToStream(FileStream fStream, long fOffset, int iDataSize, int iBlockSize, int iLength)
		{
			fStream.Seek(fOffset, SeekOrigin.Begin);
			int seekSize = iBlockSize - iDataSize;
			int i = 0;
			while(i < iLength)
			{
				fStream.Write(buffer, i, iDataSize);
				fStream.Seek(seekSize, SeekOrigin.Current);
				i += iDataSize;
			}
		}
		
		/// <summary>
		/// write the buffer to file stream
		/// </summary>
		/// <param name="fStream">file stream</param>
		/// <param name="iOffset">offset from the file begining</param>
		public void WriteToStreamAll(FileStream fStream, long fOffset, int iDataSize, int iBlockSize )
		{
			WriteToStream( fStream, fOffset,iDataSize, iBlockSize,  buffer.Length);
		}

		/// <summary>
		/// write the buffer to file stream
		/// </summary>
		/// <param name="fStream"></param>file stream
		/// <param name="iOffset"></param>offset from the file begining
		public void WriteToStream(FileStream fStream, long iOffset, int iDataSize, int iBlockSize )
		{
			WriteToStream( fStream, iOffset, iDataSize, iBlockSize,  bu.Offset - writer_begin);
		}
		/// <summary>
		/// write the buffer to file stream
		/// </summary>
		/// <param name="fStream"></param>file stream
		/// <param name="iOffset"></param>offset from the file begining
		public void WriteToStreamAll(FileStream fStream, long fOffset)
		{
			fStream.Seek(fOffset, SeekOrigin.Begin);
			fStream.Write(buffer, 0, buffer.Length);
		}
		/// <summary>
		/// write the buffer to file stream, no seek action if offset is negative
		/// </summary>
		/// <param name="fStream"></param>file stream
		/// <param name="iOffset"></param>offset from the file begining
		public void WriteToStream(FileStream fStream, long fOffset)
		{
			if(fOffset >= 0)
				fStream.Seek(fOffset, SeekOrigin.Begin);
			int s = bu.Offset - writer_begin;
			fStream.Write(buffer, writer_begin,  s);
		}

		

		#region functions of write data

		public void WriteData(byte data)
		{
			bu.b1 = data;
            bu.Write1Byte();
		}

		public void WriteData(byte[] data)
		{
			if(data != null)
			{
				Buffer.BlockCopy(data, 0, buffer, bu.Offset, data.Length);
                bu.Offset += data.Length;
			}
		}
        public void WriteData(int ioffset, int length,  byte[] data)
        {
            if (data != null)
            {
                int end = ioffset + length;
                if (end > data.Length)
                    return;
                Buffer.BlockCopy(data, ioffset, buffer, bu.Offset, length);
                bu.Offset += length;
            }
        }

        public void WriteDataSwitchEndian(int ioffset, int length, byte[] data, int e_bytes)
        {
            if (data != null)
            {
                int end = ioffset + length;
                if (end > data.Length)
                    return;

				int src_pos = ioffset + e_bytes - 1;
				while(src_pos < end)
				{
					for(int j = 0; j < e_bytes; j++)
						buffer[bu.Offset++] = data[src_pos - j];
					src_pos += e_bytes;					
                }
               // Buffer.BlockCopy(data, ioffset, buffer, offset, length);
               // offset += length;
            }
        }

        public void WriteData(ushort[] data)
		{
			if(data != null)
			{
				for(int i = 0; i < data.Length; i++)
					WriteData(data[i]);
			}
		}		
		public void WriteData(short[] data)
		{
			if(data != null)
			{
				for(int i = 0; i < data.Length; i++)
                    WriteData(data[i]);
            }
		}		
		public void WriteData(sbyte data)
		{
            bu.sb = data;
            bu.Write1Byte();
		}
		public void WriteData(short data)
		{
            bu.s = data;
            bu.Write2Byte();
		}
		public void WriteData(ushort data)
		{
            bu.us = data;
            bu.Write2Byte();
		}
		public void WriteData(int data)
		{
            bu.i = data;
            bu.Write4Byte();
		}
		public void WriteData(uint data)
		{
            bu.ui = data;
            bu.Write4Byte();
		}	
		public void WriteData(float data)
		{
            bu.f = data;
            bu.Write4Byte();
		}		
		public void WriteData(double data)
		{
            bu.d = data;
            bu.Write8Byte();
		}	
	
		public void WriteData(long data)
		{
            bu.l = data;
            bu.Write8Byte();
		}

		public void WriteData(ulong data)
		{
            bu.ul = data;
            bu.Write8Byte();
		}

      
		public void WriteData(string data)
		{
            if (data == null)
            {
                WriteData((ushort)0); return;
            }

            byte[] bs = StringConverter.ToByteArray(data);
            WriteData((ushort)bs.Length);
            WriteData(bs);
		}

        public void WriteData(object data)
        {
            if (data is byte)
            {
                WriteData((byte)data);
                return;
            }
            if (data is sbyte)
            {
                WriteData((sbyte)data);
                return;
            }
            if (data is short)
            {
                WriteData((short)data);
                return;
            }
            if (data is ushort)
            {
                WriteData((ushort)data);
                return;
            }
            if (data is int)
            {
                WriteData((int)data);
                return;
            }
            if (data is uint)
            {
                WriteData((uint)data);
                return;
            }
            if (data is float)
            {
                WriteData((float)data);
                return;
            }
            if (data is long)
            {
                WriteData((long)data);
                return;
            }
            if (data is ulong)
            {
                WriteData((ulong)data);
                return;
            }
            if (data is double)
            {
                WriteData((double)data);
                return;
            }
            if (data is byte[])
            {
                WriteData((byte[])data);
                return;
            }
            if (data is short[])
            {
                WriteData((short[])data);
                return;
            }

            if (data is string)
            {
                WriteData((string)data);
                return;
            }   
     }

        public void WriteStringWithSizeUint8(string? data)
        {

			if(data == null)
                WriteData((byte)0);
            else
            {
      			byte[] bytes = Encoding.UTF8.GetBytes(data);       
                WriteData((byte)bytes.Length);
			    WriteData(bytes);
            }

        }
        public void WriteStringWithSizeUint16(string data)
        {
			byte[] bytes = Encoding.UTF8.GetBytes(data);
			WriteData((ushort)bytes.Length);
			WriteData(bytes);
        }

        public void WriteStringWithSizeInt32(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            WriteData(bytes.Length);
            WriteData(bytes);
        }

        public void WriteString(string data, int iLength)
        {
            WriteString(data, iLength, (byte)32);
        }

        public void WriteString(string data, int iLength, byte emptyChar)
		{
            byte[] bs;
            if (data == null)
            {
                bs = new byte[iLength];
                for (int i = 0; i < iLength; i++)
                    bs[i] = emptyChar;
            }
            else 
                bs = StringConverter.ToByteArray(data, iLength, emptyChar);
            WriteData(bs);
		}		

		public void WriteDataObject(object data, DlisDataType dataType)
		{
            if (data == null)
                return;
			switch(dataType)
			{
				case DlisDataType.ASCII:
					string str1 = (string)data;
					WriteDataObject(str1.Length, DlisDataType.UVARI);
                    WriteString(str1, str1.Length);
					break;
				case DlisDataType.BINARY:
					byte[] bytes = (byte[])data;
					WriteDataObject(bytes.Length-1, DlisDataType.UVARI);
					WriteData(bytes);
					break;
				case DlisDataType.IDENT:
				case DlisDataType.UNITS:
					string str2 = (data is string)? str2 = (string)data : data.ToString();
					WriteData((byte)str2.Length);
                    WriteString(str2, str2.Length);
					break;

				case DlisDataType.LOGICL:
					this.WriteData((byte)((DlisLogicDataType)data));
					break;

				case DlisDataType.STATUS:
					if((bool)data)
						this.WriteData((byte)1);
					else
						this.WriteData((byte)0);
					break;
				case DlisDataType.USHORT:
					this.WriteData((byte)data);
					break;

				case DlisDataType.UNORM:
					this.WriteData((ushort)data);
					break;
				case DlisDataType.SNORM:
					this.WriteData((short)data);
					break;

				case DlisDataType.ULONG:
					this.WriteData((uint)data);
					break;
				case DlisDataType.SLONG:
					WriteData((int)data);
					break;

				case DlisDataType.FSINGL:
                    if(data is float)
					    this.WriteData((float)data);
                    else
                         this.WriteData((float)(double)data);
					break;
				case DlisDataType.FDOUBL:
					this.WriteData((double)data);
					break;

				case DlisDataType.UVARI:
				case DlisDataType.ORIGIN:
					if(data is byte)
						this.WriteData((byte)data);
					else
					{
						if(data is short)
						{
							this.WriteData((short)data);
							this.buffer[bu.Offset - 2] += 0x80;
						}
						else
						{
                            if (data is int)
                            {
                                this.WriteData((int)data);
                                this.buffer[bu.Offset - 4] += 0xc0;
                            }
                            else
                            {
                                if (data is int[])
                                {
                                    this.WriteData(((int[])data)[0]);
                                    this.buffer[bu.Offset - 4] += 0xc0;
                                }
                            }
						}
					}
					break;
				case DlisDataType.DTIME:
					this.WriteData((byte[])data);
					break;
				case DlisDataType.OBNAME:
					DLISObjectName on = (DLISObjectName)data;
					this.WriteDataObject(on.origin, DlisDataType.ORIGIN);
					this.WriteData((byte)on.copy);
					this.WriteDataObject(on.Indenifier, DlisDataType.IDENT);
					break;
				case DlisDataType.OBJREF:
					DLISObjectRef or = (DLISObjectRef)data;
					this.WriteDataObject(or.type, DlisDataType.IDENT);
					this.WriteDataObject(or.objectName, DlisDataType.OBNAME);
					break;
			}
			
		}
			
		#endregion

        public void WriteDoubleToByte(double data)
        {
            WriteData((byte)data);
        }

        public void WriteDoubleToSByte(double data)
        {
            WriteData((sbyte)data);
        }

        public void WriteDoubleToInt16(double data)
        {
            WriteData((short)data);
        }

        public void WriteDoubleToUint16(double data)
        {
            WriteData((ushort)data);
        }

        public void WriteDoubleToInt32(double data)
        {
            WriteData((int)data);
        }

        public void WriteDoubleToUint32(double data)
        {
            WriteData((uint)data);
        }

        public void WriteDoubleToInt64(double data)
        {
            WriteData((long)data);
        }

        public void WriteDoubleToUint64(double data)
        {
            WriteData((ulong)data);
        }

        public void WriteDoubleToFloat(double data)
        {
            WriteData((float)data);
        }

        public void WriteDouble(double data)
        {
            WriteData(data);
        }
        public void WriteDouble(string data)
        {
            WriteData(data);
        }

		/// <summary>
		/// Set the buffer read/write pointer
		/// </summary>
		/// <param name="iOffset"></param>size to move
		/// <param name="origin"></param>referance to move
		public void Seek(int offset, SeekOrigin origin)
		{
			switch(origin)
			{
				case SeekOrigin.Begin:
                    bu.Offset = writer_begin + offset;
					break;
				case SeekOrigin.Current:
                    bu.Offset += offset;
					break;
				case SeekOrigin.End:
                    bu.Offset = writer_begin + writer_size - offset;
					break;
			}
		}

        public static void WriteData(FileStream fs, long data)
        {
            for (int i = 0; i < 8; i++)
            {
                fs.WriteByte((byte)(data & 0xff));
                data = data >> 8;
            }
        }

        public static void WriteData(FileStream fs, int data)
        {
            for (int i = 0; i < 4; i++)
            {
                fs.WriteByte((byte)(data & 0xff));
                data = data >> 8;
            }
        }

        public static void WriteData(FileStream fs, short data)
        {
            for (int i = 0; i < 3; i++)
            {
                fs.WriteByte((byte)(data & 0xff));
                data = (short)(data >> 8);
            }
        }

        public static void WriteData(FileStream fs, string data)
        {
            byte[] bs = StringConverter.ToByteArray(data);
            fs.Write(bs, 0, bs.Length);
            fs.WriteByte(0);
            /*
            if (data != null && data.Length != 0)
            {
                for (int i = 0; i < data.Length; i++)
                    fs.WriteByte((byte)data[i]);             
            }
            fs.WriteByte(0);
            return;
             * */
        }

        public  void WriteDimension(int[] ds, int s)
        {
            for (int i = 0; i < ds.Length; i++)
                WriteData(ds[i]);
            for (int i = ds.Length; i < s; i++)
                WriteData(0);
        }

        public static WriteDoubleData GetDoubleWriter(DataWriter w, SparkPlugDataType dt)
        {
            switch (dt)
            {
                case SparkPlugDataType.UInt8:
                    return w.WriteDoubleToByte;
                case SparkPlugDataType.Int32:
                    return w.WriteDoubleToInt32;
                case SparkPlugDataType.Int8:
                    return w.WriteDoubleToSByte;
                case SparkPlugDataType.Int16:
                    return w.WriteDoubleToInt16;
                case SparkPlugDataType.UInt16:
                    return w.WriteDoubleToUint16;
                case SparkPlugDataType.UInt32:
                    return w.WriteDoubleToUint32;
                case SparkPlugDataType.Float:           
                    return w.WriteDoubleToFloat;
                case SparkPlugDataType.Double:
                    return w.WriteDouble;
//                case iLogDataType.String:
//                    return w.WriteString1();
                default:
                    return null;
            }

        }

    }

}
