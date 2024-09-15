using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.LogDataFile.Models.NMRecord
{
    public class CalibrationRecord : NMRecord
    {
        public int Phase { get; set; }
        public string Inst { get; set; }
        public string Asset { get; set; }
        public string Sensor { get; set; }
        public long Time { get; set; }
        public CalibrationRecord()
        {
            RType = (int)NMRecordType.Calibration;
        }

        public override void SaveExtension()
        {

        }

        public override void RestoreExt()
        {

        }
    }
}
