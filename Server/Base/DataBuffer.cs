using System;
using System.IO;
namespace OpenWLS.Server.Base
{
	public interface IDataBuffer
	{
		void SaveItem(int index, BinaryWriter writer);
		object this[int index]
		{
			get;
			set;
		}
		void SetDefaultValue(int index);
		void ResetBuffer();
		int GetItemSize();
		object GetBuffer();

	}

	#region classes derived from IDataBuffer
	/// <summary>
	/// Summary description for DataBuffer.
	/// </summary>
	public class ByteDataBuffer : IDataBuffer
	{
		byte[] buffer;
		int itemSize;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="count"></param>samples in buffer
		/// <param name="nuPerSample"></param>
		public ByteDataBuffer(int count, int nuPerSample)
		{
			buffer = new byte[count * nuPerSample];
			itemSize = nuPerSample;
		}

		public object GetBuffer()
		{
			return buffer;
		}

		public void ResetBuffer()
		{
			int l = buffer.Length;
			for(int i = 0; i < l; i++)
				buffer[i] = 0;
		}
		public void SetDefaultValue(int index)
		{
			if(itemSize == 1)
				this[index] =0;
			else
			{
				for(int i = 0; i < itemSize; i++)
					buffer[i] = 0;
			}
		}
		public int GetItemSize()
		{
			return this.itemSize;
		}



		public  object this[int index]
		{
			get
			{
				if(itemSize == 1)
					return buffer[index];
				else
				{
					byte[] outBuf = new byte[itemSize];;
					int offset  = index * itemSize;
                    Buffer.BlockCopy(buffer, offset, outBuf, 0, itemSize);
					return outBuf;
				}
			}
			set
			{
				if(itemSize == 1)
					buffer[index] = (byte)value;
				else
				{
                    int offset = index * itemSize;
					byte[] outBuf = (byte[])value;
                    Buffer.BlockCopy(outBuf, 0, buffer, offset, itemSize);
				}
			}

		}

		public void SaveItem(int index, BinaryWriter writer)
		{
			int s  = index * itemSize;
//			int end = itemSize + s;
            writer.Write(buffer, s, itemSize);
//			for(int i = s; i < end; i++)
//				writer.Write([i]);
		}
	}

	public class ShortDataBuffer : IDataBuffer
	{
		short[] buffer;
		int itemSize;
		public ShortDataBuffer(int count, int nuPerSample)
		{
			itemSize = nuPerSample;
			buffer = new short[count * nuPerSample];
		}
		public int GetItemSize()
		{
			return this.itemSize + itemSize;
		}
		public object GetBuffer()
		{
			return buffer;
		}
		public void ResetBuffer()
		{
			int l = buffer.Length;
			for(int i = 0; i < l; i++)
				buffer[i] = 0;
		}
		public void SetDefaultValue(int index)
		{
			if(itemSize == 1)
				this[index] = (short)0;
			else
			{
				for(int i = 0; i < itemSize; i++)
					buffer[i] =(short)0;
			}

		}

        public object this[int index]
        {
            get
            {
                if (itemSize == 1)
                    return buffer[index];
                else
                {
                    short[] outBuf = new short[itemSize]; ;
                    int offset = index * itemSize;
                    Buffer.BlockCopy(buffer, offset, outBuf, 0, itemSize);
                    return outBuf;
                }
            }
            set
            {
                if (itemSize == 1)
                    buffer[index] = (short)value;
                else
                {
                    int offset = index * itemSize;
                    short[] outBuf = (short[])value;
                    Buffer.BlockCopy(outBuf, 0, buffer, offset, itemSize);
                }
            }
        }


		public void SaveItem(int index, BinaryWriter writer)
		{
			int s  = index * itemSize;
			int end = itemSize + s;
			for(int i = s; i < end; i++)
				writer.Write(buffer[i]);
		}	
	}
	
	public class UshortDataBuffer : IDataBuffer
	{
		ushort[] buffer;
		int itemSize;
		public UshortDataBuffer(int count, int nuPerSample)
		{
			itemSize = nuPerSample;
			buffer = new ushort[count * nuPerSample];
		}
		public int GetItemSize()
		{
			return this.itemSize + itemSize;
		}
		public object GetBuffer()
		{
			return buffer;
		}
		public void ResetBuffer()
		{
			int l = buffer.Length;
			for(int i = 0; i < l; i++)
				buffer[i] = 0;
		}
		public void SetDefaultValue(int index)
		{
			if(itemSize == 1)
				this[index] = (ushort)0;
			else
			{
				for(int i = 0; i < itemSize; i++)
					buffer[i] = (ushort)0;
			}

		}

        public object this[int index]
        {
            get
            {
                if (itemSize == 1)
                    return buffer[index];
                else
                {
                    ushort[] outBuf = new ushort[itemSize]; ;
                    int offset = index * itemSize;
                    Buffer.BlockCopy(buffer, offset, outBuf, 0, itemSize);
                    return outBuf;
                }
            }
            set
            {
                if (itemSize == 1)
                    buffer[index] = (ushort)value;
                else
                {
                    int offset = index * itemSize;
                    ushort[] outBuf = (ushort[])value;
                    Buffer.BlockCopy(outBuf, 0, buffer, offset, itemSize);
                }
            }

        }

