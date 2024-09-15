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
using System.ComponentModel;

using OpenWLS.Server.GView.ViewDefinition;
using OpenWLS.Client.Requests;
using OpenWLS.Client.LogInstance;
using OpenWLS.Server.DBase.Models.LocalDb;
using System.Xml.Linq;
using OpenWLS.Client.LogDataFile;
using OpenWLS.Server.LogDataFile;
using OpenWLS.Client.Base;

namespace OpenWLS.Client.GView.GUI
{

     /// <summary>
    /// Interaction logic for DataFileCntl.xaml
    /// </summary>
    public partial class DataFileCntl : UserControl
    {
        List<string> dfileNames;
        VdDFiles dFiles;
        public event EventHandler<EventArgs>DFileAdded;
        public event EventHandler<EventArgs> DFileRemoveded;
        bool update_lock;
        public VdDFiles DFiles
        {
            get { return dFiles; }
            set
            {
                dFiles = value;
                UpdateFileNames();
            }
        }

        public void UpdateFileNames()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                dfnList.ItemsSource = null;
                //    dfnList.ItemsSource = dFiles;
                ICollectionView defaultView = CollectionViewSource.GetDefaultView(dFiles);
                //   defaultView.Filter = w => ((VdDFile)w).Name != Archive.real_time_ar_name;
                dfnList.ItemsSource = defaultView;
                //dfnList.ItemsSource = dFiles;

            }));
        }

        public DataFileCntl()
        {
            InitializeComponent();
            update_lock = false;
        }
        
        public async void GetJobs()
        {
            if (update_lock) return;
            update_lock = true;
         //   Dispatcher.Invoke(new Action(async () =>
            { 
                jobCb.ItemsSource = null;
                JobCs? jobs = await JobRequest.GetAllJobsAsync();
                jobCb.ItemsSource = jobs;
                if (jobs != null)
                    jobCb.SelectedItem = jobs.SelectedJob;
            }
            //));
            update_lock = false;
        }

        async void AddDfile(string dfn)
        {
            VdDFile df = new VdDFile(dFiles.GetNxtId(), ((JobC)jobCb.SelectedItem).Name, dfn);
            DataFileInfor dfi = await LdfRequest.Open(df.Job, df.Name);
            df.CreateMeasurements(dfi.Measurements);
            df.NMRecords = dfi.NMRecords;
            dFiles.Add( df );
            if (DFileAdded != null)
            {
                DFileAdded(df, new EventArgs());
                UpdateFileNames();
            }
        }


        private void Add_DFile_Click(object sender, RoutedEventArgs e)
        {
            string dfn = (string)lFileNameList.SelectedItem;
            AddDfile(dfn);
        }

        private void lFileNameList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string dfn = (string)lFileNameList.SelectedItem;
            addFileBtn.IsEnabled = !dFiles.Contains(((JobC)jobCb.SelectedItem).Name, dfn);

            
        }

        private void dfnList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dfnList.SelectedItem != null)
            {
                VdDFile df = (VdDFile)dfnList.SelectedItem;
                if (DFileRemoveded != null)
                    DFileRemoveded(df, new EventArgs());
                dFiles.Remove(df);
                dfnList.CommitEdit();
                dfnList.CommitEdit();
                UpdateFileNames();    
                dfnList.CancelEdit();
                dfnList.CancelEdit();
            }

        }

        private void lFileNameList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lFileNameList.SelectedItem != null)
            {
                string dfn = (string)lFileNameList.SelectedItem;
                if(!dFiles.Contains(((JobC)jobCb.SelectedItem).Name, dfn)){
                    AddDfile((string)lFileNameList.SelectedItem);
                  //  UpdateFileNames();
                }
            }

        }

        private void jobCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
       //     if (update_lock) return;
            if(jobCb.SelectedItem != null)
            {
                //Dispatcher.Invoke(new Action(async () =>
                {
                    List<string> fns = LdfRequest.GetDfileNamesOfJob(((JobC)jobCb.SelectedItem).Name).Result;
                    lFileNameList.ItemsSource = fns;
                }
                //));
            }
        }
    }
}
