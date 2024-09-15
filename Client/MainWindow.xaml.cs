using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using OpenWLS.Client.LogInstance;
using OpenWLS.Client.LogInstance.Instrument;
using OpenWLS.Client.Base;
using OpenWLS.Client.Dock;
using OpenWLS.Client.GView.GUI;
using OpenWLS.Client.LogDataFile;
using OpenWLS.Client.Requests;
using OpenWLS.Server.LogInstance.OperationDocument;
using OpenWLS.Server.DBase.Models.GlobalDb;
using OpenWLS.Server.DFlash;
using OpenWLS.Server.GView.Models;
using OpenWLS.Server.GView.ViewDefinition;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.Toolkit;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;
using UserControl = System.Windows.Controls.UserControl;

namespace OpenWLS.Client
{
    public interface IMainWindow
    {
        void AddLdfExplore(Explore exp);
        void OpenVdf(GViewDefinitionFile vdf, bool global, bool rt);
        void OpenOcf(OperationControlFile ocf, bool edit);
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window , IMainWindow
    {
     //   List<DockControl> displayCntls;
        SysLogCntl sysLogCntl;

        //    public LiClientMainCntl apiCntl;
        //     public CVCntl cvCntl;
        public MainWindow()
        {
            ClientGlobals.Init();
            //    displayCntls = new List<DockControl>();
            rtGViews = new LogGViews();
            InitializeComponent();
            SysLogCntl sysLogCntl = new SysLogCntl();
            syslogDock.SetCntl(sysLogCntl);
            syslogDock.CanHide = false;
            syslogDock.CanClose = false;
            ClientGlobals.SysLog = sysLogCntl;


            liDock.CanClose = false;
            liDock.CanHide = false;
            liDock.IsVisible = false;
            n1dDock.IsVisible = false;  
        }





        public DockDocument AddDisplayDocCntl(UserControl cntl)
        {
            DockDocument dc = new DockDocument();
            dc.Title = cntl.Name;
            dc.SetCntl(cntl);
            docPane.Children.Insert(0, dc);
            dc.IsSelected = true;
            return dc;
        }



        public void CloseDockDocuments(List<DockDocument> dcs)
        {
            foreach(DockDocument dc in dcs)
                docPane.Children.Remove(dc);
        }

       
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           
        }

        private void ExitApplication(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
     /*       LiState s = apiClient.LiState;
            if (s == LiState.Log || s == LiState.Playback || s == LiState.CV)
                MessageBox.Show(s + " is in process, please stop it.", "Warning");
            else
     */
           
            CloseApp();
        }

        public void CloseApp()
        {
         //   client.Close();
        //    server.Close();
        //    ClientGlobals.Save();
            System.Windows.Application.Current.Shutdown();

        }

        private void SelectUnit_Click(object sender, RoutedEventArgs e)
        {
          //  UnitSelDlg dlg = new UnitSelDlg();
          //  dlg.ShowDialog();
        }
        private void windowsMenuItem_Click(object sender, RoutedEventArgs e)
        {
         //   apiWinMenu.IsChecked = apiCntl.IsVisible;
         //   n1dWinMenu.IsChecked = apiClient.N1D.IsVisible;
         //   sysLogWinMenu.IsChecked = sysLogCntl.IsVisible;
        }

        void ShowHideWindow(DockControl c)
        {
            if (c.IsVisible)
                c.Hide();
            else
                c.Show();
        }

        private void windows_Click(object sender, RoutedEventArgs e)
        {
            MenuItem m = (MenuItem)sender;
            string name = (string)m.Header;
            if (name == "LogInstance")
                ShowHideWindow(liDock);
            else if (name == "N1D")
                ShowHideWindow(n1dDock);
            else if (name == "Syslog")
                ShowHideWindow(syslogDock);
        }



        private void JobMenuItem_Click(object sender, RoutedEventArgs e)
        {
            JobsEditor jobsEditor = new JobsEditor();
            jobsEditor.ShowDialog();
        }


        private void cvMainMenu_Click(object sender, RoutedEventArgs e)
        {
         /*   if (!cvCntl.Visible)
            {
                AddDisplayDocCntl(cvCntl);
                cvCntl.Visible = true;
            }*/ 
       }



        private void aboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("LogView 1.0 \n Freeware \n logview.wu@gmail.com ", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void dfMenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
        {
         /*   if (docPane.SelectedContent != null && docPane.SelectedContent is DockDocument)
            {
                DockDocument dd = (DockDocument)docPane.SelectedContent;
                dfExportMenu.IsEnabled = dd.Control is ArExploere;
            }
            else
                dfExportMenu.IsEnabled = false;*/
        }

        private void cvManMenuItem_Click(object sender, RoutedEventArgs e)
        {
         //   apiClient.ShowCVMan();
        }

        public void OpenFileSelected( string path, string fn, string file_filter)
        {
            if (file_filter == DFlashPageFile.file_filter)
            {
           //     string cmd = "DFP\nOpen\n" + inst.UID.ToString() + "\n" + fn;
          //      apiClient.SendRequest(cmd);
            }
        }

    }
}
