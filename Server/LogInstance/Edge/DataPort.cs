using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using OpenWLS.Server.Base;
using System.Reflection.Metadata.Ecma335;

namespace OpenWLS.Server.LogInstance.Edge
{
    public interface IPackageProc
    {
        void ProcPackage(Package p);
    }

    public class Package
    {
        public byte Code { get; set; }
        public byte[]? Body { get; set; }
    }

    public class DataPort
    {
        protected int tx_queue_max = 1024;
        protected int rx_queue_max = 1024;
        protected Queue<Package> tx_queue;
        protected Queue<Package> rx_queue;
        protected IPackageProc packageProc;
        protected bool connected;

        protected bool stop_port;
        protected bool port_stopped;
    //    protected bool port_closed;

        protected bool tx_busy;
        protected bool proc_busy;

        protected int rx_time_out;              // in ms;


        public string PortName { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public bool Connected
        {
            get
            {
                return connected;
            }
        }

        public DataPort()
        {
            connected = false;
            port_stopped = true;
            stop_port = false;
       //     port_closed = true;
            tx_busy = false;
            proc_busy = false;
        }

        public void TxLoop()
        {
            tx_busy = true;
            while (tx_queue.Count > 0)
            {
                Package? out_p;
                if(tx_queue.TryDequeue(out out_p))
                    TxData(out_p);
            }
            tx_busy = false;
        }

        protected void ProcLoop()
        {
            proc_busy = true;
            while (rx_queue.Count > 0)
            {
                Package? proc_p;
                if (rx_queue.TryDequeue(out proc_p) && proc_p != null)
                    packageProc.ProcPackage(proc_p);
            }
            proc_busy = false;
        }

        protected virtual void OnStopPort()
        {

        }

        public virtual bool Handshake(object ob)
        {
            return true;
        }


        public void StopPort()
        {
            stop_port = true;
            OnStopPort();
        }

        public bool SendPackage(Package package)
        {
            if (tx_queue.Count >= tx_queue_max)
                return false;
            tx_queue.Enqueue(package);
            if(!tx_busy)
                Task.Run(() => { TxLoop(); });  
            return true;
        }

        protected virtual bool TxData(Package package)
        {
            return false;
        }

    }

    
}
