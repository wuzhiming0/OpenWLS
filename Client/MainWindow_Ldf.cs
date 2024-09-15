using OpenWLS.Client.Base;
using OpenWLS.Client.Dock;
using OpenWLS.Client.LogDataFile;
using OpenWLS.Client.Requests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace OpenWLS.Client
{
    public partial class MainWindow
    {
        private void openDFile_Click(object sender, RoutedEventArgs e)
        {
            OpenLdfWindow w = new OpenLdfWindow();
            w.Show();
        }

        private void ImportDFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "LAS files (*.las)|*.las|DLIS files (*.dlis)|*.dlis|XTF files (*.xtf)|*.xtf|All files (*.*)|*.*",
                InitialDirectory = ClientGlobals.ActiveJob.GetJobDirectory(),
                RestoreDirectory = true,
            };
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fn = openFileDialog.FileName;
                LdfRequest.Import(fn);
            }
        }

        private void ExportDlis_Click(object sender, RoutedEventArgs e)
        {
            DockDocument dd = (DockDocument)docPane.SelectedContent;
            Explore exp = (Explore)dd.Control;
            exp.ExportDlis();
        }

        private void ExportLasV2_Click(object sender, RoutedEventArgs e)
        {
            DockDocument dd = (DockDocument)docPane.SelectedContent;
            Explore exp = (Explore)dd.Control;
            exp.ExportLasV2();
        }
        private void ExportLasV3_Click(object sender, RoutedEventArgs e)
        {
            DockDocument dd = (DockDocument)docPane.SelectedContent;
            Explore exp = (Explore)dd.Control;
            exp.ExportLasV3();
        }

        public void AddLdfExplore(Explore exp)
        {
            DockDocument dc = new DockDocument();
            dc.Title = Path.GetFileNameWithoutExtension(exp.DataFileInfor.FileName);
            dc.SetCntl(exp);
            docPane.Children.Insert(0, dc);
            dc.IsSelected = true;
        }

    }
}
