using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

using System.Reflection;
//using System.Drawing;
using System.Collections;
using System.ComponentModel;

namespace OpenWLS.Server.Base;

public enum LogIndexMode { Time = 1, Up = 2, Down = 3 };

public struct DLISObjectName
{
    public int origin;
    public int copy;
    public string Indenifier;
    public override string ToString()
    {
        return Indenifier;
    }
}


public struct DLISObjectRef
{
    public string type;
    public DLISObjectName objectName;
    public override string ToString()
    {
        return type + ":" + objectName.Indenifier;
    }

}
//long need to be supported in future 
//   public enum iLogDataType { Object = 1, Byte=2, SByte=3, Int16=4, Uint16=5, Int32=6, Uint32=7, Single=8, Double=9, Int64=10, Uint64=11, 
//       String=12, Bool=13, Enum=14, Date =15,  Unknown = -1 }
public enum XtfDataType    {        Bit = 1, Int2, Byte, Single, Int12, UInt12, Character, Double, UInt2, Int4, UInt4, Unknown = -1 }

public enum DlisDataType
{
    FSHORT = 1, FSINGL, FSING1, FSING2, ISINGL, VSINGL, FDOUBL, FDOUB1, FDOUB2, CSINGL,
    CDOUBL, SSHORT, SNORM, SLONG, USHORT, UNORM, ULONG, UVARI, IDENT, ASCII,
    DTIME, ORIGIN, OBNAME, OBJREF, ATTREF, STATUS, UNITS, RNORM, RLONG, ISNORM,
    ISLONG, IUNORM, IULONG, IRNORM, IRLONG, TIDENT, TUNORM, TASCII, LOGICL, BINARY,
    FRATIO, DRATIO
};

public enum DlisLogicDataType
{
    Unknown = -1, False = 0, True = 1
}

public class DataType
{
    public static byte GetCheckSum(byte[] bs)
    {
        byte b = 0;
        for (int i = 2; i < bs.Length - 1; i++)
            b += bs[i];
        return b;
    }
    static public SparkPlugDataType GetSparkPlugDataType(XtfDataType theDataType)
    {
        switch (theDataType)
        {
            case XtfDataType.Byte:
                return SparkPlugDataType.UInt8;
            case XtfDataType.Character:
                return SparkPlugDataType.Int8;
            case XtfDataType.Double:
                return SparkPlugDataType.Double;
     //       case XtfDataType.Int12:          
      //          break;
       //     case XtfDataType.UInt12:
                //					type = System.Type.GetType("");
       //         break;
            case XtfDataType.Int2:
                return SparkPlugDataType.Int16;
            case XtfDataType.Int4:
                return SparkPlugDataType.Int32;
            case XtfDataType.Single:
                return SparkPlugDataType.Float;
            case XtfDataType.UInt2:
                return SparkPlugDataType.UInt16;
            case XtfDataType.UInt4:
                return SparkPlugDataType.UInt32;
            default:
                return SparkPlugDataType.Unknown;
        }

    }

    static public Type GetSystemDataType(XtfDataType theDataType)
    {
        Type type = null;
        switch (theDataType)
        {
            case XtfDataType.Byte:
                type = System.Type.GetType("System.Byte");
                break;
            case XtfDataType.Character:
                type = System.Type.GetType("System.String");
                break;
            case XtfDataType.Double:
                type = System.Type.GetType("System.Double");
                break;
            case XtfDataType.Int12:
                //					type = System.Type.GetType("");
                break;
            case XtfDataType.UInt12:
                //					type = System.Type.GetType("");
                break;
            case XtfDataType.Int2:
                type = System.Type.GetType("System.Int16");
                break;
            case XtfDataType.Int4:
                type = System.Type.GetType("System.Int32");
                break;
            case XtfDataType.Single:
                type = System.Type.GetType("System.Single");
                break;
            case XtfDataType.UInt2:
                type = System.Type.GetType("System.UInt16");
                break;
            case XtfDataType.UInt4:
                type = System.Type.GetType("System.UInt32");
                break;
        }
        return type;
    }



