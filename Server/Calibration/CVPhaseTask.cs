using System;
using System.Data;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using OpenWLS.Server.Base;
using OpenWLS.Server.Calibration;

namespace OpenWLS.Server.LogInstance.Calibration
{


    public class CVTask1DItem
    {
        string name;
        public string Name { get { return name; } }
        public CV1DValue RecValue{get; set; }
        double data;
        double dataAcc;

        public double Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }


        public CVTask1DItem(string name)
        {
            this.name = name;
        }

        public void Reset()
        {
            dataAcc = 0;
        }

        public void AccData(double dData)
        {
            dataAcc += dData;
        }
        
        public double GetAccumulatededData(int count)
        {
            if (count > 0)
                return dataAcc / count;
            else
                return double.NaN;
        }

        public void ResetAcc(int stepIndex)
        {
            dataAcc = 0;
        }

        public void UpdateData(int count)
        {
            data = GetAccumulatededData(count);
        }

    }

    public class CVTask1DItems : List<CVTask1DItem>
    {

        public CVTask1DItems()
        {

        }


        public CVTask1DItem this[string name]
        {
            get{
            foreach (CVTask1DItem item in this)
                if (item.Name == name)
                    return item;
            return null;
            }
        }
    }

    public class CVPhaseTasks : List<CVPhaseTask>
    {
        protected CVTask1DItems cv1DItems;
        public CVPhase Phase { get; set; }
//		protected DataGroup DataGroup;
        protected Instrument.Instrument tool;

        public bool Modified { get; set; }
        int selectedTaskIndex;

		#region Porperties
        public int SelectedTaskIndex
        {
            get { return selectedTaskIndex; }
            set
            {
                int k = value;
                if ( k != selectedTaskIndex)
                {
                    if (k >= Count)
                        k = 0;
                    if (k < 0)
                        k = Count -1;
                    this[selectedTaskIndex].LeaveTask();
                    selectedTaskIndex = k;
                    this[selectedTaskIndex].EnterTask();
                }
            }
        }

        public CVPhaseTask SelectedTask
        {
            get 
            {
                if (selectedTaskIndex < Count)
                    return this[selectedTaskIndex];
                else
                    return null;
            }
        }

		#endregion

        public CVPhaseTasks()
        {
            Modified = false;
            selectedTaskIndex = 0;
        }

        public string GetTaskNames()
        {
            string str = "";
            foreach (CVPhaseTask st in this)
                if  ( ! ( st is CVCalculateTask ) || Modified)
                    str = str + st.Name + ",";
            str = str.Remove(str.Length - 1, 1);
            return str;
        }

        public void AddTask(CVPhaseTask t)
        {
            Add(t);
            t.CVTaskCompleted += t_CVTaskCompleted;
        }

        void t_CVTaskCompleted(object sender, EventArgs e)
        {
            Modified = !(sender is CVMeasureTask);
            SelectedTaskIndex = selectedTaskIndex + 1;
         //   Phase.Proc.SendTextMsg("CV\n\n" + CVProc.req_task_index + "\n" + selectedTaskIndex.ToString());
        }
    }

	/// <summary>
	/// Summary description for CVPhaseTask.
	/// </summary>
	public class CVPhaseTask
	{
        public event EventHandler CVTaskCompleted;

        public string Name{get; set;}
        public CVPhaseTasks Tasks { get; set; }
        public CVProc Proc { get; set; }

        public virtual void BeforeTask()
        {

        }

        public  void EnterTask()
        {
           // SendActText();
            OnEnterTask();
        }

        protected virtual void OnEnterTask()
        {
            
        }        

        public virtual void PerformTask(DataReader r)
        {

        }

        protected void CompleteTask()
        {
            EventHandler handler = CVTaskCompleted;
            if (handler != null)
                handler(this, new EventArgs());
        }

        virtual public void LeaveTask()
        {

        }
        /*
        public void SendActText()
        {
            Proc.SendTextMsg("CV\n\n" + CVProc.req_task_action + "\n" + GetSubTaskActionText() );
        }
          */      
        virtual protected string GetSubTaskActionText()
        {
            return "";
        }


    }

    public class CVReferenceTask : CVPhaseTask
    {
        public static string action_name = "Submit";
        string references;

        public CVReferenceTask(string refs, CVPhaseTasks ts)
        {
            Name = "Input Referfence";
            references = refs;
            Tasks = ts;
        }

        protected override string GetSubTaskActionText()
        {
            return action_name;
        }

        protected override void OnEnterTask()
        {
          //  Proc.SendTextMsg("CV\n\n" + CVProc.req_reference + "\n" + references);
        }

        public override void LeaveTask()
        {
           // Proc.SendTextMsg("CV\n\n" + CVProc.req_reference + "\n" );
        }

        void UpdateReference(DataReader r)
        {
            string str = r.ReadString();
            string[] strs = str.Split(new char[] { '|' });
            foreach (string s in strs)
            {
                int k = s.IndexOf(':');
                string n = s.Remove(k, str.Length - k);
                string v = s.Remove(0, k + 1);
                Tasks.Phase.SetItemValue(n, Convert.ToDouble(v));
            }
          //  Proc.SendTextMsg("CV\n\n" + CVProc.update_values + "\n" + str);
        }

        public override void PerformTask(DataReader r)
        {
            string action = r.ReadLine();
            if (action == action_name)
            {
                UpdateReference(r);
                CompleteTask();
            }
        }
    }

    public class CVMeasureTask : CVPhaseTask
    {
        static string action_name_start = "Start";
        static string action_name_stop = "Stop";

        CVTask1DItems   cv1DItems;
        CV2DDataAcc     cv2DItem;

        int recordCountCur;
        int recordCountMax;
        byte dgID;
        public byte DataGroup { get { return dgID;  } }

        public CVTask1DItems CV1DItems { get { return cv1DItems; } }
        public CV2DDataAcc CV2DItem { get { return cv2DItem; } }
   //     public int Progress { get; set;  }


        public string CV1DName
        {
            set
            {
                cv1DItems = new CVTask1DItems();
                string[] strs = value.Split(new char[] { ',' });
                foreach (string str in strs)
                    cv1DItems.Add(new CVTask1DItem(str));
            }
        }
/*
        public int ReadProgress
        {
            get
            {
                return recordCountCur * 100 / recordCountMax;
            }
        }
*/
        public CVMeasureTask()
        {
            recordCountCur = 0;
            recordCountMax = 100;
        }

        protected override string GetSubTaskActionText()
        {
            if (recordCountCur == 0)
                return action_name_start;
            else
                return action_name_stop;
        }

        public override void PerformTask(DataReader r)
        {
            string action = r.ReadLine();
            if (action == action_name_start)
            {

            }

            if (action == action_name_stop)
            {
                recordCountCur = 0;
            }
        }

        public void ProcessDataGroup(DataReader r)
        {
            int rpos = r.Position;
            OnProcessDataGroup(r);
            r.Seek(rpos, SeekOrigin.Begin);
        }

        public virtual void OnProcessDataGroup(DataReader r)
        {

        }
    }

    public class CVCalculateTask : CVPhaseTask
    {
        public CVCalculateTask( CVPhaseTasks ts)
        {
            Name = "Calculate";
            Tasks = ts;
        }
    }


    public struct CV2DDataAcc
    {
        double[] data;
        int count;

        public int Count
        {
            get
            {
                return count;
            }
        }

        public void Reset(int length)
        {
            data = new double[length];
            count = 0;
        }

        public double[] GetData()
        {
            double[] ds = new double[data.Length];
            for (int i = 0; i < data.Length; i++)
                ds[i] = data[i] / count;
            return ds;
        }

        public void InputData(double[] dData)
        {
            if (dData == null || dData.Length != data.Length)
                return;
            for (int i = 0; i < data.Length; i++)
                data[i] += dData[i];
            count++;
        }

    }

}
