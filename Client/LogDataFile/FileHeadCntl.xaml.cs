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

namespace OpenWLS.Client.LogDataFile
{

    /// <summary>
    /// Interaction logic for FileHeadCntl.xaml
    /// </summary>
    public partial class FileHeadCntl : UserControl
    {    
        int preSelectIndex;
        public FileHeadCntl()
        {
            InitializeComponent();
            shiftGd.Visibility = Visibility.Hidden;
            preSelectIndex = -1;
        }

        private void shiftOK_Click(object sender, RoutedEventArgs e)
        {
            shiftGd.Visibility = Visibility.Hidden;
            shiftBtn.Visibility = Visibility.Visible;
            double d;
            if (Double.TryParse(valTb.Text, out d))
            {
             //   FileHead fh = (FileHead)DataContext;
            }

        }

        private void shiftCancel_Click(object sender, RoutedEventArgs e)
        {
            shiftGd.Visibility = Visibility.Hidden;
            shiftBtn.Visibility = Visibility.Visible;
        }

        private void shiftBtn_Click(object sender, RoutedEventArgs e)
        {
            shiftGd.Visibility = Visibility.Visible;
            shiftBtn.Visibility = Visibility.Hidden;
        }

        private void indexUnitCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (preSelectIndex >= 0)
            {
                /*
                FileHead fh = (FileHead)DataContext;
                string s = fh.MetricDepth ?  "meter" : "feet";
                if (MessageBox.Show("Are you sure to change depth unit to " + s + "?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    s = fh.MetricDepth ? "\n0" : "\n1";
                }
                */                
            }
            preSelectIndex = indexUnitCb.SelectedIndex;
        }
    }
}
