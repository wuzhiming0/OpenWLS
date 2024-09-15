using Newtonsoft.Json;
using OpenWLS.Server.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.LogDataFile.Models.NMRecord
{
    public class ParametersRecord : NMRecord
    {
        public Parameters Parameters { get; set; }
        public ParametersRecord()
        {
            RType = (int)NMRecordType.Para;
        }
        public override void RestoreExt()
        {
            if (Ext != null)
                Parameters = Newtonsoft.Json.JsonConvert.DeserializeObject<Parameters>(Ext);
            else
                Parameters = new Parameters();
        }

        public override void SaveExtension()
        {
            if (Parameters == null || Parameters.Count == 0)
                Ext = null;
            else
            {
                Ext = Newtonsoft.Json.JsonConvert.SerializeObject(Parameters, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            }
        }

    }
}
