using OpenWLS.Server.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.DFlash
{
    public enum BlockType
    {
        //Flash Blocks
      //  Continued = 0,
        Root = 1, LogHead = 2, Gap = 3, 
   //     Instruments = 0x11, 
        AcqItems = 0x11,  
        Parameters = 0x12, 
        OCF = 0x13, 
        Calibrations = 0x14,

        StartStamp = 0x30,
        ParaChange = 0x31,
        ChannelData_CB = 0x32,          // no cross block 
        ChannelData_NCB = 0x33,

        DataGroup_CB = 0x51,            //cross block bountry            
        DataGroup_NCB = 0x52,
        MSG_CB = 0x53,                  //cross block bountry            
        MSG_NCB = 0x54,

        StopStamp = 0x70,
        LogEnd = 0x71

        //  Command = 0x11, CommandResponse = 0x12, DataGroup = 0x13, SystemLog = 0x14, OCF = 0x15,
        //  SimpleDataGroups = 0x20, ComplexDataGroups = 0x21,
        //  CommunicationPackage = 0x80
    };
    public enum DFVersion { Lv_1 = 1, Lv_G1 = 2 };

    public class DataBlock
    {
        public BlockType BlockType { get; set; }

        public SPageLocation PagePos { get; set; }
        public byte[] Val { get; set; }

        public virtual string GetSummary()
        {
            return BlockType.ToString() + "\n";
        }

        public virtual void UpdateValue(DFVersion v)
        {

        }
        public virtual void Restore(DataReader r)
        {

        }
       /* public virtual void Restore(DataReader r, DFVersion v, LogInstance api)
        {

        }*/
    }


    public class DataBlocks : List<DataBlock>
    {


    }



}
