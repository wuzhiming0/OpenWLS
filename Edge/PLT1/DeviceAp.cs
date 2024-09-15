using OpenWLS.Edge.USB;
using OpenWLS.PLT1;
using OpenWLS.Server.Base;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Edge.PLT1
{
    public class DeviceAp : PLT1UsbDevice
    {
        
        public DeviceAp()
        {
            Type = Server.LogInstance.Edge.DeviceType.PLT1Ap;
        }

        protected override byte[]? GetAssetBytes()
        {
            if(asset == null) return null;
            else return BitConverter.GetBytes((uint)asset); 
        }

        protected override List<uint> SelectPort(byte[]? bs )
        {
            Close();
            asset = bs == null? null : BitConverter.ToUInt32(bs);
            PLT1UsbPorts ports = PLT1UsbPort.Scan();

            //connect to first port, if requested port is not defined.  
            if (ports.Count > 0 && asset == null)
                asset = ports[0].InstGenInfor.Subs[0].Asset;

            List<uint> assets = new List<uint>();
            for (int i = 0; i < ports.Count; i++)
            {
                if (ports[i].InstGenInfor.Subs[0].Asset == asset)
                {
                    usbport = ports[i];
                    usbport.StartRx(this);
                }
                else
                    ports[i].Close();
                assets.Add(ports[i].InstGenInfor.Subs[0].Asset);
            }
            return assets;
        }
    }
}
