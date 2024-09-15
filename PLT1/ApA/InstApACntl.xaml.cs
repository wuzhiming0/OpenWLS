using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OpenWLS.Client;
using OpenWLS.Client.LogInstance.Instrument;
using OpenWLS.PLT1.Edge;
using OpenWLS.Server.LogInstance.Edge;
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



namespace OpenWLS.PLT1.ApA
{
    
    /// <summary>
    /// Interaction logic for USBTestCntl.xaml
    /// </summary>
    public partial class ApACntl : InstCntl
    {
     /*   bool usbPortConnected;
        public bool UsbPortConnected
        {
            set
            {
                usbPortConnected = value;
                Dispatcher.Invoke(() => {
                   ((Image)usbBtn.Content).Source = usbPortConnected?  new BitmapImage(new Uri(@"pack://application:,,,/images/usb-on.png", UriKind.Absolute))
                   : new BitmapImage(new Uri(@"pack://application:,,,/images/usb-off.png", UriKind.Absolute));
                });
            }
        }*/
        public ApACntl()
        {
            InitializeComponent();
         //   usbGrid.Visibility = ClientGlobals.Simulation == null? Visibility.Visible : Visibility.Collapsed; 
         //   usbBtn.Content = new Image();
         //   UsbPortConnected = false;
        }

        private void chBtn_Click(object sender, RoutedEventArgs e)
        {
            ((InstCApA)Inst).IdentifyInstruments();
        }


    }
}
