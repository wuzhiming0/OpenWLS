using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OpenWLS.Client;
using OpenWLS.Client.LogInstance.Instrument;
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

namespace OpenWLS.Client.LogInstance
{
    
    /// <summary>
    /// Interaction logic for USBTestCntl.xaml
    /// </summary>
    public partial class EdgeDevicePlaybackCntl : EdgeDeviceCntl
    {
    
        public EdgeDevicePlaybackCntl()
        {
            InitializeComponent();

        }

        private void dfBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
