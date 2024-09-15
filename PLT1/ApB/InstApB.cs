using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Windows.Media;

using Newtonsoft.Json;
using OpenWLS.PLT1.Edge;
using OpenWLS.Server.Base;
using OpenWLS.Server.LogInstance.Calibration;
using OpenWLS.Server.LogInstance;


namespace OpenWLS.PLT1.ApB
{
    public class InstApB: PLT1Instrument
    {   
        public static string msg_mod_usb = "USBPort";
        public static string msg_mod_usb_req_list = "List";
        public static string msg_mod_usb_req_connect = "Connect";
        public static string msg_mod_usb_req_disconnect = "Disconnect";

        public static string msg_mod_tx = "Tx";
        public static string msg_mod_tx_req_gain = "Gain";
        public static string msg_mod_tx_req_speed = "Speed";

        public static string msg_mod_rx = "Rx";
        public static string msg_mod_rx_req_gain = "Gain";
        public static string msg_mod_rx_req_speed = "Speed";

        public const int CMD_EXTAND_TX_GAIN = 0x1;
        public const int CMD_EXTAND_TX_SPEED = 0x2;

        public const int CMD_EXTAND_RX_GAIN = 0x11;
        public const int CMD_EXTAND_RX_SPEED = 0x12;


     
        public InstApB()
        {
           // edgeDev = new DevPLT1Ap();
            default_addr = IBProtocol.S_MOD_ADDR;
        }



        public string[] GetAvailableComms()
        {
            return null;
        }

        string ProcTxTxtMsg(DataReader r)
        {
            string req = r.ReadLine();
            if (req == msg_mod_tx_req_gain)
            {
                byte b1 = (byte)(255 * Convert.ToByte(r.ReadLine()) / 100); 
       //        ((PLT1DataPort)usbPort).SendExtendCmd((byte)Address, (byte)CMD_EXTAND_TX_GAIN, b1, false);
                //  api.SysLog.AddMessage(name_ext + " Scan", Colors.Blue);
                return null;
            }
            if (req == msg_mod_tx_req_speed)
            {
                byte b1 = Convert.ToByte(r.ReadLine());
         //       ((PLT1DataPort)usbPort).SendExtendCmd((byte)Address, (byte)CMD_EXTAND_TX_SPEED, b1, false);
                //  api.SysLog.AddMessage(name_ext + " Scan", Colors.Blue);
                return null;
            }

            return null;
        }

        string ProcRxTxtMsg(DataReader r)
        {
            return null;
        }

        string ProcUSBPortTxtMsg(DataReader r)
        {
            string req = r.ReadLine();
            if (req == msg_mod_usb_req_list)
            {
             //   List<string> sns = HwDataPort.GetUsbPortSerialNumbers("Arrow USB Blaster B");
             //   SendInstGuiMsg("USBPort\nList\n" + String.Join("|", sns.ToArray()) );
                return null;
            }
            if (req == msg_mod_usb_req_connect)
            {
            //    if(usbPort.OpenPortBySn(r.ReadLine()) == 0);
            //        SendInstGuiMsg("USBPort\nConnect");
                return null;
            }
            

            return null;
        }

        public string ProcGUIText(DataReader r)
        {
            string mod = r.ReadLine();

            if (mod == msg_mod_tx) return ProcTxTxtMsg(r);
            if (mod == msg_mod_rx) return ProcRxTxtMsg(r);
            if (mod == msg_mod_usb) return ProcUSBPortTxtMsg(r);          
            return null;
        }

        override public void ProcessParameter(string name, string value)
        {

        }

        protected override void InitCV(CVInstrument cvInst)
        {


        }




        void ProcessExtCmdResponse(DataReader r)
        {
            /*
            SgrAExtCmdCde ec = (SgrAExtCmdCde)r.ReadByte();
            switch (ec)
            {
                case SgrAExtCmdCde.HvGain:
                    break;
                case SgrAExtCmdCde.SignalInput:
                    break;
                case SgrAExtCmdCde.TestPulseWidth:
                    break;

               default:
                    break;
            } 
            */
        }

          /*    override public void ProcessUplinkPackage(DataReader r, ushort size)
        {
            byte cmd = r.ReadByte();
      InstCommandCode cmdCode = (InstCommandCode)cmd;
            switch (cmdCode)
            {
                case InstCommandCode.INST_EXTAND:
                    ProcessExtCmdResponse(r);
                    break;
                default:
                    break;
            }
      
        }*/

        override public void OnConnected()
        {
      //      api.HwPort.SendExtendCmd(Address, (byte)SgrAExtCmdCde.GetEFInfor);
      //      api.HwPort.SendExtendCmd(Address, (byte)SgrAExtCmdCde.GetBBS);
      //      api.HwPort.SendExtendCmd(Address, (byte)SgrAExtCmdCde.GetDFiles);
        }

    }
}
