using Newtonsoft.Json;
using OpenWLS.Server.Base;
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



namespace OpenWLS.Client.LogInstance.Instrument
{
    /// <summary>
    /// Interaction logic for GInstCntl.xaml
    /// </summary>
    public abstract class EdgeDeviceCntl : UserControl
    {
        protected LiClientMainCntl liClient;
        public LiClientMainCntl LiClient { set { liClient = value; } }

        public EdgeDeviceCntl()
        {

            //       InitializeComponent();
        }

        public virtual void ProcessEdgeDeviceMsg(byte[]? bs)
        {

        }
     /*   public void SendTxtRequest(string msg)
        {
          ///  string str = API.LogInstance.str_api_mod_inst + "\n" + Inst.UID.ToString() + "\n" + msg;
            Inst.SendRequest(str);
        }
     */
    }
    



}
