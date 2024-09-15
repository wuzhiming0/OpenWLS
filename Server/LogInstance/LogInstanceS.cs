using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;



using OpenWLS.Server.LogInstance.Instrument;

//using OpenLS.PLT1;
//using OpenLS.PLT1.DataPort;
using Newtonsoft.Json;

using OpenWLS.Server.LogInstance.Calibration;

using OpenWLS.Server.Base;
//using OpenWLS.Server.LogDataFile.Models;
using OpenWLS.Server.LogInstance.RtDataFile;
using OpenWLS.Server.DBase.Repositories.InstrumentM;
using OpenWLS.Server.DBase;
using OpenWLS.Server.DBase.Models.GlobalDb;
using Microsoft.AspNetCore.Components.Forms;
using OpenWLS.Server.WebSocket;
using OpenWLS.Server.LogDataFile;
using OpenWLS.Server.LogDataFile.Models.NMRecord;
using OpenWLS.Server.DBase.Models.LocalDb;
using System.Diagnostics;
using OpenWLS.Server.LogInstance.OperationDocument;
using OpenWLS.Server.GView.Models;
using System.Net.WebSockets;
using OpenWLS.Server.LogInstance.Edge;

namespace OpenWLS.Server.LogInstance
{
    public class LogInstanceS : LogInstance
    {
        Edge.Edge? edge;
        Instruments insts;
        Measurements ms;
        FrameRts frames;
        AcqItems acqItems;
        Depth depth;
        Time time;

        DataFileRt dataFile;
        CVProc cv_proc;
        CVManProc cv_man_proc;
        Job job;

        public OperationDocument.OperationDocument OpDoc { get; set; }
        public AcqItems ActItems { get { return acqItems; } }
        public Instruments Insts { get { return insts; } }   
    //    public OperationDocument.InstSubs Subs { get; set; }
        public Edge.Edge? Edge { 
            get { return edge; }
        }

        public Measurements Measurements { get { return ms; } }

        public DataFileRt DataFile { get { return dataFile; } }

        public Depth Depth { get { return depth; } }
        public Time Time { get { return time; } }

        public LogIndexMode LogMode { get; set; }
        public bool Record { get; set; }
        public bool CVMode { get; set; }
        public bool SimulationEdge { get; set; }
        public string? PlaybackFn { get; set; }
        public LogInstanceWsClients WsClients { get; set; }
        public override int ConnectedClients { get { return WsClients.Count; }
            set { }
        }
            

        LiState state;
        //   string jobName;

        // public string JobName { get { return jobName; } }
        public override LiState State
        {
            get { return state; }
            set
            {
                state = value;
                WsClients.SendPackage(ClientType.Inst, GetStateWsBytes());
            }
        }

        public byte[] GetStateWsBytes()
        {
            DataWriter w = new DataWriter(6);
            w.WriteData((ushort)LiWsMsg.LiState);
            w.WriteData((ushort)state);
            if(Record)
                w.WriteData((ushort)1);
            else
                w.WriteData((ushort)0);
            return w.GetBuffer();
        }

        public byte[] GetEdgeDeviceWsBytes()
        {
            byte[]? bs = edge == null || edge.Device==null? null: edge.Device.GetDeviceInforBytes();
            return Server.LogInstance.Edge.Edge.GetEdgeDeviceWsBytes(bs);
        }
        public SystemIndex Index
        {
            get
            {
                if (LogMode == LogIndexMode.Time || CVMode)
                    return Time;
                else
                    return Depth;
            }
        }

        public CVProc CVProc {  get { return cv_proc; }  }
        public CVManProc CVManProc
        {
            get
            {
                return cv_man_proc;
            }
        }

