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
    public interface IItemEditor
    {
        object GetSelectedObject();
        object GetCopiedObject();
        void CreatNewObject();
        void CopySelectedObject();
        void PasteCopiedObject();
        void DeleteSelectedObject();
    }

    /// <summary>
    /// Interaction logic for CurvesCntl.xaml
    /// </summary>
    public partial class CurvesCntl : UserControl, IItemEditor
    {
        VdCurve copiedOb;

        PdEditorCntl editor;

        public bool RealTime
        {
            set{
            curveCntl.RealTime = value;
            }
        } 

        public PdEditorCntl Editor
        {
            get { return editor; }
            set
            {
                editor = value;
            }
        }

        public VdItems VdItems
        {
            set
            {
                listCntl.VdItems = value;
            }
        }

        public Scales Scales
        {
            set
            {
                curveCntl.Scales = value;
            }
        } 
                
        public VdTracks Tracks
        {
            set
            {
                curveCntl.Tracks = value;                
            }
        }

        public VdDFiles Dfiles
        {
            set
            {
                curveCntl.Dfiles = value;

            }
        }



        public CurvesCntl()
        {
            InitializeComponent();
            listCntl.ItemType = LogViewItemType.Curve;
        }


        private void listCntl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listCntl.itemLb.SelectedItem == null && listCntl.VdItems.Count > 0)
                listCntl.itemLb.SelectedItem = listCntl.VdItems[0];
            if(listCntl.itemLb.SelectedItem != null)
                curveCntl.Curve = (VdCurve)listCntl.itemLb.SelectedItem;

        }

        public object GetSelectedObject()
        {
            return curveCntl.Curve;
        }

        public object GetCopiedObject()
        {
            return copiedOb;
        }

        public void CopySelectedObject()
        {
            copiedOb = curveCntl.Curve;
            Editor.CheckEditorButtons();
        }

        public void PasteCopiedObject()
        {

        }

        public void CreatNewObject()
        {
            listCntl.VdItems.AddNew(LogViewItemType.Curve);
            listCntl.VdItems = listCntl.VdItems;
        }

        public void DeleteSelectedObject()
        {
            if (curveCntl.Curve != null)
                listCntl.VdItems.Remove(curveCntl.Curve);
            curveCntl.Curve = null;
            listCntl.VdItems = listCntl.VdItems;
        }

        private void listCntl_AddNewItem(object sender, EventArgs e)
        {
            CreatNewObject();
        }

        private void listCntl_DeleteItem(object sender, EventArgs e)
        {
            if(MessageBox.Show($"Delete Curve {curveCntl.Curve.Name}?", "Delete Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                DeleteSelectedObject();
        }
    }
}
