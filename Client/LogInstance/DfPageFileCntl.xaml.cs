using OpenWLS.Client.Base;
using OpenWLS.Client.LogInstance.Instrument;
using OpenWLS.Server.Base;
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
    /// Interaction logic for DfPageFileCntl.xaml
    /// </summary>
    public partial class DfPageFileCntl : UserControl
    {
        DFlashPageFile dataFile;
        InstrumentC inst;
        public InstrumentC Inst
        {
            set
            {
                inst = value;
            }
        }
//        public GuiClient GuiClient { get; set; }
        public DFlashPageFile DataFile 
        {
            get { return dataFile; }
            set
            {
                dataFile = value;                 
            }
        }

        public DfPageFileCntl()
        {
            InitializeComponent();
        }
        private void pfBtn_Click(object sender, RoutedEventArgs e)
        {
            string str = (string)pfShowBtn.Content;
            if (str.EndsWith("-"))
            {
                pfGrid.Visibility = Visibility.Collapsed;
                str = str.Replace('-', '+');
            }
            else
            {
                pfGrid.Visibility = Visibility.Visible;
                str = str.Replace('+', '-');
            }
            pfShowBtn.Content = str;
        }
        private void openDfBtn_Click(object sender, RoutedEventArgs e)
        {
     //       OpenFileWindow odw = new OpenFileWindow();
     //       odw.FileFilter = DFlashPageFile.file_filter;
     //       odw.SubDir = @"das/";
     //       odw.Inst = inst;
     //       odw.Show();
        }

        private void closeDfBtn_Click(object sender, RoutedEventArgs e)
        {
          //  if (inst != null)
          //      inst.SendRequest("DFP\nClose");
        }

        private void convertDfBtn_Click(object sender, RoutedEventArgs e)
        {
          //  if(inst != null)
          //      inst.SendRequest("DFP\nConvert");
            //  inst.SendRequest("DFlash\nRotate");
        }

        public void UpdateDPFile(TreeNode tn)
        {
            Dispatcher.Invoke(() => {
                DataContext = tn;
                if(tn == null)
                {
                    fileInforCntl.Visibility = Visibility.Collapsed;
                    closeBtn.Visibility = Visibility.Hidden;
                    convertBtn.Visibility = Visibility.Hidden;
                    openBtn.Visibility = Visibility.Visible;
                }
                else
                {
                    fileInforCntl.Visibility = Visibility.Visible;
                    closeBtn.Visibility = Visibility.Visible;
                    convertBtn.Visibility = Visibility.Visible;
                    openBtn.Visibility = Visibility.Hidden;
                }
            });
        }
        
    }
}