        public static LogInstanceS Create(int ocf_id, Job job, OperationDocument.OperationDocument doc, DBase.Models.LocalDb.Edge? edge_db, int sim)
        {
            LogInstanceS li = new LogInstanceS()
            {
                Id = ServerGlobals.logInstances.Count == 0 ? 1 : ServerGlobals.logInstances.Max(a => a.Id) + 1,
                OcfId = ocf_id,
                job = job,
                SimulationEdge = sim != 0
            };
            li.Init(doc);
            if (edge_db != null)
            {
                li.CreateEdge(edge_db);
                System.Diagnostics.Debug.WriteLine($"Created LI {li.Id}, Simulation:{sim}.");
            }
            else
                System.Diagnostics.Debug.WriteLine($"Failed to created li.");
            ServerGlobals.logInstances.Add( li );
            return li;

        }
        public static LogInstanceS? Playback(string fn)
        {
            /*
            DataFile? df = Server.LogDataFile.DataFile.OpenDataFile(fn, null);
            if (df == null) return null;

            df.LoadFileInfor();

            //load operation doc
            NMRecord? nmr = df.NMRecords.Where(a=> a.RType == (int)NMRecordType.OpDoc ).LastOrDefault();
            if(nmr == null) return null;
            byte[] bs = df.GetNmObj(nmr.Id);
            if(bs == null)return null;
            OperationDocument.OperationDocument? doc = Newtonsoft.Json.JsonConvert.DeserializeObject<OperationDocument.OperationDocument>( UTF8Encoding.UTF8.GetString(bs));
            if(doc == null) return null;
            */
            LogInstanceS li = new LogInstanceS();
            li.PlaybackFn = fn;

            /*
             li.Init(doc);

            //load calibration
            nmr = df.NMRecords.Where(a => a.RType == (int)NMRecordType.Calibration).LastOrDefault();
            if (nmr == null) return null;
            bs = df.GetNmObj(nmr.Id);
            if (bs == null) return null;
            */

            return li;
        }
        public static LogInstance Stop(int id)
        {
            LogInstanceS? li_res = ServerGlobals.logInstances.Where(a => a.Id == id).FirstOrDefault();
            li_res.Close();
            ServerGlobals.logInstances.Remove(li_res);
            return li_res;
        }
        public LogInstanceS()
        {
         //   ms = new Measurements();
         //   dataFile = new DataFileRt(this);
            time = new Time();
            depth = new Depth();

            WsClients = new();
            cv_man_proc = new CVManProc(this);
         //   State = LiState.Blank;
        }

        public void Init(OperationDocument.OperationDocument doc)
        {
            OpDoc = doc;
            ms = Measurements.CreateMeasurements(doc.Measurements);
            insts = Instruments.CreateInstruments(doc, this, null);
            acqItems = doc.ACT;
            frames = new FrameRts(acqItems, ms);
          //  State = LiState.Log_NoEdge;
        }

        void CreateEdge(DBase.Models.LocalDb.Edge edge_db)
        {
            if (SimulationEdge)
            {
                // simulation supported by std edge
                if (edge_db.EType == null || edge_db.EType == (int)EdgeType.OpenWLS_Std)
                    edge = Server.LogInstance.Edge.Edge.CreateEdge(this, edge_db);
            }
            else
            {
                // check edge Compatiblity
                IEdgeDevice? ed = (IEdgeDevice?)Insts.Where(a=>a is IEdgeDevice).FirstOrDefault();
                if(ed != null)
                {
                    if(ed.IsCompatible(edge_db))
                        edge = Server.LogInstance.Edge.Edge.CreateEdge(this, edge_db);
                }
            }
            //connect if edge created
            if (edge != null)
            {
                edge.EdgeStateChanged += Edge_EdgeStateChanged;
                edge.Connect();

                if (edge.Connected)
                {
                    if (SimulationEdge)
                        edge.RequestNewSimulator(OpDoc.GetInstsOnlyJsonString());
                    else
                    {
                        if (PlaybackFn != null)
                            edge.RequestNewPlayback(PlaybackFn);
                        else
                        {
                            IEdgeDevice? ed = (IEdgeDevice?)insts.Where(a => a is IEdgeDevice).FirstOrDefault();
                            if (ed != null )
                            {
                                edge.Device = ed.GetDevice();
                            }
                            edge.RequestNewPhysicalDevice();
                        }

                    }
                }
            }
            Edge_EdgeStateChanged(edge, null);
        }
        
