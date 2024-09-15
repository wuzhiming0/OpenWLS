using OpenWLS.Client.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for InstrumentList.xaml
    /// </summary>
    public partial class InstrumentList : UserControl
    {
        public event EventHandler IntrumentDbClicked;

        public List<Server.DBase.Models.GlobalDb.InstrumentDb> Instruments
        {
            set
            {
                instLV.ItemsSource = value;
                ICollectionView view = CollectionViewSource.GetDefaultView(instLV.ItemsSource);
                view.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
            }
        }
        public InstrumentList()
        {
            InitializeComponent();
        }


        private void instLV_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(IntrumentDbClicked != null && instLV.SelectedItem != null)
            {
                Server.DBase.Models.GlobalDb.InstrumentDb inst = (Server.DBase.Models.GlobalDb.InstrumentDb)instLV.SelectedItem;
               // if (!inst.IsAcqDev)
                IntrumentDbClicked(inst, e);
            }

            
        }
    }
}
