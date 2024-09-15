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

namespace OpenWLS.Client.GView.GUI
{

    /// <summary>
    /// Interaction logic for CurveCntl.xaml
    /// </summary>
    public partial class FillCntl : UserControl
    {
        VdFill fill;

        public VdFill Fill
        {
            get { return fill; }
            set
            {
                fill = value;
                SetPatternOptions();
                DataContext = fill;     
        
            }
        }

        public VdItems VdItems
        {
            set
            {
                string s1 = null;
                string s2 = null;
                if (fill != null)
                {
                     s1 = fill.LeftCurve;
                     s2 = fill.RightCurve;
                }
                leftCurveCntl.Items.Clear();
                rightCurveCntl.Items.Clear();
                leftCurveCntl.Items.Add("");
                rightCurveCntl.Items.Add("");
                foreach (VdItem i in value)
                {
                    if (i is VdItem)
                    {
                        if (((VdItem)i).Type == LogViewItemType.Curve)
                        {
                            leftCurveCntl.Items.Add(i.Name);
                            rightCurveCntl.Items.Add(i.Name);
                        }
                    }
                }

                rightCurveCntl.Items.Add("OverScale");
                rightCurveCntl.Items.Add("UnderScale");

                if (fill != null)
                {
                    leftCurveCntl.SelectedItem = s1;
                    rightCurveCntl.SelectedItem = s2;
                }

            }
        }



        void SetPatternOptions()
        {
            if (patCntl.Items.Count > 0)
                return;
            foreach(string str in VdFill.PatternNames)
                patCntl.Items.Add(str) ;
        }

   




        public FillCntl()
        {
            InitializeComponent();
     //       chCntl.D1Channel = true;
   //         colorCntl.SelectedColor = Color.FromArgb(255,0,0,0);
        }


    }

}
