using OpenWLS.Server.LogInstance.Instrument;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.LogInstance.RtDataFile.DCs
{
    public class TemperatureDC1 : Measurement
    {
        double[] cs;
        double d24;
        public TemperatureDC1(LogInstanceS li)
        {
            logInstance = li;
         //   DataRow dr = GetDCDataRow(api, "TEMP");
         //   Restore(dr);
            d24 = 1.0 / (2 ^ 24);
        }


    }
}
