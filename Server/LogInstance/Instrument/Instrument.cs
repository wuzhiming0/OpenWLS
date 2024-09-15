using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using System.Xml;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

using System.Data.SQLite;
using OpenWLS.Server.LogInstance.Calibration;
using OpenWLS.Server.LogInstance;


using OpenWLS.Server.DBase;
using OpenWLS.Server.Base;
using OpenWLS.Server.LogInstance.RtDataFile;
using OpenWLS.Server.DBase.Repositories.InstrumentM;
using OpenWLS.Server.DBase.Models.GlobalDb;
using OpenWLS.Server.LogInstance.OperationDocument;
using OpenWLS.Server.WebSocket;
using OpenWLS.Server.LogDataFile.Models;
//using Parameter = OpenWLS.Server.Base.Parameter;

namespace OpenWLS.Server.LogInstance.Instrument
{
    public interface IParameterOwner
    {
        void SetParameter(Parameter para);
        void SaveParameters(Parameters paras);
    }


   // [Flags]
   // public enum InstStatus { AddrVerified = 1, AddrChanged = 2, AssetMatched = 4 };

    public enum InstGuiMsgType { 
        InstCntl = 1,           // gui -> server -> edge -> instrument  
        ProcParameter = 2,      // gui -> server 
    //    EdgeDevice = 3          // gui -> server -> edge 
    };
    public class Instrument : InstrumentOd, IParameterOwner
    {
        public string Alias { get; set; }
        protected LogInstanceS logInstance;


    //    protected UInt32 status;
 
        public LogInstanceS LogInstance { get { return logInstance; } }

        CVInstrument cv;
        public CVInstrument CVInst { get { return cv; } }

     

        public static Instrument? CreateInst(InstrumentOd inst_od, LogInstanceS li )
        {
            string cn = $"OpenWLS.{inst_od.Category}.{inst_od.Name}.Inst{inst_od.Name}";
            string dn = inst_od.ApDll == null? inst_od.Category : inst_od.Category;
            Instrument inst = (Instrument)DataType.CreatObject(cn, $"{dn}.dll");
            if (inst == null) return null;
            inst.CopyFrom(inst_od);
            inst.Id = inst_od.Id;
            inst.Address = inst_od.Address;
            inst.Desc = inst.Name + "-" + inst.Address.ToString();
            inst.Init(li);
            return inst;
        }



        public Instrument()
        {

        }

        public virtual void Init(LogInstanceS li)
        {
            logInstance = li;
            InstSubs ss = SurfaceEqu == null ? li.OpDoc.DhTools.Subs : li.OpDoc.SfEquipment.Subs;
            Subs = new InstSubs(ss.Where(a => a.IId == Id).ToList());
        }

        public  void ProcGuiMsg(DataReader r)
        {
            ushort msg_code = r.ReadUInt16();
            ushort s = r.ReadUInt16();
            byte[] bs = r.ReadByteArray(s);
            switch (msg_code)
            {
                case (ushort)InstGuiMsgType.InstCntl:
                    ProcInstCntlGuiMsg(bs);
                    if (logInstance != null)
                        logInstance.SendDlInstPackage(bs);  // forward to edge 
                    break;
                case (ushort)InstGuiMsgType.ProcParameter:
                    ProcParameterGuiMsg(bs);
                    break;

                default:
                    ProcSpecialGuiMsg(msg_code, bs);
                    break;
            }
        }
        protected virtual void ProcSpecialGuiMsg(ushort msg_code, byte[] bs)
        {

        }
        protected virtual  void ProcInstCntlGuiMsg(byte[] bs)
        {

        }


        protected virtual  void ProcParameterGuiMsg(byte[] bs)
        {

        }
        /*
        virtual public void ProcessUplinkPackage(DataReader r, ushort size)
        {

        }
        */
        virtual public void SetParameter(Parameter para)
        {

        }

        virtual public void SaveParameters(Parameters paras)
        {

        }

        virtual protected void OnStartLog(bool depthIndex, bool record, double? from, double? to )
        {

        }

        public void StartLog( bool depthIndex, bool record, double? from, double? to)
        {
         //   dgs_active.StartLog(depthAsIndex, indexIncreasing, ar);
            OnStartLog( depthIndex, record, from, to);
        }
        

        virtual public void ProcessParameter(string name, string value)
        {

        }        
        /*
        virtual public void ProcessFlashPages( DataReader r, ushort size)
        { 
            
        }
        */
               
        public static void SetVal(Measurement dc,  double v)
        {
          //  dc.Raw = raw;

            dc.AddSample(v);
        }



        virtual public void OnConnected()
        {

        }

        
        virtual protected void InitCV(CVInstrument cvInst)
        {
            cv = cvInst;
            if(cv!= null)
                cv.Inst = this;
        }

