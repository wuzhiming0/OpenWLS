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
using OpenWLS.Server.LogDataFile;
using OpenWLS.Server.LogDataFile.Models;
using Frame = OpenWLS.Server.LogDataFile.Models.Frame;

namespace OpenWLS.Client.LogDataFile
{
    /// <summary>
    /// Interaction logic for MDetailListView.xaml
    /// </summary>
    public partial class MDetailListView : UserControl
    {
        public event EventHandler ItemSelected;

        delegate void UpdateChDetailDelegate();

        public Button DataListBtn { get; set; }

        Measurements measurements;
        Frames frames;
        public Measurements Measurements
        {
            get { return measurements; }
            set
            {
                measurements = value;
                frames = new Frames();
                frames.Init(measurements);
                UpdateChDetail();
            }
        }
        /*
        Measurements ms;
        public Measurements Measurements
        {
            get
            {
                return ms;
            }
            set
            {
                ms = value; UpdateChDetail();
            }
        }*/

        public IList SelectedItems {
        get { return chLV.SelectedItems; }
        }

        public void UpdateChDetail()
        {
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke(new UpdateChDetailDelegate(UpdateChDetail), null);
            else
            {
                chLV.ItemsSource = null;
                chLV.ItemsSource = measurements;

                ICollectionView view = CollectionViewSource.GetDefaultView(chLV.ItemsSource);
                PropertyGroupDescription groupDescription = new PropertyGroupDescription("Frame");
                view.GroupDescriptions.Add(groupDescription);
            }

        }

        public MDetailListView()
        {
     //       chTbl = new MHeadTbl();
            InitializeComponent();

            chLV.SelectionChanged += chLV_SelectionChanged;
           
        }

        void chLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ItemSelected != null && e.AddedItems.Count == 1)
                ItemSelected(e.AddedItems[0], new EventArgs());          
        }

        private void chLV_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Media.HitTestResult htr = System.Windows.Media.VisualTreeHelper.HitTest(chLV, e.GetPosition(this));
            FrameworkElement fe = (FrameworkElement)htr.VisualHit;
            CollectionViewGroup ob = (CollectionViewGroup)fe.DataContext;
            if (ob == null)
                return; 
            Frame f = (Frame)ob.Name;
            if (ItemSelected != null)
                ItemSelected(f, new EventArgs());
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            List<Measurement> chs1 = measurements.Where(h => h.Selected).ToList();
            if (chs1.Count == 0)
            {
                DataListBtn.IsEnabled = false; return;
            }
            Measurement h0 = chs1[0];

            foreach (Measurement h in chs1)
            {
                if (h.Selected)
                {
                    if (h.Head.SampleElements > 1 || (!h.Head.NumberType))
                    {
                        DataListBtn.IsEnabled = false; return;
                    }
                    if (h0.Frame != null)
                    {
                        if (h0.Frame != h.Frame)
                        {
                            DataListBtn.IsEnabled = false; return;
                        }
                    }
                    else
                    {
                        if (h.Frame != h0.Frame)
                        {
                            DataListBtn.IsEnabled = false; return;
                        }
                    }
                }
            }

            DataListBtn.IsEnabled = true;

        }
    }
    public class DimensionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int[] a = (int[])value;
            if (a == null || a.Length == 0)
                return "";
            string str = a[0].ToString();
            for (int i = 1; i < a.Length; i++)
                if (a[i] > 0)
                    str = str + "," + a[i].ToString();
            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        public static int[] GetFromString(string s)
        {
            if (s == null)
                return null;
            s = s.Trim();
            if (s.Length == 0)
                return null;
            string[] ss = s.Trim().Split(',');
            int[] ns = new int[ss.Length];
            for (int i = 0; i < ss.Length; i++)
                ns[i] = System.Convert.ToInt32(ss[i]);
            return ns;
        }
        public static string ToString(int[] ns)
        {
            if (ns == null)
                return null;
            if (ns.Length == 0)
                return null;
            string s = ns[0].ToString();
            for (int i = 1; i < ns.Length; i++)
                s = s + "," + ns[i].ToString();
            return s;
        }

    }
}
