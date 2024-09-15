using OpenWLS.Client.Dock;
using OpenWLS.Client.Requests;
using OpenWLS.Server.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace OpenWLS.Client.LogInstance.Instrument
{
    public class MeasurementDisplay : UserControl, IWsMsgProc
    {
        public DockControl DockControl { get; set; }
        public InstrumentC Instrument { get; set; }

        public virtual void ProcessWsRxMsg(DataReader r)
        {

        }
        public virtual void RequestEnd()
        {

        }

        public virtual void SetWebSocket(ClientWebSocket ws)
        {

        }
    }

    public class MeasurementDisplays: List<MeasurementDisplay>
    {
        public void AddDisplays(InstrumentCs? insts)
        {
            if (insts == null) return;
            foreach(InstrumentC inst in insts)
            {
                foreach (MeasurementDisplay md in inst.Displays)
                    Add(md);
            }
        }
    }
}
