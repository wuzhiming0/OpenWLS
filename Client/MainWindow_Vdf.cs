using OpenWLS.Client.Base;
using OpenWLS.Client.Dock;
using OpenWLS.Client.GView.GUI;
using OpenWLS.Server.DBase.Models.GlobalDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OpenWLS.Client
{
    public partial class MainWindow
    {
        LogGViews rtGViews;

        public LogGViews RtGViews { get { return rtGViews;  } }

        public void OpenVdf(GViewDefinitionFile vdf, bool global, bool rt)
        {
            LogGView logGView = new LogGView();

            logGView.Open(vdf, global, rt);         
            logGView.DockDocument = AddDisplayDoc(logGView, rt);
        }

        private void rtGViewDockDocument_Closed(object? sender, EventArgs e)
        {
            rtGViews.Remove((LogGView)((DockDocument)sender).Control);
        }

        public DockDocument AddDisplayDoc(LogGView logGView, bool rt)
        {
            DockDocument dc = new DockDocument();
            dc.Title = logGView.Name;
            dc.SetCntl(logGView);
            docPane.Children.Insert(0, dc);
            dc.IsSelected = true;            
            if (rt && liCntl != null)
            {
                logGView.DockDocument.Closed += rtGViewDockDocument_Closed;
                logGView.Id = rtGViews.Count == 0 ? 1 : rtGViews.Max(a => a.Id) + 1;
                rtGViews.Add(logGView);
                liCntl.SendNewGViewRequest(logGView.GetRtVdDoc(false));
            }
            return dc;
        }

        //  #region Presentation menu
        private void openDisplayMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenVdfWindow ofw = new OpenVdfWindow()
            {
                MainWindow = this
            };
            ofw.Show();
        }
        private void saveDisplayMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveAsDisplayMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // SaveAsFile(displayClient);
        }

        private void newRtDisplayMenuItem_Click(object sender, RoutedEventArgs e)
        {
            LogGView logGView = new LogGView();
            logGView.New(false, true);
            AddDisplayDoc(logGView, true);
        }

        private void newNrtDisplayMenuItem_Click(object sender, RoutedEventArgs e)
        {
            LogGView logGView = new LogGView();
            logGView.New(false, false);
            AddDisplayDoc(logGView, false);
        }
        private void headerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            /*  HeaderEditor he = new HeaderEditor();
               he.JobFolder = clients.SelectedJob.Folder;
               he.Show();*/
        }

        private void plotManMenuItem_Click(object sender, RoutedEventArgs e)
        {
            /*   PlotMan pm = new PlotMan();
               pm.JobFolder = clients.SelectedJob.Folder;
               pm.Show();*/
        }
     //   #endregion
    }
}
