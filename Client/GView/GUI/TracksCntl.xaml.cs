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
    /// Interaction logic for TracksCntl.xaml
    /// </summary>
    public partial class TracksCntl : UserControl, IItemEditor
    {
        delegate void UpdateTracksDelegate();

        double dpiX;
        VdTracks tracks;
        PdEditorCntl editor;
        Label copiedOb;
        Label selectedOb;

        public PdEditorCntl Editor
        {
            get { return editor; }
            set
            {
                editor = value;
            }
        }


        public VdTracks Tracks
        {
            get { return tracks; }
            set { tracks = value;
            UpdateTracks();
            }
        }

        public TracksCntl()
        {
            InitializeComponent();
            dpiX = 96 * VisualTreeHelper.GetDpi(this).DpiScaleX;
           
         //   CreateTrackBoxes();
        }

        public void UpdateTracks()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new UpdateTracksDelegate(UpdateTracks), null);
            }
            else
            {
                CreateTrackBoxes();
                DataContext = tracks;
            }

        }

        public void CreateTrackBoxes()
        {
            trackBar.Children.Clear();
            Label c = new Label();
            c.Width = dpiX * tracks.LeftMargin;
            trackBar.Children.Add(c);
            bool b = false;
            int k = 0;
            foreach(VdTrack t in Tracks)
            {
                t.ID = k;
                c = new Label();
                c.Content = t.Name;
                c.Tag = t;
                c.Background = b? Brushes.LightGray : Brushes.LightBlue;
                b = !b;
                c.Width = t.Width * dpiX;
                c.HorizontalContentAlignment = HorizontalAlignment.Center;
                trackBar.Children.Add(c);
                c.MouseDown += C_MouseDown;
                k++;
            }
            SetSelectObject((Label)trackBar.Children[1]);
        }

        void SetSelectObject(Label ob)
        {
            selectedOb = ob;
            ob.Background = Brushes.LightCoral;
            trackCntl.DataContext = ob.Tag;
        }

        void SelectObject(Label ob)
        {
            bool b = false;
            for( int i = 1; i < trackBar.Children.Count; i++)
            {
                Label c = (Label)trackBar.Children[i];
                if (c == ob)
                    SetSelectObject(c);
                else
                {
                    c.Background = b ? Brushes.LightGray : Brushes.LightBlue;
                }
                b = !b;
            }
        }

        private void C_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SelectObject((Label)sender);
        }

        public object GetSelectedObject()
        {
            return selectedOb;
        }

        public object GetCopiedObject()
        {
            return copiedOb;
        }

        public void CopySelectedObject()
        {
           // copiedOb = curveCntl.Curve;
           // Editor.CheckEditorButtons();
        }

        public void PasteCopiedObject()
        {

        }


        public void CreatNewObject()
        {
            //    if (selectedOb != null)
            //        tracks.Insert(tracks.GetIndex((VdTrack)selectedOb.Tag), new VdTrack());
            //    else
            VdTrack t = new VdTrack();
                tracks.Add(t);
            UpdateTracks();
            SelectObject((Label)trackBar.Children[tracks.Count-1]);

        }

        public void DeleteSelectedObject()
        {
            if (selectedOb != null)
            {
                VdTrack t = (VdTrack)selectedOb.Tag;
                int k = t.ID;
                tracks.Remove(t);
                UpdateTracks();
                if (k >= tracks.Count)
                    k--;
                SelectObject((Label)trackBar.Children[k+1]);
            }

        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SelectObject(null);
        }


        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            CreatNewObject();
        }
    }



}
