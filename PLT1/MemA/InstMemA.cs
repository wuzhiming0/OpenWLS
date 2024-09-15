using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Windows.Media;
using Newtonsoft.Json;

using OpenWLS.PLT1.Edge;
using OpenWLS.Server.Base;
using OpenWLS.Server.LogInstance.Instrument;

namespace OpenWLS.PLT1.MemA
{

    public class MemNanFlashArch
    {
        public ushort Devices { get; set; }
        public byte[] DeviceTypes { get; set; }
        public ushort BadBlockMax { get; set; }
        public ushort PageSpares { get; set; }
        public ushort PageBytes { get; set; }
        public ushort BlockPages { get; set; }
        public ushort PlaneBlocks { get; set; }
        public ushort DevicePlanes { get; set; }

        public int TotalMBytes { 
            get { 
                return PageBytes * BlockPages * PlaneBlocks * DevicePlanes * Devices / 1000000;
            }
        }
        
        static public MemNanFlashArch GetMem1EFArch(string json)
        {
            return JsonConvert.DeserializeObject<MemNanFlashArch>(json);
        }

        public MemNanFlashArch()
        {

        }

        public void Update(DataReader r)
        {
            Devices = r.ReadUInt16();
            DeviceTypes = r.ReadByteArray(16);
          //  BadBlockMax = r.ReadUInt16();
            PageSpares = r.ReadUInt16();
            PageBytes = r.ReadUInt16();
            BlockPages = r.ReadUInt16();
            PlaneBlocks = r.ReadUInt16();
            DevicePlanes = r.ReadUInt16();
        }



        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }




    }

    public class InstMemA : PLT1Instrument
    {
        public enum MemAExtCmdCde{
            Scan = 1, 
            UploadDFlash = 2,
            EraseDFlash = 4, 

            SetRecordMode = 2, 
            GetDFlashInfor = 3, 
            GetDFiles = 4, 
            GetBBS = 5,
            AddBadBlock = 6, 
            GetPages = 7, 
            CancelGetPages = 8, 
            WriteDFlash = 9,            //
     //       WriteEFlash = 0xa,          //
            DownloadProgFile = 0xf, 
            NewDataFile = 0x10, 
            CloseFile = 0x11, 

            UploadFile = 0x13, 
            DownloadLBlock = 0x14 
        };


        static string bbsStr = @"
Device,System.Int32
Plane,System.Int32
Block,System.Int32";

        static string fileStr = @"
ID, System.Int32
Name,System.String
StartTime,System.String
StartPage,System.Int32
StopTime,System.String
Status,System.Int32
Size,System.Int32";

        public DataTable BBS { get; set; } //bad blocks
        public DataTable Files { get; set; }
        public MemNanFlashArch MemInfor { get; set; }
        public int BadMBytes { get { return MemInfor.PageBytes * MemInfor.BlockPages * BBS.Rows.Count / 1000000; } }
        public uint UsedPages { get; set; }
        public InstMemA()
        {
            Address = default_addr = IBProtocol.MEM_ADDR;
            BBS = DataType.CreateTable(bbsStr);
            Files = DataType.CreateTable(fileStr);
            MemInfor = new MemNanFlashArch();
        }

         public string ProcGUIText(DataReader r)
        {
            string str = r.ReadLine();
            /*
            if (str == "Scan")
            {
                api.HwPort.SendExtendCmd((byte)Address, (byte)MemAExtCmdCde.Scan);
                api.SysLog.AddMessage(FullName + " Scan", Colors.Blue);
                return null;
            }
            if (str == "NewFile")
            {
                api.HwPort.SendExtendCmd((byte)Address, (byte)MemAExtCmdCde.NewDataFile, StringConverter.ToByteArray( api.OCF.Name, 20) );
                api.SysLog.AddMessage(FullName + " Create New File", Colors.Blue);
                return null;
            }
            if (str == "Erase")
            {
                api.HwPort.SendExtendCmd((byte)Address, (byte)MemAExtCmdCde.EraseDFlash, new byte[] { Convert.ToByte(r.ReadLine()) } );
                api.SysLog.AddMessage(FullName + " Create New File", Colors.Blue);
                return null;
            }
            if (str == "Upload")
            {
                api.HwPort.SendExtendCmd((byte)Address, (byte)MemAExtCmdCde.UploadFile, new byte[] { Convert.ToByte(r.ReadLine()) } );
                api.SysLog.AddMessage(FullName + " Create New File", Colors.Blue);
                return null;
            }
            */
            return "";
        }

        void DownloadOCF()
        {

        }
        
        override public void ProcessParameter(string name, string value)
        {

        }



        string ReadTime(DataReader r)
        {
            DateTime dt = DataType.GetDateTimeFromRTC(r);
            return dt.ToShortDateString() + dt.ToShortTimeString();
        }

        void ProcessDFiles(DataReader r)
        {
            Files.Clear();
            int c = r.ReadByte();
            uint p = 0;
            for(int i = 0; i < c; i++)
            {
                DataRow dr = Files.NewRow();
                Files.Rows.Add(dr);
                dr["ID"] = r.ReadUInt16();
                r.ReadByte();   //type
                dr["Status"] = r.ReadByte();
                dr["Name"] = r.ReadStringAndTrim(20);

                dr["StartTime"] = ReadTime(r);
                dr["StartPage"] = r.ReadUInt32();
                r.ReadUInt32();  // backup

                r.ReadUInt16();         //ms
                r.ReadUInt16();

                dr["StopTime"] = ReadTime(r);
                r.ReadUInt32();  // backup
                uint s =  r.ReadUInt32();
                dr["Size"] = s;
                p += s;
            }
            UsedPages = p;
            p = p * MemInfor.PageBytes / 1000000;
           // api.SysLog.AddMessage(FullName + ": " + p.ToString() + "MBytes used");
        }

        void ProcessBB(DataReader r)
        {
            DataRow dr = BBS.NewRow();
            BBS.Rows.Add(dr);
            dr["Device"] = r.ReadUInt16();
            dr["Plane"] = r.ReadByte();
            dr["Block"] = r.ReadByte();
        }

        void ProcessBBS(DataReader r)
        {
            BBS.Clear();
            int c = r.ReadUInt16();
            for (int i = 0; i < c; i++)
                ProcessBB(r);
        //    api.SysLog.AddMessage(FullName + ": " + BadMBytes.ToString() + "MBytes bad");
        }

        void BeginEFlashScan(DataReader r)
        {
            MemInfor.Update(r);
          //  api.SysLog.AddMessage(FullName + ": Found " + MemInfor.TotalMBytes.ToString() + "MBytes totally");
            BBS.Rows.Clear();
            Files.Rows.Clear();
          //  api.SendData(msg_head + "Scan");
        }

        void SendSpaceUsage()
        {
            string str =  BadMBytes.ToString() + ", 0, " + MemInfor.TotalMBytes.ToString();
            str = "Usage\n" + str;
            //api.SendData(msg_head + str);
        }

        void EndEFlashScan()
        {
           // api.SysLog.AddMessage(FullName + ": Scan completed.", Colors.Orange);
            SendSpaceUsage();
        }

        void ErrorEFlashScan()
        {
           // api.SysLog.AddMessage(FullName + ": Scan completed with error.", Colors.Orange);
            string str = BadMBytes.ToString() + ", 0, " + MemInfor.TotalMBytes.ToString();
            str = "Usage\n" + str;
           // api.SendData(msg_head + str);
        }

        void ProcessScan(DataReader r)
        {
            byte b = r.ReadByte();
            switch (b)
            {
                case 0:
                    BeginEFlashScan(r);
                    break;
                case 1:
                    ProcessBB(r);
                    break;
                case 2:
            //        api.SysLog.AddMessage(FullName + ": Scanned 1024 blocks");
                    break;
                case 3:
                    EndEFlashScan();
                    break;
                case 4:
                    EndEFlashScan();
                    break;
                default:
                    break;
            }
                
        }

        void ProcessExtCmdResponse(DataReader r)
        {
            MemAExtCmdCde ec = (MemAExtCmdCde)r.ReadByte();
            switch (ec)
            {
                case MemAExtCmdCde.GetDFiles:
                    ProcessDFiles(r);
                    SendSpaceUsage();
                    break;
                case MemAExtCmdCde.GetBBS:
                    ProcessBBS(r);
                    break;
                case MemAExtCmdCde.GetDFlashInfor:
                    MemInfor.Update(r);
                //    api.SysLog.AddMessage(FullName + ":  " + MemInfor.TotalMBytes.ToString() + "MBytes totally");
                    break;
                case MemAExtCmdCde.Scan:
                    ProcessScan(r);
                    break;
               default:
                    break;
            } 

        }



        override public void OnConnected()
        {
           // api.HwPort.SendExtendCmd((byte)Address, (byte)MemAExtCmdCde.GetDFlashInfor);
           // api.HwPort.SendExtendCmd((byte)Address, (byte)MemAExtCmdCde.GetBBS);
          //  api.HwPort.SendExtendCmd((byte)Address, (byte)MemAExtCmdCde.GetDFiles);
        }     

       
    }
}