    static public SparkPlugDataType GetSparkPlugDataType(DlisDataType theDataType)
    {
        switch (theDataType)
        {
            case DlisDataType.STATUS:
                return SparkPlugDataType.Int8;
            case DlisDataType.USHORT:
                return SparkPlugDataType.UInt8;
            case DlisDataType.SNORM:
                return SparkPlugDataType.Int16;
            case DlisDataType.UNORM:
                return SparkPlugDataType.UInt16;
            case DlisDataType.SLONG:
                return SparkPlugDataType.Int32;
            case DlisDataType.ULONG:
                return SparkPlugDataType.UInt32;
            case DlisDataType.FSINGL:
                return SparkPlugDataType.Float;
            case DlisDataType.FDOUBL:
                return SparkPlugDataType.Double;
            case DlisDataType.ASCII:
                return SparkPlugDataType.String;

        }
        return SparkPlugDataType.Unknown;
    }

    static public Type GetDataType(DlisDataType theDataType)
    {
        Type type = null;
        switch (theDataType)
        {
            case DlisDataType.STATUS:
                type = System.Type.GetType("System.Bool");
                break;
            case DlisDataType.USHORT:
                type = System.Type.GetType("System.Byte");
                break;
            case DlisDataType.SNORM:
                type = System.Type.GetType("System.Int16");
                break;
            case DlisDataType.UNORM:
                type = System.Type.GetType("System.UInt16");
                break;
            case DlisDataType.SLONG:
                type = System.Type.GetType("System.Int32");
                break;
            case DlisDataType.ULONG:
                type = System.Type.GetType("System.UInt32");
                break;
            case DlisDataType.FSINGL:
                type = System.Type.GetType("System.Single");
                break;
            case DlisDataType.FDOUBL:
                type = System.Type.GetType("System.Double");
                break;
        }
        return type;
    }

    /// <summary>
    /// get data size of a xtf data type
    /// </summary>
    /// <param name="theDataType"></param>xtf data type
    /// <returns></returns>data size
    static public int GetDataSize(XtfDataType theDataType)
    {
        int i = -3;
        switch (theDataType)
        {
            case XtfDataType.Bit:
                i = -1; break;
            case XtfDataType.Byte:
            case XtfDataType.Character:
                i = 1; break;
            case XtfDataType.Int12:
            case XtfDataType.UInt12:
                i = -2; break;
            case XtfDataType.Int2:
            case XtfDataType.UInt2:
                i = 2; break;
            case XtfDataType.Int4:
            case XtfDataType.UInt4:
            case XtfDataType.Single:
                i = 4; break;
            case XtfDataType.Double:
                i = 8; break;
        }
        return i;
    }

    static public int GetDataSize(SparkPlugDataType theDataType)
    {
        switch (theDataType)
        {
          //  case SparkPlugDataType.Object:
            case SparkPlugDataType.UInt8:
            case SparkPlugDataType.Int8:
            case SparkPlugDataType.Boolean:
            return 1;
            case SparkPlugDataType.Int16:
            case SparkPlugDataType.UInt16:
                return 2;
            case SparkPlugDataType.Int32:
            case SparkPlugDataType.UInt32:
            case SparkPlugDataType.Float:
                return 4;
            case SparkPlugDataType.Int64:
            case SparkPlugDataType.UInt64:
            case SparkPlugDataType.Double:
                return 8;
            default:
                return -1;
        }

    }

