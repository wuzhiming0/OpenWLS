using OpenWLS.Client.LogInstance.Instrument;
using OpenWLS.Server.DFlash;
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

namespace OpenWLS.Client.LogInstance
{
    /// <summary>
    /// Interaction logic for DFlashFSCntl.xaml
    /// </summary>
    public partial class DFlashFSCntl : UserControl
    {
        LvDFlashFS dfFs;
        InstrumentC inst;

        public LvDFlashFS DfFs
        {
            get
            {
                return dfFs;
            }
        }
        public InstrumentC Inst
        {
            set
            {
                inst = value;
            }
        }

        public int UploadCopy
        {
            get
            {
                return readCopyCb.SelectedIndex;
            }
        }
        public DFlashFSCntl()
        {
            InitializeComponent();
            chipListGrid.Visibility = Visibility.Collapsed;
        }
        delegate void UpdateDfDelegate(LvDFlashFS df);
        public void UpdateDf(LvDFlashFS df)
        {
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke(new UpdateDfDelegate(UpdateDf), df);
            else
            {
                DataContext = null;
                DataContext = df;
                dfFs = df;
                copiesCb.ItemsSource = null;
                readCopyCb.ItemsSource = null;
                copiesCb.SelectedIndex = -1;
                readCopyCb.SelectedIndex = -1;
                if (df.Empty)
                {
                    string[] ss = new string[df.Chips.Length];
                    ss[0] = "None";
                    for (int i = 1; i < df.Chips.Length; i++)
                        ss[i] = i.ToString();
                    copiesCb.ItemsSource = ss;
                    copiesCb.SelectedIndex = 0;
                }
                else
                {
                    string[] ss = new string[df.Chips.Length];
                    ss[0] = "Primary";
                    for (int i = 1; i < df.Chips.Length; i++)
                        ss[i] = "Copy-" + i.ToString();
                    readCopyCb.ItemsSource = ss;
                    readCopyCb.SelectedIndex = 0;
                }
            }
        }
        private void copiessCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void eraseBtn_Click(object sender, RoutedEventArgs e)
        {
        //    inst.SendRequest("DFlash\nErase");      
        }

        private void uploadBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void readCopyCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void chipListBtn_Click(object sender, RoutedEventArgs e)
        {
            string str = (string)chipListBtn.Content;
            if (str.EndsWith("-"))
            {
                chipListGrid.Visibility = Visibility.Collapsed;
                str = str.Replace('-', '+');
            }
            else
            {
                chipListGrid.Visibility = Visibility.Visible;
                str = str.Replace('+', '-');
            }
            chipListBtn.Content = str;
        }

        private void rotateBtn_Click(object sender, RoutedEventArgs e)
        {
            //inst.SendRequest("DFlash\nRotate");
        }

        private void dfGridBtn_Click(object sender, RoutedEventArgs e)
        {
          //  inst.SendRequest("DFlash\nRotate");
        }




    }
}
