using OpenWLS.Server.Base;

namespace OpenWLS.Server.LogInstance.Edge
{
 //   public enum UsbPortState { NotAvailable = 0, Disconnected = 1, Connected = 2 };
    public interface IEdgeDevice
    {
     //   void SendPortInforsToClient();
        Device GetDevice();
        bool IsCompatible(DBase.Models.LocalDb.Edge edge);
        bool? IsToolPowerOn();
    }
  
    public enum DeviceType { Playback = 1, Simulator = 2, PLT1Tester = 0x10, PLT1Ap = 0x11 };
  
    public class DeviceBase
    {
        public int Id { get; set; }       
        public DeviceType Type { get; set; }
    //    public virtual bool  Connected { get; set; }
        protected virtual byte[]? GetAssetBytes()
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

    }

    public class Device : DeviceBase
    {

        public const ushort msg_dev_log_state = 1;


        protected bool hasDepth;
        protected LogInstanceS logInstance;
        public Instrument.Instrument Instrument { get; set; }
   //     public Edge Edge { get; set; }
        public  bool HasDepth { get { return hasDepth;  } }

        protected virtual void ProcessPortList(byte[]? bs)
        {
        }

        protected virtual void OnInit()
        {

        }

        public void Init( int id, LogInstanceS li)
        {
            Id = id;
            logInstance = li;
       //     Edge = li.Edge;
            OnInit();
        }

        public virtual byte[]? GetDeviceInforBytes()
        {
            return null;
        }
        public virtual void ProcMsgFromDevice(byte[]? bs)
        {

        }

        public virtual void ProcMsgFromInsts(byte[]? bs)
        {
            //int iid = r.ReadInt32();
            //Server.LogInstance.Instrument.Instrument? inst = logInstance.Insts.Find(a => a.Id == iid);
            //if (inst != null)
            //     inst.ProcPackageFromInst(r);
        }
        public  void ProcInstsPackage(byte[]? bs)
        {
            if (bs == null) return;
            DataReader r = new DataReader(bs);
            int iid = r.ReadInt32();
            Server.LogInstance.Instrument.Instrument? inst = logInstance.Insts.Find(a => a.Id == iid);
            if (inst != null)
                 inst.ProcGuiMsg(r);
        }
        public virtual void OnConnect()
        {

        }

        public virtual void StartLog( bool depthIndex, double? from, double? to)
        {

        }

        public virtual void StopLog()
        {

        }

        public virtual void Close()
        {
            StopLog();
            //   base.OnClosePort();
        }
       

    }

}
