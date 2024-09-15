using OpenWLS.Server.Base;
using OpenWLS.Server.LogDataFile.Models;

namespace OpenWLS.Server.LogDataFile
{

    public class MVWriter :  SampleBuffer , IMeasurementValueConsumer
    {
     //   public delegate void VBufferFlushHandler(object sender, EventArgs e);
     //   public event VBufferFlushHandler FlushvBufferEvent;

        public delegate void WriteObject(object v);
        WriteObject writeObject;
        DataWriter.WriteDoubleData writeDouble;

        MeasurementValue.ConvertValueObjectToDouble convertValueObjectToDouble;

        DataWriter w;

        public int Postion{ get { return w.Position; } set { w.Seek(value, SeekOrigin.Begin); } }
        public double StartIndex{ get; set;}
        protected double index_cur;
        public double StopIndex { get{  return index_cur;  } set { index_cur = value; }   }
        double? maxVal;
        double? minVal;
        double? firstVal;
        double? lastVal;
        public double? FirstVal { get { return firstVal; } }
        public double? LastVal { get { return lastVal; } }

        public Measurement Measurement { get; set; }
        public object Tag { get; set; }    
        public static MVWriter CreateMVWriter(MHead m, byte[] buffer)
        {
            return new MVWriter(m, buffer);
        }
        public static MVWriter CreateMVWriter(MHead m, int samples)
        {
            return new MVWriter(m, samples);
        }
        public  WriteObject GetObjectWriter(SparkPlugDataType dt)
        {
            switch (dt)
            {
                case SparkPlugDataType.UInt8:
                    return new WriteObject((val) => { w.WriteData((byte)val); });
                case SparkPlugDataType.Int8:
                    return new WriteObject((val) => { w.WriteData((sbyte)val); });
                case SparkPlugDataType.Int16:
                    return new WriteObject((val) => { w.WriteData((short)val); });
                case SparkPlugDataType.UInt16:
                    return new WriteObject((val) => { w.WriteData((ushort)val); });
                case SparkPlugDataType.Int32:
                    return new WriteObject((val) => { w.WriteData((int)val); });
                case SparkPlugDataType.UInt32:
                    return new WriteObject((val) => { w.WriteData((uint)val); });
                case SparkPlugDataType.Float:
                    return new WriteObject((val) => { w.WriteData((float)val); });
                case SparkPlugDataType.Double:
                    return new WriteObject((val) => { w.WriteData((double)val); });
                case SparkPlugDataType.Int64:
                    return new WriteObject((val) => { w.WriteData((long)val); });
                case SparkPlugDataType.UInt64:
                    return new WriteObject((val) => { w.WriteData((ulong)val); });
            }
            return null;
        }

        void Init(SparkPlugDataType dt)
        {
            index_cur = double.NaN;
            w = new DataWriter(buffer);
            writeObject = GetObjectWriter(dt);
            writeDouble = DataWriter.GetDoubleWriter(w, dt);
            convertValueObjectToDouble = MeasurementValue.GetValueObjectToDoubleConverter(dt);
        }
        public MVWriter(SampleBuffer sb, SparkPlugDataType dt ) : base(sb)
        {
            Init(sb);
            Init(dt);
        }

        public MVWriter(MHead m, int samples) : base(m, samples) {
            Init(m.DType);
        }
        public MVWriter(MHead m, byte[] buffer) : base(m, buffer) {
            Init(m.DType);
        }

        public MVWriter(MHead m) 
        {
            Init(m.DType);
        }

        public virtual void ResetBuffer()
        {
            maxVal = null; 
            minVal = null;
            firstVal = null;
            lastVal = null;
            index_cur = double.NaN;
            w.Seek(0, SeekOrigin.Begin);
        }


        //return false if buffer full, return true if not 
 
        public void FlushBuffer()
        {
            if (w.Position > 0 && Measurement != null)
                Measurement.AddMVBlock();
            w.Seek(0, SeekOrigin.Begin);

        }

        public int WriteSample(double val) {
            if ( w.END ) FlushBuffer();
            writeDouble( val );
            if(firstVal == null) firstVal = val;
            lastVal = val;
            return sample_bytes;
        }

        public int WriteSample(double[] val) {
            if (w.END ) FlushBuffer();
            foreach (double d in val)
                writeDouble( d );
            return sample_bytes; 
        }

        public  int WriteSample(object val) {
            if (w.END) FlushBuffer();
            if (sample_elements == 1)
            {
                writeObject(val);
                lastVal = convertValueObjectToDouble(val);
                if (firstVal == null) firstVal = lastVal;
            }
            else
            {
                foreach (object v in (Array)val)
                    writeObject(v);
            }
            return sample_bytes;
        }

        public int WriteSample(byte[] dat, int dat_offset)
        {
            if (w.END)  FlushBuffer();
            w.WriteData(dat_offset, sample_bytes, dat);
             return sample_bytes;          
           // return dat.Length - dat_offset;
        }

        public int WriteSampleBigEndian(byte[] dat, int dat_offset)
        {
            if (w.END) FlushBuffer();
            w.WriteDataSwitchEndian(dat_offset, sample_bytes, dat, element_bytes);
            return sample_bytes;
            // return dat.Length - dat_offset;
        }

        public int WriteBuffer(byte[] dat)
        {
            if (w.END) FlushBuffer();
            w.WriteData( dat );
            return dat.Length;
        }

        public int WriteBuffer(double[] dat)
        {
            for (int i = 0; i < dat.Length; i++)
                writeDouble(dat[i]);
            return dat.Length * element_bytes;
        }

        public virtual void ProcessMValue(MeasurementValue mv )
        {
            index_cur++;
            WriteSample(mv.Val);
        }
    }
    public class MVWriterES : MVWriter
    {
        double spacing;
        public MVWriterES(MHead m, int samples) : base (m, samples)
        {

        }
         public override void ResetBuffer()
         {
            base.ResetBuffer();
         }

        /*
        public override void ProcessMValue(MeasurementValue mv )
        { 
            if (double.IsNaN(index_cur))
            {
                StartIndex = mv.IndexStamp;
                index_cur = mv.IndexStamp;
                WriteSample(mv.Val);
            }
            else
            {
                if(spacing > 0)
                {
                    while(mv.IndexStamp >= ( index_cur + spacing + spacing))
                    {
                        WriteSample(mv.EmptyVal);
                        index_cur += spacing; 
                    }
                    if( mv.IndexStamp >= ( index_cur + spacing ) )
                    {
                        WriteSample(mv.Val);
                        index_cur += spacing; 
                    }      
                } 
                else
                {
                    while(mv.IndexStamp <= ( index_cur + spacing + spacing))
                    {
                        WriteSample(mv.EmptyVal);
                        index_cur += spacing; 
                    }
                    if( mv.IndexStamp <= ( index_cur + spacing ) )
                    {
                        WriteSample(mv.Val);
                        index_cur += spacing; 
                    }   
                }
            }
        }*/
  
    }
}