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
using Xceed.Wpf.AvalonDock;

namespace OpenWLS.Client.Dock
{
    public interface IDockableCntl
    {
        DockControl DockControl { get; set; }
    }
    /// <summary>
    /// Interaction logic for DockControl.xaml
    /// </summary>
    public partial class DockControl : Xceed.Wpf.AvalonDock.Layout.LayoutAnchorable 
    {
        FrameworkElement cntl;
        public DockControl()
        {
            InitializeComponent();
        }

        public FrameworkElement Control
        {
            get { return cntl; }             
        }

        public void SetCntl(FrameworkElement? cntl)
        {
            if (cntl == null)
                grid.Children.Clear();
            else
            {
                this.cntl = cntl;
                cntl.VerticalAlignment = VerticalAlignment.Stretch;
                cntl.HorizontalAlignment = HorizontalAlignment.Stretch;
                grid.Children.Add(cntl);
                if(cntl is IDockableCntl)
                    ((IDockableCntl)cntl).DockControl = this;
            }
            
        }

       
    }
}