    /// <summary>
		/// convert the system type to XTF data type
		/// </summary>
		/// <param name="theDataType"></param>system type
		/// <param name="xtfType"></param>xtf data type
		/// <returns></returns>true if convertable
		static public XtfDataType GetXtfDataType(Type theDataType)
    {

        string str = theDataType.FullName;
        switch (str)
        {
            case "System.Byte":
                return XtfDataType.Byte;

            case "System.String":
                return XtfDataType.Character;

            case "System.Double":
                return XtfDataType.Double;

            case "System.Int16":
                return XtfDataType.Int2;

            case "System.Int32":
                return XtfDataType.Int4;

            case "System.Single":
                return XtfDataType.Single;

            case "System.UInt16":
                return XtfDataType.UInt2;

            case "System.UInt32":
                return XtfDataType.UInt4;

            default:
                return XtfDataType.Unknown;
        }


    }
/*
    static public System.Windows.Media.Color GetColor(string hex)
    {
        hex = hex.Replace("#", string.Empty);
        byte a = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
        byte r = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
        byte g = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
        byte b = (byte)(Convert.ToUInt32(hex.Substring(6, 2), 16));
        return System.Windows.Media.Color.FromArgb(a, r, g, b);
    }

    static public  SolidColorBrush GetSolidColorBrush(string hex)
    {
        SolidColorBrush myBrush = new SolidColorBrush(GetColor(hex));
        return myBrush;
    }
*/
    static public DataTable CreateTable(string cols)
    {
        DataTable dt = new DataTable();
        using (TextReader sr = new StringReader(cols))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] strs = line.Split(new char[] { ',' });
                if (strs.Length == 2)
                {
                    DataColumn column = new DataColumn();
                    column.DataType = System.Type.GetType(strs[1]);
                    column.ColumnName = strs[0];
                    dt.Columns.Add(column);
                }
            }
        }
        return dt;
    }

    public static int CovertCOLORREFtoArgb(int colorRef)
    {
        int r = (0xff & colorRef) << 16;
        int g = (0xff0000 & colorRef) >> 16;
        int b = 0xff00 & colorRef;
        return (int)(0xff000000 | r | b | g);
    }


    public static Type GetType(string name, string assemble)
    {
        Type type = Type.GetType(name);
        if (type == null && assemble != null)
        {
            try
            {
                Assembly a = Assembly.LoadFrom(assemble);
                type = a.GetType(name);
            }
            catch (Exception e)
            {
            }
        }
        return type;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="assemble"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static object CreatObject(string name, string assembly, Type[] paraTypes, object[] parameters)
    {
        Type type = GetType(name, assembly);
        if (type == null)        return null;
        return type.GetConstructor(paraTypes).Invoke(parameters);
    }

    public static object CreatObject(string name, string assembly)
    {
        return CreatObject(name, assembly, Type.EmptyTypes, null);
    }

    public static int FromDecimalByte(int b)
    {
        b = b & 0xff;
        return (b >> 4) * 10 + (b & 0xf);
    }
    public static int ToDecimalByte(int b)
    {
        int d1 = b % 10;
        int d10 = b / 10;
        return d1 + (d10 << 4);
    }

    public static ushort GetRTCWordValue(int hb, int lb)
    {
        return (ushort)((ToDecimalByte(hb) << 8) + ToDecimalByte(lb));
    }

    public static DateTime GetDateTimeFromRTC(DataReader r)
    {
        ushort u = r.ReadUInt16();
        int year = 2000 + FromDecimalByte(u);
        u = r.ReadUInt16();
        int month = FromDecimalByte(u >> 8);
        int day = FromDecimalByte(u);
        u = r.ReadUInt16();
        int week_day = FromDecimalByte(u >> 8);
        int hour = FromDecimalByte(u);
        u = r.ReadUInt16();
        int min = FromDecimalByte(u >> 8);
        int sec = FromDecimalByte(u);

        return new DateTime(year, month, day, hour, min, sec);
    } 

    static string GeTRTCWordString(int b)
    {
        return FromDecimalByte((b >> 16)).ToString() + "," + FromDecimalByte(b);
    }

    public static byte[] GetInstRTCBytes(DateTime dt)
    {
        DataWriter w = new DataWriter( 8);
        w.WriteData((ushort)(ToDecimalByte(dt.Year - 2000)));

        w.WriteData(GetRTCWordValue(dt.Month, dt.Day));

        w.WriteData(GetRTCWordValue((int)dt.DayOfWeek, dt.Hour));

        w.WriteData(GetRTCWordValue(dt.Minute, dt.Second));

        return w.GetBuffer();
    }

    public static int GetDimensionTotalElements(int[] dims)
    {
        int k = 1;
        if (dims != null)
        {
            foreach (int i in dims)
                if (i > 0)
                    k = k * i;
        }
        return k;
    }

    /*
    static public System.Windows.Media.Color ConvertToColor(uint c)
    {
        byte a = (byte)(c >> 24);
        byte r = (byte)(c >> 16);
        byte g = (byte)(c >> 8);
        byte b = (byte)(c >> 0);
        return System.Windows.Media.Color.FromArgb(a, r, g, b);    
    }

    static public uint ConvertToUint32(System.Windows.Media.Color c)
    {
        return (uint)((c.A << 24) | (c.R << 16) |
                (c.G << 8) | (c.B << 0));
    }
    */
    static public uint ConvertToUint32(System.Drawing.Color c)
    {
        return (uint)((c.A << 24) | (c.R << 16) |
                (c.G << 8) | (c.B << 0));
    }

    static public bool IndexIncreasing(LogIndexMode lm)
    {
        return lm != LogIndexMode.Up;
    }

    static public uint ConvertToUint(byte[] bs)
    {
        uint u = bs[0];
        u += (uint)(bs[1] << 8);
        u += (uint)(bs[2] << 16);
        u += (uint)(bs[3] << 24);
        return u;
    }
    static public uint ConvertToUintReverse(byte[] bs)
    {
        uint u = bs[3];
        u += (uint)(bs[2] << 8);
        u += (uint)(bs[1] << 16);
        u += (uint)(bs[0] << 24);
        return u;
    }

    static public void ChangeEndian(int element_size, byte[] bs)
    {
        if (element_size == 1)
            return;
        int i = 0; 
        int b = element_size - 1;
        while(i < bs.Length)
        {
            int top = i;
            int bot = i + b;
            while(top < bot)
            {
                byte v = bs[top]; bs[top] = bs[bot]; bs[bot] = v;
                top++; bot--;
            }
            i += element_size;
        }
    }

    public static List<Tuple<string, string>>? GetChangedProperties<T>(T a, T b) where T:class
    {   
        if (a != null && b != null)
        {
            if (object.Equals(a, b))
            {
                return null;
            }
            var allProperties = a.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            List<Tuple<string, string>> res = new();
            foreach (PropertyInfo p in allProperties)
            {
                object v = p.GetValue(b);
                if (!object.Equals(p.GetValue(a), v))
                {
                    if(v == null) res.Add(Tuple.Create(p.Name, string.Empty ));
                    else res.Add(Tuple.Create(p.Name, v.ToString() ));
                }          
            }
            if (res.Count == 0) return null;
            return res;
        }
        else
        {
            return null;
          /*  var aText = $"{(a == null ? ("\"" + nameof(a) + "\"" + " was null") : "")}";
            var bText = $"{(b == null ? ("\"" + nameof(b) + "\"" + " was null") : "")}";
            var bothNull = !string.IsNullOrEmpty(aText) && !string.IsNullOrEmpty(bText);
            throw new ArgumentNullException(aText + (bothNull ? ", ":"" )+ bText );*/
        }
    }

    public static void UpdateProperties<T>(T a, List<Tuple<string, string>> n_vs)
        where T : class
    {
        Type ta = typeof(T);
        foreach (Tuple<string, string> n_v in n_vs)
        {
            PropertyInfo pi = ta.GetProperty(n_v.Item1);
            TypeConverter typeConverter = TypeDescriptor.GetConverter(pi.PropertyType);
            pi.SetValue(a, typeConverter.ConvertFromString(n_v.Item2));
        }
    }

    public static void CopyProperties<T1, T2>(T1 a, T2 b)
     where T1:class
     where T2:class
    {
        var allProperties_src = a.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var allProperties_dst = b.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);      
        foreach(PropertyInfo pi_src in allProperties_src)
        {
            var pi_dst = allProperties_dst.Where(p => p.Name == pi_src.Name).FirstOrDefault();
            if(pi_dst != null)
                pi_dst.SetValue(b, pi_src.GetValue(a));
        }
    }   

    public static long lcm_of_array_elements(int[] element_array)
    {
        long lcm_of_array_elements = 1;
        int divisor = 2;

        while (true)
        {
            int counter = 0;
            bool divisible = false;
            for (int i = 0; i < element_array.Length; i++)
            {
                // lcm_of_array_elements (n1, n2, ... 0) = 0. 
                // For negative number we convert into 
                // positive and calculate lcm_of_array_elements. 
                if (element_array[i] == 0)
                    return 0;
                else
                {
                    if (element_array[i] < 0)
                        element_array[i] = element_array[i] * (-1);
                }

                if (element_array[i] == 1)
                    counter++;
                // Divide element_array by devisor if complete 
                // division i.e. without remainder then replace 
                // number with quotient; used for find next factor 
                if (element_array[i] % divisor == 0)
                {
                    divisible = true;
                    element_array[i] = element_array[i] / divisor;
                }
            }

            // If divisor able to completely divide any number 
            // from array multiply with lcm_of_array_elements 
            // and store into lcm_of_array_elements and continue 
            // to same divisor for next factor finding. 
            // else increment divisor 
            if (divisible)
                lcm_of_array_elements = lcm_of_array_elements * divisor;
            else
                divisor++;

            // Check if all element_array is 1 indicate  
            // we found all factors and terminate while loop. 
            if (counter == element_array.Length)
                return lcm_of_array_elements;
        }
    }
    /*
    public byte[] GetBytes(SparkPlugDataType dt, object v)
    {

    }

    public object GetValue(SparkPlugDataType dt, byte[] bs)
    {

    }*/

}
public enum SparkPlugDataType
{
    /// <summary>
    /// The unknown data type, for future extension.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The 8 bit integer data type.
    /// </summary>
    Int8 = 1,

