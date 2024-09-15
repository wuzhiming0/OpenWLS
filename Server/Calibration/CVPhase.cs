using OpenWLS.Server.Base;
using OpenWLS.Server.Calibration;
using OpenWLS.Server.DBase.Models.CalibrationDb;
using OpenWLS.Server.GView.Models;
using System;
using System.Data;
using System.Drawing;
//using System.Windows.Forms;
using System.IO;



namespace OpenWLS.Server.LogInstance.Calibration
{
    public enum CVPhaseStatus { NotAvailable = 0, NotDone, OK, Failed };
    /// <summary>
    /// Summary description for CVPhase.
    /// </summary>
    public class CVPhase
    {
        CVProc proc;
        public CVProc Proc
        {
            get { return proc; }
            set
            {
                proc = value;
                if (Tasks != null)
                {
                    foreach (CVPhaseTask t in Tasks)
                        t.Proc = proc;
                }
            }
        }
        CVReportTables report;
        public CVReportTables Report { get{return report;} }
        protected string name;
        protected CVRecord record;
		protected CVRecordValue  recValue;

		protected CVPhaseStatus status;
        protected int subItemIndex;

        //       CVPhaseTasks tasks;
        //      string taskNames;
        //       OpenLS.Acquisition.Calibration.CVPhaseControl cvPhaseControl;
        //      int progress;



        #region Properties

        public CVInstrument CVInstrument { get; set; }
        public CVRecord Record
		{
			get
			{
				return record;
			}
            set
            {
                record = value;
                recValue = record.GetRecordValue();
            }
		}

		public CVRecordValue  DataRec
		{
			get
			{
				return recValue;
			}

		}

        CVPhaseTasks tasks;
        public CVPhaseTasks Tasks
        {
            get { return tasks; }
            set { tasks = value; tasks.Phase = this; }
        }

        public virtual CVPhaseStatus Status
        {
            get  {   return status;  }
            set  {     status = value;  }
        }


        public string Name
		{
			get	{   return name;    }
		}


 
        #endregion
        public CVPhase()
        {
        }
            
        public CVPhase(string strSerial, DataRow dr)
		{
            report = new CVReportTables();
			InitPhase(strSerial, dr);

		}

        void LoadReportData()
        {
            foreach (CVReportTable t in Report)
              t.LoadDatRecord(recValue);
        }

        public int ProcGUIText(DataReader r)
        {
            string str = r.ReadLine();
            if(str == CVProc.req_phase_infor)
            {
                float y = 0;
                AGvDocument doc = new AGvDocument();
                foreach (CVReportTable t in Report)
                {
                    t.LoadDatRecord(recValue);
                    y = t.AddItemsToAGDoc(doc, y);
                }
                if (CVInstrument != null && CVInstrument.Inst != null)
                    CVInstrument.Inst.AfterUpdateCalPhase(this);
             /*  
              *  doc.DataProc = (OpenLS.Base.DataPort.IOutPort)Proc;
              //  doc.AddElemnet(new GeBOS());
                doc.Size = SizeF((float)8.5, y);
                doc.InsertElemnet(new GvBOS());
                doc.AddElemnet(new GvEOS());
                doc.Elements.OutputGElements();
             */
         //       Proc.SendTextMsg("CV\n\n" + CVProc.req_task_names + "\n" + Tasks.GetTaskNames() );
                CVPhaseTask st = Tasks.SelectedTask;
                if (st != null)
                {
               //     Proc.SendTextMsg("CV\n\n" + CVProc.req_task_index + "\n" + Tasks.SelectedTaskIndex.ToString() );
                   // st.Proc = Proc;
                    st.EnterTask();
                }
                Proc.SelectedTask = st;
 //             
            //    if(CVInstrument != null && CVInstrument.Inst.StoredCalInTool(Name))
            //        Proc.SendTextMsg("CV\n\nStoredCalInTool\n1");
            //    else
            //        Proc.SendTextMsg("CV\n\nStoredCalInTool\n0");
            }
            return 0; 
        }


        public int GetDataRecordLength()
        {
            if ((status == CVPhaseStatus.NotDone) || (status == CVPhaseStatus.NotAvailable))
                return 0;
            return recValue.Length + CVRecord.RecordSize;
        }
        
        /*
        public void SaveDataRecord(DataWriter w)
        {
            if ((status == CVPhaseStatus.NotDone) || (status == CVPhaseStatus.NotAvailable))
                return;
            recValue.WriteDataRecord(w);
        }

        public void SaveCVPhaseToCVD(CVRecordSs cvIRs, FileStream fs)
        {
            if ((status == CVPhaseStatus.NotDone) || (status == CVPhaseStatus.NotAvailable))
                return;
            foreach (CVRecord cvInfor in cvIRs)
                if (record.SameAs(cvInfor)) return; 
        }
        */



		public void SetItemValue(string strItem, double data)
		{
			this.recValue.SetItemValue(strItem, data);
		}


        public double GetItemValue(string strItem)
        {
            return this.recValue.GetCV1DValue( strItem);
        }

        public double[] GetMatrixesData()
        {
            return recValue.Matrixes;
        }

        public void SetMatrixes(double[] data)
        {
            this.recValue.Matrixes = data;
        }





        public void InitPhase(string strSerial, DataRow dr)
        {
            name = (string)dr["Name"];
            //     taskNames = (string)dr["Tasks"];
            status = CVPhaseStatus.NotDone;
      //      recValue = CVRecordValue.FromDataRow(dr);
            record = new CVRecord();
      //      record.LoadInforRecordFromDataRow(strSerial, dr);
            subItemIndex = -1;
            if (name == "CP")
                subItemIndex = 3;
            if (name == "VP")
                subItemIndex = 4;
            if (name == "VB")
                subItemIndex = 5;
            if (name == "VA")
                subItemIndex = 6;
        }


       
        public void StartReading(bool start)
        {
    //        cvPhaseControl.StartReading(start);
        }
       
	}

}
