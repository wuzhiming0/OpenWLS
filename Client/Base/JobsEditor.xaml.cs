using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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


using OpenWLS.Client.Requests;
using OpenWLS.Server.DBase.Models.LocalDb;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace OpenWLS.Client.Base
{
    /// <summary>
    /// Interaction logic for JobEditor.xaml
    /// </summary>
    public partial class JobsEditor : Window
    {

        public JobsEditor()
        {
            InitializeComponent();
            UpdateList();
        }

        private void jobList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (jobList.SelectedItem == null)
                return;
            propertyGrid.SelectedObject = (JobC)jobList.SelectedItem;
        }

        public void UpdateList()
        {
            Dispatcher.Invoke(new Action( async () =>
            {
                jobList.ItemsSource = null;
                JobCs? jobs = await JobRequest.GetAllJobsAsync();
                jobList.ItemsSource = jobs;
                if(jobs != null)
                    propertyGrid.SelectedObject = jobs.SelectedJob;
            }));
        }
        /*
        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            foreach(JobC job in jobList.ItemsSource)
                job.Selected = false;
            if (jobList.SelectedItem != null)
            {
                JobC job = (JobC)jobList.SelectedItem;
                job.Selected = true;
                Globals.ActiveJob = job;
            }
        }
        */
        private void newButton_Click(object sender, RoutedEventArgs e)
        {
            JobC job = new JobC();
            propertyGrid.SelectedObject = job; 
        }

        private void jobList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            JobCs jobs = (JobCs)jobList.ItemsSource;       
            if (jobList.SelectedItem != null)
            {
                JobC job = (JobC)jobList.SelectedItem;
                ClientGlobals.ActiveJob = job;
                jobs.SelectJob(job.Id);
            }
            jobList.ItemsSource = null;
            jobList.ItemsSource = jobs;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (propertyGrid.SelectedObject != null)
            {
                JobC job = (JobC)propertyGrid.SelectedObject;
                job.Id = -1;
                Dispatcher.Invoke(new Action(async () =>
                {
                    job = await JobRequest.CreateJobAsync(job);
                    if (job != null)
                    {
                        JobCs jobs = (JobCs)jobList.ItemsSource;
                        jobs.Add(job);
                        jobs.SelectJob((int)job.Id);
                        jobList.ItemsSource = null;
                        jobList.ItemsSource = jobs;
                    }
                }));
            }
        }
        /*
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        */

    }


    public class JobC
    {
        [Browsable(false)]
        public int Id { get; set; }
        [PropertyOrder(1)]
        public string? Name { get; set; }
        [PropertyOrder(3)]
        public string? Company { get; set; }
        [PropertyOrder(4)]
        public string? WellId { get; set; }
        [PropertyOrder(5)]
        public string? WellName { get; set; }
        [PropertyOrder(6)]
        public string? Unit { get; set; }         //skid/truck/crew 
        [PropertyOrder(2)]
        public string? Engineer { get; set; }
        [ReadOnly(true)]
        [PropertyOrder(8)]
        public long TimeStart { get; set; }
        [ReadOnly(true)]
        [PropertyOrder(9)]
        public long TimeStop { get; set; }
        [PropertyOrder(10)]
        public string? Note { get; set; }
        [Browsable(false)]
        public bool? Deleted { get; set; }

        [Browsable(false)]
        [JsonIgnore]
        public bool Selected { get; set; }
        public override string ToString()
        {
            if (Selected) return $"{Name}*";
            else return Name;
        }
        public string GetJobDirectory()
        {
            return GetJobDirectory(Name);
        }
        public static string GetJobDirectory(string job_name)
        {
            return $"{ClientGlobals.projectDir}data\\server\\jobs\\{job_name}";
        }


    }
    public class JobCs : List<JobC>
    {
        [JsonIgnore]
        public JobC SelectedJob { get; set; }
        public void SelectJob(int id)
        {
            SelectedJob = null;
            foreach (JobC job in this)
            {
                job.Selected = job.Id == id;
                if (job.Selected) SelectedJob = job;
            }
        }
    }
}
