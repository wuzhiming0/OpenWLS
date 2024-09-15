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

using System.Data;
using OpenWLS.Server.LogDataFile.Models.NMRecord;

namespace OpenWLS.Client.LogDataFile
{
    /// <summary>
    /// Interaction logic for NmListView.xaml
    /// </summary>
    public partial class NmListView : UserControl
    {
        public event EventHandler NcItemDbClicked;

        NMRecords ncItems;
        public NMRecords NCItems
        {
            set
            {
                ncItems = value; UpdateNcList();
            }
        }
        public NmListView()
        {
            InitializeComponent();
        }

        public void UpdateNcList()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                chLV.ItemsSource = null;
                chLV.ItemsSource = ncItems;
            }));
        }

        private void chLV_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(chLV.SelectedItem != null)
            {
                NMRecord a = (NMRecord)chLV.SelectedItem;
                if (NcItemDbClicked != null)
                    NcItemDbClicked(a, null);
            }
        }
    }
}
