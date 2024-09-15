using OpenLS.Base.UOM;
using OpenWLS.Server.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Edge.Simulator.PLT1.ApA
{
    public enum DepthCmd
    {
        SetVal = 1, SetUnit = 2, SetSource = 3, SetCountsPerCycle = 4,
        SetSimulatorSpeed = 0x10, 
    }
    public enum DepthSource { Encoder1 = 1, Encoder2 = 2, Encoder3 = 3,  Simulator = 4, Encoder1A2 = 5,}
    public class ModuleDepth : PLT1InstModule
    {
        DepthEncoder[] encoders;
        DepthSimulator simulator;
        string uom;
  
        public double Val
        {
            get
            {
                switch (DepthSource)
                {
                    case DepthSource.Encoder1:
                        return encoders[0].Val;
                    case DepthSource.Encoder2:
                        return encoders[1].Val;
                    case DepthSource.Encoder3:
                        return encoders[2].Val;
                    case DepthSource.Encoder1A2:
                        return (encoders[0].Val + encoders[1].Val) / 2;
                    default:// DepthSource.Simulator:
                        return simulator.Val;
                };
            }
            set
            {
                encoders[0].Val = value;
                encoders[1].Val = value;
                encoders[2].Val = value;
                simulator.Val = value;
            }
        }
        public string UOM
        {
            get { return uom; }
            set
            {
                double m = MeasurementUnit.GetDepthConvertMul(uom, value);
                foreach (DepthEncoder e in encoders)
                    e.SetDepthConvertMul(m);
                uom = value;
            }
        }
        public DepthSource DepthSource { get; set; }
        public ModuleDepth()
        {
            encoders = new DepthEncoder[] { new DepthEncoder(), new DepthEncoder(), new DepthEncoder() };
            simulator = new DepthSimulator();
            uom = "ft";
            Val = 0;
        }

        public override void ProcGUIMsg(DataReader r)
        {
            DepthCmd cmd = (DepthCmd)r.ReadByte();

            switch (cmd)
            {
                case DepthCmd.SetVal:
                    double v = r.ReadDouble();
                    encoders[0].Val = v; encoders[1].Val = v; encoders[2].Val = v;
                    simulator.Val = v;
                    break;
                case DepthCmd.SetUnit:
                    string? unit = r.ReadStringWithSizeByte();
                    if (unit != null)
                        UOM = unit;
                    break;
                case DepthCmd.SetSource:

                    break;

            }
        }

    }

    public class DepthEncoder
    {
        public int CountsPerCycle { get; set; }
        public double Circumference { get; set; }

        long count;
        public long Count
        {
            get { return count; }
            set { count = value; }
        }

        double k;
        public double Val
        {
            get { return k * count; }
            set { count = (long)(value / k); }
        }

        public DepthEncoder()
        {
            CountsPerCycle = 1024;
            Circumference = 10;
            k = Circumference / CountsPerCycle;
        }

        public void SetDepthConvertMul(double m)
        {
            k *= m;
        }
    }

    public class DepthSimulator
    {
        double k;
        long start_ticks;
        double start_v;


        // per hour
        public double Speed
        {
            get { return k * 3600000 * TimeSpan.TicksPerMillisecond; }
            set { k = value / 3600000 / TimeSpan.TicksPerMillisecond; }
        }
        public double Val
        {
            get { return k * (DateTime.Now.Ticks - start_ticks) + start_v; }
            set
            {
                start_ticks = DateTime.Now.Ticks;
                start_v = value;
            }
        }


        public void Start()
        {

        }

        public void Stop()
        {

        }
    }
}
