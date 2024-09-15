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
    /// Interaction logic for CurvesCntl.xaml
    /// </summary>
    public partial class FillsCntl : UserControl, IItemEditor
    {
        VdFill copiedOb;

        PdEditorCntl editor;

        public bool RealTime
        {
            set{
       //     curveCntl.RealTime = value;
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
           //     fillCntl.VdItems = value;
            }
        }

  


        public void ReSetCurveOptions(VdItems items)
        {
            fillCntl.VdItems = items;
        }        





        public FillsCntl()
        {
            InitializeComponent();
            listCntl.ItemType = LogViewItemType.Fill;
        }


        private void listCntl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listCntl.itemLb.SelectedItem == null && listCntl.VdItems.Count > 0)
                listCntl.itemLb.SelectedItem = listCntl.VdItems[0];
            if(listCntl.itemLb.SelectedItem != null)
                fillCntl.Fill = (VdFill)listCntl.itemLb.SelectedItem;

       }
        public object GetSelectedObject()
        {
            return fillCntl.Fill;
        }

        public object GetCopiedObject()
        {
            return copiedOb;
        }

        public void CopySelectedObject()
        {
             copiedOb = fillCntl.Fill;
            Editor.CheckEditorButtons();
        }

        public void PasteCopiedObject()
        {

        }

        public void CreatNewObject()
        {
            listCntl.VdItems.AddNew(LogViewItemType.Fill);
            listCntl.VdItems = listCntl.VdItems;
        }

        public void DeleteSelectedObject()
        {
              if (fillCntl.Fill != null)
                  listCntl.VdItems.Remove(fillCntl.Fill);
              fillCntl.Fill = null;
            listCntl.VdItems = listCntl.VdItems;
        }

    }
}
