using OpenWLS.Server.LogInstance.Instrument;
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
    /// Interaction logic for ModemCntl.xaml
    /// </summary>
    public partial class ModemCntl : UserControl
    {
        InstCApA inst;
        bool update_lock;
        public InstCApA Inst { set { inst = value; } }

        public ushort TxGain
        {
            set
            {
//                Dispatcher.Invoke(() =>
//                {
                    update_lock = true;
                    tbTxGain.Value = value;
                    update_lock = false;
  //              });
            }
        }
        public ushort TxEqu
        {
            set
            {
//                Dispatcher.Invoke(() =>
//                {
                    update_lock = true;
                    tbTxEqu.Value = value;
                    update_lock = false;
//                });
            }
        }


        public ModemCntl()
        {
            update_lock = true;
            InitializeComponent();
            update_lock = false;
        }

        public void UpdateCntlTblItem(int offset)
        {
            if (inst == null || inst.CntlTbl == null) return;
            update_lock = true;
            switch (offset)
            {
                case CntlTblAp.offset_tx_gain:
                    tbTxGain.Value = ((CntlTblAp)inst.CntlTbl).TxGain;
                    break;
                case CntlTblAp.offset_tx_equ:
                    tbTxEqu.Value = ((CntlTblAp)inst.CntlTbl).TxEqu;
                    break;
                case CntlTblAp.offset_tx_speed:
                    break;
                case CntlTblAp.offset_rx_gain:
                    tbRxGain.Value = ((CntlTblAp)inst.CntlTbl).RxGain;
                    break;
                case CntlTblAp.offset_rx_equ:
                    tbRxEqu.Value = ((CntlTblAp)inst.CntlTbl).RxEqu;
                    break;
                case CntlTblAp.offset_rx_speed:
                    break;
            }
        }

        private void tbTxGain_ValueChanged(object sender, EventArgs e)
        {
            if (update_lock) return;
            update_lock = true;
            inst.SendCntTblItem(CntlTblAp.offset_tx_gain, BitConverter.GetBytes((ushort)tbTxGain.Value));
            tbTxGain.Value = ((CntlTblAp)inst.CntlTbl).TxGain;
            update_lock = false;
        }
        private void tbTxEqu_ValueChanged(object sender, EventArgs e)
        {
            if (update_lock) return;
            update_lock = true;
            inst.SendCntTblItem(CntlTblAp.offset_tx_equ, BitConverter.GetBytes((ushort)tbTxEqu.Value));
            tbTxEqu.Value = ((CntlTblAp)inst.CntlTbl).TxEqu;
            update_lock = false;
        }

        private void cb_TxSpeed_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void tbRxGain_ValueChanged(object sender, EventArgs e)
        {
            if (update_lock) return;
            update_lock = true;
            inst.SendCntTblItem(CntlTblAp.offset_rx_gain, BitConverter.GetBytes((ushort)tbRxGain.Value));
            tbTxGain.Value = ((CntlTblAp)inst.CntlTbl).RxGain;
            update_lock = false;
        }

        private void tbRxEqu_ValueChanged(object sender, EventArgs e)
        {
            if (update_lock) return;
            update_lock = true;
            inst.SendCntTblItem(CntlTblAp.offset_tx_equ, BitConverter.GetBytes((ushort)tbTxEqu.Value));
            tbTxEqu.Value = ((CntlTblAp)inst.CntlTbl).TxEqu;
            update_lock = false;
        }
        private void cb_RxSpeed_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void trainBtn_Click(object sender, RoutedEventArgs e)
        {
            if((string)trainBtn.Content == "Start Train")
            {
                trainBtn.Content = "Stop Train";
                inst.SendInstMsg((byte)InstApA.InstCmdCode.StartTrain);
            }
            else
            {
                trainBtn.Content = "Start Train";
                inst.SendInstMsg((byte)InstApA.InstCmdCode.StopTrain);
            }
        }
    }
}
