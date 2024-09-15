using OpenWLS.Server.Base;
using System.Net.Sockets;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OpenWLS.Server.LogInstance.Edge
{
    public class OpenWLSTcpPort : TcpPort
    {
        // package format : start_sync,  length, body
        static ushort sync_word_begin = 0x5A5A;
  //      static ushort sync_word_end = 0xA5A5;
        public const int port_nu = 0x4321;
        /*   public bool LittleEndian
           {
               set
               {
                   hr.SetByteOrder(value);
                   hw.SetByteOrder(value);
               }
           }*/
        //   DataWriter hw;
        //    DataReader hr;
        byte[] hw_bs = new byte[] { 0x5a, 0x5a, 0, 0, 0, 0 };
        enum RxState { reset = 0, sync1, sync2,  length, msg_code, body };

        public OpenWLSTcpPort()
        {
            Init();
        }
        public OpenWLSTcpPort(Socket socket)
        {
            sock = socket;
            Init();
        }

        void Init()
        {
            tx_queue = new Queue<Package>(tx_queue_max);
            rx_queue = new Queue<Package>(rx_queue_max);
        //    hr = new DataReader(2);
        //    hw = new DataWriter(5);

         //   hw.WriteData(sync_word_begin);
            connected = true; 
            port_stopped = false; 
            stop_port = false;
        }



        public override void RxLoop(IPackageProc proc)
        {
            Package rx_p = null;
            packageProc = proc;
            RxState state = RxState.reset;
            byte[] bs = new byte[2];
            int s = 0;
            int length = 0;
            while (!stop_port)
            {
                switch (state)
                {
                    case RxState.sync1:
                        s = sock.Receive(bs, 0, 1, SocketFlags.None);
                        if (s == 1 && bs[0] == 0x5A)
                            state = RxState.sync2;
                        if (s <= 0)
                            return;
                        break;
                    case RxState.sync2:
                        s = sock.Receive(bs, 0, 1, SocketFlags.None);
                        if (s == 1 && bs[0] == 0x5A)
                            state = RxState.length;
                        if (s <= 0)
                            return;
                        break;
                    case RxState.length:
                        s = RxBytes(sock, bs, 0, 2);
                        if (s <= 0)
                            return;
                        length = BitConverter.ToUInt16(bs);
                        state = RxState.msg_code;
                        break;
                    case RxState.msg_code:
                        s = sock.Receive(bs, 0, 1, SocketFlags.None); 
                        if (s <= 0)  return;
                        rx_p = new Package   {   Code = bs[0]    };
                        if (length == 1)
                        {
                            rx_queue.Enqueue(rx_p);
                            if (!proc_busy)
                                Task.Run(() => { ProcLoop(); });
                            state = RxState.sync1;
                        }
                        else
                        {
                            length--;
                            rx_p.Body = new byte[length];
                            state = RxState.body;
                        }
                        break;
                    case RxState.body:
                        s = RxBytes(sock, rx_p.Body, 0, length);
                        if (s <= 0)
                            return;

                        rx_queue.Enqueue(rx_p);
                        if (!proc_busy)
                            Task.Run(() => { ProcLoop(); });
                        state = RxState.sync1;
                        break;
                    default:    
                    //case RxState.reset:
                        state = RxState.sync1;
                        break;
                }
            }
            Close();
        }

        protected override bool TxData(Package package)
        {
            if ((!connected) || port_stopped || stop_port)
                return false;
            int length = 1;
            if (package.Body != null)
                length += package.Body.Length;

            hw_bs[2] = (byte)length;
            hw_bs[3] = (byte)(length>>8);
            hw_bs[4] = (byte)package.Code;
            try
            {
                if (package.Body != null)
                {
                    byte[] bs = new byte[4 + length];
                    Buffer.BlockCopy(hw_bs, 0, bs, 0, 5);
                    Buffer.BlockCopy(package.Body, 0, bs, 5, length - 1);
                    sock.Send(bs);
                }
                else
                    sock.Send(hw_bs);                
                return true;
            }
            catch (Exception e)
            {
                stop_port = true;
            }
            //closesocket(sockfd);

            return false;
        }
    }
}
