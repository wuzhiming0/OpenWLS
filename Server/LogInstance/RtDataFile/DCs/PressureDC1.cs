using OpenWLS.Server.LogDataFile.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OpenWLS.Server.LogInstance.RtDataFile.DCs
{
    public class QdDataChannel : Measurement
    {
        public QtCalOutput QtCalOutput { get; set; }
        public void AddSample(uint xp, uint xt)
        {
            //  Raw = xt;
            QtCalOutput.qdHexCalc(xp, xt, false);
        }
    }

    public class PressureDC1 : QdDataChannel
    {

        public PressureDC1(LogInstance api)
        {
          //  api = _api;
          //  DataRow dr = GetDCDataRow(api, "PRES");
           // Restore(dr);
        }
    }
}
