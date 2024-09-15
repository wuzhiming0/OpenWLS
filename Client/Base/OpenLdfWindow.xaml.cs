using System;
using System.Collections;
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
using System.Windows.Shapes;
using Client;

//using OpenWLS.Client.LogInstance;
using OpenWLS.Client.LogInstance;
using OpenWLS.Client.LogDataFile;
using OpenWLS.Client.Requests;
using OpenWLS.Server.DBase.Models.LocalDb;
using OpenWLS.Server.LogDataFile;

namespace OpenWLS.Client.Base
{
    /// <summary>
    /// Interaction logic for OpenDisplayWindow.xaml
    /// </summary>
    public partial class OpenLdfWindow : Window
    {
     /*   string file_filter;
        public string FileFilter
        {
            get { return file_filter; }
            set { file_filter = value; }
        }
     */

        bool update_lock = false;
        async void SetJobListItems()
        {
            JobCs? jobs = await JobRequest.GetAllJobsAsync();
            jobList.ItemsSource = jobs;
            if (jobs != null && jobs.SelectedJob != null)
            {
                update_lock = true;
                jobList.SelectedItem = jobs.SelectedJob;
                fileList.ItemsSource = await LdfRequest.GetDfileNamesOfFolder(jobs.SelectedJob.GetJobDirectory());
            //    fileList.ItemsSource = await LdfRequest.GetDfileNamesOfJob(jobs.SelectedJob.Name);
                update_lock = false;
            }
        }

     


        public OpenLdfWindow()
        {
            InitializeComponent();
            SetJobListItems();
        }


        private async void fileList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (fileList.SelectedItem != null)
            {
                string fn = fileList.SelectedItem.ToString();
                DataFileInfor dfi = await LdfRequest.Open($"{((JobC)jobList.SelectedItem).GetJobDirectory()}{fn}");
                if(dfi != null){
                    Explore exp = new Explore
                    {
                        DataFileInfor = dfi,
                    };
                    ((IMainWindow)Application.Current.MainWindow).AddLdfExplore(exp);
                   Close();
                }

            }

        }

        private async void jobList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(jobList.SelectedItem != null && update_lock == false)
            {
                JobC job = (JobC)jobList.SelectedItem;
                fileList.ItemsSource = await LdfRequest.GetDfileNamesOfFolder(job.GetJobDirectory());
            }

        }

        /*
        private async void opMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(opModeCb.SelectedIndex == (int)LiMode.Playback)
            {
                ocfCb.Visibility = Visibility.Hidden;  ocfCb.SelectedIndex = 1;
            }
            else
            {
                ocfCb.Visibility = Visibility.Visible;  ocfCb.SelectedIndex = 0;
            }
           

        }*/
  
    }
}
