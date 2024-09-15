using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OpenWLS.Client.Requests;
using OpenWLS.Server.DBase.Models.GlobalDb;
using OpenWLS.Server.DBase.Models.LocalDb;
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

//using OpenWLS.Client.LogInstance;
//using OpenWLS.Client.LogInstance;



namespace OpenWLS.Client.Base
{
    public enum FileType { OCF = 0, VDF = 1, LDF = 2 };
    /// <summary>
    /// Interaction logic for OpenDisplayWindow.xaml
    /// </summary>
    public partial class SaveAsFilefWindow : Window
    {

    //    public IMainWindow MainWindow { get; set; }
        string file_filter;
        string sub_folder;
        string file_content;
        IEnumerable globalFns;
        IEnumerable localFns;
        FileType fileType;

        async void SetJobListItems()
        {
            JobCs jobs = await JobRequest.GetAllJobsAsync();
            jobList.ItemsSource = jobs;
            if (ClientGlobals.ActiveJob != null)
                jobList.SelectedItem = jobs.Where(a => a.Id == ClientGlobals.ActiveJob.Id);
        }

        public SaveAsFilefWindow()
        {
            InitializeComponent();
            SetJobListItems();
        }


        public void Init(FileType f_type )
        {
            fileType = f_type;
            global_Checked(null, new RoutedEventArgs());
        }

        IEnumerable GetFns(bool global)
        {
            switch (fileType)
            {
                case FileType.VDF:
                    return VdfRequest.GetAll(global).Result;
                case FileType.OCF:
                    return OcfRequest.GetAll(global).Result;
            }
            return null;
        }

        private void global_Checked(object sender, RoutedEventArgs e)
        {
            bool g = (bool)globalCb.IsChecked;
            if( g )
            {
                if (globalFns == null)
                    globalFns = GetFns(g);
                fileList.ItemsSource = globalFns;
            }
            else
            {
                if (localFns == null)
                    localFns = GetFns(g);
                fileList.ItemsSource = localFns;
            }

        }

        private void jobList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void fileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            fnTb.Text = fileList.SelectedItem == null? null:(string)fileList.SelectedItem;
        }

        private async void OkButton_Click(object sender, RoutedEventArgs e)
        {
            string fn = System.IO.Path.GetFileName(fnTb.Text.Trim());
            if (fn.Length == 0)
                return;
            // check same name, 


            bool g = (bool)globalCb.IsChecked;
            switch ( fileType)
            {
                case FileType.VDF:
                    GViewDefinitionFile vdf = (GViewDefinitionFile)jobList.SelectedItem;
                    if(vdf.Body == null)
                        vdf.Body = VdfRequest.GetBody(vdf.Id, g).Result.Val;
                    break;
                case FileType.OCF:
                    OperationControlFile ocf = (OperationControlFile)jobList.SelectedItem;
                    if(ocf.Body == null)
                        ocf.Body = OcfRequest.GetBody(ocf.Id, g).Result;
                    break;
            }
            Close();

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
