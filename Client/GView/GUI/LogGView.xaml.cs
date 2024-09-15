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
using OpenWLS.Server.GView.ViewDefinition;
using OpenWLS.Client.GView.Models;
using OpenWLS.Server.Base;
using OpenWLS.Server.LogDataFile.Models;
using Orientation = OpenWLS.Server.GView.ViewDefinition.Orientation;
using OpenWLS.Server.DBase.Models.GlobalDb;
using OpenWLS.Server.LogDataFile;
using OpenWLS.Client.Dock;

namespace OpenWLS.Client.GView.GUI
{
 /*   public interface IDisplayCntrol
    {
        
    }*/
    /// <summary>
    /// Interaction logic for OpenLS.xaml
    /// </summary>
    public partial class LogGView : System.Windows.Controls.UserControl , IDockableDocumentControl
    {

  //      delegate void UpdateSizeDelegate();
        VdDocument doc;
        GvView insertView;
        GvView plotView;

        //      public GvView Insert { get { return insertView; } }
        bool realTime;
        public bool RealTime
        {
            set
            {
                realTime = value;
                editorCntl.RealTime = realTime;
                plotView.RealTime = realTime;
                insertView.RealTime = realTime;
                editorCntl.inforCntl.Visibility = realTime ? Visibility.Hidden : Visibility.Visible;
            }
            get
            {
                return realTime;
            }
        }
        public int? Id
        {
            get
            {
                if (RealTime) return ((VdDocumentRt)doc).Id; else return null;
            }
            set
            {
                if (RealTime) ((VdDocumentRt)doc).Id = value;
            }
        }
        public bool Global { get; set; }

        GViewDefinitionFile vdf;
        public GViewDefinitionFile ViewDefinitionFile {
            get { return vdf; }
            set { 
                vdf = value;
                VdDocument doc = RealTime? new VdDocumentRt(): new VdDocument();
                if(vdf.Body == null)
                {
                    doc.CreateNew();
                    doc.Scales.AddDefaultScales();
                }
                else
                    doc.UpdateDocument(vdf.Body);

                VdDocument = doc;
            }      
        }

        bool orientationSet;
        public VdDocument VdDocument
        {
            get
            {
                return doc;
            }
            set
            {
                doc = value;
                RealTime = doc is VdDocumentRt;
                bool horizontal = (doc.Orientation == Orientation.Horizontal) || (((doc.Orientation == Orientation.Auto)) && (doc.IndexUnit == IndexUnit.date_time));
                SetOrientation(horizontal);
                editorCntl.Scales = doc.Scales;
                editorCntl.VdItems = doc.Items;
                editorCntl.Tracks = doc.Tracks;
                editorCntl.DFiles = doc.DFiles;
                editorCntl.Doc = doc;
                scrollBarView.UpdateColorMask(MediaColorConverter.ConvertToMediaColor(doc.IndexBarMaskColor));
            }
        }
        public DockDocument DockDocument { get; set; }

        System.Windows.Controls.Orientation orientation;
        bool indexAutoScroll;
        public bool IndexAutoScroll
        {
            get { return indexAutoScroll; }
            set { indexAutoScroll = value; }
        }
  /*      public List<string> DFNs
        {
            set { editorCntl.DFNS = value; }
        }

        public void SetMeasurementNames(string ar,  IEnumerable<Measurement> ms)
        {
            doc.DFiles.SetMeasurements(ar, ms);
        }
  


        public void SetArIndexes(string ar, string indexes)
        {
            doc.DFiles.SetIndexes(ar,  indexes);
        }
   */
        public LogGView()
        {
            orientation = System.Windows.Controls.Orientation.Vertical;
            InitializeComponent();
            editorCntl.View = this;
            insertView = new GvView();
            plotView = new GvView();

            //        
            insertCntl.Child = insertView;
            scrollBar1.Minimum = 0;
            plotCntl.Child = plotView;
            insertView.EOSReceived += EOSReceived;
            insertView.ViewTotalSizeChanged += InsertView_ViewTotalSizeChanged;
        
             plotView.ViewTotalSizeChanged += plotView_ViewTotalSizeChanged;
            plotView.EOSReceived += EOSReceived;

            editorCntl.autoScrollCb.Click += AutoScrollCb_Click;
            editorCntl.autoScrollCb.IsChecked = false;

            scrollBarView.PlotView = plotView;
            editorCntl.ShowEditor = false;
            editorCntl.ShowInsert = false;
            indexAutoScroll = false;
            orientationSet = false;


        }

