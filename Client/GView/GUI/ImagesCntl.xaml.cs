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
    /// Interaction logic for ImagesCntl.xaml
    /// </summary>
    public partial class ImagesCntl : UserControl, IItemEditor
    {
        VdImage copiedOb;
        PdEditorCntl edtior;

        public bool RealTime
        {
            set
            {
                imageCntl.RealTime = value;
            }
        } 

        public PdEditorCntl Editor
        {
            get { return edtior; }
            set
            {
                edtior = value;
            }
        }

        public VdItems VdItems
        {
            set
            {
                listCntl.VdItems = value;
            }
        }

        public VdDFiles Dfiles
        {
            set
            {
                imageCntl.Dfiles = value;
            }
        }


        public Scales Scales
        {
            set
            {
                imageCntl.Scales = value;
            }
        } 
                
        public VdTracks Tracks
        {
            set
            {
                imageCntl.Tracks = value;                
            }
        }

        public ImagesCntl()
        {
            InitializeComponent();
            listCntl.ItemType = LogViewItemType.Image;
        }


        private void listCntl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            imageCntl.Image = (VdImage)listCntl.itemLb.SelectedItem;
        }

        public object GetSelectedObject()
        {
            return imageCntl.Image;
        }

        public object GetCopiedObject()
        {
            return copiedOb;
        }

        public void CopySelectedObject()
        {
            copiedOb = imageCntl.Image;
        }

        public void PasteCopiedObject()
        {

        }

        public void CreatNewObject()
        {
            listCntl.VdItems.AddNew(LogViewItemType.Image);
            listCntl.VdItems = listCntl.VdItems;
        }

        public void DeleteSelectedObject()
        {
            if (imageCntl.Image != null)
                listCntl.VdItems.Remove(imageCntl.Image);
            imageCntl.Image = null;
            listCntl.VdItems = listCntl.VdItems;
        }
    }
}