    /// <summary>
    /// The 16 bit integer data type.
    /// </summary>
    Int16 = 2,

    /// <summary>
    /// The 32 bit integer data type.
    /// </summary>
    Int32 = 3,

    /// <summary>
    /// The 64 bit integer data type.
    /// </summary>
    Int64 = 4,

    /// <summary>
    /// The unsigned 8 bit integer data type.
    /// </summary>
    UInt8 = 5,

    /// <summary>
    /// The unsigned 16 bit integer data type.
    /// </summary>
    UInt16 = 6,

    /// <summary>
    /// The unsigned 32 bit integer data type.
    /// </summary>
    UInt32 = 7,

    /// <summary>
    /// The unsigned 64 bit integer data type.
    /// </summary>
    UInt64 = 8,

    /// <summary>
    /// The float data type.
    /// </summary>
    Float = 9,

    /// <summary>
    /// The double data type.
    /// </summary>
    Double = 10,

    /// <summary>
    /// The boolean data type.
    /// </summary>
    Boolean = 11,

    /// <summary>
    /// The string data type.
    /// </summary>
    String = 12,

    /// <summary>
    /// The date time data type.
    /// </summary>
    DateTime = 13,

    /// <summary>
    /// The text data type.
    /// </summary>
    Text = 14,

    /// <summary>
    /// The UUID data type.
    /// </summary>
    Uuid = 15,

    /// <summary>
    /// The data set data type.
    /// </summary>
    DataSet = 16,

    /// <summary>
    /// The bytes type.
    /// </summary>
    Bytes = 17,

    /// <summary>
    /// The file data type.
    /// </summary>
    File = 18,

    /// <summary>
    /// The template data type.
    /// </summary>
    Template = 19,

    /// <summary>
    /// The property set data type.
    /// </summary>
    PropertySet = 20,

    /// <summary>
    /// The property set list data type.
    /// </summary>
    PropertySetList = 21
}