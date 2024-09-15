using OpenWLS.PLT1.Edge;
using OpenWLS.Server.Base;
using OpenWLS.Server.DFlash;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;
using System.Windows.Controls;
using System.Windows.Navigation;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using System.Net;
using OpenWLS.PLT1;
using OpenWLS.Edge.Simulator.PLT1;
using OpenWLS.Edge.Simulator;
using OpenWLS.Server.LogInstance.OperationDocument;
using Frame = OpenWLS.PLT1.Edge.Frame;
using OpenWLS.Server.LogInstance.Edge;
using OpenWLS.PLT1.ApA;

namespace OpenWLS.Edge.PLT1
{
    public class PLT1UsbDevice : Device
    {
        protected PLT1UsbPort? usbport;
        protected uint? asset;
//        public uint[]? PortAssets { get; set; }
        public void ProcessUsbMsg(byte[] bs)
        {
            if (bs[0] == IBProtocol.EDGE_DEV_ADDR)
                ProcMsgFromInsts(bs);
            else
            {
                if (edgeClient != null)
                    edgeClient.SendPackageFromInsts(bs);
            }

        }
         
        void ProcMsgFromInsts(byte[] bs)
        {
            Frame f = Frame.ReadFrame(new DataReader(bs));
            if(f.SrcAddress == IBProtocol.S_MOD_ADDR)
            {
                foreach (Block b in f.Blocks)
                {
                    switch (b.Type)
                    {
                        case (byte)PLT1InstMsgCode.GENERAL_INFOR:
                         //   SetGenInfor(b.Body);
                            break;

                    }
                }              
            }
        }

        protected virtual List<uint> SelectPort(byte[]? bs)
        {
            return new List<uint>();
        }

        byte[]? GetSelectPortMsgBytes( List<uint> assets)
        {
            List<uint> us = new List<uint>();
            if (usbport == null) // not connected, send available port list
            {
                us.Add(0);
                us.AddRange(assets);
            }
            else  // connected send back
            {
                us.Add(1);
                us.Add(usbport.InstGenInfor.Subs[0].Asset);
            }
            return IntArrayConverter.GetBytes( us.ToArray() );
        }

        public override void ProcDeviceMsg(byte[]? bs)
        {
            List<uint> assets = SelectPort(bs);
            if (edgeClient != null)
                edgeClient.SendPackage((byte)EdgeULinkMsgCode.FromDevice, GetSelectPortMsgBytes(assets));
        }

        public override void ProcInstMsg(byte[]? bs)
        {
            if (usbport != null && bs != null)
                usbport.SendMsgToInst(bs);
         /*   Frame f = Frame.ReadFrame(r);
            foreach(Block b in f.Blocks)
            {

            } */
        }

        public override void Close()
        {
            if(usbport != null)
            {
                usbport.Close();   usbport = null; 
            }
            
            base.Close();
        }
    }
}
