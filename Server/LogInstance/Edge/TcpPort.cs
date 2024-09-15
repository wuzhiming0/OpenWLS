using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using OpenWLS.Server.Base;
using Microsoft.Extensions.Hosting;

namespace OpenWLS.Server.LogInstance.Edge
{
    public class TcpPort : DataPort
    {
        public event EventHandler<bool?> PortConnected;
        protected Socket sock;

        public static int RxBytes(Socket sock, byte[] bs, int offset, int size)
        {
            int s = size;
            int o = offset;
            int k;
            while (s > 0)
            {
                k = sock.Receive(bs, o, s, SocketFlags.None);
                if (k <= 0)
                    return k;
                o += k;
                s -= k;
            }
            return size;
        }

        public void SetIpAddrPort()
        {

        }
        public bool Connect(string ip_addr, int port)
        {
            stop_port = false;
            sock = new Socket(SocketType.Stream, ProtocolType.IP);
            string err = null;
            try
            {
                var result = sock.BeginConnect(IPAddress.Parse(ip_addr), port, null, null);
             //   System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                bool success = result.AsyncWaitHandle.WaitOne(2000, true);
                sock.EndConnect(result);
                if (sock.Connected)
                {
                    connected = true;//"Client Connected"
                    port_stopped = false;
                    PortConnected? .Invoke(this, true);
                    return true;
                }
            }
            catch (Exception e)
            {
                err = e.Message;
            }
            //DaConsole.WriteLine("Failed to connect, " + err);
            sock.Close();
           // System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
            return false;
        }

        public void Close()
        {
            PortConnected?.Invoke(this, false);   
            sock.Close();
            connected = false;
            port_stopped = true;
            sock = null;
        }

        public virtual void RxLoop(IPackageProc proc) { }
    }
   


   

}
