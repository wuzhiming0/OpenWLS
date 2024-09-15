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
    /// Interaction logic for ItemTrackCntl.xaml
    /// </summary>
    public partial class ItemTrackCntl : UserControl
    {
        delegate void UpdateItemTrackDelegate();

        VdTracks tracks;
        VdxPosition tPosition;
        public VdTracks Tracks
        {
            set
            {
                tracks = value;
                UpdateItemTrack();
            }
        }

        public VdxPosition TPosition
        {
            set
            {
                tPosition = value;
                UpdateItemTrack();
             //   trackCb.SelectedItem = value.Track;
            }
        }
        public void UpdateItemTrack()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new UpdateItemTrackDelegate(UpdateItemTrack), null);
            }
            else
            {
                trackCb.ItemsSource = tracks;
                DataContext = tPosition;
            }

        }

        public ItemTrackCntl()
        {
            InitializeComponent();
        }
    }
}
