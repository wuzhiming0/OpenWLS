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
using OpenWLS.Client.LogInstance.Instrument;

namespace OpenWLS.PLT1.TesterA
{
    /// <summary>
    /// Interaction logic for USBTestCntl.xaml
    /// </summary>
    public partial class TesterACntl : InstCntl
    {
        delegate void UpdateButtonDelegate(Button btn, string str);

        public TesterACntl()
        {
            InitializeComponent();

            ToolScanEnable = false;
            PowerOn = false;    
        }
        bool powerOn;
        public bool PowerOn
        {
            get { return powerOn; }
            set
            {
                powerOn = value;
                string str = powerOn ? "PowerOn" : "PowerOff";
                UpdateButton(powerBtn, str);
            }
        }

        bool ToolScanEnable
        {
            set
            {
                string str = value ? "ScanOn" : "ScanOff";
                scanToolBtn.Content = FindResource(str);
                scanToolBtn.IsEnabled = value;
            }
        }

        public void UpdateButton(Button btn, string str)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new UpdateButtonDelegate(UpdateButton), new object[] { btn, str });
            }
            else
            {
                if (btn != null)
                {   
                    btn.Content = FindResource(str);
                }

            }

        }

        private void powerBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void scanToolBtn_Click(object sender, RoutedEventArgs e)
        {

        }


    }
}
