using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


using System.Windows.Input;
using System.IO.Ports;

using System.Text.RegularExpressions;
using Microsoft.Win32;
using OpenWLS.Server.Base;
using OpenWLS.Server.LogDataFile.Models.NMRecord;
using OpenWLS.Server.LogInstance.Instrument;
using System.Net.Sockets;
using Newtonsoft.Json.Serialization;
using System.Reflection.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Http;

/*
Tool Table
* Tool Count 
*  Tool 1 Address
*  Tool 1 Subs
*  Tool 1 Sub 1 Serial
*  Tool 1 Sub 1 Asset
*  ......... 
*  Tool 2 Address
*  Tool 2 Subs
*  Tool 2 Sub 1 Serial
*  Tool 2 Sub 1 Asset
* */

namespace OpenWLS.Server.LogInstance.Edge
{
    //public enum EdgeState {None = 0, Disconnected, Connected }
  //  public enum EdgeMsgDst { Edge = 0, Device };
    public enum EdgeDLinkMsgCode { 
        GetDevices = 1, 
        NewPlayback = 0x10, NewSimulator = 0x11, NewPhysicalDevice = 0x12, 
        ToDevice = 0x20, 
        ToInsts = 0x30 
    };
    public enum EdgeULinkMsgCode {
        DevicesList = 1,  
        NewDevice = 0x10,  
        FromDevice = 0x20,
        FromInsts = 0x30, 
    };




    public class Edge : DBase.Models.LocalDb.Edge, IPackageProc
    {
 //       public event EventHandler<EdgeULinkMsgCode> EdgeDeviceMsgRxed;
        public event EventHandler<bool?> EdgeStateChanged;

        public const int port_nu = 0x5432;
        protected LogInstanceS logInstance;

        protected TcpPort tcpPort;
        protected Device? device;

        public Device? Device { get { return device; } set { device = value; } }
        public Instrument.Instrument? edgeDevInst;
        public Instruments Equipments { get; set; }
        public OperationDocument.InstSubs Pannels { get; set; }
        public bool Connected { get { return tcpPort.Connected;  } }

        public static Edge? CreateEdge(LogInstanceS li, Server.DBase.Models.LocalDb.Edge edge_db)
        {
            Edge? edge = null;
            // create standard OpenWLS edge
            if (edge_db.EType == null || edge_db.EType == Server.DBase.Models.LocalDb.EdgeType.OpenWLS_Std)
                edge = new Edge();
            // create special edge
            else
            {
                // create special edge
            }
            if (edge != null)
            {
                edge.logInstance = li;
                edge.CloneFrom(edge_db);                
                edge.CreatePort();
                edge.edgeDevInst = li.Insts.Where(a => a is IEdgeDevice).FirstOrDefault();
            }
            return edge;
        }

        public static byte[] GetEdgeDeviceWsBytes(byte[]? bs)
        {
            int c = 4;
            if (bs != null)
                c += bs.Length;
            DataWriter w = new DataWriter(c);
            w.WriteData((ushort)LiWsMsg.EdgeDev);
            if (bs == null)
                w.WriteData((ushort)0);
            else
            {
                w.WriteData((ushort)bs.Length);
                w.WriteData(bs);
            }
            return w.GetBuffer();
        }

        public Edge()
        {
        }

        public virtual void CreatePort()
        {
            tcpPort = new OpenWLSTcpPort();
            tcpPort.PortConnected += TcpPort_PortConnected;
        }

