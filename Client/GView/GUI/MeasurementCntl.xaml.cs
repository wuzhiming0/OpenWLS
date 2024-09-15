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
using OpenWLS.Client.LogInstance;
using OpenWLS.Server.GView.ViewDefinition;

namespace OpenWLS.Client.GView.GUI
{
    /// <summary>
    /// Interaction logic for MeasurementCntl.xaml
    /// </summary>
    public partial class MeasurementCntl : UserControl
    {

        VdDFiles dfiles;
        bool file_update;
        public bool D1Channel { get; set; }
        public bool realTime;
        public bool RealTime
        {
            set
            {
                realTime = value;
                if (realTime)
                    fileCntl.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        public VdDFiles DFiles
        {
            set
            {
                if (realTime) return;
                dfiles = value;
                fileCntl.ItemsSource = null;
                fileCntl.ItemsSource = dfiles;
                UpdateDMeasurement();
            }
        }

        public VdMeasurements Measurements
        {
            set
            {
                chCntl.ItemsSource = D1Channel ? value : value.Where(a => a.Dims != null).ToList();
            }
        }

        public VdMeasurement Measurement
        {
            set {
                DataContext = value;
                UpdateDMeasurement();
            }  
        }

        public void UpdateDMeasurement()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (DataContext != null && DataContext is VdMeasurement && dfiles != null)
                {
                    bool rt = fileCntl.Visibility == System.Windows.Visibility.Hidden;
                    VdMeasurement c = (VdMeasurement)DataContext;

                    VdDFile f = rt ? LiClientMainCntl.GetLiClientMainCntl().RtVdFile : dfiles.GetDfile(c.FileID);
                    if (f != null)
                    {
                        c.DFile = f;
                        Measurements = f.Measurements;
                    }

                    file_update = true;
                    DataContext = null;
                    DataContext = c;
                    chCntl.Foreground = c.DFile == null ? Brushes.Red : Brushes.Green;
                    file_update = false;
                }
            }));
        }

        public MeasurementCntl()
        {
            InitializeComponent();
            file_update = false;
        }

   
        private void fileCntl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (file_update) return;
            if (DataContext is VdMeasurement)
            {
                VdMeasurement c = (VdMeasurement)DataContext;
                if (fileCntl.SelectedItem is VdDFile)
                {
                    VdDFile f = ((VdDFile)fileCntl.SelectedItem);
                    if (f == null && dfiles.Count > 0)
                    {
                        f = dfiles[0];
                        fileCntl.SelectedItem = f;
                    }

                    if (f != null)
                    {
                        c.FileID = f.Id;
                      //  c.fName = f.Name;
                        if (D1Channel)
                            chCntl.ItemsSource = f.Measurements;
                        else
                            chCntl.ItemsSource = f.Measurements.Where(a => a.Dims == null).ToList(); 
                    }
                }
            }
        }
    }
}
