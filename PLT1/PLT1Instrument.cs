using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Newtonsoft.Json;
using OpenWLS.PLT1.Edge;
using OpenWLS.Server.Base;
using OpenWLS.Server.LogInstance;
using OpenWLS.Server.LogInstance.Instrument;
using OpenWLS.Server.LogInstance.OperationDocument;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace OpenWLS.PLT1
{
    public class PLT1Instrument : Server.LogInstance.Instrument.Instrument
    {
        public uint[] AssetNus { get; set; }
        public uint[] ModelNus { get; set; }
        protected byte default_addr;
        [JsonIgnore]
        public byte DefaultAddr { get { return default_addr; } }  //perfferred logical ID
        public PLT1Instrument()
        {

        }
        /*
        public override void Init(LogInstanceS li)
        {
            base.Init(li);

        }*/
  
        public  void ProcFrameFromInst(Frame p)
        {
            foreach (Block b in p.Blocks)
            {
                switch (b.Type)
                {
                    case (byte)PLT1InstMsgCode.M_GROUP:
                        ProcessMGroup(b.Body);
                        break;
                    case (byte)PLT1InstMsgCode.GENERAL_INFOR:
                        ProcessGenInfor(b);
                        break;
                    default:

                        break;
                }
            }
        }
        void ProcessGenInfor(Block b)
        {
            PLT1InstGenInfor instGenInfor = new PLT1InstGenInfor();
            instGenInfor.Restore(b.Body);
            instGenInfor.UpdateAssetOfSubs(Subs);
            SendInstBlockToClient(b);
        }

        void SendInstBlockToClient(Block b)
        {
            SendInstMsgToClient((ushort)InstGuiMsgType.InstCntl, b.GetBytes());
        }

        void ProcessMGroup(byte[]? bs)
        {
            if (bs == null) return;
            DataReader r = new DataReader(bs);
            byte mg = r.ReadByte();
        }

        void ProcInstCntlFrame(Frame f) {
            if (f.Checksum)
            {
                foreach (Block b in f.Blocks)
                {
                    switch (b.Type)
                    {
                        case (byte)PLT1InstMsgCode.RESET_ADDR:
                            Address = default_addr;
                            break;

                    }
                }
            }        
        }
        protected override void ProcInstCntlGuiMsg(byte[] bs)
        {
            Frame f = Frame.ReadFrame(new DataReader(bs));
            if (f.DstAddress == IBProtocol.GLOBAL_ADDR)
            {
                foreach(PLT1Instrument inst in logInstance.Insts)
                    inst.ProcInstCntlFrame(f);
            }
            else
                ProcInstCntlFrame(f);
        }

    }


}
/*
 tool EEPROM
 General information
 8:Asset
 4:firmware date, year(2), month(1),day(1) 
 4:latest FMN, year(2), month(1), seq(1)

 operation parameters

 Whiteboard - re-write
 Notes - only add

 calibration/verification records 
 1:Selected
 8:Unit: GR
 1:Level: MC,BC,AC
 6:DateTime
 8:Asset
 2:Size
 N: CV Bolb
 */



/*
 Frame structure
     Dest address(a)
     Src address (d)
     Frame body  (d)  
     Dest address(a)

 Command Frame body 
     Command
     Command Parameters (optional)

 Command Response Frame body 
     Status
     Command (optional)
     Command Response Body (optional)

*/


/* 

 mem tool
 Mem chain: one only one is Buscontroller; recording chain.
 When previous mem is AlmostFull, it start recording 

 Status: Recording, AlmostFull, Full  
 */