        private void EOSReceived(object? sender, EventArgs e)
        {
            procView = sender == insertView ? plotView : null;
        }


        private void AutoScrollCb_Click(object sender, RoutedEventArgs e)
        {
            indexAutoScroll = (bool)editorCntl.autoScrollCb.IsChecked;
            scrollBarView.IndexAutoScroll = (bool)editorCntl.autoScrollCb.IsChecked;
        }

        public void SetOrientation(bool horizontal)
        {
            Dispatcher.Invoke(() =>
            {
                orientation = horizontal ? System.Windows.Controls.Orientation.Horizontal : System.Windows.Controls.Orientation.Vertical;
                if (orientationSet && (orientation == plotView.Orientation))
                    return;
                if (orientation == System.Windows.Controls.Orientation.Vertical)
                    LayoutVertical();
                else
                    LayoutHorizontal();
                orientationSet = true;
            });
        }

        public void ClearPlotView()
        {
            plotView.ClearView();
        }

        void plotView_ViewTotalSizeChanged(object sender, EventArgs e)
        {
            UpdateSize();
        }

        void SetHScrollBar(double w)
        {
            scrollbar_size.Width = w;
            if (double.IsNaN(w))
                return;

            if (orientation == System.Windows.Controls.Orientation.Vertical)
            {
                System.Drawing.Size s = insertView.sizeView;
                if (w > (s.Width + scrollBarView.ActualWidth + 5))
                {
                    viewCntl.RowDefinitions[2].Height = new GridLength(0);
                    scrollBar1.Maximum = 0;
                    scrollBar1.Visibility = Visibility.Hidden;
                }
                else
                {
                    viewCntl.RowDefinitions[2].Height = new GridLength(20);
                    scrollBar1.Visibility = Visibility.Visible;
                    scrollBar1.Maximum = s.Width + scrollBarView.ActualWidth - w - 5;
                }
                insertView.ScrollX = scrollBar1.Value;
                plotView.ScrollX = scrollBar1.Value;            
                insertView.UpdateView(false);
            }
            else
            {
                System.Drawing.Size s = plotView.sizeView;
                double h1 = s.Height;
                scrollBarView.Maximum = h1 < 0 ? 0 : h1;
                double sc = indexAutoScroll ? scrollBarView.Maximum - plotView.Width : scrollBarView.Value;
                if (sc < 0) 
                    sc = 0;
                plotView.ScrollX = double.IsNaN(scrollBarView.Value) || double.IsInfinity(scrollBarView.Value) ? 0 : sc;
            }
            plotView.UpdateView(false);
        }

        void SetVScrollBar(double h)
        {
            scrollbar_size.Height = h;
            if (double.IsNaN(h))
                return;

            //   double h1 = s.Height - h;
            if(orientation == System.Windows.Controls.Orientation.Vertical)
            {
                System.Drawing.Size s = plotView.sizeView;
                double h1 = s.Height;
                scrollBarView.Maximum = h1 < 0 ? 0 : h1;
                plotView.ScrollY = scrollBarView.Value;
            }
            else
            {
                System.Drawing.Size s = insertView.sizeView;
                if (h > s.Width + scrollBarView.ActualHeight + 5)
                {
                    viewGd.ColumnDefinitions[2].Width = new GridLength(0);
                    scrollBar1.Maximum = 0;
                    scrollBar1.Visibility = Visibility.Hidden;
                }
                else
                {
                    viewGd.ColumnDefinitions[2].Width = new GridLength(20);
                    scrollBar1.Visibility = Visibility.Visible;
                    scrollBar1.Maximum = s.Width + scrollBarView.ActualHeight - h - 5;
                }
                insertCntl.Height = h;
                insertView.ScrollY = scrollBar1.Value;
                plotView.ScrollY = scrollBar1.Value;
            }

            plotView.UpdateView(false);
        }
        Size scrollbar_size;
        private void plotCntl_SizeChanged(object sender, EventArgs e)
        {
            //  ShowInsert(editorCntl.ShowInsert);     
            if(scrollbar_size.Width != viewCntl.ActualWidth)
                SetHScrollBar(viewCntl.ActualWidth);
            if(scrollbar_size.Height != plotView.Height)
                SetVScrollBar(plotView.Height);
        }