        private void Edge_EdgeStateChanged(object? sender, bool? e)
        {
            if (edge == null)
                State = PlaybackFn == null ? LiState.Log_NoEdge : LiState.Playback_NoEdge;
            else
            {
                if (edge.Connected)
                    State = PlaybackFn == null ? LiState.Log_EdgeConnected : LiState.Playback_EdgeConnected;
                else
                    State = PlaybackFn == null ? LiState.Log_EdgeDisconnected : LiState.Playback_EdgeDisconnected;
            }
        }

        public void SendDlDevicePackage(byte[]? bs)
        {
            if(edge != null)
                edge.SendPackageToDevice(bs);
        }

        public void SendDlInstPackage(byte[] bs)
        {
            if(edge != null)
                edge.SendPackageToInsts(bs);
        }

        public void CreateMGroups(OperationDocument.AcqItems ACT,   ISyslogRepository syslog)
        {
            cv_proc = new CVProc(this, Insts.GetCVSensors());
        }


        #region State
        public void StartEdit()
        {
            State = LiState.Edit;
        }

        public void StartCV()
        {
            //to-do;
            State = LiState.CV;
        }

        void StopCV()
        {
            //to -do
            State = state == LiState.Playback ? LiState.Playback_Standby : LiState.Log_Standby;
        }
        
        public void StopLog()
        {
            if (State != LiState.Log && State != LiState.Playback)
                return ;
            dataFile.StopLog();
            if(edge != null)
                edge.StopLog();
            State = state == LiState.Playback? LiState.Playback_Standby : LiState.Log_Standby;
        }

        public void StartLog( LogIndexMode lm, bool record )
        {

            LogMode = lm;
            OpenWLS.Server.LogDataFile.Models.IndexUnit u = Server.LogDataFile.Index.ConvertToIndexUnit(Index.Unit);
            bool indexIncreasing = DataType.IndexIncreasing(LogMode);
            bool depthAsIndex = LogMode != LogIndexMode.Time;

            frames.StartLog(depthAsIndex);


            if (record)
            {             
                dataFile = new DataFileRt(this);
                dataFile.StartLog(lm,  job);
                foreach(Measurement m in ms)
                {
                    if (m.Record != null)
                        dataFile.Measurements.Add(m.MeasurementDf);
                }
            }

            edge.StartLog(depthAsIndex, null, null);

            double index  = depthAsIndex? depth.Value : time.Value;
            foreach (LogInstanceWsClient c in WsClients)
                c.VdDocRts.StartLog(Depth,Time, indexIncreasing);

            State = state == LiState.Playback_Standby? LiState.Playback :  LiState.Log;

        }
        #endregion
       
        public  bool  Close()
        {
            if( State == LiState.Log || State == LiState.CV ||  State == LiState.Playback  )
                return false;

            if( edge != null )
                edge.Close();
            WsClients.Close();
            ServerGlobals.logInstances.Remove( this );
            System.Diagnostics.Debug.WriteLine($"LI {Id} stopped.");
            return true;

        }
        public string GetJsonString()
        {
            LogInstance li = new LogInstance();
            li.CloneFrom( this );
            return Newtonsoft.Json.JsonConvert.SerializeObject(li);
        }


    }

    public class LogInstanceSs : List<LogInstanceS>
    {

    }


}
/*
 * title Start LogInstance 

participant Client
participant Server
Edgeparticipant Edge

participant Edge

Client->Server:Start LogInstance(Op Doc)
Client<--Server:LogInstance
Client->Server:Get Edges
Client<-Server:Return Edges
Client->Server:Select Edge

Server->Server:Connect
Server->Edge:Get Device List
Server<-Edge:Device List
Client<-Server:Device List
Client->Server:Select Device
Server->Edge:info
Server<-Edge:info
Client<-Server:info
 * */