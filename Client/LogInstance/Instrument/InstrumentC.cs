using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using OpenWLS.Server.LogInstance.Instrument;
using System.Data;
using System.Data.SQLite;
using OpenWLS.Server.Base;
using OpenWLS.Server.DBase.Models.GlobalDb;
using OpenWLS.Server.LogInstance.OperationDocument;
using System.Windows.Controls;
using OpenWLS.Client.Base;
using OpenWLS.Client.Requests;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OpenWLS.Client.LogInstance.Instrument
{
    public enum IdentifyInstState { Unkown = 0, NotVerified = 1, ToByAsset = 2, ToByDefualt  = 3, Conflict = 4, Verified = 0x10 };
    public interface IEdgeDeviceC
    {
        EdgeDeviceCntl GetEdgeDeviceCntl();
        IdentifyInstState IdentifyInstruments();
        bool? IsToolPowerOn();
        void RequestConnectedInstInfors();
    }
    public class InstrumentC : InstrumentOd, INotifyPropertyChanged
    {

        //   protected string name;
        protected FrameworkElement cntl;
        [JsonIgnore]
        public bool Verified {  get; set; }


        [JsonIgnore]
        public FrameworkElement Cntl
        {
            //  set { cntl = value; }
            get { return cntl; }
        }
        static public InstrumentC? CreateInstOd(InstrumentOd inst_od)
        {
            InstrumentC? inst = CreateInstDb(inst_od);
            if (inst != null)
            {
                inst.Address = inst_od.Address;
                inst.Id = inst_od.Id;
            }            
            return inst;
        }

        public static InstrumentC? CreateInstDb(InstrumentDb inst_db)
        {
            string cname = $"OpenWLS.{inst_db.Category}.{inst_db.Name}.InstC{inst_db.Name}";
            if (inst_db.GuiDll == null)
                inst_db.GuiDll = inst_db.Category;
            InstrumentC inst = (InstrumentC)DataType.CreatObject(cname, $"{inst_db.GuiDll}.dll");
            if (inst != null)
            {
                inst.CopyFrom(inst_db);
            }
            return inst;
        }

       // protected string msg_head;
        protected List<MeasurementDisplay> displays;
        [JsonIgnore]
        public List<MeasurementDisplay> Displays
        {
            get { return displays; }
        }
        [JsonIgnore]
        public virtual string InstName { get { return $"{Name}:{Address}"; } }
        [JsonIgnore]
        public virtual System.Windows.Media.SolidColorBrush Color {
            get {
                if (Verified) return System.Windows.Media.Brushes.Green;
                else return System.Windows.Media.Brushes.Gray;
            } 
        }



        protected LiClientMainCntl liClient;
        public LiClientMainCntl LiClient { get { return liClient; } }

        public event PropertyChangedEventHandler? PropertyChanged;

        public InstrumentC()
        {
            displays = new ();
        }
        protected void OnPropertyChanged( string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public virtual void InitInst(LiClientMainCntl liClient)
        {
            this.liClient = liClient;
            OperationDocument doc = liClient.GetOperationDocument();
            InstSubs ss = SurfaceEqu == null ? doc.DhTools.Subs : doc.SfEquipment.Subs;
            Subs = new InstSubs(ss.Where(a=>a.IId == Id).ToList());

         //   main_sub = Subs.Count == 1 ? Subs[0] : Subs.Where(s => s.Main != null).FirstOrDefault();
        }

        public virtual void CreateCntl(){

        }

        public virtual void ResetInst()
        {

        }

        public void ProcInstMsg(DataReader r)
        {
            ushort m_type = r.ReadUInt16();
            ushort s = r.ReadUInt16();
            byte[]? bs = s == 0? null : r.ReadByteArray(s);
            switch(m_type)
            {
                case (ushort)InstGuiMsgType.InstCntl:
                    ProcInstCntlGuiMsg(bs);
                    break;
                case (ushort)InstGuiMsgType.ProcParameter:
                    ProcParameterGuiMsg(bs);
                    break;
              //  case (ushort)InstGuiMsgType.EdgeDevice:
              //      ProcEdgeDeviceMsg(bs);
              //      break;
                default:
                    ProcSpecialGuiMsg(m_type, bs);
                    break;
            }
        }

        public  void SendEdgeDeviceMsg(byte[]? bs)
        {
            liClient.SendEdgeDeviceGuiPackage(bs);
        }
        protected virtual void ProcSpecialGuiMsg(ushort msg_code, byte[]? bs) { }

        protected virtual void ProcInstCntlGuiMsg(byte[]? bs)
        {
        }
     /*   protected virtual void ProcEdgeDeviceMsg(byte[]? bs)
        {
        }*/
        protected virtual void ProcParameterGuiMsg(byte[]? bs)
        {

        }

        public override string ToString()
        {
            return Name;
        }
 
        public void Close()
        {
            cntl = null;
            foreach (MeasurementDisplay md in displays)
                if (md.DockControl != null)
                {
                    md.DockControl.CanClose = true;
                    md.DockControl.Close();
                }
            displays = null;
        }
    }

    public class InstrumentCs : List<InstrumentC>
    {
        public InstrumentCs() 
        { }

        void AddInst(InstrumentC inst)
        {
            List<InstrumentC> list = this.Where(a=> a.Name == inst.Name).ToList();
            inst.Seq = list.Count == 0 ? null : list.Count;
            inst.CreateCntl();
            Add(inst);
        }
        public InstrumentCs( InstrumentGroup inst_group) 
        {
            foreach (InstrumentOd inst_od in inst_group.Insts)
            {
                InstrumentC? ginst = InstrumentC.CreateInstOd(inst_od);
                if (ginst != null) AddInst(ginst);
            }
        }

    }
 
    public interface IMeasurementDisplay
    {
        //int[] GetMeasurementIds();
    }
}
