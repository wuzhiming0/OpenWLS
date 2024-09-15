using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Specialized;
using System.Text;
using static OpenWLS.Server.Base.DataReader;

namespace OpenWLS.Server.Base
{
  
    /// <summary>
    /// Read the Data from log data File and store the data to buffer
    /// Read the data out of the buffer, re-arrange the byte order according the system code, decode the data to different type data  
    /// </summary>
    /// 

    public class DataReader
	{
        public delegate object ReadObject();
        public delegate double ReadObjectDouble();

		#region Fields of the buffer

		/// <summary>
		/// 
		/// </summary>
		bool even;


        int	reader_size;
		int reader_begin;
		byte[]		buffer;
		ByteUnion bu;
        public static byte sep;
		#endregion

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
			{	return (bu.Offset >= reader_size);	}
		}


		public int Length
		{
			get
			{
				return reader_size;
			}
		}
        #endregion

        #region Constuctors, buffer managers

        public DataReader(Stream fs, int iSize, bool normalOrder)
        {
			reader_size = iSize;
            this.even = true;
            buffer = new byte[iSize];
            for (int i = 0; i < iSize; i++)
                buffer[i] = 0;
            fs.Read(buffer, 0, (int)iSize);
			bu = normalOrder? new ByteUnion(buffer) : new ByteUnionR(buffer);
        }

        public DataReader(Stream fs,  int iSize)
		{

            reader_size = iSize;
            this.even = true;
			buffer = new byte[iSize];
			for (int i = 0; i < iSize; i++)
				buffer[i] = 0;
			fs.Read(buffer, 0, (int)iSize);
			bu = new ByteUnion(buffer);
        }
		/// <summary>
		///  Init the buffer only
		/// </summary>
		/// <param name="littleEndian"></param>normal byte order?
		/// <param name="iSize"></param>size of the buffer
		public DataReader(int iSize)
		{
			this.even = true;
            reader_size = iSize;
            buffer = new byte[iSize];
			bu = new ByteUnion(buffer);
        }

		public DataReader( byte[] buf, int iOffset, int iSize)
		{
            reader_size = iSize;
            this.even = true;
			buffer = buf;
			bu = new ByteUnion(buffer);
            reader_begin = iOffset;
            bu.Offset = iOffset;
            //	int s = iOffset;
            //	Buffer.BlockCopy(buf, iOffset, buffer, 0, iSize);
        }

		public DataReader(byte[] buf)
		{ 
			reader_size = buf.Length;
            this.even = true;
			buffer = buf;
            bu = new ByteUnion(buffer);
        }

		public void SetByteOrder(bool littleEndian)
		{
            bu = littleEndian ? new ByteUnion(buffer) : new ByteUnionR(buffer); ;
		}
		public bool GetByteOrder()
		{
			return !(bu is ByteUnionR);
		}

		/// <summary>
		///  Read all of the data from the file to buffer 
		/// </summary>
		/// <param name="fs"></param>
		public void ReadDataFile(FileStream fs)
		{
			fs.Read(buffer, 0, reader_size);
			bu.Offset = 0;
		}

		/// <summary>
		///  Read struct data 
		/// </summary>
		/// <param name="fs"></param>
		public void ReadDataFile(FileStream fs, int structSize, int sampleSize)
		{
			int i = 0;
			int l = reader_size - sampleSize;
			int seek = structSize - sampleSize;
			while(i <= l)
			{
				fs.Read(buffer, i, sampleSize);
				fs.Seek(seek, SeekOrigin.Current);
				i += sampleSize;
			}
			bu.Offset = 0;
		}

		/// <summary>
		/// Reading size of iLength data from the file to buffer 
		/// for read the conventional data out of nD or struct data
		/// </summary>
		/// <param name="fs"></param>xtf file
		/// <param name="littleEndian"></param>byte order
		/// <param name="iBufferSize"></param>buffer size
		/// <param name="iDataSize"></param>data size
		/// <param name="iBlockSize"></param>size of the nD or struct data
		public DataReader(FileStream fs, bool littleEndian, int iBufferSize, int iDataSize, int iBlockSize)
		{
			int offset = 0;
			buffer = new byte[iBufferSize];
			int seekStep = iBlockSize - iDataSize;
			while(offset < iBufferSize)
			{
				fs.Read(buffer, offset, iDataSize);
				offset += iDataSize;
				fs.Seek(seekStep, SeekOrigin.Current);
			}
            bu = littleEndian ? new ByteUnion(buffer) : new ByteUnionR(buffer);
        }

