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
using OpenWLS.Server.GView.Models;
//using System.Windows.Data



using OpenWLS.Server.GView.ViewDefinition;


namespace OpenWLS.Client.GView.GUI
{

    /// <summary>
    /// Interaction logic for ImageCntl.xaml
    /// </summary>
    public partial class ImageCntl : UserControl
    {
        VdImage image;

        public bool RealTime
        {
            set
            {
                chCntl.RealTime = value;
            }
        } 

        public VdImage Image
        {
            get { return image; }
            set
            {
                image = value;
                DataContext = image;
                scaleCntl.ScaleParent = image;
                genCntl.CVIDItem = image;
                if (image != null)
                {
                    leftTCntl.TPosition = image.LeftPos;
                    rightTCntl.TPosition = image.RightPos;
                    chCntl.Measurement = image.Measurement;
                }
                else
                {
                leftTCntl.TPosition = null;
                rightTCntl.TPosition = null;
                chCntl.Measurement = null;
                }

            }
        }
        public VdDFiles Dfiles
        {
            set
            {
                chCntl.DFiles = value;
            }
        }


        public Scales Scales
        {
            set
            {
                scaleCntl.Scales = value;
            }
        }

        public VdTracks Tracks
        {
            set
            {
                leftTCntl.Tracks = value;
                rightTCntl.Tracks = value;
            }
        }



        public ImageCntl()
        {
            InitializeComponent();
            chCntl.D1Channel = false;
   //         colorCntl.SelectedColor = Color.FromArgb(255,0,0,0);
        }

        private void ColorMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmCntl.SelectedItem == null)
                return;
            switch ((ColorMode)cmCntl.SelectedItem)
            {
                case ColorMode.Earth:
                    colorHCntl.SelectedColor = Color.FromArgb(0xff, 0, 0, 0xf0);
                    colorLCntl.SelectedColor = Color.FromArgb(0xff, 0xf0, 0xf0, 0xff);
                    colorHCntl.IsEnabled = false;
                    colorLCntl.IsEnabled = false;
                    break;
                case ColorMode.VDL:
                    colorHCntl.SelectedColor = Color.FromArgb(0xff, 0, 0, 0xf0);
                    colorLCntl.SelectedColor = Color.FromArgb(0xff, 0xf0, 0xf0, 0xff);
                    colorHCntl.IsEnabled = false;
                    colorLCntl.IsEnabled = false;
                    break;
                default:
                    colorHCntl.IsEnabled = true;
                    colorLCntl.IsEnabled = true;
                    break;
            }
        }


    }
}
