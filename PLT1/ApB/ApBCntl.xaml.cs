using OpenWLS.Client.LogInstance;
using OpenWLS.Client.LogInstance.Instrument;
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



namespace OpenWLS.PLT1.ApB
{
    /// <summary>
    /// Interaction logic for USBTestCntl.xaml
    /// </summary>
    public partial class ApBCntl : InstCntl
    {
        delegate void UpdateButtonDelegate(Button btn, string str);

        public ApBCntl()
        {
            InitializeComponent();
            UpdateButton(dpConnectioBtn, "USBOff");
        }

        public void SetPortSerialNumbers(string[] sns)
        {
            Dispatcher.Invoke(() => {            
                cb_commPort.ItemsSource = sns;
                if(sns.Length > 0 && sns[0].Length > 0)
                    cb_commPort.SelectedIndex = 0;
                //Update the UI
            });
        }

        public void UpdateButton(Button btn, string str)
        {

            Dispatcher.Invoke(() => {
                if (btn != null)
                {   
                    btn.Content = FindResource(str);
                }

            });

        }




        private void cb_commPort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
          //  Inst.SendRequest("TxSpeed\n" + cb_TxSpeed.SelectedItem);
        }

        private void commsUpdateBtn_Click(object sender, RoutedEventArgs e)
        {
          //  Inst.SendRequest("USBPort\nList");
        }
        private void dpConnectionBtn_Click(object sender, RoutedEventArgs e)
        {
          /*  if ( ((APIClient.APIClient)Inst.Client).LiState == Base.API.LiState.HwDPDisconnected_Log)
                Inst.SendRequest("USBPort\nConnect\n" + cb_commPort.SelectedItem);
            else
            {
                if (((APIClient.APIClient)Inst.Client).LiState == Base.API.LiState.Standby_Log)
                    Inst.SendRequest("USBPort\nDisconnect");
            }
           */     
            
                

        }

        private void tbTxGain_ValueChanged(object sender, EventArgs e)
        {
           // if (Inst != null)
           //     Inst.SendRequest("Tx\nGain\n" + tbTxGain.Value);
        }
        /*
*                         UpdateButton(dpConnectioBtn, "USBOn");
               ShowAcqBtns(Visibility.Visible);
           }
           else
           {

*/
    }
}
