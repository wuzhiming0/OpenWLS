using Microsoft.EntityFrameworkCore.Diagnostics;
using OpenWLS.Edge.USB;
using OpenWLS.PLT1;
using OpenWLS.PLT1.ApA;
using OpenWLS.PLT1.Edge;
using OpenWLS.Server.Base;
using OpenWLS.Server.LogInstance.Edge;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using static System.Windows.Forms.AxHost;

namespace OpenWLS.Edge.PLT1
{
    public class PLT1UsbPort
    {
        public const byte edge_usb_dev_pn = 0x10;    // port name
        public const byte edge_usb_dev_rxState = 0x10;

        public const byte usb_block_end = 0x40;
        public const byte usb_block_size = 0x40;
        public const byte usb_block_body_size = 0x3f;
        public const byte usb_block_size_mask = 0x3f;

        protected string? port_name;
        protected SerialPort? serialPort;
        protected Queue<byte[]> q_tx;
        protected Queue<byte[]> q_rx;
        protected int tx_q_max;
        protected int rx_q_max;
        protected bool tx_busy;
        protected bool proc_busy;
        protected bool connected;
 //       int rx_buf_length;
 //       int rx_wr;
        List<byte[]> rx_bufs;
        byte[] tx_buf;
        Task? task_tx, task_proc, task_rx;

        PLT1UsbDevice? device;
        PLT1InstGenInfor? instGenInfor;

