using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;

using Newtonsoft.Json;


namespace OpenWLS.Server.LogInstance.Calibration
{
    public class CVReports : List<CVReport>
    {
        public CVReport GetReport(string name, string phase)
        {
            foreach (CVReport r in this)
                if (r.Name == name && r.Phases.Contains(phase))
                    return r;
            return null;
        }
    }

    public class CVReport
    {
        public string Name { get; set; }  // Type
        public string Phases { get; set; }
        public DataTable Tables { get; set; }

        //      public static CVReport
    }


}
