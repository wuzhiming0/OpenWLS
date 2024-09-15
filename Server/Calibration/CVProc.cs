using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



using Newtonsoft.Json;

using System.Data;
using OpenWLS.Server.Base;
using OpenWLS.Server.GView.Models;

namespace OpenWLS.Server.LogInstance.Calibration
{
    public enum CVBinMsgType { AGE = 0 };
    public class CVProc 
    {

        public const string req_summary = "SUMM";
        public const string req_phase_infor = "Phase_Infor";
        public const string req_task_names = "TaskNames";
        public const string req_task_index = "TaskIndex";
        public const string req_task_action = "TaskAction";
        public const string req_reference = "Reference";
        public const string update_values = "Values";
        public const string stored_in_tool = "StoredCalInTool";
        LogInstance api;
        CVSensors sensors;

        public CVPhaseTask SelectedTask
        {
            get;
            set;
        } 

        public CVProc(LogInstance api, CVSensors sensors)
        {
            this.api = api;
            this.sensors = sensors;
            foreach(CVSensor s in sensors)
            {
                CVPhase[] ps = s.GetCVPhases();
                foreach (CVPhase p in ps)
                    p.Proc = this;
            }
        }

        // CV
        // sensor 
        // 
         /*  
         void SendTextMsg(string sensor, string msg){
         ///   api.SendData("CV\n" + sensor + "\n" + msg);
        }

        public void SendTextMsg( string msg)
        {
           // api.SendData(msg);
        }

        public void SendBinMsg(byte[] msg)
        {
            DataWriter w = new DataWriter( msg.Length + 4);
        //    w.SetByteOrder(false);
            w.WriteData((ushort)LiMsgType.CV);
            w.WriteData((ushort)CVBinMsgType.AGE);
            w.WriteData(msg);
            byte[] bs = w.GetBuffer();
        //    api.SendData(bs, 0, bs.Length);
        }
        */
        public string GetCVSummary()
        {
            List<CVSensorStatus> ss = new List<CVSensorStatus>();

            foreach (CVSensor s in sensors)
            {
                ss.Add(s.GetPhasesStatus() );
            }

            string str = JsonConvert.SerializeObject(ss);
           // SendTextMsg("", req_summary + "\n" + str);
            return null;
        }

        public string ProcRequestMsg(DataReader r)
        {                                 
            return "to-do";
        }

        string PerformAction(DataReader r)
        {
            if (SelectedTask != null)
                SelectedTask.PerformTask(r);
            return null;
        }

        string ChangeTaskIndex(DataReader r)
        {
            if (SelectedTask != null)
                SelectedTask.Tasks.SelectedTaskIndex = Convert.ToInt32(r.ReadLine());
            return null;
        }

        public string ProcGUIText(DataReader r)
        {
            string sensor = r.ReadLine();
            if (sensor.Length == 0)
            {
                string req = r.ReadLine();
                if (req == req_summary)
                    return GetCVSummary();
                if (req == req_task_action)
                    return PerformAction(r);
                if (req == req_task_index)
                    return ChangeTaskIndex(r);
            }
            else
            {
                int id = Convert.ToInt32(sensor);
                CVSensor s = sensors.GetSensor(id);
                if (s != null)
                {
                    string str = r.ReadLine();
                    CVPhase p = s.GetCVPhase(str);
                    if (p != null)
                    {  
                        p.ProcGUIText(r);
                    }

                }
                return null;
            }
            return "CVProc.ProcGUIText: unknown sensor: " + sensor;
        }

    }

    public class CVManProc 
    {
        LogInstance api;

        public CVManProc(LogInstance _api) { 
            api = _api;
        }


        public void SendTextMsg(string msg)
        {
           // api.SendData(msg);
        }

        public void SendBinMsg(byte[] msg)
        {
            DataWriter w = new DataWriter(msg.Length + 4);
            w.SetByteOrder(false);
            w.WriteData((ushort)LiMsgType.CVMan);
            w.WriteData((ushort)CVBinMsgType.AGE);
            w.WriteData(msg);
            byte[] bs = w.GetBuffer();
          //  api.SendData(bs, 0, bs.Length);
        }

     
        public string ProcGUIBinary(DataReader r)
        {

            return "to-do";
        }



        string GetSerials()
        {
         //   string ss = CVRecords.GetAllSerials(api.Servers.CalibrationDB);
         //   api.SendData("CVMan\nSerials\n" + ss);
            return null;
        }

        string GetAssets(DataReader r)
        {
           // string ss = CVRecords.GetAssets(api.Servers.CalibrationDB, r.ReadLine());
           // api.SendData("CVMan\nAssets\n" + ss);
            return null;
        }

        string GetInfors(DataReader r)
        {
           // CVRecords infors = CVRecords.GetInfors(api.Servers.CalibrationDB, r.ReadLine());
           // api.SendData("CVMan\nInfors\n" + infors.Count.ToString() + "\n" + infors.Count.ToString() + "\n" +
           //     JsonConvert.SerializeObject(infors)); 
            return null;
        }
        /*
        string GetDRecord(DataReader r)
        {
            CVRecord infor = JsonConvert.DeserializeObject<CVRecord>(r.ReadString());
            string sql = "SELECT report FROM calibration WHERE TMID = " + infor.TMID.ToString();
            DataTable dt = api.MasterDB.GetDataTable(sql);
            if (dt == null || dt.Rows.Count == 0)
                return null;
            CVInstrument inst = new CVInstrument();
            //  inst.SerialNu = 
            string r1 = (string)dt.Rows[0]["report"];
            CVReports reports = JsonConvert.DeserializeObject<CVReports>(r1);
            CVReport rep = reports.GetReport(infor.Type, infor.Phase);   

            CVDataRecord dr = CVRecords.GetDataRecord(infor.ID, api.Servers.CalibrationDB);
            if (dr == null)
                return null;

            float y = 0;
            AGvDocument doc = new AGvDocument();

            //doc.DataProc = this;
            //doc.AddElemnet(new GeBOS()); 
            foreach (DataRow dr1  in rep.Tables.Rows)
            {
                CVReportTable rt = new CVReportTable();
                rt.Restore(dr1);
                rt.LoadDatRecord(dr);
                //   t.LoadDatRecord(dataRec);
                y = rt.AddItemsToAGDoc(doc, y);
            }
       //     if (CVInstrument != null && CVInstrument.Inst != null)
       //         CVInstrument.Inst.AfterUpdateCalPhase(this);

            doc.Size = new SizeF((float)8.5, y);

         //   doc.AddElemnet(new GeEOS());
         //   doc.Elements.OutputGElements();
            return null;
        }*/

        public string ProcGUIText(DataReader r)
        {
            string req = r.ReadLine();
            if (req == "GetSerials")
                return GetSerials();
            if (req == "GetAssets")
                return GetAssets(r);
            if (req == "GetInfors")
                return GetInfors(r);
         //   if (req == "GetDRecord")
         //       return GetDRecord(r);

            return "CVProc.ProcCvManGUIText: unknown sensor: " + req;
        }
    }


}
