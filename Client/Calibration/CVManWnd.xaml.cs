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

using Newtonsoft.Json;
using OpenWLS.Server.Base;

namespace OpenWLS.Client.LogInstance.Calibration
{
    /// <summary>
    /// Interaction logic for CVMan.xaml
    /// </summary>
    public partial class CVManWnd : Window
    {

        CVReportView reportView;

        public CVManWnd()
        {
            InitializeComponent();
            phaseCb.ItemsSource = new string[] { "", "CP", "VP", "VB", "VA" };

            reportView = new CVReportView();
            reportCntl.Child = reportView;
        }

        public string UpdateList(DataReader r)
        {
            int total = Convert.ToInt32(r.ReadLine());
            int c = Convert.ToInt32(r.ReadLine());
        //    CVInforRecords items = JsonConvert.DeserializeObject<CVInforRecords>(r.ReadString());
        //    cvList.ItemsSource = items;

            CollectionView cv = (CollectionView)CollectionViewSource.GetDefaultView(cvList.ItemsSource);
            PropertyGroupDescription group1 = new PropertyGroupDescription("Serial");
            PropertyGroupDescription group2 = new PropertyGroupDescription("Asset");
            cv.GroupDescriptions.Add(group1);
            cv.GroupDescriptions.Add(group2);
            return null;
        }

        private void applyBtn_Click(object sender, RoutedEventArgs e)
        {
            string s = "";
            if (serialCb.SelectedItem != null)
            {
                string s1 = (string)serialCb.SelectedItem;
                if (!string.IsNullOrEmpty(s1))
                    s = s + "SN= '" + s1 + "' AND";
            }
            if (assetCb.SelectedItem != null)
            {
                string s1 = (string)assetCb.SelectedItem;
                if (!string.IsNullOrEmpty(s1))
                    s = s + "AN = '" + s1 + "' AND";
            }
            if (dateFromPk.SelectedDate != null)
            {
                string s1 = (string)dateFromPk.SelectedDate.Value.Ticks.ToString();
                s = s + "DT >= " + s1 + " AND";
            }
            if (dateToPk.SelectedDate != null)
            {
                string s1 = (string)dateToPk.SelectedDate.Value.Ticks.ToString();
                s = s + "DT <=" + s1 + " AND";
            }
            if (phaseCb.SelectedItem != null)
            {
                string s1 = (string)phaseCb.SelectedItem;
                if (!string.IsNullOrEmpty(s1))
                    s = s + "PH = '" + s1 + "' AND";
            }
    /*        if (s.Length > 0)
                client.SendRequest("CVMan\nGetInfors\n" + s.Substring(0, s.Length - 3));
            else
                client.SendRequest("CVMan\nGetInfors\n\n");
    */
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void caliImportQtHexFileMenu_Click(object sender, RoutedEventArgs e)
        {

        }
        private void caliExportQtHexFileMenu_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
         //   client.CloseCVMan();
        }


        string UpdateSerials(DataReader r)
        {
            string s = r.ReadString();
            string[] ss = s.Split('|');
            serialCb.ItemsSource = null;
            serialCb.ItemsSource = ss;
            return null;
        }

        string UpdateAssets(DataReader r)
        {
            string s = r.ReadString();
            string[] ss = s.Split('|');
            assetCb.ItemsSource = null;
            assetCb.ItemsSource = ss;
            return null;
        }
        public string ProcPackageBinary(DataReader r)
        {
            Dispatcher.Invoke(() =>
            {
              /*  CVBinMsgType t = (CVBinMsgType)r.ReadUInt16();
                switch (t)
                {
                    case CVBinMsgType.AGE:
                        reportView.ProcessBinMsg(r);
                      //  reportView.UpdateView();
                        break;
                    default:
                        break;
                }
              */
            });
            return null;
        }

        public string ProcPackageText(DataReader r)
        {
            string str = null;
            Dispatcher.Invoke(() =>
            {
                string s = r.ReadLine();
                if (s == "Serials")
                    str = UpdateSerials(r);
                if (s == "Assets")
                    str = UpdateAssets(r);  
                if (s == "Infors")
                    str = UpdateList(r);
            });
            return str;
        }

        private void serialCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
         //   client.SendRequest("CVMan\nGetAssets\n"+ serialCb.SelectedItem);
        }

        private void assetCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cvList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(cvList.SelectedItem != null)
            {
              //  CVInforRecord infor = (CVInforRecord)cvList.SelectedItem;
             //   client.SendRequest("CVMan\nGetDRecord\n" + JsonConvert.SerializeObject(infor));
            }
        }
    }

    public class GroupItemStyleSelector : StyleSelector
    {
        public Style FirstLevel
        { get; set; }

        public Style SecondLevel
        { get; set; }
        public override Style SelectStyle(object item, DependencyObject container)
        {
            Style s;

            CollectionViewGroup group = item as CollectionViewGroup;

            if (group.Items.Count > 0)
            {
           /*     CVInforRecord stinfo = group.Items[0] as CVInforRecord;

                if (stinfo != null)
                    return FirstLevel;
                else
                    return SecondLevel;
           */
            }
            /* 
             Window window = Application.Current.MainWindow;

             if (!group.IsBottomLevel)
             {
                 s = window.FindResource("Serial") as Style;
             }
             else
             {
                 s = window.FindResource("Asset") as Style;
             }
            
            return s;
            */
            return null;
        }
    }
    /*
    public class CVListSelector : DataTemplateSelector
    {
        public DataTemplate SerialTemplate
        { get; set; }

        public DataTemplate AssetTemplate
        { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            ContentPresenter cp = container as ContentPresenter;

            if (cp != null)
            {
                CollectionViewGroup cvg = cp.Content as CollectionViewGroup;

                if (cvg.Items.Count > 0)
                {
                    CVInforRecord stinfo = cvg.Items[0] as CVInforRecord;

                    if (stinfo != null)
                        return SerialTemplate;
                    else
                        return AssetTemplate;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
    */
}

