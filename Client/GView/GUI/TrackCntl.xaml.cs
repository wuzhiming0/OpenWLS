using OpenWLS.Server.GView.ViewDefinition;
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

namespace OpenWLS.Client.GView.GUI
{
    /// <summary>
    /// Interaction logic for TrackCntl.xaml
    /// </summary>
    public partial class TrackCntl : UserControl
    {
        public TrackCntl()
        {
            InitializeComponent();
            dtCb.ItemsSource = System.Enum.GetValues(typeof(ShowIndex));
        //    dtMinorCb.ItemsSource = System.Enum.GetValues(typeof(TrackDateTime));
        //    dtMajorCb.ItemsSource = System.Enum.GetValues(typeof(TrackDateTime));
        }

        private void dtCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(dtCb.SelectedIndex == 2)
            {
                dtGrid.Visibility = Visibility.Visible;
                dtGrid1.Visibility = (bool)sbCb.IsChecked? Visibility.Visible : Visibility.Hidden;
          //      dtGrid2.Visibility =  Visibility.Visible;   
            }
            else
            {
                dtGrid.Visibility = Visibility.Hidden;
                dtGrid1.Visibility = Visibility.Hidden;
            //    dtGrid2.Visibility = Visibility.Hidden;
            }
        }

        private void linearCb_Click(object sender, RoutedEventArgs e)
        {
            logStartGd.Visibility = (bool)linearCb.IsChecked ? Visibility.Hidden : Visibility.Visible;
        }

        private void sbCb_Click(object sender, RoutedEventArgs e)
        {
            dtGrid1.Visibility = (bool)sbCb.IsChecked && dtCb.SelectedIndex == 2 ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
