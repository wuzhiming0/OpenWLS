using OpenWLS.Server.Base;
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
    /// Interaction logic for ScaleCntl.xaml
    /// </summary>
    /// 

    public partial class ScaleCntl : UserControl
    {

        IScaleContainer scaleParent;

        delegate void UpdateScaleDelegate();
        Scales scales;

        public IScaleContainer ScaleParent
        {
            set
            {
                scaleParent = value;
                if (scaleParent != null)
                {
                   DataContext  = scaleParent.Scale;
                    scaleCb.SelectedItem = DataContext;
                }

                else
                    DataContext = null;
            }
        }


        public Scales Scales
        {
            set
            {
                scales = value;
                UpdateScale();
            }
        }



        public ScaleCntl()
        {
            InitializeComponent(); 
           // DataContext = this;
        }

        public void UpdateScale()
        {
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke(new UpdateScaleDelegate(UpdateScale), null);
            else
                scaleCb.ItemsSource = scales;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Scale s = (Scale)scaleCb.SelectedItem;
            Scales ss = (Scales)scaleCb.ItemsSource;
            if (s != null && s.Name == "NEW")
            {
                s = ss.AddNewScale();
                scaleCb.ItemsSource = null;
                scaleCb.ItemsSource = ss;
                scaleCb.SelectedItem = s;
                //              DataContext = null;
            }
            if(scaleParent != null)
                scaleParent.Scale=s;
            DataContext = s;
        }


        private void scaleCb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && DataContext != null)
            {
                Scale scale = (Scale)DataContext;
                scale.Name = scaleCb.Text;
                DataContext = null;
                DataContext = scale;
            }
        }


    }
}
