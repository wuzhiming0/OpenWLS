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
using OpenWLS.Server.DBase.Models.GlobalDb;
using OpenWLS.Server.DBase.Models.LocalDb;
using Xceed.Wpf.Toolkit;

namespace OpenWLS.Client.Base
{

    /// <summary>
    /// Interaction logic for OpenDisplayWindow.xaml
    /// </summary>
    public partial class OpenVdfWindow : Window
    {

        public IMainWindow MainWindow { get; set; }
        string file_filter;
        List<GViewDefinitionFile> globalFns;
        List<GViewDefinitionFile> localFns;



        private void global_Checked(object sender, RoutedEventArgs e)
        {
            bool g = (bool)globalCb.IsChecked;
            if (g)
            {
                if (globalFns == null)
                    globalFns = VdfRequest.GetAll(g).Result; 
                fileList.ItemsSource = globalFns;
            }
            else
            {
                if (localFns == null)
                    localFns = VdfRequest.GetAll(g).Result;
                fileList.ItemsSource = localFns;
            }

        }

        public OpenVdfWindow()
        {
            InitializeComponent();
            globalFns = null;
            localFns = null;
        }


        private async void fileList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (fileList.SelectedItem != null)
            {
                OpenVdf((GViewDefinitionFile)fileList.SelectedItem);
                Close();
            }
        }


        private void file_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GViewDefinitionFile vdf = fileList.SelectedItem as GViewDefinitionFile;
            descTb.Text = vdf.Desc;

        }

        void OpenVdf(GViewDefinitionFile vdf)
        {
            bool global = (bool)globalCb.IsChecked;
            if(vdf.Body == null)
                vdf.Body = VdfRequest.GetBody(vdf.Id, global).Result.Val;
            MainWindow.OpenVdf(vdf, global, (bool)rtCb.IsChecked);
        }

        
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (fileList.SelectedItem != null)
            {
                var v  = fileList.SelectedItem;
                OpenVdf((GViewDefinitionFile)v);
            }
            Close();

        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
