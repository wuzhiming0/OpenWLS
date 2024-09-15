//using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.Base
{

    public class Zone
    {
        public double Top { get; set; }
        public double Bottom { get; set; }
        public string? Units { get; set; }
        public Zone()
        {

        }
        public Zone(double top, double bot, string? units)
        {
            Top = top; Bottom = bot; Units = units;
        }
        public Zone(string str)
        {
            string[] ss = str.Split('|');
            Top = int.Parse(ss[0]);
            Bottom = int.Parse(ss[1]);
            if (ss.Length == 3)
                Units = ss[2];
        }
        public override string ToString()
        {
            if (Units == null) return "{Top}|{Bottom}";
            else return "{Top}|{Bottom}|{Units}";
        }

    }
    public class Parameter
    {
        public string Name { get; set; }
        public string? MNEM { get; set; }
        public SparkPlugDataType DataType { get; set; }
        public string? Description { get; set; }
        public string? Zone { get; set; }
        public string? Units { get; set; }
        public string[]? Val { get; set; }
 //       public object Tag { get; set; }
       /* public string GetValString()
        {
            if (Val == null) return "";
            if (Val is Array)
            {
                Array os = (Array)Val;
                string str = os.GetValue(0).ToString();
                for (int i = 1; i < os.Length; i++)
                    str = str + "," + os.GetValue(i).ToString();
                return str;
            }
            else return Val.ToString();
        }
        public void SetValue(string strVal)
        {

        }*/

        public override string ToString()
        {
            string str = Name;
            str = str + ":";
            if (Description != null)
                str = str + Description;
            str = str + ":";
            if (Zone != null)
                str = str + Zone;
            str = str + ":" + Val;
            return str;
        }
        public string ToShortString()
        {
            if (MNEM != null)
                return $"{MNEM}: {Val}";
            return $"{Name}: {Val}";
        }
    }

    public class Parameters : List<Parameter>
    {
        public void SetValByMnem(string mnem, string[] v, string units)
        {
            foreach (Parameter p in this)
            {
                if (p.MNEM == mnem)
                {
                    p.Val = v;
                    p.Units = units;
              //      p.DataType = SparkPlugDataType.String;
                    return;
                }
            }
            Add(new Parameter()
            {
                MNEM = mnem,
                Val = v,
                Units = units,
            //    DataType = SparkPlugDataType.String
            });
        }
    }
}