        public virtual bool ReadyToLog(bool record)
        {
            return true;
        }
        public virtual bool StoredCalInTool(string phase)
        {
            return false;
        }

        public virtual void UpLoadCalFromTool(string phase)
        {

        }

        public virtual void DownlodCalToTool(string phase)
        {

        }

        public virtual bool AbleLoadCalFromLogFile(string phase)
        {
            return false;
        }

        public virtual void LoadCalFromLogFile(string phase, string fn)
        {

        }

        public virtual void AfterUpdateCalPhase( CVPhase cvPhase)
        {

        }

        public void SendInstMsgToClient(ushort m_type, byte[]? bs)
        {
            int c = 10;
            if (bs != null) c += bs.Length;
            DataWriter w = new DataWriter( c );
            w.WriteData((ushort)LiWsMsg.Inst);          //2
            w.WriteData(Id);                            //4
            w.WriteData(m_type);                        //2
            if (bs == null)
                w.WriteData((ushort)0);                 //2
            else
            {
                w.WriteData((ushort)bs.Length);
                w.WriteData(bs);
            }

            logInstance.WsClients.SendPackage(ClientType.Inst, w.GetBuffer());
        }
    }

    public class Instruments : List<Instrument>{
        public static Instruments CreateInstruments(OperationDocument.OperationDocument doc, LogInstanceS li,  ISyslogRepository? syslog )
        {
            Instruments insts = new Instruments();
            foreach (OperationDocument.InstrumentOd inst_od in doc.DhTools.Insts)
                insts.Add(Instrument.CreateInst(inst_od, li ));
            foreach (OperationDocument.InstrumentOd inst_od in doc.SfEquipment.Insts)
                insts.Add(Instrument.CreateInst(inst_od, li));
            return insts;
        }

        public Instruments()
        {

        }

        public CVSensors GetCVSensors()
        {
            CVSensors ss = new CVSensors();
            int k = 0;
            foreach (Instrument inst in this)
            {
                if (inst.CVInst != null)
                {
                    foreach (CVSensor s in inst.CVInst.Sensors)
                    {
                        s.ID = k;     ss.Add(s);        k++;
                    }
                }
            }
            return ss;
        }
        public Instrument GetInstrument(string str)
        {
            byte lid;
            if (byte.TryParse(str, out lid))
            {
                foreach (Instrument inst in this)
                    if (lid == inst.Address)
                        return inst;
            }
            else
            {
                foreach (Instrument inst in this)
                    if (str == inst.FullName)
                        return inst;
            }
            return null;
        }
        /*
        public void ProcessFlashPages(ushort address,  DataReader r, ushort size)
        {
            Instrument? inst = GetInstrument(address);
            if (inst != null)
                inst.ProcessFlashPages(r, size);
        }

        public void ProcessDataGroup(ushort address,  DataReader r, ushort size)
        {
            Instrument inst = GetInstrument(address);
            if (inst != null)
                inst.ProcessDataGroup(r, size);
        }

        public void ProcessCmdResponse(ushort address, DataReader r, ushort size)
        {
            Instrument inst = GetInstrument(address);
            if (inst != null)
                inst.ProcessCmdResponse( r, size);
        }

        public void ProcessSystemLog(byte address, DataReader r)
        {

        }
        */
        public void OnConnected()
        {
            foreach (Instrument inst in this)
                inst.OnConnected();
        }

        public  bool ReadyToLog(bool record)
        {
            foreach (Instrument inst in this)
                if (!inst.ReadyToLog(record))
                    return false;
            return true;
        }
        /*
        public void StartLog(bool record, bool depthAsIndex, bool indexIncreasing,  DataFileRt ar)
        {
            foreach (Instrument inst in this)
                inst.StartLog(record, depthAsIndex, indexIncreasing, ar);            
        }
        
        public string Restore(string str)
        {
            string[] ss = str.Split('\n');
            foreach (string s in ss)
            {
                string s1 = s.Trim();
                if (s1.Length == 0)
                    continue;
                if (s1[0] == '*')
                    continue;
                Instrument inst = new Instrument();
                string err = inst.Restore(s1);
                if (err == null)
                    Add(inst);
                else
                    return err;
            }
            return null;
        }
      
        public void SetParameters(Parameters paras)
        {
            foreach(Parameter p in paras)
            {
                Instrument inst = GetInstrument(p.Owner);
                if (inst != null)
                    inst.SetParameter(p);
            }
        }
        public void SaveParameters(Parameters paras)
        {
            foreach (Instrument inst in this)
                inst.SaveParameters(paras);
        }  */



    }

}
