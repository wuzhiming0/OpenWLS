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

namespace OpenWLS.Client.GView.GUI
{
    /// <summary>
    /// Interaction logic for PdEditBar.xaml
    /// </summary>
    public partial class PdEditBar : Grid
    {
        IItemEditor itemEditor;
        bool leftActive, copyActive, pasteActive;

        public string Name
        {
            get { return nameTb.Text; }
            set { nameTb.Text = value; }
        }
        LogGView view;
//        HLogGView viewH;
        public LogGView View
        {
            get { return view; }
            set
            {
                view = value;
            }
        }
     /*   public HLogGView ViewH
        {
            get { return viewH; }
            set
            {
                viewH = value;
            }
        }
        */
        public IItemEditor ItemEditor
        {
            set { itemEditor = value;
                CheckButtons();
            }
        }

        public bool RealTime
        {
            set
            {
                if (value)
                {
                    prnBtn.Visibility = System.Windows.Visibility.Hidden;
                    saveBtn.Visibility = System.Windows.Visibility.Hidden;
                    nameTb.Visibility = System.Windows.Visibility.Hidden;
                }
            }
        }

        public void CheckButtons()
        {
            bool b = itemEditor != null && (itemEditor.GetSelectedObject() != null);
            if (copyActive != b)
            {
                copyActive = b;
                copyBtn.Content = b ? FindResource("CopyOn") : FindResource("CopyOff");
                delBtn.Content = b ? FindResource("DelOn") : FindResource("DelOff");
            }

            b = itemEditor != null && (itemEditor.GetCopiedObject() != null);
            if (pasteActive != b)
            {
                pasteActive = b;
                pasteBtn.Content = b ? FindResource("PasteOn") : FindResource("PasteOff");
            }
            b = itemEditor != null && (itemEditor is TracksCntl);
            if (leftActive != b)
            {
                leftActive = b;
                leftBtn.Content = b ? FindResource("LeftOn") : FindResource("LeftOff");
                rightBtn.Content = b ? FindResource("RightOn") : FindResource("RightOff");
            }
        }

        private void newBtn_Click(object sender, RoutedEventArgs e)
        {
            if (itemEditor != null)
                itemEditor.CreatNewObject();
        }

        private void copyBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void pasteBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void delBtn_Click(object sender, RoutedEventArgs e)
        {
            if (itemEditor != null)
                itemEditor.DeleteSelectedObject();
        }

        private void rightBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
           view.SaveCVIDFile(nameTb.Text, view.VdDocument.GetJSon(!view.RealTime));
        }

        public PdEditBar()
        {
            InitializeComponent();
            saveBtn.Content = FindResource("Save");
            updateBtn.Content = FindResource("UpdateOn");
            newBtn.Content = FindResource("New");
            leftActive = false; copyActive = false; pasteActive = false;

            copyBtn.Content = FindResource("CopyOff");
            delBtn.Content = FindResource("DelOff");

            pasteBtn.Content = FindResource("PasteOff");

            leftBtn.Content = FindResource("LeftOff");
            rightBtn.Content = FindResource("RightOff");
            prnBtn.Content = FindResource("Print");
            CheckButtons();
        }

        private async void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            await view.UpdateDisplayAsync(view.VdDocument);
        }

        private void printBtn_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
