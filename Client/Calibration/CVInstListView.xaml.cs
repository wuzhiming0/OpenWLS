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


using System.Collections;
using System.ComponentModel;
using OpenWLS.Server.LogInstance.Calibration;

namespace OpenWLS.Client.LogInstance.Calibration
{
    public class StatusToColorConverter : IValueConverter
    {
        #region IValueConverter implementation

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var s = (CVPhaseStatus)System.Convert.ToInt32(value);

            switch (s)
            {
                case  CVPhaseStatus.OK:
                    return Colors.Green;
                case  CVPhaseStatus.Failed:
                    return Colors.Red;
                default:  
                    return Colors.Gray;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class StatusToTextConverter : IValueConverter
    {
        #region IValueConverter implementation

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var s = (CVPhaseStatus)System.Convert.ToInt32(value);

            if (s == CVPhaseStatus.NotAvailable)
                return "";
            else
                return s.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    /// <summary>
    /// Interaction logic for CVInstListView.xaml
    /// </summary>
    public partial class CVInstListView : UserControl
    {
        delegate void UpdateSummaryDelegate();
        public event EventHandler CVPhaseClicked;
        List<CVSensorStatus> summary;
        TextBlock tbSel;

        List<string> selectedPhases;
        public List<CVSensorStatus> Summary
        {
            set
            {
                summary = value;
                UpdateSummary();
            }
        }

        public string GetSelectedPhase()
        {
            if (tbSel != null)
                return tbSel.Name.Remove(0, 3);
            return "";
        }

        void AddSelectedPhase(string str)
        {

        }

        void RemoveSelectedPhase(string str)
        {

        }

        public CVInstListView()
        {
            selectedPhases = new List<string>(); 
            InitializeComponent();
            cvLV.MouseDoubleClick += cvLV_MouseDoubleClick;

        }

        void cvLV_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (cvLV.SelectedItem != null && tbSel != null)
            {
                EventHandler handler = CVPhaseClicked;
                if (handler != null)
                    handler(cvLV.SelectedItem, new EventArgs());
            }
        }

        public void UpdateSummary()
        {
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke(new UpdateSummaryDelegate(UpdateSummary), null);
            else
            {
                cvLV.ItemsSource = null;
                cvLV.ItemsSource = summary;
                ICollectionView view = CollectionViewSource.GetDefaultView(cvLV.ItemsSource);
                PropertyGroupDescription groupDescription = new PropertyGroupDescription("SerialNumber");
                view.GroupDescriptions.Add(groupDescription);
            }

        }

        private void Txt_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            tb.Background = Brushes.Blue;
            tbSel = tb;
        }

        private void Txt_MouseLeave(object sender, MouseEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            tb.Background = tb.Tag == null ? Brushes.White : Brushes.Yellow;
        }

        private void Txt_MouseDown(object sender, MouseEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            if (tb.Tag == null)
            {
                tb.Tag = 1;
                tb.Background = Brushes.Yellow;
                AddSelectedPhase(tb.Text.Remove(0, 3));
            }
            else
            {
                tb.Tag = null;
                RemoveSelectedPhase(tb.Text.Remove(0, 3));
            }

        }
    }
}
