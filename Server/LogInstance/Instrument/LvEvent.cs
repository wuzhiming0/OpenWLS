using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.LogInstance.Instrument
{
    public enum LvEventOpCode { SetParaTblItem = 1, SetActItem = 2};
    public class LvEvent
    {
        public int size { get; set; }
        public LvEventOpCode OpCode { get; set; }

    }

    public class LvTimeEvent : LvEvent
    {
        public uint Time { get; set; }
    }

    public class LvConditionalEvent : LvEvent
    {

    }

}