		public void SaveItem(int index, BinaryWriter writer)
		{
			int s  = index * itemSize;
			int end = itemSize + s;
			for(int i = s; i < end; i++)
				writer.Write(buffer[i]);
		}	
	}

    public class IntDataBuffer : IDataBuffer
	{
		int[] buffer;
		int itemSize;
		public IntDataBuffer(int count, int nuPerSample)
		{
			itemSize = nuPerSample;
			buffer = new int[count * nuPerSample];
		}
		public int GetItemSize()
		{
			return 4 * this.itemSize;
		}
		public object GetBuffer()
		{
			return buffer;
		}

		public void ResetBuffer()
		{
			int l = buffer.Length;
			for(int i = 0; i < l; i++)
				buffer[i] = 0;
		}
		public void SetDefaultValue(int index)
		{
			if(itemSize == 1)
				this[index] = 0;
			else
			{
				for(int i = 0; i < itemSize; i++)
					buffer[i] = 0;
			}

		}

        public object this[int index]
        {
            get
            {
                if (itemSize == 1)
                    return buffer[index];
                else
                {
                    int[] outBuf = new int[itemSize]; ;
                    int offset = index * itemSize;
                    Buffer.BlockCopy(buffer, offset, outBuf, 0, itemSize);
                    return outBuf;
                }
            }
            set
            {
                if (itemSize == 1)
                    buffer[index] = (int)value;
                else
                {
                    int offset = index * itemSize;
                    int[] outBuf = (int[])value;
                    Buffer.BlockCopy(outBuf, 0, buffer, offset, itemSize);
                }
            }

        }

		public void SaveItem(int index, BinaryWriter writer)
		{
			int s  = index * itemSize;
			int end = itemSize + s;
			for(int i = s; i < end; i++)
				writer.Write(buffer[i]);
		}	
	}

    public class FloatDataBuffer : IDataBuffer
	{
		float[] buffer;
		int itemSize;
		public int GetItemSize()
		{
			return 4*this.itemSize;
		}

		public FloatDataBuffer(int count, int nuPerSample)
		{
			itemSize = nuPerSample;
			buffer = new float[count * nuPerSample];
		}
		public object GetBuffer()
		{
			return buffer;
		}
		public void ResetBuffer()
		{
			int l = buffer.Length;
			for(int i = 0; i < l; i++)
				buffer[i] = Single.NaN;
		}
		public void SetDefaultValue(int index)
		{
			if(itemSize == 1)
				this[index] = Single.NaN;
			else
			{
				for(int i = 0; i < itemSize; i++)
					buffer[i] = Single.NaN;
			}

		}

        public object this[int index]
        {
            get
            {
                if (itemSize == 1)
                    return buffer[index];
                else
                {
                    float[] outBuf = new float[itemSize]; ;
                    int offset = index * itemSize;
                    Buffer.BlockCopy(buffer, offset, outBuf, 0, itemSize);
                    return outBuf;
                }
            }
            set
            {
                if (itemSize == 1)
                    buffer[index] = (float)value;
                else
                {
                    int offset = index * itemSize;
                    float[] outBuf = (float[])value;
                    Buffer.BlockCopy(outBuf, 0, buffer, offset, itemSize);
                }
            }

        }

		public void SaveItem(int index, BinaryWriter writer)
		{
			int s  = index * itemSize;
			int end = itemSize + s;
			for(int i = s; i < end; i++)
				writer.Write(buffer[i]);
		}	
	}
	public class DoubleDataBuffer : IDataBuffer
	{
		double[] buffer;
		int itemSize;
		public DoubleDataBuffer(int count, int nuPerSample)
		{
			itemSize = nuPerSample;
			buffer = new double[count * nuPerSample];
		}
		public int GetItemSize()
		{
			return 8*this.itemSize;
		}
		public object GetBuffer()
		{
			return buffer;
		}
		public void ResetBuffer()
		{
			int l = buffer.Length;
			for(int i = 0; i < l; i++)
				buffer[i] = Double.NaN;
		}
		public void SetDefaultValue(int index)
		{
			if(this.itemSize == 1)
				this[index] = Double.NaN;
			else
			{
				for(int i = 0; i < itemSize; i++)
					buffer[i] = Double.NaN;
			}
		}

        public object this[int index]
        {
            get
            {
                if (itemSize == 1)
                    return buffer[index];
                else
                {
                    double[] outBuf = new double[itemSize]; ;
                    int offset = index * itemSize;
                    Buffer.BlockCopy(buffer, offset, outBuf, 0, itemSize);
                    return outBuf;
                }
            }
            set
            {
                if (itemSize == 1)
                    buffer[index] = (double)value;
                else
                {
                    int offset = index * itemSize;
                    double[] outBuf = (double[])value;
                    Buffer.BlockCopy(outBuf, 0, buffer, offset, itemSize);
                }
            }

        }

		public void SaveItem(int index, BinaryWriter writer)
		{
			int s  = index * itemSize;
			int end = itemSize + s;
			for(int i = s; i < end; i++)
				writer.Write(buffer[i]);
		}	
	}
	#endregion
}
