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
using Microsoft.AspNetCore.Mvc;

//using OpenWLS.Client.LogInstance;
using OpenWLS.Client.LogInstance;
using OpenWLS.Client.Requests;
using OpenWLS.Server.LogInstance;
using OpenWLS.Server.DBase.Models.GlobalDb;
using OpenWLS.Server.DBase.Models.LocalDb;
using Xceed.Wpf.Toolkit;

namespace OpenWLS.Client.Base
{
    // public enum FileType { OCF=0, VDF=1, LDF=2 };
    /// <summary>
    /// Interaction logic for OpenDisplayWindow.xaml
    /// </summary>
    public partial class OpenOcfWindow : Window
    {

        public IMainWindow MainWindow { get; set; }
        public bool Editable { get; set; }
        string file_filter;
        List<OperationControlFile> globalFns;
        List<OperationControlFile> localFns;

        private void global_Checked(object sender, RoutedEventArgs e)
        {
            bool g = (bool)globalCb.IsChecked;
            if (g)
            {
                if (globalFns == null)
                    globalFns = OcfRequest.GetAll(g).Result;
                fileList.ItemsSource = globalFns;
            }
            else
            {
                if (localFns == null)
                    localFns = OcfRequest.GetAll(g).Result;
                fileList.ItemsSource = localFns;
            }

        }

        public OpenOcfWindow()
        {
            InitializeComponent();
            opModeCb.ItemsSource = Enum.GetNames(typeof(LiMode)).ToList();
            ocfCb.ItemsSource = new List<string> { "Ocf", "Ldf" };
            opModeCb.SelectedIndex = 0;
            globalFns = null;
            localFns = null;
            global_Checked(null, null);
        }


        private async void fileList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (fileList.SelectedItem != null)
            {
                OpenOcf((OperationControlFile)fileList.SelectedItem);
                Close();
            }
        }



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
            

        }
        private void ocf_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
        private void file_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OperationControlFile ocf = fileList.SelectedItem as OperationControlFile;
            descTb.Text = ocf.Desc;
        }


        
        void OpenOcf(OperationControlFile ocf)
        {
            if (ocf.Body == null)
                ocf.Body = OcfRequest.GetBody(ocf.Id, (bool)globalCb.IsChecked).Result;
            MainWindow.OpenOcf(ocf, Editable);

        }
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (fileList.SelectedItem != null)
            {
                var v  = fileList.SelectedItem;
                 OpenOcf((OperationControlFile)v);
            }
            Close();

        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