		/// <summary>
		/// Set the buffer read/write pointer
		/// </summary>
		/// <param name="offset"></param>size to move
		/// <param name="origin"></param>referance to move
		public void Seek(int offset, SeekOrigin origin)
		{
			switch(origin)
			{
				case SeekOrigin.Begin:
					bu.Offset = reader_begin + offset;
					break;
				case SeekOrigin.Current:
					bu.Offset +=  offset;
					break;
				case SeekOrigin.End:
					bu.Offset = reader_begin + reader_size - offset;
					break;
			}
		}
		public byte[] GetBuffer()
		{
			return buffer;
		}
		#endregion



		#region functions to read from the buffer
		/// <summary>
		/// Read one byte from the buffer
		/// </summary>
		/// <returns></returns>byte
		public byte ReadByte()
		{
			bu.Read1Byte();
			return bu.b1;
		}
		/// <summary>
		/// read one signed byte from the buffer
		/// </summary>
		/// <returns></returns>sbyte
		public sbyte ReadSByte()
		{
			bu.Read1Byte();
			return bu.sb;
		}
		/// <summary>
		/// read one short from the buffer
		/// </summary>
		/// <returns></returns>short
		public short ReadInt16()
		{
            bu.Read2Byte();
			return bu.s;
		}
		public int[] ReadInt16ArrayToInt32s(int count)
		{
			int[] ibuf = new int[count];
			for(int i = 0 ; i  < count; i++)
			{
				bu.Read2Byte();
				ibuf[i] =  bu.s;
			}
			return ibuf;
		}

