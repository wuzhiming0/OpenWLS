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


namespace OpenWLS.PLT1.GrA
{
    /// <summary>
    /// Interaction logic for GSgrAInstCntl.xaml
    /// </summary>
    public partial class GrACntl : InstCntl
    {
        public GrACntl()
        {
            InitializeComponent();
        }

        private void hvBtn_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            int v = Convert.ToInt32(e.NewValue);
            if (hvTbox != null)
                hvTbox.Text = v.ToString();
        }


        private void hvTbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                hvBtn.Value = Convert.ToInt32(hvTbox.Text);
            }
        }


    }
}
