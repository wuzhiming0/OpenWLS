using OpenWLS.Server.LogDataFile.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.LogInstance.RtDataFile.DCs
{
    public class UIn32D1DC : Measurement
    {
        public UIn32D1DC(LogInstance _api, string _name)
        {
           /* api = _api;
            DataRow dr = GetDCDataRow(api, _name);
            Restore(dr);
           */
        }
    }
}