        private void TcpPort_PortConnected(object? sender, bool? e)
        {
            Task.Run(() => tcpPort.RxLoop(this));
            EdgeStateChanged?.Invoke(this, null);
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>0=sucess</returns>
        public virtual void Connect()
        {
            string ipAddr = IpAddr == null ? "127.0.0.1" : IpAddr;
            int portNu = Port == null ? Edge.port_nu : (int)Port;
            tcpPort.Connect(ipAddr, portNu);
        }

        #region down link request
 /*       public virtual int RequestDevices()
        {
            Package p = new Package() { Code = (byte)EdgeDLinkMsgCode.GetDevices };
            tcpPort.SendPackage(p);
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dev_id"></param>
        /// <returns>0=sucess,  </returns>
        public virtual int RequestSelectDevice(int dev_id)
        {
            Package p = new Package() {
                Code = (byte)EdgeDLinkMsgCode.UseDevice,
                Body = BitConverter.GetBytes(dev_id)
            };
            tcpPort.SendPackage(p);
            return 0;
        }
        public virtual int RequestFreeDevice(int dev_id)
        {
            Package p = new Package()
            {
                Code = (byte)EdgeDLinkMsgCode.FreeDevice,
                Body = BitConverter.GetBytes(dev_id)
            };
            tcpPort.SendPackage(p);
            return 0;
        }
*/
        public void RequestNewPhysicalDevice()
        {
            if (device != null)
            {
                Package p = new Package()
                {
                    Code = (byte)EdgeDLinkMsgCode.NewPhysicalDevice,
                    Body = BitConverter.GetBytes((int)device.Type)
                };
                tcpPort.SendPackage(p);
            }
        }

        public void RequestNewSimulator(string str_od)
        {
            byte[] bs = UTF32Encoding.UTF8.GetBytes(str_od);
            DataWriter w = new DataWriter(4 + bs.Length);
            w.WriteData(bs.Length);
            w.WriteData(bs);

            Package p = new Package()
            {
                Code = (byte)EdgeDLinkMsgCode.NewSimulator,
                Body = w.GetBuffer()
            };
            tcpPort.SendPackage(p);
        }

        public void RequestNewPlayback(string fn)
        {

        }
        public void SendPackageToDevice(byte[]? bs)
        {
            Package p = new Package()
            {
                Code = (byte)EdgeDLinkMsgCode.ToDevice,
                Body = bs
            };
            tcpPort.SendPackage(p);
        }
        public void SendPackageToInsts(byte[] bs)
        {
            Package p = new Package()
            {
                Code = (byte)EdgeDLinkMsgCode.ToInsts,
                Body = bs
            };
            tcpPort.SendPackage(p);
        }

        #endregion


        public virtual void StartLog(bool depthIndex, double? from, double? to )
        {
            if (device != null)
                device.StartLog(depthIndex, from, to);

        }

        public virtual void StopLog()
        {

        }

        public virtual void Close()
        {
            StopLog();
            //   base.OnClosePort();
        }

 
        void NewDevice(byte[]? bs )
        {
            if (bs == null) return;

            IEdgeDevice? edge_dev = (IEdgeDevice?)logInstance.Insts.Where(a => a is IEdgeDevice).FirstOrDefault();
            if (edge_dev == null) return;
            device = edge_dev.GetDevice();
            device.Init(BitConverter.ToInt32(bs), logInstance);
            logInstance.State =  LiState.Log_EdgeDeviceConnected;

        }

        public virtual void ProcPackage(Package p)
        {
            switch (p.Code)
            {
                case (byte)EdgeULinkMsgCode.FromInsts:
                    if(device != null)
                        device.ProcMsgFromInsts(p.Body);
                    break;
                case (byte)EdgeULinkMsgCode.FromDevice:
                    if (device != null)
                    {
                        device.ProcMsgFromDevice(p.Body);
                        logInstance.WsClients.SendPackage(WebSocket.ClientType.Inst, GetEdgeDeviceWsBytes(p.Body));
                    }
                    break;
                case (byte)EdgeULinkMsgCode.DevicesList:

                    break;
                case (byte)EdgeULinkMsgCode.NewDevice:
                    NewDevice(p.Body);
                    break;
           //     case (byte)EdgeULinkMsgCode.CloseClient:
           //         RequestFreeDevice(r.ReadInt32()); 
                    break;

            }
        }


    }

}

