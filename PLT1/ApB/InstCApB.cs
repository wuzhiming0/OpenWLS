using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using OpenWLS.PLT1.Edge;
using OpenWLS.Client.LogInstance.Instrument;
using OpenWLS.Server.Base;

namespace OpenWLS.PLT1.ApB
{
    public class InstCApB : PLT1InstrumentC
    {

        public InstCApB()
        {

        }

        public override void CreateCntl()
        {
            cntl = new ApBCntl();
            cntl.Name = Name;
            ((ApBCntl)cntl).Inst = this;
        }

        public override void ResetInst()
        {
           // ((GInstMemACntl)cntl).ResetStatus();
        }

        string ProcTxTxtMsg(DataReader r)
        {
            string req = r.ReadLine();
            if (req == InstApB.msg_mod_tx_req_gain)
            {
                byte b1 = Convert.ToByte(r.ReadLine());

                //  api.SysLog.AddMessage(name_ext + " Scan", Colors.Blue);
                return null;
            }
            if (req == InstApB.msg_mod_tx_req_speed)
            {
                byte b1 = Convert.ToByte(r.ReadLine());
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
            if (req == InstApB.msg_mod_usb_req_list)
            {
                string[] ss = r.ReadLine().Split('|');
                ((ApBCntl)cntl).SetPortSerialNumbers(ss);
               // if (ss.Length == 1 && ss[0].Length > 1)
               //     SendRequest("USBPort\nConnect\n" + ss[0]);
                return null;
            }
            if (req == InstApB.msg_mod_usb_req_connect)
            {
                ((ApBCntl)cntl).UpdateButton(((ApBCntl)cntl).dpConnectioBtn, "USBOn");
                return null;
            }
            if (req == InstApB.msg_mod_usb_req_disconnect)
            {
                ((ApBCntl)cntl).UpdateButton(((ApBCntl)cntl).dpConnectioBtn, "USBOff");
                return null;
            }
            return null;
        }

        public  string ProcTxtMsg(DataReader r)
        {
            string mod = r.ReadLine();

            if (mod == InstApB.msg_mod_tx) return ProcTxTxtMsg(r);
            if (mod == InstApB.msg_mod_rx) return ProcRxTxtMsg(r);
            if (mod == InstApB.msg_mod_usb) return ProcUSBPortTxtMsg(r);

            return null;
        }
    /*    public override void InitInst()
        {
            SendRequest("USBPort\nList");
        }
        
                private void powerBtn_Click(object sender, RoutedEventArgs e)
                {
                    string str = PowerOn ? "nOff" : "\nOn";
                    gui.SendRequest(LogInstance.str_api_req_tpower + str);
                }

                private void scanToolBtn_Click(object sender, RoutedEventArgs e)
                {
                    //apiGui.SendRequest("mainTBar\nScan\n");
                }*/
    }
}
