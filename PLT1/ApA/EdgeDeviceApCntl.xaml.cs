using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OpenWLS.Client;
using OpenWLS.Client.LogInstance.Instrument;
using OpenWLS.PLT1.Edge;
using OpenWLS.Server.Base;
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
using System.Windows.Forms;
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
    public partial class EdgeDeviceApCntl : EdgeDeviceCntl
    {
        bool usbPortConnected;
        InstCApA inst;

        public InstCApA Inst { set { inst = value; } }
        public bool UsbPortConnected
        {
            set
            {
                usbPortConnected = value;
              //  Dispatcher.Invoke(() => {
                   ((Image)usbBtn.Content).Source = usbPortConnected?  new BitmapImage(new Uri(@"pack://application:,,,/images/usb-on.png", UriKind.Absolute))
                   : new BitmapImage(new Uri(@"pack://application:,,,/images/usb-off.png", UriKind.Absolute));
               // });
            }
        }
        public EdgeDeviceApCntl()
        {
            InitializeComponent();
            usbGrid.Visibility = ClientGlobals.Simulation == null? Visibility.Visible : Visibility.Collapsed; 
            usbBtn.Content = new Image();
            UsbPortConnected = false;
        }

        public override void ProcessEdgeDeviceMsg(byte[]? bs)
        {
            if (bs == null) return;
            uint[] us = IntArrayConverter.GetUint32Array(bs);
            switch (us[0])
            {
                case 0:                                         //disconnected, available port list 
                    UsbPortConnected = false;
                    List<uint?> assets = new List<uint?>();
                    assets.Add(null);
                    if (inst != null)
                    {
                        uint? asset = inst.Asset;
                        if (asset != null)
                            assets.Add((uint)asset);
                    }
                    for (int i = 1; i < us.Length; i++)
                        assets.Add(us[i]);
                    assetsCb.ItemsSource = assets; 
                    assetsCb.SelectedIndex = 0;
                    break;
                case 1:                                     //connected, port  
                    UsbPortConnected = true;
                    assetsCb.ItemsSource = new uint[] { us[1] }; 
                    assetsCb.SelectedIndex = 0;
                    break;
                case 2:                                     //disconnected
                    UsbPortConnected = false;
                    break;
                case 3:                                    //connected 
                    UsbPortConnected = true;
                    break;

            }

        }

        private void usbBtn_Click(object sender, RoutedEventArgs e)
        {
            // return available port list
            uint? asset = usbPortConnected? 0   // disconnect the port if connected,  - connect to port with device asset = 0 ( while 0 is invalid ).
                                          : (uint?)assetsCb.SelectedItem;
    
            if(asset == null)
                liClient.SendEdgeDeviceGuiPackage(null);
            else
                liClient.SendEdgeDeviceGuiPackage( BitConverter.GetBytes((uint)asset));

        }
    }
}
