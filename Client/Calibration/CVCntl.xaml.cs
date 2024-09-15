using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

//using System.Windows.Forms;
using System.Windows.Forms.Integration;


using Newtonsoft.Json;
using OpenWLS.Server.LogInstance.Calibration;
using OpenWLS.Server.Base;

namespace OpenWLS.Client.LogInstance.Calibration
{
    /// <summary>
    /// Interaction logic for CVCntl.xaml
    /// </summary>
    public partial class CVCntl : UserControl
    {

        CVSensorStatus selectedCVSensorStatus;


        CVReportView reportView;

        public bool Visible { get; set; }

        public CVCntl()
        {
            Name = "CVMain";
            InitializeComponent();


            summCntl.CVPhaseClicked += SummCntl_CVPhaseClicked;

            reportView = new CVReportView();
            reportCntl.Child= reportView;

    //        taskBar.Items = reportView.Items;
            Visible = false;
        }

        private void SummCntl_CVPhaseClicked(object sender, EventArgs e)
        {
            selectedCVSensorStatus  = (CVSensorStatus)sender;
       //     Client.SendRequest("CV\n" + selectedCVSensorStatus.ID.ToString() + "\n" + summCntl.GetSelectedPhase() + "\n" + CVProc.req_phase_infor );
        }

        string ProcessSummary(DataReader r) 
        {
            string str = r.ReadString();
            try
            {
                summCntl.Summary = (List<CVSensorStatus>)JsonConvert.DeserializeObject<List<CVSensorStatus>>(str); 
                return null;
            }
            catch (Exception e)
            {
                return "";
            }        
        }

        string ProcessTaskNames(DataReader r)
        {
            string str = r.ReadString();
            string[] strs = str.Split(new char[] { ',' });
            List<string> ss = new List<string>();
            foreach (string s in strs)
                if(s.Length>0)
                    ss.Add(s);
            taskBar.TaskNames = ss;

            return null;
        }

        string ProcessIndex(DataReader r)
        {
            string str = r.ReadString();
            int k = Convert.ToInt32(str);
            taskBar.SelectedTask = k;
            return null;
        }

        string ProcessAction(DataReader r)
        {
            string str = r.ReadString();
            taskBar.TaskActionName = str;
            return null;
        }

        string ProcessReference(DataReader r)
        {
            string str = r.ReadString();
            reportView.References = str;
            return null;
        }

        string ProcessUpdateValues(DataReader r)
        {
            string str = r.ReadString();
            reportView.UpdateValues(str);
            return null;
        }

        public  string ProcPackageBinary(DataReader r)
        {
            CVBinMsgType t = (CVBinMsgType)r.ReadUInt16();
            switch (t)
            {
                case CVBinMsgType.AGE:
               //     reportView.ProcessBinMsg(r);
                    break;
                default:
                    break;
            }
            return null;
        }
  

        public string ProcPackageText(DataReader r)
        {
            string sensor = r.ReadLine();
            if (sensor.Length > 0)
            {
                return "";
            }
            string t = r.ReadLine();
            if (t == CVProc.req_summary)
                return ProcessSummary(r);

            if (t == CVProc.req_task_names)
                return ProcessTaskNames(r);

            if (t == CVProc.req_task_index)
                return ProcessIndex(r);

            if (t == CVProc.req_task_action)
                return ProcessAction(r);

            if (t == CVProc.req_reference)
                return ProcessReference(r);

            if (t == CVProc.update_values)
                return ProcessUpdateValues(r);

            if (t == CVProc.stored_in_tool)
            {
                bool b = Convert.ToByte(r.ReadLine()) != 0;                
       //         LiClientMainCntl apiCntl = (LiClientMainCntl)Client.GetControl("MainControl");
        //        Dispatcher.Invoke(() => { apiCntl.MenuStatus.CalStoredInTool = b; });
                return null;
            }

            return "";
        }

    }
}