		public short[] ReadInt16Array(int count)
		{
			short[] sbuf = new short[count];
			for(int i = 0 ; i  < count; i++)
			{
				bu.Read2Byte();
				sbuf[i] =  bu.s;
			}
			return sbuf;
		}
		/// <summary>
		/// read one ushort from the buffer
		/// </summary>
		/// <returns></returns>ushort
		public ushort ReadUInt16()
		{
			bu.Read2Byte();
			return bu.us;
		}
		/// <summary>
		/// read one int from the buffer
		/// </summary>
		/// <returns></returns>int
		public int ReadInt32()
		{
            bu.Read4Byte();
			return bu.i;
		}
		/// <summary>
		/// read one uint from the buffer
		/// </summary>
		/// <returns></returns>uint
		public uint ReadUInt32()
		{
			bu.Read4Byte();
			return bu.ui;
		}
		/// <summary>
		/// read one single/float from the buffer
		/// </summary>
		/// <returns></returns>single
		public float ReadSingle()
		{
			bu.Read4Byte();
			return bu.f;
		}
		/// <summary>
		/// read a double from the buffer
		/// </summary>
		/// <returns></returns>double
		public double ReadDouble()
		{
            bu.Read8Byte();
			return bu.d;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public long ReadLongInt()
		{
            bu.Read8Byte();
			return bu.l;
		}

        /// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public ulong ReadULongInt()
		{
			bu.Read8Byte();
			return bu.ul;
		}
        /// <summary>
        /// one byte of lenght, string
        /// </summary>
        /// <returns></returns>
        public string? ReadStringWithSizeByte()
        {
            byte s = ReadByte();
            if (s == 0)
                return null;
            byte[] bs = ReadByteArray(s);
            return Encoding.UTF8.GetString(bs).Trim((char)0);
        }
        /// <summary>
        /// two bytes of lenght, string
        /// </summary>
        /// <returns></returns>string
        public string? ReadStringWithSizeUint16()
        {
            int k = ReadUInt16();
            if (k <= 0)
                return null;
            byte[] bs = ReadByteArray(k);
            return Encoding.UTF8.GetString(bs);
        }
        public string? ReadStringWithSizeInt32()
        {
            int k = ReadInt32();
            if (k <= 0)
                return null;
            byte[] bs = ReadByteArray(k);
            return Encoding.UTF8.GetString(bs);
        }
		/// <summary>
		/// read iLength of bytes from the buffer and convert them to string, 
		/// </summary>
		/// <param name="iLength"></param>the bytes of characters
		/// <returns></returns>string
		public string ReadString(int iLength)
		{
            if (iLength == 0)
                return "";
            if( iLength < 0)
            {
                System.ArgumentException argEx = new System.ArgumentException("negtive length", "length");
                throw argEx;
            }
 
            byte[] bs = ReadByteArray(iLength);
            return Encoding.UTF8.GetString(bs); 
            /*
			char[] c = new char[iLength];
			for(i = 0; i < iLength; i++)
				c[i] = (char)buffer[offset++];
			return new string(c);
             */
		}

		public string ReadString1(int iLength)
		{
			if (iLength == 0)
				return "";
			if (iLength < 0)
			{
				System.ArgumentException argEx = new System.ArgumentException("negtive length", "length");
				throw argEx;
			}

			byte[] bs = ReadByteArray(iLength);
			return Encoding.UTF8.GetString(bs).Trim( (char)0);
		}


        /// <summary>
        /// read char from the buffer until 0 char and convert them to string, 
        /// </summary>
        /// <returns></returns>string
        public string ReadString()
        {
           int o = bu.Offset;
           while( o < this.Length && buffer[o] != 0 )o++; 

          int s = o - bu.Offset;

          string str =  ReadString(s);
            bu.Offset++;
            return str;
        }

        public string ReadLine()
        {
            int o = bu.Offset;
            while (o < this.Length && buffer[o] != '\n') o++;

            int s = o - bu.Offset;

            string str = ReadString(s);
            bu.Offset++;
            return str;
            /*
            int o = offset;
            while (o < this.Length && buffer[o] != '\n') o++;

            int l = o - offset;
            char[] c = new char[l];
            for (i = 0; i < l; i++)
                c[i] = (char)buffer[offset++];
            offset++;
            return new string(c);
            */
        }

        public string ReadParameter()
        {
            int o = bu.Offset;
            while (o < this.Length && buffer[o] != '\n' || buffer[o] != sep) o++;

            int l = o - bu.Offset;
            char[] c = new char[l];
            for (int i = 0; i < l; i++)
                c[i] = (char)buffer[bu.Offset++];
            bu.Offset++;
            return new string(c);

        }

        /// <summary>
        /// read iLength of bytes from the buffer, remove all space(s) and null(s), and convert them to string  
        /// </summary>
        /// <param name="iLength"></param>the bytes of characters
        /// <returns></returns>string
        public string ReadStringAndTrim(int iLength)
		{
            string str = ReadString(iLength);
            str = str.TrimEnd('\0');
            return str.Trim();
            /*
			char[] c = new char[iLength];
			int l = 0;
			for(i = 0; i < iLength; i++)
			{
				c[l] = (char)buffer[offset++];
				if(c[i] != ' ')
					l++;
			}
			string str = new string(c);
			l = str.IndexOf((char)0);
			if(l >= 0)
				str = str.Remove(l, iLength - l);
			return str;*/
		}

		/// <summary>
		/// Read iLength of bytes from the buffer and split them to strings
		/// </summary>
		/// <param name="Seperator"></param>sepperator between the strings
		/// <param name="iLength"></param>the bytes of characters
		/// <returns></returns>string[]
		public string[] ReadStrings(char sep, int iLength)
		{
            string str = ReadString(iLength);
            return str.Split(new char[] { sep });
            /*
			char[] c = new char[iLength];
			for(int i = 0; i < iLength; i++)
				c[i] = (char)buffer[i];
			string str = new string(c);
			char[] sep = new char[1];
			sep[0] = (char)Seperator;
			string[] strings1 = str.Split(sep, iLength / 2 );
			int k = 0;
			for(int i = 0; i < strings1.Length; i++)
				if(strings1[i].Length > 0 )
					k++;
			string [] strings2 = new string[k];
			for(int i = 0; i < k; i++)
				strings2[i] = strings1[i];
			return strings2;
             */ 
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object ReadDataObject(XtfDataType type)
        {
            return ReadDataObject(type, DataType.GetDataSize(type));
        }
 
		/// <summary>
		/// read certain type of data from the buffer
		/// iSize should = the size of the data type except string( Character for XTF )
		/// it's the bytes to read if the data type is Character
		/// </summary>
		/// <param name="type"></param>data type
		/// <param name="iSize"></param>size to read
		/// <returns></returns>object
		public object ReadDataObject(SparkPlugDataType type)
		{

			switch(type)
			{
				case SparkPlugDataType.UInt8:
					return this.ReadByte();
				case SparkPlugDataType.Int8:
                    return this.ReadSByte();
				case SparkPlugDataType.Double:
					return this.ReadDouble();
				case SparkPlugDataType.Int16:
					return this.ReadInt16();
				case SparkPlugDataType.Int32:
					return this.ReadInt32();
				case SparkPlugDataType.Float:
					return this.ReadSingle();
				case SparkPlugDataType.UInt16:
					return this.ReadUInt16();
				case SparkPlugDataType.UInt32:
					return this.ReadUInt32();
			}
			return null;
		}

        /// <summary>
		/// read certain type of data from the buffer
		/// iSize should = the size of the data type except string( Character for XTF )
		/// it's the bytes to read if the data type is Character
		/// </summary>
		/// <param name="type"></param>data type
		/// <param name="iSize"></param>size to read
		/// <returns></returns>object
		public object ReadDataObject(XtfDataType type, int iSize)
        {
            if (DataType.GetDataSize(type) != iSize && type != XtfDataType.Character)
                return null;
            switch (type)
            {
                case XtfDataType.Byte:
                    return this.ReadByte();
                case XtfDataType.Character:
                    return this.ReadStringAndTrim(iSize);
                case XtfDataType.Double:
                    return this.ReadDouble();
                case XtfDataType.Int12:
                    break;
                case XtfDataType.UInt12:
                    break;
                case XtfDataType.Int2:
                    return this.ReadInt16();
                case XtfDataType.Int4:
                    return this.ReadInt32();
                case XtfDataType.Single:
                    return this.ReadSingle();
                case XtfDataType.UInt2:
                    return this.ReadUInt16();
                case XtfDataType.UInt4:
                    return this.ReadUInt32();
            }
            return null;
        }

        public object ReadDataObject(DlisDataType type)
		{
			int i, n;
			byte b;
			switch(type)
			{
				case DlisDataType.ASCII:
					n = (int)ReadDataObject(DlisDataType.UVARI);
					return ReadString(n);
				case DlisDataType.BINARY:
					n = (int)ReadDataObject(DlisDataType.UVARI);
					byte[] bytes = new byte[n+1];
					bytes[0] = buffer[bu.Offset++];
					for( i = 0; i < n; i++)
						bytes[i+1] = buffer[bu.Offset++];
					return bytes;
				case DlisDataType.IDENT:
				case DlisDataType.UNITS:
					n = (int)buffer[bu.Offset++];
					return ReadString(n);

				case DlisDataType.LOGICL:
					return (DlisLogicDataType)buffer[bu.Offset++];

				case DlisDataType.STATUS:
					b = buffer[bu.Offset++];
					if(b == 0)
						return false;
					else
						return true;
				case DlisDataType.USHORT:
					return buffer[bu.Offset++];

				case DlisDataType.UNORM:
					return ReadUInt16();
				case DlisDataType.SNORM:
					return ReadInt16();

				case DlisDataType.ULONG:
					return ReadUInt32();
				case DlisDataType.SLONG:
					return ReadInt32();

				case DlisDataType.FSINGL:
					return ReadSingle();
				case DlisDataType.FDOUBL:
					return ReadDouble();

				case DlisDataType.UVARI:
				case DlisDataType.ORIGIN:
					if( (buffer[bu.Offset] & 0x80) == 0)
						return (int)buffer[bu.Offset++];
					else
					{
						buffer[bu.Offset] = (byte)( buffer[bu.Offset] - 0x80);
						if( ( buffer[bu.Offset] & 0x40) == 0 )
							return (int)ReadInt16();
						else
						{
							buffer[bu.Offset] = (byte)( buffer[bu.Offset] - 0x40 );
							return (int)ReadInt32();
						}
					}
				case DlisDataType.DTIME:
					return ReadByteArray(8, false);
				case DlisDataType.OBNAME:
					DLISObjectName on = new DLISObjectName();
					on.origin = (int)this.ReadDataObject(DlisDataType.ORIGIN);
					on.copy = (int)this.ReadByte();
					on.Indenifier = (string)this.ReadDataObject(DlisDataType.IDENT);
					return on;
				case DlisDataType.OBJREF:
					DLISObjectRef or = new DLISObjectRef();
					or.type = (string)this.ReadDataObject(DlisDataType.IDENT);
					or.objectName = (DLISObjectName)ReadDataObject(DlisDataType.OBNAME);
					return or;
				
			}
			return null;
		
		}

		public void EvenBoundary()
		{
			if(!even)
			{
				this.bu.Offset++;
				this.even = true;
			}
		}

		public int[] Read12BitShortData(int count)
		{
			int a;
			int[] dat = new int[count];
			for(int i = 0; i < count; i++)
			{
				a = (int)this.ReadUInt16();
				if(even)
				{
					a = a >> 4;
					this.bu.Offset--;
				}
				else
					a = a & 0xfff;
				if((a&0x800) == 0)
					dat[i] = a;
				else
					dat[i] = ( a & 0x7ff) - 0x800;
				even = !even;
			}
			return dat;
		}

		public int[] Read12BitUshortData(int count)
		{
			int a;
			int[] dat = new int[count];
			for(int i = 0; i < count; i++)
			{
				a = (int)this.ReadUInt16();
				if(even)
				{
					dat[i] = a >> 4;
					bu.Offset--;
				}
				else
					dat[i] = a & 0xfff;
				even = !even;
			}
			return dat;
		}
	
		public sbyte[] ReadSByteArray(int count)
		{
			return ReadSByteArray(count, false);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="count"></param>
		/// <param name="convert"></param>
		/// <returns></returns>
		public sbyte[] ReadSByteArray(int count, bool convert)
		{
			sbyte[] b = new sbyte[count];
			if(GetByteOrder() || (!convert))
			{
				for(int i = 0; i < count; i++)
					b[i] = this.ReadSByte();
			}
			else
			{
				bool even = true;
				for(int i = 0; i < count; i++)
				{
					bu.b1 = even ? buffer[this.bu.Offset + 1] :buffer[this.bu.Offset - 1];
					b[i] = bu.sb;
					bu.Offset++;
					even = !even;
				}
			}
			return b;
		}

		public byte[] ReadByteArray(int count)
		{
			return ReadByteArray(count, false);
		}

        public byte[] ReadByteArrayToEnd()
        {
			int count = reader_size-Position;
            return ReadByteArray(count);
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="count"></param>
		/// <param name="convert"></param>
		/// <returns></returns>
        public byte[] ReadByteArray(int count,  bool convert)
		{
			if (count < 0)
				count = 0;
			byte[] b = new byte[count];
			if( GetByteOrder() || (!convert))
			{
				Buffer.BlockCopy(buffer, bu.Offset, b, 0, count);
                bu.Offset += count;
			}
			else
			{
				bool even = true;
				for(int i = 0; i < count; i++)
				{
					b[i] = even ? buffer[bu.Offset + 1] :buffer[bu.Offset - 1];
                    bu.Offset++;
					even = !even;
				}
			}
			return b;
		}

		public double Read16BitFloatData()
		{
			int i = this.ReadUInt16();
			int g =  ( i & 0x7800 ) >> 11;
			g -= 10;
			double d = ( i & 0x7ff + 0x800 ) * Math.Pow(2, g);
			if((i & 0x8000) != 0)
				d = -d;
			return d;
		}
        #endregion

        #region get object

        public object  ReadByteObject()
		{
			return ReadByte();
		}

        public object ReadSByteObject()
		{
            return ReadSByte();
		}

        public object ReadInt16Object()
		{
			return ReadInt16();
		}

        public object ReadUInt16Object()
		{
            return ReadUInt16();
		}

        public object ReadInt32Object()
		{
			return ReadInt32();
		}

        public object ReadUInt32Object()
		{
			return ReadUInt32();
		}

        public object ReadSingleObject()
		{
			return ReadSingle();
		}
        public object ReadDoubleObject()
		{
			return ReadDouble();
		}

        public object ReadLongIntObject()
		{
			return ReadLongInt();
		}

        public object ReadULongIntObject()
		{
            return ReadULongInt();
		}

        #endregion

        #region get double

        public double ReadByteDouble()
        {
            return ReadByte();
        }

        public double ReadSByteDouble()
        {
            return ReadSByte();
        }

        public double ReadInt16Double()
        {
            return ReadInt16();
        }

        public double ReadUInt16Double()
        {
            return ReadUInt16();
        }

        public double ReadInt32Double()
        {
            return ReadInt32();
        }

        public double ReadUInt32Double()
        {
            return ReadUInt32();
        }

        public double ReadSingleDouble()
        {
            return ReadSingle();
        }
        public double ReadDoubleDouble()
        {
            return ReadDouble();
        }

        public double ReadLongIntDouble()
        {
            return ReadLongInt();
        }

        public double ReadULongIntDouble()
        {
            return ReadULongInt();
        }

        #endregion


        public static long ReadLong(FileStream fs)
        {
            long data = 0;
            int k = 0;
            for (int i = 0; i < 8; i++)
            {
                data += fs.ReadByte() << k;
               k =+ 8;
            }
            return data;
        }

        public static int ReadInt(FileStream fs)
        {
            int data = 0;
            int k = 0;
            for (int i = 0; i < 4; i++)
            {
                data += fs.ReadByte() << k;
                k = +8;
            }
            return data;
        }

        public static short ReadShort(FileStream fs)
        {
            short data = 0;
            int k = 0;
            for (int i = 0; i < 2; i++)
            {
                data += (short)(fs.ReadByte() << k);
                k = +8;
            }
            return data;
        }

        public static string ReadString(FileStream fs, int maxSize)
        {
            byte[] bs = new byte[maxSize];

            try
            {
                int k = 0;
                byte b= (byte)fs.ReadByte();
                while (b != 0 && k < maxSize)
                {
                    bs[k++] = b;
                    b = (byte)fs.ReadByte();
                }
                 byte[] bs1 = new byte[k];
                 Buffer.BlockCopy(bs, 0, bs1, 0, k);
                 return StringConverter.ToString(bs);
            }
            catch (Exception c1)
            {
                return null;
            }


            /*
            string str = "";
            try
            {
                char c = (char)fs.ReadByte();
                while (c != 0)
                {
                    str = str + new string(c, 1);
                    c = (char)fs.ReadByte();
                }
            }
            catch (Exception c1)
            {
            }
            return str;
             */ 
        }
        public  int[] ReadDimension(int s)
        {
            int[] a = new int[s];
            int k = 0;
            for (int i = 0; i < s; i++)
            {
                a[i] = ReadInt32();
                if (a[i] >= 0)
                    k++;
            }
            if(k == 0)
                return new int[]{1};
            int[] b = new int[k];
            for (int i = 0; i < k; i++)
                b[i] = a[i];

            return b;
        }

        public  static ReadObject GetObjectReader(DataReader r, SparkPlugDataType dt)
        {
            switch (dt)
            {

                case SparkPlugDataType.UInt8:
                case SparkPlugDataType.Boolean:
                    return r.ReadByteObject;
                case SparkPlugDataType.Int32:
                    return r.ReadInt32Object;
                case SparkPlugDataType.Int8:
                    return r.ReadSByteObject;
                case SparkPlugDataType.Int16:
                    return r.ReadInt16Object;
                case SparkPlugDataType.UInt16:
                    return r.ReadUInt16Object;
                case SparkPlugDataType.UInt32:
         //       case iLogDataType.Date:
         //       case iLogDataType.Time:
         //           return r.ReadUInt32Object;
                 case SparkPlugDataType.Float:
                    return r.ReadSingleObject;
                case SparkPlugDataType.Double:
                    return r.ReadDoubleObject;
                case SparkPlugDataType.String:
      //          case iLogDataType.Enum:
      //              return r.ReadStringWithSize;
                default:
                    return null;
            }

        }
        public static ReadObjectDouble GetDoubleReader(DataReader r, SparkPlugDataType dt)
        {
            switch (dt)
            {

                case SparkPlugDataType.UInt8:
                case SparkPlugDataType.Boolean:
                    return r.ReadByteDouble;
                case SparkPlugDataType.Int32:
                    return r.ReadInt32Double;
                case SparkPlugDataType.Int8:
                    return r.ReadSByteDouble;
                case SparkPlugDataType.Int16:
                    return r.ReadInt16Double;
                case SparkPlugDataType.UInt16:
                    return r.ReadUInt16Double;
                case SparkPlugDataType.UInt32:
                //       case iLogDataType.Date:
                //       case iLogDataType.Time:
                //           return r.ReadUInt32Object;
                case SparkPlugDataType.Float:
                    return r.ReadSingleDouble;
                case SparkPlugDataType.Double:
                    return r.ReadDoubleDouble;
                case SparkPlugDataType.String:
                //          case iLogDataType.Enum:
                //              return r.ReadStringWithSize;
                default:
                    return null;
            }

        }
    }


}