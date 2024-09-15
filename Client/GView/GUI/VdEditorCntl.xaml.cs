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
using OpenWLS.Server.Base;
using OpenWLS.Server.GView.ViewDefinition;

namespace OpenWLS.Client.GView.GUI
{
    /// <summary>
    /// Interaction logic for VdItemsCntl.xaml
    /// </summary>
    public partial class PdEditorCntl : UserControl
    {
        bool showEditor;
        bool showInsert;
        IItemEditor iCntl;
        double editorHeight;
        VdItems cvidItems;

  //      public List<string> DFNS { set { dfnsCntl.DFileNamesAll = value; } }

        public bool RealTime
        {
            set
            {
                if (value)
                {
                    ((TabItem)tabCntl.Items[0]).Visibility = Visibility.Hidden;
                    dfTI.Header = "Repeat";
                    tabCntl.SelectedIndex = 1;
                }

                curvesCntl.RealTime = value;
                imagesCntl.RealTime = value;
                editBar.RealTime = value;
                
            }
        }

        public VdDocument Doc
        {
            set
            {
                inforCntl.Doc = value;
            }
        }

        public VdMeasurements RtMeasurements
        {
            set
            {
                curvesCntl.curveCntl.chCntl.Measurements = value;
                imagesCntl.imageCntl.chCntl.Measurements = value;
            }
        }

        public VdDFiles DFiles
        {
            set
            {
                dfnsCntl.DFiles = value;
                curvesCntl.Dfiles = value;
                imagesCntl.Dfiles = value;
                inforCntl.DFiles = value;
                foreach(VdItem vdItem in cvidItems)
                {
                    if(vdItem is VdItem)
                    {
                        VdItem vdItemM = vdItem;
                        VdMeasurement? m = vdItem.Measurement;
                        if (m != null && m.DFile != null)
                            m.DFile = value.GetDfile(m.FileID); 
                    }
                }
            }
        }

        public VdItems VdItems
        {
            set
            {
                cvidItems = value;
                curvesCntl.VdItems = value;
                imagesCntl.VdItems = value;
                fillsCntl.VdItems = value;
            }
        }

        public VdTracks Tracks
        {
            set
            {
                tracksCntl.Tracks = value;
                curvesCntl.Tracks = value;
                imagesCntl.Tracks = value;
            }
        }

        public Scales Scales
        {
            set
            {
                curvesCntl.Scales = value;
                imagesCntl.Scales = value;
            }
        }

        public bool ShowEditor
        {
            get
            {
                return showEditor;
            }
            set
            {
                showEditor = value;
                if (showEditor)
                {
                    if (editorHeight == 0)
                        editorHeight = 290;
                    editBar.View.ShowEditor(editorHeight);
                }
                else
                {
                    editorHeight = ActualHeight;
                    editBar.View.ShowEditor(toolBar.Height);
                }
                //tabCntl.Visibility = value ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public bool ShowInsert
        {
            get
            {
                return showInsert;
            }
            set
            {
                showInsert = value;
                editBar.View.ShowInsert(showInsert);
            }
        }

        public LogGView View
        {
            set
            {
                editBar.View = value;
            }
        }

        public PdEditorCntl()
        {
            InitializeComponent();
            DataContext = this;
            dfnsCntl.DFileAdded += DfnsCntl_DFileAdded;
            dfnsCntl.DFileRemoveded += dfnsCntl_DFileRemoveded;

        }

        void dfnsCntl_DFileRemoveded(object sender, EventArgs e)
        {
            VdDFile df = (VdDFile)sender;
            foreach (VdItem c in cvidItems)
            {
                if (c.Measurement != null)
                {
                    if (c.Measurement.FileID == df.Id)
                    {
                        c.Measurement.DFile = null;
                    //    c.Measurement.FileID = -999;
                    }
                }
            }
         //   DFiles = dfnsCntl.DFiles;
        }

        private void DfnsCntl_DFileAdded(object sender, EventArgs e)
        {
            VdDFiles dfs = dfnsCntl.DFiles;
            DFiles = null;
            DFiles = dfs;
       //     string str = DisplayProc.str_req_get_channel_names + "\n" + ((VdDFile)sender).Name;
       //     editBar.View.GuiItem.GetRequest(str); 
        }

        public void CheckEditorButtons()
        {
            editBar.CheckButtons();
        }

        private void tabCntl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int k = tabCntl.SelectedIndex;

            switch (k)
            {
                case 1:
                    iCntl = curvesCntl;
                    break;
                case 2:
                    iCntl = imagesCntl;
                    break;
                case 3:
                    if (iCntl == fillsCntl) return;

                    fillsCntl.ReSetCurveOptions(cvidItems);
                    iCntl = fillsCntl;
                    break;
                case 4:
                    iCntl = tracksCntl;
                    break;
                case 5:
                    dfnsCntl.GetJobs();
                    break;
                default:
                    iCntl = null;
                    break;
            }

            editBar.ItemEditor = iCntl;

        }
    }
}
