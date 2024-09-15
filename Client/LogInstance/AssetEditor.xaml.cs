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

namespace OpenWLS.Client.LogInstance
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class AssetEditor : UserControl
    {
        public event EventHandler<EventArgs> SaveClicked;
        InstrumentC inst;

        public InstrumentC Inst
        {
            set
            {
                inst = value;
             //   assetTb.Text = inst.AssetNumber;
            }
        }

        public bool EnableEdit
        {
            set
            {
                if(value)
                {
                    assetTb.IsReadOnly = false;
                    saveBtn.Visibility = Visibility.Visible;
                }
                else
                {
                    assetTb.IsReadOnly = true;
                    saveBtn.Visibility = Visibility.Hidden;
                }
            }
        }
        public AssetEditor()
        {
            InitializeComponent();
            assetTb.IsReadOnly = true;
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SaveClicked != null && (!string.IsNullOrEmpty(assetTb.Text) ))
                SaveClicked(assetTb.Text, e);
        }
   
    }
}
