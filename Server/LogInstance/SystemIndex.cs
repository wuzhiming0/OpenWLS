using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenLS.Base.UOM;

namespace OpenWLS.Server.LogInstance
{
    public class SystemIndex
    {
        public double Value {get; set;}
        public string Unit { get; set; } 
        public double Mul { get; set; }  // from meter to selected 
    }

    public class Depth : SystemIndex
    {
        public Depth()
        {
            Unit = MeasurementUnit.GetSelectedUnit("Depth");
            Mul  = MeasurementUnit.GetDepthConvertMul("m" );
        }
    }

    public class Time : SystemIndex
    {
        public Time()
        {
            Unit = "s";
            Mul = 1;
        }
    }
}
