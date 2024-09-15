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


using OpenWLS.Server.GView.Models;

namespace OpenWLS.Client.LogInstance.Calibration
{
    /// <summary>
    /// Interaction logic for CVTaskBar.xaml
    /// </summary>
    /// 
    public partial class CVTaskBar : UserControl
    {
        delegate void UpdateTaskNamesDelegate();
        delegate void UpdateTaskSelectionDelegate();
        delegate void UpdateTaskProgressDelegate();
        delegate void UpdateTaskActionNameDelegate();


        List<string> taskNames;

        bool selectReady;
        public GvItems Items
        {
            get; set;
        }
        public List<string> TaskNames
        {
            set
            {
                taskNames = value;
                UpdateTaskNames();
                selectReady = false;
            }
        }

        int selectedTask;
        public int SelectedTask
        {
            set
            {
                selectedTask = value;
                UpdateTaskSelection();
                selectReady = true;
            }
        }
        int progress;
        public int Progress
        {
            set
            {
                progress = value;
                UpdateProgress();
            }
        }

        string actionName;
        public string TaskActionName
        {
            set
            {
                actionName = value;
                UpdateActionName();
            }
        }




        public CVTaskBar()
        {
            InitializeComponent();
        }


        public void UpdateTaskNames()
        {
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke(new UpdateTaskNamesDelegate(UpdateTaskNames), null);
            else
                taskCb.ItemsSource = taskNames;
        }

        public void UpdateTaskSelection()
        {
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke(new UpdateTaskSelectionDelegate(UpdateTaskSelection), null);
            else
                taskCb.SelectedIndex = selectedTask;
        }

        public void UpdateProgress()
        {
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke(new UpdateTaskProgressDelegate(UpdateProgress), null);
            else
                taskPb.Value = progress;
        }

        public void UpdateActionName()
        {
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke(new UpdateTaskActionNameDelegate(UpdateActionName), null);
            else
                taskBtn.Content = actionName;
        }

        private void taskBtn_Click(object sender, RoutedEventArgs e)
        {
            string s1 = (string)taskBtn.Content;
        //    string cmd = "CV\n\n" + CVProc.req_task_action + "\n" + s1;
        //    if(s1 == CVReferenceTask.action_name)
         //       Client.SendRequest( cmd + "\n" + Items.Paras.GetParaNameValString() );

        }

        private void taskCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
       //     if (selectReady)
        //        Client.SendRequest("CV\n\n" + CVProc.req_task_index + "\n" + taskCb.SelectedIndex.ToString() );
        }

    }
}
