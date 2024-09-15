using OpenWLS.Server.LogInstance;
using OpenWLS.Server.DBase.Models.LocalDb;
using System;
using System.Data;


/// <summary>
/// /////////////////
/// </summary>

namespace OpenWLS.Server
{
    public class ServerGlobals
    {

        public static string projectDir;
        public static string uom_db_fn;
        public static LogInstanceSs logInstances;
        public static async void Init()
        {
            projectDir = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            int k = projectDir.IndexOf("Server");
            projectDir = projectDir.Substring(0, k);

            uom_db_fn = $"{ServerGlobals.projectDir}data/server/db/uom.json";
            logInstances = new LogInstanceSs();
        }
     /*   
        public static void AddAPInatnce(LogInstance.LogInstanceS li)
        {
            li.Id = logInstances.Count == 0? 1 : logInstances.Max(a => a.Id) + 1;
            logInstances.Add(li);
        }
*/
    }

    public enum LiMsgType { Inst, OCF, Log, SysLog, CV, CVMan, N1D, ACT, DFP };
    public enum LiDfpMsgType { Open, Close };
    public enum LiOcfMsgType { New, Open, Close, Name, OpenPlayback, InstTblM, InstTblA, NewInst, DelInst, UpInst, DownInst, Check, Save, SaveAs };
    public enum LiActMsgType { Active, All, Add, Del, Update, Enable };
    public enum LiN1dMsgType { Chs, List, Add, Del, Vals };
    public enum LiLogMsgType { Inst, TPower, State, DataFName, Category };




    public enum LDFMsgType
    {
    //Get,
        GetDFileNames,GetJobNames,Get1DData,GetxDData,GetNcObject,
        Open,Close,Import,
        JobNames, DataFileNames, NCItemList, 
        NCItemDetail, ChannelData1D,  ChannelDataXD, ChannelDataString,
        IndexShift,IndexUnit,ChannelHead,TxtRecord, ExportLas, ExportDlis
    }

    public enum GViewMsgType
    {
        Open, Update,  Save,
        DFileNames, MHeads, MVBlock, 
        RtHeads, RtMVs
    }
}