using OpenWLS.Edge.Playback;
using OpenWLS.Server.LogInstance.Edge;
using OpenWLS.Server.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using OpenWLS.Edge.Simulator;
using System.Data.Entity.Core.Metadata.Edm;
using OpenWLS.Edge.PLT1;

namespace OpenWLS.Edge
{
    public class EdgeClient : IPackageProc
    {
        public event EventHandler Closed;

        protected EdgeServer server;
        protected TcpPort tcpClient;
        protected Device? device;

        public EdgeClient(Socket sock, EdgeServer server)
        {
            tcpClient = new OpenWLSTcpPort(sock);
            this.server = server;
        }
        public void StartRx(EdgeServer server)
        {
            this.server = server;
            Task.Run(() => tcpClient.RxLoop(this));
        }

        public  void ProcPackage(Package p){
            switch (p.Code)
            {
                case (byte)EdgeDLinkMsgCode.ToInsts:
                    if (device != null)
                        device.ProcInstMsg(p.Body); 
                    break;
                case (byte)EdgeDLinkMsgCode.ToDevice:
                    if (device != null)
                        device.ProcDeviceMsg(p.Body); 
                    break;
                case (byte)EdgeDLinkMsgCode.GetDevices:
                    //server.ScanDevices(r);
                    //SendPackage((byte)EdgeULinkMsgCode.DevicesList, server.Devices.GetBytes());  
                    break;
           //     case (byte)EdgeDLinkMsgCode.NewDevice:
           //         UseDevice(r); break;
           //     case (byte)EdgeDLinkMsgCode.FreeDevice:
           //         FreeDevice(r); break;
                case (byte)EdgeDLinkMsgCode.NewPhysicalDevice:
                    NewPhysicalDevice(new DataReader(p.Body)); break;
                case (byte)EdgeDLinkMsgCode.NewPlayback:
                    NewPlayback(new DataReader(p.Body)); break;
                case (byte)EdgeDLinkMsgCode.NewSimulator:
                    NewSimulator(new DataReader(p.Body)); break;
            }
        }
/*
        void UseDevice(DataReader r)
        {
            int dev_id = r.ReadUInt16();
            Device? dev = server.Devices.Where(a=>a.Id == dev_id).FirstOrDefault();
            if (dev != null)
            {
                dev.SetClient(this);
                dev.DeviceClosed += Dev_DeviceClosed;
                SendPackage((byte)EdgeULinkMsgCode.NewDevice, dev.GetBytes());
            }
        }
*/
        private void Dev_DeviceClosed(object? sender, EventArgs e)
        {
            
        }



        void NewPhysicalDevice(DataReader r)
        {
            DeviceType dt = (DeviceType)r.ReadInt32();   // device type
            switch (dt)
            {
                case DeviceType.PLT1Ap:
                    device = new DeviceAp();
                    break;
                default: break;    
            }
            if(device != null)
            {
                server.Devices.AddDevice(device);
                device.Init(this);
                SendPackage((byte)EdgeULinkMsgCode.NewDevice, BitConverter.GetBytes(device.Id));
            }
        }

        void NewPlayback(DataReader r)
        {
            device = new RDataPlayer();
            server.Devices.AddDevice(device);
            device.Init(this);
        }
        void NewSimulator(DataReader r)
        {
            string json = r.ReadStringWithSizeInt32();
            device = Simulator.Simulator.CreateSimulator(json);
            if (device == null)
                EdgeServer.WriteLine("failed to create simulator.");
            else
            {
                server.Devices.AddDevice(device);
                EdgeServer.WriteLine($"created simulator {device.Id}.");
                device.Init(this);
                SendPackage((byte)EdgeULinkMsgCode.NewDevice, BitConverter.GetBytes(device.Id));
            }

        }


        public void Close()
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        public void SendPackageFromInsts(byte[] bs)
        {
            SendPackage((byte)EdgeULinkMsgCode.FromInsts, bs);
        }

        public void SendPackage(byte m_type, byte[]? bs)
        {
            tcpClient.SendPackage(new Package()
            {
                Code = m_type,
                Body = bs
            });
        }
    }

    public class EdgeClients : List<EdgeClient> { 
    }
}
