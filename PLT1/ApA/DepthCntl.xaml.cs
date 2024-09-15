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
    /// Interaction logic for DepthCntl.xaml
    /// </summary>
    public partial class DepthCntl : UserControl
    {
        InstCApA inst;
        public InstCApA Inst { set { inst = value; } }
        public bool EditEnable
        {
            set
            {
                valTb.IsEnabled = value;
                valBtn.IsEnabled = value;
                srcCb.IsEnabled = value;
                simSpeedInput.IsEnabled = value;
            }
        }

        bool update_lock;
        public DepthCntl()
        {
            update_lock = true;
            InitializeComponent();
            simSpeedInput.Min = -12000;
            simSpeedInput.Max = 12000;
            update_lock = false;
        }

        public void UpdateCntlTblItem(int offset)
        {
            if (inst == null || inst.CntlTbl == null) return;
            update_lock = true;
            switch (offset)
            {
                case CntlTblAp.offset_depth_src:
                    srcCb.SelectedIndex = ((CntlTblAp)inst.CntlTbl).DepthSrc;
                    break;
                case CntlTblAp.offset_depth_sim_speed:
                    simSpeedInput.Value = ((CntlTblAp)inst.CntlTbl).DepthSimSpeed;
                    break;
            }
            update_lock = false;
        }
        private void valBtn_Click(object sender, RoutedEventArgs e)
        {
            if (inst == null || update_lock) return; 
         //   update_lock = true;
            double d;
            if(double.TryParse(valTb.Text, out d)){
                inst.SendInstMsg((byte)InstApA.InstCmdCode.SetDepth, BitConverter.GetBytes(d));
            }
          //  update_lock = false;
        }

        private void simSpeedInput_ValueChanged(object sender, EventArgs e)
        {
            if (inst == null || update_lock) return; 
       //     update_lock = true;
            inst.SendCntTblItem(CntlTblAp.offset_depth_sim_speed, BitConverter.GetBytes(simSpeedInput.Value));

       //     update_lock = false;
        }

            
        private void srcCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (inst == null || update_lock) return;
            update_lock = true;
            inst.SendCntTblItem(CntlTblAp.offset_depth_src, new byte[] { (byte)srcCb .SelectedIndex});

            update_lock = false;
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            if(expander.IsExpanded)
            {
                valTb.Text = inst.LiClient.ToolBar.Depth.ToString("f2"); 
            }
        }
    }
}