        public void UpdateSize()
        {
            Dispatcher.Invoke(() =>{
                ShowInsert(editorCntl.ShowInsert);
                //    System.Drawing.Size s = insertView.sizeView;
                double w = viewCntl.ActualWidth;
                // if(Double.IsNaN(w))
                //    w = viewCntl.Width;
                SetHScrollBar(w);
                SetVScrollBar(plotView.Height);
            });
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
            if(orientation == System.Windows.Controls.Orientation.Vertical)
                insertCntl.Height = b ? s.Height : 0;
            else
                insertCntl.Width = b ? s.Height : 0;
        }

        private void viewCntl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(orientation == System.Windows.Controls.Orientation.Vertical)
                SetHScrollBar(e.NewSize.Width);
            else
                SetVScrollBar(e.NewSize.Height);
        }



        private void ScrollBar1_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            if(orientation == System.Windows.Controls.Orientation.Vertical)
            {
                insertView.ScrollX = e.NewValue;
                plotView.ScrollX = e.NewValue;
            }
            else
            {
                insertView.ScrollY = e.NewValue;
                plotView.ScrollY = e.NewValue;
            }
            insertView.UpdateView(false);
            plotView.UpdateView(false);
        }

        private void ScrollBarView_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            if(orientation == System.Windows.Controls.Orientation.Vertical)
                plotView.ScrollY = e.NewValue;
            else
            {
             //   insertView.ScrollX = e.NewValue;
                plotView.ScrollX = e.NewValue;
             //   insertView.UpdateView();
            }
            plotView.UpdateView(false);
        }


        void LayoutVertical()
        {
       //     viewCntl.Children.Clear();
      //      viewGd.Children.Clear();
            viewGd.Children.Remove(splitter1);
            viewCntl.Children.Remove(splitter1);

            viewCntl.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Auto);
            viewCntl.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);
            viewCntl.RowDefinitions[2].Height = new GridLength(1, GridUnitType.Auto);

            viewGd.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
            viewGd.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Auto);
            viewGd.ColumnDefinitions[2].Width = new GridLength(30, GridUnitType.Auto);

            splitter1.Width = 5;
            scrollBarView.Orientation = System.Windows.Controls.Orientation.Vertical;
            plotView.Orientation = System.Windows.Controls.Orientation.Vertical;
            insertView.Orientation = System.Windows.Controls.Orientation.Vertical;
            insertCntl.Height = 200;
            viewGd.Children.Add(splitter1);
            Grid.SetColumn(plotCntl, 0);
            Grid.SetColumn(splitter1, 1);
            Grid.SetColumn(scrollBarView, 2);

            Grid.SetRow(insertCntl, 0);
            Grid.SetRow(viewGd, 1);
            Grid.SetRow(scrollBar1, 2);  
            scrollBar1.Orientation = System.Windows.Controls.Orientation.Horizontal;          
        }

        void LayoutHorizontal()
        {
            viewCntl.Children.Clear();            
            viewGd.Children.Clear();

            viewCntl.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
            viewCntl.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Auto);
            viewCntl.RowDefinitions[2].Height = new GridLength(1, GridUnitType.Auto);

            viewGd.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Auto);
            viewGd.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
            viewGd.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Auto);

            splitter1.Width = double.NaN;
            splitter1.Height = 5;
            scrollBarView.Orientation = System.Windows.Controls.Orientation.Horizontal;
            plotView.Orientation = System.Windows.Controls.Orientation.Horizontal;
            insertView.Orientation = System.Windows.Controls.Orientation.Horizontal;
            insertCntl.Width = 200;
            insertCntl.VerticalAlignment = VerticalAlignment.Stretch;
            viewGd.Children.Add(insertCntl);
            viewGd.Children.Add(plotCntl);
            viewGd.Children.Add(scrollBar1);

            viewCntl.Children.Add(viewGd);
            viewCntl.Children.Add(splitter1);
            viewCntl.Children.Add(scrollBarView);          
            Grid.SetColumn(insertCntl, 0);
            Grid.SetColumn(plotCntl, 1);
            Grid.SetColumn(scrollBar1, 2);           
            
            Grid.SetRow(viewGd, 0);
            Grid.SetRow(splitter1, 1);
            Grid.SetRow(scrollBarView, 2);
            
            scrollBar1.Orientation = System.Windows.Controls.Orientation.Vertical;
        }

        public void OnClose()
        {

        }
    }
}
