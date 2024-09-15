using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWLS.Server.Base;
using OpenWLS.Server.LogDataFile;
using OpenWLS.Server.LogDataFile.Models;

namespace OpenWLS.Server.LogDataFile
{
    public class SampleBuffer
    {
    //    protected MeasurementValue.ConvertValueObjectToDouble convertValueObjectToDouble;
    //    protected MeasurementValue.ConvertDoubleToValueObject convertDoubleToValueObject;

        protected int total_samples;
        public int TotalSamples { get { return total_samples; } }
        protected int sample_elements;
        public int SampleElements { get { return sample_elements; } }
        protected int sample_bytes;
        public int SampleBytes { get { return sample_bytes; } }
        protected int element_bytes;
        public int ElementBytes { get { return element_bytes; } }
        protected int buffer_bytes;
        public int BufferBytes { get { return buffer_bytes; } }
        protected int total_elements;
        public int TotalElements { get { return total_elements; } }
        protected byte[] buffer;
        public byte[] Bytes { get { return buffer; } set { buffer = value;  } }

        public static Array CreateArray(SparkPlugDataType dt, int size)
        {
            switch (dt)
            {
                case SparkPlugDataType.UInt8:
                    return new byte[size];
                case SparkPlugDataType.Int8:
                    return new sbyte[size];
                case SparkPlugDataType.Int16:
                    return new short[size];
                case SparkPlugDataType.UInt16:
                    return new ushort[size];
                case SparkPlugDataType.Int32:
                    return new int[size];
                case SparkPlugDataType.UInt32:
                    return new uint[size];
                case SparkPlugDataType.Float:
                    return new float[size];
                case SparkPlugDataType.Double:
                    return new double[size];
                case SparkPlugDataType.Int64:
                    return new long[size];
                case SparkPlugDataType.UInt64:
                    return new ulong[size];
            }
            return null;
        }
        public SampleBuffer() { }

        public SampleBuffer(MHead h, int samples)
        {
            Init(h, samples);
        }

        public SampleBuffer(SampleBuffer sb)
        {
            Init(sb);
        }
        protected void Init(SampleBuffer  sb)
        {
            total_samples = sb.total_samples;
            sample_elements = sb.sample_elements;

            sample_bytes = sb.sample_bytes;
            element_bytes = sb.element_bytes;
            buffer_bytes = sb.buffer_bytes;
            total_elements = sb.total_elements;

            buffer = sb.buffer;
         //   convertDoubleToValueObject = sb.convertDoubleToValueObject;
         //   convertValueObjectToDouble = sb.convertValueObjectToDouble;            
        }
        void Init(MHead h)
        {
            sample_elements = h.SampleElements;
            element_bytes = Base.DataType.GetDataSize(h.DType);
            sample_bytes = sample_elements * element_bytes;

            //    convertDoubleToValueObject = MeasurementValue.GetDoubleToValueObjectConverter(m.Head.DType);
            //    convertValueObjectToDouble = MeasurementValue.GetValueObjectToDoubleConverter(m.Head.DType);
        }
        void Init(MHead h, int samples)
        {
            Init(h);
            total_samples = samples;
            total_elements = sample_elements * total_samples;
            buffer_bytes = total_samples * sample_bytes;
            if(buffer == null && buffer_bytes > 0) 
                buffer = new byte[buffer_bytes];
        //    convertDoubleToValueObject = MeasurementValue.GetDoubleToValueObjectConverter(m.Head.DType);
        //    convertValueObjectToDouble = MeasurementValue.GetValueObjectToDoubleConverter(m.Head.DType);
        }
        public SampleBuffer(MHead h, byte[] bs)
        {
            Init(h);
            buffer = bs;
            buffer_bytes = bs.Length;
            total_samples = buffer_bytes / sample_bytes;
            total_elements = buffer_bytes / element_bytes;
        }

        public void ReverseSamples()
        {
            buffer = ReverseSamples(buffer_bytes);
        }

        public byte[]  ReverseSamples(int total_bytes)
        {
            byte[] new_buf = new byte[total_bytes];
            byte[] bs = new byte[sample_bytes];
            int top = 0;
            int bot = total_bytes - sample_bytes;
            while(top < bot)
            {
                Buffer.BlockCopy(buffer, top, new_buf, bot, sample_bytes);
                Buffer.BlockCopy(buffer, bot, new_buf, top, sample_bytes);                   
                top += sample_bytes;
                bot -= sample_bytes;
            }
            return new_buf;
        }
        
    }

}
