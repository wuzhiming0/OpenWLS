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
using OpenWLS.Server.Base;
using Xceed.Wpf.Toolkit;
using Newtonsoft.Json;
using OpenWLS.Client.GView.Models;

namespace OpenWLS.Client.GView.GUI
{

    /// <summary>
    /// Interaction logic for CurveCntl.xaml
    /// </summary>
    public partial class CurveCntl : UserControl
    {
        VdCurve curve;
        public bool RealTime
        {
            set
            {
                chCntl.RealTime = value;
            }
        } 
        public VdCurve Curve
        {
            get { return curve; }
            set
            {
                curve = value;
                DataContext = curve;
                scaleCntl.ScaleParent = curve;
                genCntl.CVIDItem = curve;
                if (curve != null){

                    leftTCntl.TPosition = curve.LeftPos;
                    rightTCntl.TPosition = curve.RightPos;
                    chCntl.Measurement = curve.Measurement;
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



        public CurveCntl()
        {
            InitializeComponent();
            chCntl.D1Channel = true;
   //         colorCntl.SelectedColor = Color.FromArgb(255,0,0,0);
        }


    }

}
