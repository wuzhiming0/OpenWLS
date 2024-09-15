using OpenWLS.Client.LogInstance;
using OpenWLS.Client.LogInstance.Instrument;
using OpenWLS.PLT1.Edge;
using OpenWLS.Server.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;


namespace OpenWLS.PLT1.ApA
{
    public class InstCApA : PLT1InstrumentC, IEdgeDeviceC
    {
        IdentifyInstState inst_addr_state;
        EdgeDeviceApCntl edCntl;
        public InstCApA()
        {
            Address = default_addr = IBProtocol.S_MOD_ADDR;
            genInfor = new PLT1InstGenInfor();
        }
        public override void CreateCntl()
        {
            cntlTbl = new CntlTblAp();
            cntl = new ApACntl();
            cntl.Name = Name;
            ((ApACntl)cntl).Inst = this;
            ((ApACntl)cntl).depthCntl.Inst = this;
            ((ApACntl)cntl).modemCntl.Inst = this;
            edCntl = new EdgeDeviceApCntl() { Inst = this };
        }
        public override void InitInst(LiClientMainCntl liClient)
        {
            base.InitInst(liClient);
            edCntl.LiClient = liClient;
            if (Address == null)
                Address = default_addr;
        }
        public override void ResetInst()
        {
            // ((GInstMemACntl)cntl).ResetStatus();
        }
        public void RequestConnectedInstInfors()
        {

        }
        public EdgeDeviceCntl GetEdgeDeviceCntl()
        {
            return edCntl;
        }
        public bool? IsToolPowerOn()
        {
            return null;
        }

        void SendVerifyRequest()
        {
            foreach (PLT1InstrumentC inst in liClient.Instruments)
                IBProtocol.SendReadGenInfor(inst);
        }

        protected override void ProcessOtherInstCntlBlock(Block b)
        {
            switch (b.Type)
            {

            }
        }

        public IdentifyInstState IdentifyInstruments()
        {
            inst_addr_state = IBProtocol.IdentifyInstruments(liClient.Instruments);
            switch(inst_addr_state)
            {
                case IdentifyInstState.ToByDefualt:
                    IBProtocol.SendSetAddrToDefault(this);
                    foreach (PLT1InstrumentC instr in liClient.Instruments)
                        instr.Address = instr.DefaultAddr;
                    SendVerifyRequest();
                    break;
                case IdentifyInstState.ToByAsset:
                    foreach ( PLT1InstrumentC instr in liClient.Instruments )
                        instr.SendInstMsg( IBProtocol.GLOBAL_ADDR, (byte)PLT1InstMsgCode.ADDR_BY_ASSET, PLT1InstGenInfor.GetBytes(instr) );
                 //   SendVerifyRequest();
                    break;
                case IdentifyInstState.NotVerified:
                    SendVerifyRequest();
                    break;
            }
            return inst_addr_state;
        }

        protected override void OnCntlTblChanged()
        {           
            if (CntlTbl == null) return;
            foreach (ushort offset in CntlTblAp.GetOffsets())
            {
                ((ApACntl)cntl).modemCntl.UpdateCntlTblItem(offset);
                ((ApACntl)cntl).depthCntl.UpdateCntlTblItem(offset);
            }
        }

        protected override void OnCntlTblItemChanged(ushort offset, int size)
        {
            if (CntlTbl == null) return;
            ((ApACntl)cntl).modemCntl.UpdateCntlTblItem(offset);
            ((ApACntl)cntl).depthCntl.UpdateCntlTblItem(offset);
        }

    }
}