        public PLT1InstGenInfor? InstGenInfor { get{ return instGenInfor;  } }
        public string? PortName { get { return port_name; } }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="devs"></param>
        public static PLT1UsbPorts Scan()
        {
            List<string> cpns = USBPort.GetComPortNames("04D8", "000A");
            PLT1UsbPorts ports = new PLT1UsbPorts();
            foreach (string cpn in cpns)
            {
                PLT1UsbPort? port = CheckComPort(cpn);
                if(port != null) ports.Add(port);
            }
            return ports;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pn"></param>
        /// <returns>device if it's Ap device, null if not</returns>
        public static PLT1UsbPort? CheckComPort(string pn)
        {
            PLT1UsbPort port = new PLT1UsbPort()
            { port_name = pn };
            port.CreateSerialPort();
            if (port.instGenInfor == null)
            {
                port.Close(); return null;
            }
            return port;
        }

        public PLT1UsbPort()
        {
            q_tx = new Queue<byte[]>();
            q_rx = new Queue<byte[]>();
            tx_q_max = 1024;
            rx_q_max = 1024;
            tx_busy = false;
            proc_busy = false;
            tx_buf = new byte[ usb_block_size+1];
            connected = false;
        }

        protected void CreateSerialPort()
        {
            instGenInfor = null;
            serialPort = new SerialPort();
            serialPort.PortName = port_name;
            rx_bufs = new List<byte[]>();
            try
            {
              //  serialPort.RtsEnable = true;
                serialPort.ReceivedBytesThreshold = usb_block_size;
                serialPort.ReadTimeout = 2000;
                serialPort.Open();
                //  serialPort.DataReceived += SerialPort_DataReceived;
                byte[] bs = Frame.GetFrameBytes(IBProtocol.S_MOD_ADDR, IBProtocol.EDGE_DEV_ADDR, (byte)PLT1InstMsgCode.GENERAL_INFOR | (byte)IBProtocol.MSG_READ_MASK, null, true);
                SendMsgToInst(bs);
                byte[] buffer = new byte[usb_block_size];
                int s = serialPort.Read(buffer, 0, usb_block_size);
                if ( s < usb_block_size )
                    s += serialPort.Read(buffer, s, usb_block_size-s);
                if (s == usb_block_size)
                {
                    ProceRxBytes(buffer);
                    if (!task_proc.IsCompleted)
                        task_proc.Wait();
                }
            }
            catch (Exception ex)
            {
               // Close();
            }
        }

        void ProceRxBytes(byte[] bs)
        {

            if ((bs[0] & usb_block_end) != 0) // end block
            {
                int size_eb = bs[0] & usb_block_size_mask;
                int size_total = rx_bufs.Count * usb_block_size_mask + size_eb;
                byte[] buf = new byte[size_total];
                int offset = 0;
                for (int i = 0; i < rx_bufs.Count; i++)
                {
                    Buffer.BlockCopy(rx_bufs[i], 1, buf, offset, usb_block_size_mask);
                    offset += usb_block_size_mask;
                }
                Buffer.BlockCopy(bs, 1, buf, offset, size_eb);
                q_rx.Enqueue(buf);
                if (!proc_busy)
                    task_proc = Task.Run(() => { ProcLoop(); });
            }
            else
                rx_bufs.Add(bs);
        }
        public void StartRx(PLT1UsbDevice dev)
        {
            device = dev;
            task_rx = Task.Run(() => { RxLoop(); });
        }
        void RxLoop()
        {
            byte[] buffer = new byte[usb_block_size];
            int s = 0;
   
            while (serialPort != null)
            {
                if (serialPort.IsOpen)
                {
                    if(!connected)
                    {
                        connected = true;
                        if(device != null && device.EdgeClient != null)
                            device.EdgeClient.SendPackage((byte)EdgeULinkMsgCode.FromDevice, BitConverter.GetBytes(3));
                    }
                    if (serialPort.BytesToRead > 0)
                    {
                        while (s < usb_block_size)
                            s += serialPort.Read(buffer, s, usb_block_size - s);
                        ProceRxBytes(buffer);
                    }
                    else
                        Thread.Sleep(100);
                }
                else
                {
                    if(connected)
                    {
                        connected = false;
                        if (device != null && device.EdgeClient != null)
                            device.EdgeClient.SendPackage((byte)EdgeULinkMsgCode.FromDevice, BitConverter.GetBytes(2));
                    }
                    Thread.Sleep(1000);
                    try { serialPort.Open(); }
                    catch (Exception e) { }                   
                }
            }


        }


        public  void SendMsgToInst(byte[] bs)
        {
            if (q_tx.Count >= tx_q_max) return;
            q_tx.Enqueue(bs);
            if (!tx_busy)
                task_tx = Task.Run(() => { TxLoop(); });
        }

        void TxLoop()
        {
            tx_busy = true;
            while (q_tx.Count > 0)
            {
                byte[]? bs_out;
                if (q_tx.TryDequeue(out bs_out) && bs_out != null)
                {
                    if (serialPort != null)
                    {
                        while (!serialPort.IsOpen) Thread.Sleep(100);  // wait port is open
                        try
                        {
                            tx_buf[0] = usb_block_size_mask;
                            int size = bs_out.Length;
                            while (size > 0)
                            {
                                int offset = 0;
                                if (size > usb_block_size_mask)
                                {
                                    Buffer.BlockCopy(bs_out, offset, tx_buf, 1, usb_block_size_mask);
                                    serialPort.Write(tx_buf, 0, tx_buf.Length);
                                    offset += usb_block_size_mask;
                                    size -= usb_block_size_mask;
                                }
                                else
                                {
                                    tx_buf[0] = (byte)(usb_block_end + size);
                                    Buffer.BlockCopy(bs_out, offset, tx_buf, 1, size);
                                    serialPort.Write(tx_buf, 0, tx_buf.Length);
                                    size = 0;
                                }
                            }
                        }
                        catch (Exception e) { }
                    }
                }
            }
            tx_busy = false;
        }


        void ProcessUsbMsg(byte[] bs)
        {
            Frame f = Frame.ReadFrame(new DataReader(bs));
            if (f.SrcAddress == IBProtocol.S_MOD_ADDR)
            {
                foreach (Block b in f.Blocks)
                {
                    switch (b.Type)
                    {                        
                        case (byte)PLT1InstMsgCode.GENERAL_INFOR:
                            PLT1InstGenInfor igr = new PLT1InstGenInfor();
                            if (b.Body != null)
                            {
                                igr.Restore(b.Body);
                                if (igr.Subs.Count == 1 && igr.Subs[0].ModelNu == 5002)
                                    instGenInfor = igr;
                            }
                            break;
                    }
                }
            }
        }
        void ProcLoop()
        {
            proc_busy = true;
            while (q_rx.Count > 0)
            {
                byte[]? bs_proc;
                if (q_rx.TryDequeue(out bs_proc) && bs_proc != null)
                {
                    if (device != null)
                        device.ProcessUsbMsg(bs_proc);
                    else
                        ProcessUsbMsg(bs_proc);
                }
            }
            proc_busy = false;
        }

        public  void Close()
        {
            if (serialPort != null)
            {
                serialPort.Close();
                serialPort = null;
            }

            if (task_proc != null && (!task_proc.IsCompleted))
                task_proc.Wait();
            task_proc = null;

            if (task_rx != null && (!task_rx.IsCompleted))
                task_rx.Wait();
            task_proc = null;
             
            if (task_tx != null && (!task_tx.IsCompleted))
                task_tx.Wait();           
            task_tx = null;
        }
    }

    public class PLT1UsbPorts : List<PLT1UsbPort>
    {

    }
}
