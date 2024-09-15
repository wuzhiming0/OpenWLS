using Microsoft.AspNetCore.Hosting.Server;
using OpenWLS.Server.LogInstance.Edge;
using OpenWLS.Server.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWLS.Server.LogInstance;
using System.Runtime.CompilerServices;

namespace OpenWLS.Edge
{
    public  class Device : DeviceBase
    {
        public event EventHandler? DeviceClosed;

        protected EdgeClient? edgeClient;
        public EdgeClient? EdgeClient { get { return edgeClient; } }
        LiState liState;
 
        public LiState LiState
        {
            get { return liState; }
            set
            {
                liState = value;
            }
        }

        public virtual void OnInit()
        {

        }
        public  void Init(EdgeClient client)
        {
            edgeClient = client;

        }

        public virtual void ProcDeviceMsg(byte[]? bs)
        {

        }

        public virtual void ProcInstMsg(byte[]? bs )
        {

        }



        public virtual void SendMsgToInst(byte[] bs)
        {

        }

        public virtual void Close()
        {
            if(DeviceClosed != null) 
                DeviceClosed(this, null);
        }
        
        public void SendPackageFromInst(byte[] bs)
        {
            if (edgeClient != null)
                edgeClient.SendPackageFromInsts(bs);
        }

    }

    public class Devices : List<Device> 
    {
        public void AddDevice(Device new_dev)
        {
            new_dev.Id = Count == 0 ? 1 : this.Max(a => a.Id) + 1;
            Add(new_dev);
        }
    
    }
}
