using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Infrastructure;
using System.IO;
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

using Newtonsoft.Json;
using OpenWLS.Client.LogInstance.Instrument;
using OpenWLS.Client.GView.Models;
using OpenWLS.Client.LogDataFile;
using OpenWLS.Client.Requests;
using OpenWLS.Server.LogInstance.OperationDocument;
using OpenWLS.Server.DBase.Models.GlobalDb;
using OpenWLS.Server.LogInstance;
using OpenWLS.Server.DBase.Models.LocalDb;

namespace OpenWLS.Client.LogInstance
{
    /// <summary>
    /// Interaction logic for AcqItemList.xaml
    /// </summary>
    public partial class EdgeItemView : Window
    {
      


        public EdgeItemView()
        {
            InitializeComponent();
            EdgeCs es = EdgeRequest.GetAllEdgesAsync().Result;
            edgeDg.ItemsSource = es;
            typeCb.ItemsSource = Enum.GetNames(typeof(EdgeType));
            //    edgeDg.ItemsSource = acqItems;

        }




        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void edgeDg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (edgeDg.SelectedItem != null)
            {
                EdgeC se = edgeDg.SelectedItem as EdgeC;
                EdgeC ne = new EdgeC();
                ne.CloneFrom(se);
                editPanel.DataContext = ne;
            }
        }
    }
}
