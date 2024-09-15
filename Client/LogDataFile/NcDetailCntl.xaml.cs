using System;
using System.Collections.Generic;
using System.Data;
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



using OpenWLS.Client.LogInstance;
using OpenWLS.Client.Base;
using OpenWLS.Server.Base;
using OpenWLS.Server.DBase.Models.LocalDb;

namespace OpenWLS.Client.LogDataFile
{
    /// <summary>
    /// Interaction logic for NcDetailCntl.xaml
    /// </summary>
    public partial class NcDetailCntl : UserControl
    {
        object ncDetail;

        public NcDetailCntl()
        {
            InitializeComponent();
        }


        delegate void UpdateNcDetailDelegate();
        public void UpdateNcDetail()
        {
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke(new UpdateNcDetailDelegate(UpdateNcDetail), null);
            else
            {
                if (ncDetail is DataTable)
                {
                    DataGrid dg = new DataGrid();
                    dg.IsReadOnly = true;
                    dg.ItemsSource = ((DataTable)ncDetail).DefaultView;
                    grid.Children.Clear();
                    grid.Children.Add(dg);
                    Visibility = System.Windows.Visibility.Visible;
                    return;
                }
                /*
                if (ncDetail is FileHead)
                {
                    FileHeadCntl fh = new FileHeadCntl();
                    fh.DataContext = ncDetail;
                    grid.Children.Clear();
                    grid.Children.Add(fh);
                    Visibility = System.Windows.Visibility.Visible;
                    return;
                }
                */
                if (ncDetail is JobC)
                {
                 /*   JobEditor je = new JobEditor();
                    je.Job = (JobC)ncDetail;
                    Visibility = System.Windows.Visibility.Visible;
                    grid.Children.Clear();
                    grid.Children.Add(je);*/
                    return;
                }
                if (ncDetail is Parameters)
                {
                    ParaEditor pe = new ParaEditor();
                    pe.Parameters = (Parameters)ncDetail;
                    Visibility = System.Windows.Visibility.Visible;
                    grid.Children.Clear();
                    grid.Children.Add(pe);
                    return;
                }
            }


        }
    }
}