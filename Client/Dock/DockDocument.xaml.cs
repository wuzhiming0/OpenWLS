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
using System.Xml;
using Xceed.Wpf.AvalonDock;

//using OpenLS.Base.GUI;
//using OpenLS.Display;
//using OpenWLS.Client.LogInstance;
//using OpenWLS.Client.LogInstance.Calibration;

namespace OpenWLS.Client.Dock
{
    public interface IDockableDocumentControl
    {
        DockDocument DockDocument { get; set; }
        void OnClose();

    }
    /// <summary>
    /// Interaction logic for DockDocument.xaml
    /// </summary>
    public partial class DockDocument : Xceed.Wpf.AvalonDock.Layout.LayoutDocument
    {
        public event EventHandler DocumentClosed;
        UserControl uc;
        
        public FrameworkElement Control
        {
            get {
          //      if (gui != null)
          //          return gui.Cntl;
          //      else
                    return uc;
            }             
        }

        public DockDocument()
        {
            InitializeComponent();
        }

        /*
        void DockDocument_IsSelectedChanged(object sender, EventArgs e)
        {
          
        }*/




        public void SetCntl(UserControl _uc)
        {
            uc = _uc;
            uc.VerticalAlignment = VerticalAlignment.Stretch;
            uc.HorizontalAlignment = HorizontalAlignment.Stretch;
            if( !grid.Children.Contains(uc) )
                grid.Children.Add(uc);
            uc.Visibility = Visibility.Visible;

            if (uc is IDockableDocumentControl)
                ((IDockableDocumentControl)uc).DockDocument = this;   
            //         this.Focus();
        }


        void DockDocument_ViewTitleChanged(object sender, EventArgs e)
        {
            Title = uc.Name;
        }

        protected override void OnClosed()
        {
            if (Control is IDockableDocumentControl) { 
                ((IDockableDocumentControl)Control).OnClose();
               // ((APIClient)ac.Client).RequestStandby();
               // ac.Visible = false;
            }
            grid.Children.Remove(Control);
            base.OnClosed();
            if (DocumentClosed != null)
                DocumentClosed(this, new EventArgs());
        }
    }
}
