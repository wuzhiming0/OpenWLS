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

using System.Windows.Forms;
using System.Windows.Forms.Integration;
using OpenWLS.Client.GView.Models;
using OpenWLS.Server.GView.ViewDefinition;
using OpenWLS.Server.Base.DataType;
using OpenWLS.Server.LogDataFile;

namespace OpenWLS.Client.GView.GUI
{
    /// <summary>
    /// Interaction logic for LogView.xaml
    /// </summary>
    public partial class HLogGView1 : System.Windows.Controls.UserControl
    {

        delegate void UpdateSizeDelegate();
        VdDocument doc;
        GvView insertView;
        GvView plotView;
    //    public PdGui GuiItem { get; set; }
        //      public GvView Insert { get { return insertView; } }

        public bool RealTime
        {
            set
            {
                editorCntl.RealTime = value;
                plotView.RealTime = value;
                insertView.RealTime = value;
            }
        }

        public VdDocument Document
        {
            get
            {
                return doc;
            }
            set
            {
                doc = value;
                editorCntl.Scales = doc.Scales;
                editorCntl.VdItems = doc.Items;
                editorCntl.Tracks = doc.Tracks;
                editorCntl.DFiles = doc.DFiles;
                editorCntl.Doc = doc;                
                hScrollBar.UpdateColorMask(MediaColorConverter.ConvertToMediaColor(doc.IndexBarMaskColor));
            }
        }

    /*    public List<string> DFNs
        {
            set { editorCntl.DFNS = value; }
        }
 
        public void SetMeasurements(string ar, IEnumerable<Measurement> ms)
        {
            doc.DFiles.SetMeasurements(ar, ms);

        }
    */
        public void SetArIndexes(string ar, string indexes)
        {
            doc.DFiles.SetIndexes(ar,  indexes);
        }
        public HLogGView1()
        {
            InitializeComponent();
            insertView = new GvView();
            plotView = new GvView();
            plotView.Orientation = System.Windows.Controls.Orientation.Horizontal;
   //         editorCntl.ViewH = this;
            insertCntl.Child = insertView;
            vScrollBar.Minimum = 0;
            plotCntl.Child = plotView;
            insertView.ViewTotalSizeChanged += InsertView_ViewTotalSizeChanged;
            plotView.ViewTotalSizeChanged += plotView_ViewTotalSizeChanged;
            r2Def.Height = new GridLength(30, GridUnitType.Pixel);
            hScrollBar.PlotView = plotView;
            editorCntl.ShowEditor = false;
            editorCntl.ShowInsert = false;
        }

        void plotView_ViewTotalSizeChanged(object sender, EventArgs e)
        {
            UpdateSize();
        }

        void SetHScrollBar(double w)
        {
            if (double.IsNaN(w))
                return;
            System.Drawing.Size s = plotView.sizeView;
            //   double h1 = s.Height - h;
            double h1 = s.Width;
            hScrollBar.Maximum = h1 < 0 ? 0 : h1;

            plotView.ScrollY = hScrollBar.Value;
            plotView.UpdateView();
        }

        void SetVScrollBar(double h)
        {
            if (double.IsNaN(h))
                return;
            System.Drawing.Size s = insertView.sizeView;

            if (h > (s.Height + hScrollBar.ActualHeight + 5))
            {
                viewCntl.ColumnDefinitions[2].Width = new GridLength(0);
                vScrollBar.Maximum = 0;
                vScrollBar.Visibility = Visibility.Hidden;
            }
            else
            {
                viewCntl.ColumnDefinitions[2].Width = new GridLength(20);
                vScrollBar.Visibility = Visibility.Visible;
                vScrollBar.Maximum = s.Height + hScrollBar.ActualHeight - h - 5;
            }

            insertView.ScrollY = vScrollBar.Value;
            insertView.UpdateView();
            plotView.ScrollY = vScrollBar.Value;
            plotView.UpdateView();
        }

        private void plotCntl_SizeChanged(object sender, EventArgs e)
        {
            SetHScrollBar(plotView.Height);
            SetVScrollBar(viewCntl.ActualHeight);
        }

        public void UpdateSize()
        {
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke(new UpdateSizeDelegate(UpdateSize), null);
            else
            {
                ShowInsert(editorCntl.ShowInsert);
                //SetHScrollBar(viewCntl.Width);
                //SetVScrollBar(plotView.Height);
                SetHScrollBar(plotView.Height);
                SetVScrollBar(viewCntl.ActualHeight);
            }
        }

        private void InsertView_ViewTotalSizeChanged(object sender, EventArgs e)
        {
            UpdateSize();
        }

        public void ShowEditor(double d)
        {
            gridCntl.RowDefinitions[0].Height = new GridLength(d);
        }

        public void ShowInsert(bool b)
        {
            System.Drawing.Size s = insertView.sizeView;
            insertCntl.Height = b ? s.Height : 0;
        }

        private void viewCntl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetVScrollBar(e.NewSize.Width);
        }



        private void vScrollBar_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {            
            plotView.ScrollY = e.NewValue;
            plotView.UpdateView();
        }

        private void hScrollBar_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            insertView.ScrollX = e.NewValue;
            plotView.ScrollX = e.NewValue;
            insertView.UpdateView();
            plotView.UpdateView();
        }
        

    }
}
