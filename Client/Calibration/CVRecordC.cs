using OpenWLS.Server.DBase.Models.CalibrationDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenWLS.Client.Calibration
{
    public class CVRecordC : CVRecord
    {

        [JsonIgnore]
        public string Title
        {
            get
            {
                return Serial + "/" + Asset + "  " + Type + "  " + GetPhaseLongString();
            }
        }

        [JsonIgnore]
        public bool Selected { get; set; }


    }
}
