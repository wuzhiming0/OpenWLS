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

namespace OpenWLS.Client.Base
{
    /// <summary>
    /// Interaction logic for DFEraseProgressCntl.xaml
    /// </summary>
    public partial class ProgressCntl : UserControl
    {
        public ProgressCntl()
        {
            InitializeComponent();
            pb.Minimum = 0; 
            okBtn.Visibility = Visibility.Hidden;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }

        public void UpdateProgress(int cur, int total)
        {
            Dispatcher.Invoke(new Action(() =>
            {      
                Visibility = Visibility.Visible;
                pb.Maximum = total;
                pb.Value = cur;
                if (total == cur)
                    okBtn.Visibility = Visibility.Visible;
            }));
        }
    }
}
