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

using OpenWLS.Server.GView.ViewDefinition;

namespace OpenWLS.Client.GView.GUI
{
    /// <summary>
    /// Interaction logic for VdItemCntl.xaml
    /// </summary>
    public partial class VdItemCntl : UserControl
    {
        public VdItem CVIDItem 
        { 
            set { DataContext = value; } 
        }

        public VdItemCntl()
        {
            InitializeComponent();
        }

    }
}
