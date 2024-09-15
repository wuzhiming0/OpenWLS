using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;



using OpenWLS.Server.LogInstance.Instrument;
using Newtonsoft.Json;

using OpenWLS.Server.LogInstance.Calibration;

using OpenWLS.Server.Base;
using OpenWLS.Server.LogInstance.OperationDocument;
//using OpenWLS.Server.LogDataFile.Models;
using OpenWLS.Server.LogInstance.RtDataFile;
using OpenWLS.Server.DBase.Repositories.InstrumentM;
using OpenWLS.Server.DBase;
using OpenWLS.Server.DBase.Models.GlobalDb;
using Microsoft.AspNetCore.Components.Forms;
using OpenWLS.Server.WebSocket;

namespace OpenWLS.Server.LogInstance
{
    public enum LiMode { Log = 0, Playback = 1, Edit = 2};

    public enum LiState { 
        Blank = 0,  
        Loading = 1, 
        Edit = 0x10, 
        Log_NoEdge = 0x20, Log_EdgeDisconnected = 0x21, Log_EdgeConnected = 0x22,  Log_EdgeDeviceConnected = 0x23,
        Log_Powered = 0x25, Log_IdChecked = 0x26, Log_Standby = 0x27, Log = 0x28, CV = 0x29,
        Playback_NoEdge = 0x30, Playback_EdgeDisconnected = 0x31, Playback_EdgeConnected = 0x32, Playback_Standby = 0x33,  Playback = 0x34
    };


    public enum LiWsMsg
    {
        Connect = 1,  LogInstance = 2, Disconnect = 3, Close = 4, 
        OperationDoc = 0x10, LiState = 0x11, CV = 0x12,
        //CVMan = 0x13, post process, CV manager, move to API request
        NewGView = 0x20, UpdateGView = 0x21, RemoveGView = 0x22, Gv = 0x23,
        NewMv1Ds = 0x30, RemoveMv1Ds = 0x31,
        NewMvxDs = 0x40, RemoveMvxDs = 0x41,
        Inst = 0x50, EdgeDev = 0x51            
    };


    public class LogInstance 
    {                                             
        //public string SelectedCategory { get; set; }
        public int Id { get; set;  }
        public int OcfId { get; set;  }      
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual LiState State { get; set; }
        public virtual int ConnectedClients { get; set; }

        public LogInstance()
        {

        }

        public void CloneFrom(LogInstance from)
        {
            Id = from.Id;
            Name = from.Name;
            Description = from.Description;
            State = from.State;
            ConnectedClients = from.ConnectedClients;
        }
      



    }

    public class LogInstances : List<LogInstance>
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