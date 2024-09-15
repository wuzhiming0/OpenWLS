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
    /// Interaction logic for ScaleBarWithInputBox.xaml
    /// </summary>

    public partial class ScaleBarWithInput : UserControl
    {
        public event EventHandler<EventArgs> ValueChanged;
        public  ScaleBarWithInput()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(ScaleBarWithInput),
                new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    new PropertyChangedCallback(OnValueChanged)));


        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScaleBarWithInput control = (ScaleBarWithInput)d;
            control.Value = (int)e.NewValue;
            if (control.ValueChanged != null)
                control.ValueChanged(control.Value, null);
        }
        public double Min
        {
            get { return scaleSlider.Minimum; }
            set { scaleSlider.Minimum = value; }
        }

        public double Max
        {
            get { return scaleSlider.Maximum; }
            set { scaleSlider.Maximum = value; }
        }

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public int TextWidth
        {
            get
            {
                return (int)valueTextBox.Width;
            }
            set
            {
                valueTextBox.Width = value;
            }
        }
       
    }
}