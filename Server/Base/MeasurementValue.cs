using OpenWLS.Server.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.Base
{
    public interface IMeasurementValueConsumer
    {
        void ProcessMValue(MeasurementValue m);
    }
    public class MeasurementValue
    {
        public delegate object ConvertDoubleToValueObject(double val);
        public delegate double ConvertValueObjectToDouble(object val);
        ConvertValueObjectToDouble convertValueObjectToDouble;
        ConvertDoubleToValueObject convertDoubleToValueObject;
        
//        double indexStamp;
        object val;
        public int Id { get; set; }
        SparkPlugDataType dataType;
        public SparkPlugDataType DataType {
            get { return dataType;  }
            set { 
                dataType = value;
                convertDoubleToValueObject = GetDoubleToValueObjectConverter(dataType);
                convertValueObjectToDouble = GetValueObjectToDoubleConverter(dataType);
            }
        } 
        public object EmptyVal { get; set; }
 //       public double IndexStamp { get { return indexStamp; } }
        public object Val { get { return val; } }
        public double DoubleVal
        {
            get { return convertValueObjectToDouble(val); }
            set { val = convertDoubleToValueObject(value);  }
        }
        public static ConvertValueObjectToDouble GetValueObjectToDoubleConverter(SparkPlugDataType dt)
        {
            switch (dt)
            {
                case SparkPlugDataType.UInt8:
                    return new ConvertValueObjectToDouble((val) => { return (double)(byte)val; });
                case SparkPlugDataType.Int8:
                    return new ConvertValueObjectToDouble((val) => { return (double)(sbyte)val; });
                case SparkPlugDataType.Int16:
                    return new ConvertValueObjectToDouble((val) => { return (double)(short)val; });
                case SparkPlugDataType.UInt16:
                    return new ConvertValueObjectToDouble((val) => { return (double)(ushort)val; });
                case SparkPlugDataType.Int32:
                    return new ConvertValueObjectToDouble((val) => { return (double)(int)val; });
                case SparkPlugDataType.UInt32:
                    return new ConvertValueObjectToDouble((val) => { return (double)(uint)val; });
                case SparkPlugDataType.Float:
                    return new ConvertValueObjectToDouble((val) => { return (double)(float)val; });
                case SparkPlugDataType.Double:
                    return new ConvertValueObjectToDouble((val) => { return (double)val; });
                case SparkPlugDataType.Int64:
                    return new ConvertValueObjectToDouble((val) => { return (double)(long)val; });
                case SparkPlugDataType.UInt64:
                    return new ConvertValueObjectToDouble((val) => { return (double)(ulong)val; });
            }
            return null;
        }
        public static ConvertDoubleToValueObject GetDoubleToValueObjectConverter(SparkPlugDataType dt)
        {
            switch (dt)
            {
                case SparkPlugDataType.UInt8:
                    return new ConvertDoubleToValueObject((val) => { return (byte)val; });
                case SparkPlugDataType.Int8:
                    return new ConvertDoubleToValueObject((val) => { return (sbyte)val; });
                case SparkPlugDataType.Int16:
                    return new ConvertDoubleToValueObject((val) => { return (short)val; });
                case SparkPlugDataType.UInt16:
                    return new ConvertDoubleToValueObject((val) => { return (ushort)val; });
                case SparkPlugDataType.Int32:
                    return new ConvertDoubleToValueObject((val) => { return (int)val; });
                case SparkPlugDataType.UInt32:
                    return new ConvertDoubleToValueObject((val) => { return (uint)val; });
                case SparkPlugDataType.Float:
                    return new ConvertDoubleToValueObject((val) => { return (float)val; });
                case SparkPlugDataType.Double:
                    return new ConvertDoubleToValueObject((val) => { return val; });
                case SparkPlugDataType.Int64:
                    return new ConvertDoubleToValueObject((val) => { return (long)val; });
                case SparkPlugDataType.UInt64:
                    return new ConvertDoubleToValueObject((val) => { return (ulong)val; });
            }
            return null;
        }

        public MeasurementValue(double index, object val)
        {
          //  indexStamp = index;
            this.val = val;
        }



    }
}
