using OpenWLS.Server.LogDataFile.Models;
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

namespace OpenWLS.Client.LogDataFile
{
    /// <summary>
    /// Interaction logic for MHeadEditor.xaml
    /// </summary>
    public partial class MHeadEditor : UserControl
    {
        public MHeadEditor()
        {
            InitializeComponent();
        }

        private void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            MHead ch = (MHead)DataContext;
            string str = null;
            if(emptyTb.Text != ch.VEmpty.ToString())
                str = "ValueEmpty = " + emptyTb.Text + "|";

            if(dispFormatTb.Text != ch.DFormat)
                str = str + "DisplayFormat = " + dispFormatTb.Text + "|";


            if (ch.Frame != null)
            {
                if(depthShiftTb.Text != ch.IndexShift.ToString())
                    str = str + "DepthShift = " + depthShiftTb.Text + "|";
            }
            else
            {
                if (depthShiftTb.Text.Length > 0)
                {
                    double d;
                    if(double.TryParse(depthShiftTb.Text, out d))
                    {
                        if(d != 0)
                            str = str + "DepthShift = " + depthShiftTb.Text + "|";
                    }
                }
            }
            if (str != null)
            {
                str = str.Substring(0, str.Length - 1);
            //    ArGui.SendRequest(ArProc.str_req_ch_head + "\n"
            //        + ArGui.FullFileName + "\n" + ch.ID.ToString() + "\n" + str);
            }

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            updateBtn.Visibility = Visibility.Visible;
        }
    }
}
