using OpenWLS.Server.LogInstance.Edge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.PLT1.Edge
{
    public class DevPLT1Ap : DevicePLT1
    {
        public DevPLT1Ap()
        {
            Type = DeviceType.PLT1Ap;
        }

        protected override void OnInit()
        {
            if (Instrument != null && Instrument.Subs.Count != 0 && Instrument.Subs[0].Asset != null)
                asset = Convert.ToUInt32(Instrument.Subs[0].Asset);
            if (logInstance != null && logInstance.Edge != null)
            {
                if (asset == null)
                    logInstance.Edge.SendPackageToDevice(null);
                else
                    logInstance.Edge.SendPackageToDevice(BitConverter.GetBytes((uint)asset));
            }
        }
    }
}
