using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using OpenWLS.Server.LogInstance.Edge;
using OpenWLS.Edge.Simulator.PLT1;
using OpenWLS.PLT1.Edge;
using OpenWLS.Edge.PLT1;
using OpenWLS.Server.Base;

namespace OpenWLS.Edge
{
    public class EdgeServer 
    {
        public static void WriteLine(string z) {
            Task.Run(() => Console.WriteLine(z));
                }
        TcpListenPort tcpListenPort;
        EdgeClients clients;
        Devices devices;

        public EdgeClients Clients { get { return clients; } }
        public Devices Devices { get { return devices; } }  
        public bool StopServer
        {
            get { return tcpListenPort.StopListen; }
        }
        public EdgeServer()
        {
            clients = new EdgeClients();
            devices = new Devices();
        }

        public void StartServer()
        {
            tcpListenPort = new TcpListenPort("127.0.0.1", Server.LogInstance.Edge.Edge.port_nu);
            tcpListenPort.StartListen(this);
            //tcpListenPort.ListenLoop(this);
        }

        /*
        public void ScanDevices(DataReader r)
        {

            List<Device> devs_used = devices.Where(a => (a.State & DeviceState.Used) != 0).ToList();
            Devices devs = new Devices();
            devs.AddRange(devs_used);
            DeviceAp.Scan(devs);
            devices = devs;
        }
        */
    }

    public class TcpListenPort
    {
        string ip_address;
        int port;
        bool stopListen;

        TcpListener listener;
        EdgeServer server;

        public bool StopListen
        {
            get { return stopListen; }
            set { stopListen = value; }
        }

        public TcpListenPort(string ipAddress, int port)
        {
            this.ip_address = ipAddress;
            this.port = port;
            //     this.server = server;
        }

        public async void ListenLoop(EdgeServer server)
        {
            this.server = server;
            stopListen = false;
            listener = new TcpListener(IPAddress.Parse(ip_address), port);
            listener.Start();
            EdgeServer.WriteLine($"Started listen {ip_address} at port {port}.");
            while (!stopListen)
            {
                try
                {
                    TcpClient tc = await listener.AcceptTcpClientAsync();
                    EdgeClient ec = new EdgeClient(tc.Client, server);

                    EdgeServer.WriteLine($"Accepted {tc.Client.RemoteEndPoint}.");
                    ec.StartRx(server);
                    server.Clients.Add(ec);
                    ec.Closed += ClientClosed;

                }
                catch (Exception ex)
                {
                    EdgeServer.WriteLine($"Edge port error: {ex.Message}");
                }

            }
            EdgeServer.WriteLine("Edge Stoped");
        }

        private  void ClientClosed(object s, EventArgs e)
        {
             server.Clients.Remove((EdgeClient)s);
            EdgeServer.WriteLine($"Closed client {((EdgeClient)s)}.");
        }

        public void StartListen(EdgeServer server)
        {
            var t = Task.Run(() => ListenLoop(server));
            t.Wait();
        }

        public int StopProc()
        {
            stopListen = true;
            listener.Stop();
            return 0;
        }
    }
}
