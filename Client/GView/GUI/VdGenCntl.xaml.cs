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

using OpenWLS.Server.GView.ViewDefinition;
using OpenWLS.Server.LogDataFile.Models;

namespace OpenWLS.Client.GView.GUI
{
    /// <summary>
    /// Interaction logic for PdGenCntl.xaml
    /// </summary>
    public partial class PdGenCntl : UserControl
    {

        VdDFiles dFiles;
        VdDocument doc;

        static List<double> indexScales = new List<double> { 10, 20, 50, 200, 500, 1000 };

        public VdDocument Doc
        {
            set
            {
               doc  = value;
               UpdateInfor();
            }
        }
    /*    public VdDFile RtDFile
        {
            set
            {

            }
        }*/
        public VdDFiles DFiles
        {
            set
            {
                dFiles = value;
                UpdateInfor();
                //    if(dFiles != null)
                //        dFiles.FilesInforChanged += DFiles_FilesInforChanged; 
            }
        }

        private void DFiles_FilesInforChanged(object sender, EventArgs e)
        {
            UpdateInfor();
        }

        void SetIndexesMinMax()
        {
            if (doc != null && dFiles != null)
            {
                switch (doc.IndexUnit)
                {
                    case IndexUnit.meter:
                        topMinCntl.Text = dFiles.TopDepth.ToString();
                        botMaxCntl.Text = dFiles.BottomDepth.ToString();
                        break;
                    case IndexUnit.ft:
                        topMinCntl.Text = (dFiles.TopDepth / 0.3048).ToString();
                        botMaxCntl.Text = (dFiles.BottomDepth / .3048).ToString();
                        break;
                    case IndexUnit.second:
                        topMinCntl.Text = dFiles.StartTime.ToString();
                        botMaxCntl.Text = dFiles.StopTime.ToString();
                        break;

                }
            }
        }

        public void UpdateInfor()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                DataContext = null;
                DataContext = doc;
                SetIndexesMinMax();
            }));
        }

        public PdGenCntl()
        {
            InitializeComponent();

            indexUnitCntl.ItemsSource = Enum.GetValues(typeof(IndexUnit));
            indexScale.ItemsSource = indexScales;
            minorCb.ItemsSource = System.Enum.GetValues(typeof(TrackDateTime));
            orientionCb.ItemsSource = System.Enum.GetValues(typeof(OpenWLS.Server.GView.ViewDefinition.Orientation));
            orientionCb.SelectedIndex = 0;
        }

        private void indexUnitCntl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetIndexesMinMax();
            if(doc!=null && doc.IndexUnit == IndexUnit.date_time)
            {
                scaleGrid.Visibility = Visibility.Hidden;
                minorGrid.Visibility = Visibility.Visible;
            }
            else
            {
                scaleGrid.Visibility = Visibility.Visible;
                minorGrid.Visibility = Visibility.Hidden;
            }
        }

        private void SetIndexes_Btn_Click(object sender, RoutedEventArgs e)
        {
            doc.Top = Convert.ToDouble(topMinCntl.Text);
            doc.Bottom = Convert.ToDouble(botMaxCntl.Text);
            DataContext = null;
            DataContext = doc;
        }
    }
}
