
using OpenWLS.Client.LogInstance;
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

namespace OpenWLS.Client.LogDataFile
{
    /// <summary>
    /// Interaction logic for ParaEditor.xaml
    /// </summary>
    public partial class ParaEditor : UserControl
    {

        public Parameters parameters;
        public Parameters Parameters
        {
            get { return parameters; }
            set
            {
                parameters = value;
                dg.ItemsSource = parameters;
            }
        }
                
        public ParaEditor()
        {
            InitializeComponent();
        }
    }
}
