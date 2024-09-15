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

using OpenWLS.Client.LogInstance.Instrument;

namespace OpenWLS.PLT1.TelA
{
    /// <summary>
    /// Interaction logic for USBTestCntl.xaml
    /// </summary>
    public partial class TelACntl : InstCntl
    {
        delegate void UpdateButtonDelegate(Button btn, string str);

        public TelACntl()
        {
            InitializeComponent();
        }
        bool powerOn;
   

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



        private void TxGain_ValueChanged(object sender, EventArgs e)
        {

        }

        private void cb_commPort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
