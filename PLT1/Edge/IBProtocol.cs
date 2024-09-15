using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using OpenWLS.Server.Base;
using OpenWLS.Server.LogInstance;
using OpenWLS.Client.LogInstance.Instrument;
using System.Windows.Documents;
using OpenWLS.PLT1.GrA;
using System.Net;
using OpenWLS.Server.LogInstance.Instrument;
using OpenWLS.Server.LogInstance.OperationDocument;

namespace OpenWLS.PLT1.Edge
{
    /// <summary>
    /// Instrument Bus Protocol
    /// </summary>
    public class IBProtocol
    {
        //      public static IBProtocol? protocol;
        public const byte MSG_READ_MASK = 0x80;
        public const byte MSG_CODE_MASK = 0x7f;

        public const byte GLOBAL_ADDR = 0xff; //255
        public const byte EOF_BT_ADDR = 0xfe; //254
        public const byte EOF_NBT_ADDR = 0xfd; //253
        public const byte OpenWLS_ADDR = 0xfc; //252
        public const byte EDGE_DEV_ADDR = 0xfb; //251    
        public const byte S_MOD_ADDR = 0xfa; //250    
        public const byte D_TEL_ADDR = 0xf9;
        public const byte MEM_ADDR = 0xf0;
        public const byte RESERVED_ADDR = 0;
        public const byte GR_ADDR = 0x1;
        public const byte SGR_ADDR = 0x2;
        public const byte PTC_ADDR = 0x3;

        public static void SetCheckSum(byte[] bs)
        {
            byte b = 0; bs[bs.Length - 2] = 0;
            for (int i = 0; i < bs.Length; i++)
                b += bs[i];
            bs[bs.Length - 2] = b;
        }
        public static IdentifyInstState IdentifyInstruments(InstrumentCs insts)
        {
            //check if all verified
            InstrumentC? inst = insts.Where(a => !a.Verified).FirstOrDefault();
            if (inst == null) return IdentifyInstState.Verified;
            //check address assigned
            inst = insts.Where(a => a.Address == null).FirstOrDefault();
            if (inst == null)
            {
                List<int> ids = insts.Select(a => a.Id).ToList();
                if (ids.Count == ids.Distinct().Count())
                    return IdentifyInstState.NotVerified;
                else
                    return IdentifyInstState.Conflict;
            }
            //check asset assign
            inst = insts.Where(a => a.Subs[0].Asset != null).FirstOrDefault();
            if (inst == null)
                return IdentifyInstState.ToByAsset;
            //check default assign
            List<byte> addrs = insts.Select(a => ((PLT1InstrumentC)a).DefaultAddr).ToList();
            if (addrs.Count == addrs.Distinct().Count())
                return IdentifyInstState.ToByDefualt;
            return IdentifyInstState.Unkown;
        }


        public static void SendCmd(Server.LogInstance.Edge.Edge edge, byte addr_dst, byte m_type, bool bt)
        {
            edge.SendPackageToInsts(Frame.GetFrameBytes(addr_dst, OpenWLS_ADDR, m_type, bt));
        }

        public static void SendCmd(Server.LogInstance.Edge.Edge edge, byte addr_dst, byte cmd, byte[] paras)
        {
            edge.SendPackageToInsts(Frame.GetFrameBytes(addr_dst, OpenWLS_ADDR, cmd, paras, true));
        }

        ///////////////////////////////////////////////////////////////////////////
        public static void SendSetAddrToDefault(PLT1InstrumentC inst)
        {
            inst.SendInstMsg(IBProtocol.GLOBAL_ADDR, (byte) PLT1InstMsgCode.RESET_ADDR, false);
        }        
        /// <summary>
        /// ///
        /// </summary>
        /// <param name="inst"></param>
        public static void SendReadGenInfor(PLT1InstrumentC inst)
        {
            inst.SendInstMsg( (byte) PLT1InstMsgCode.GENERAL_INFOR | (byte) IBProtocol.MSG_READ_MASK, BitConverter.GetBytes(inst.Id));
        }


    }
    public enum PLT1InstMsgCode
    {
        Dummy = 0,                  //return hyper package including all measurement groups and all other messages in tool out buffer, including time stamp.
        GENERAL_INFOR = 1,          // address, serial, asset,   
        M_GROUP = 2,         
        ACQ_TBL = 3,
        ACQ_ITEM = 4,
        INST_CNTL_TBL = 5,
        INST_CNTL_ITEM = 6,

        ASSET = 0x10,
        CLOCK = 0x11,
        DFlash = 0x12,
        SysLog = 0x13,

        RESET_TOOL = 0x20,  
        ADDR_BY_ASSET = 0x21,
        RESET_ADDR = 0x22,
        START_ACQ = 0x23,
        STOP_ACQ = 0x24,

        UNLOCK_TOOL = 0x30,  // 
        LOCK_TOOL = 0x31,       //protect infromation like asset, cal... 

        // 0x40 -0x7f instrumrnt define
    }


}