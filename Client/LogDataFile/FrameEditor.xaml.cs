
using OpenWLS.Server.LogDataFile.Models;
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
using Index = OpenWLS.Server.LogDataFile.Index;

namespace OpenWLS.Client.LogDataFile
{
    /// <summary>
    /// Interaction logic for FrameEditor.xaml
    /// </summary>
    public partial class FrameEditor : UserControl
    {
        public FrameEditor()
        {
            InitializeComponent();     
            HideCntls();
        }

        private void indexesLv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HideCntls();
            if (indexesLv.SelectedItem != null)
            {
                Index index = (Index)indexesLv.SelectedItem;
                if(index.Type != LogIndexType.SAMPLE_NUMBER)
                    shiftBtn.Visibility = Visibility.Visible;
                if(index.Type == LogIndexType.TIME)
                    dtBtn.Visibility = Visibility.Visible;
            }
        }

        void HideCntls()
        {
            shiftGd.Visibility = Visibility.Hidden;
            dtGd.Visibility = Visibility.Hidden;
            shiftBtn.Visibility = Visibility.Hidden;
            dtBtn.Visibility = Visibility.Hidden;
        }

        private void shiftOK_Click(object sender, RoutedEventArgs e)
        {
            HideCntls();
        }
        private void shiftCancel_Click(object sender, RoutedEventArgs e)
        {
            HideCntls();
        }
        private void dtOK_Click(object sender, RoutedEventArgs e)
        {
            HideCntls();
        }
        private void dtCancel_Click(object sender, RoutedEventArgs e)
        {
            HideCntls();
        }

        private void shiftBtn_Click(object sender, RoutedEventArgs e)
        {
            shiftGd.Visibility = Visibility.Visible;
            shiftBtn.Visibility = Visibility.Hidden;
        }

        private void dtBtn_Click(object sender, RoutedEventArgs e)
        {
            dtGd.Visibility = Visibility.Visible;
            dtBtn.Visibility = Visibility.Hidden;     
        }
    }
}
