using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.LogDataFile.Models.NMRecord;

public class TelemetryPackage : NMRecord
{
    public long Time { get; set; }
    public double Depth { get; set; }
    public int? DstId { get; set; }
    public int? SrcId { get; set; }
    public int? MsgType { get; set; }
    //      public byte[] Body { get; set; }

}
