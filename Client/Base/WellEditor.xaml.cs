
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

using OpenWLS.Server.DBase.Models.LocalDb;

namespace OpenWLS.Client.Base
{
    /// <summary>
    /// Interaction logic for JobEditor.xaml
    /// </summary>
    public partial class WellEditor : UserControl
    {
        JobC job;
        public JobC Job
        {
            get { return job; }
            set
            {
                job = value;
                DataContext = job;
                /*
                if (job == null)
                    Visibility = Visibility.Hidden;
                else
                {
                    if (job.Casings == null)
                    {
                        job.Casings = new Casings();
                        job.DrillingBits = new Casings();
                        TextParas ps = job.Casings.RestoreCasings(job.WellInfor);
                        job.WellInfor = job.DrillingBits.RestoreCasings(ps);
                    }

                    wiEditor.dg.ItemsSource = job.WellInfor;
                    wiEditor.titleTb.Text = "Well Information";

                    if (job.DrillingBits == null)
                        job.DrillingBits = new Casings();
                    bitEditor.dg.ItemsSource = job.DrillingBits;

                    bitEditor.titleTb.Text = "Drilling Bits";
                    if (job.Casings == null)
                        job.Casings = new Casings();
                    casingEditor.dg.ItemsSource = job.Casings;
                    casingEditor.titleTb.Text = "Casings";
                    Visibility = Visibility.Visible;
                }*/
            }
        }

        public WellEditor()
        {
            InitializeComponent();
        }


        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (job != null)
            {
        /*        job.Casings.Cleanup();
                job.DrillingBits.Cleanup();
                bitEditor.dg.ItemsSource = null;
                casingEditor.dg.ItemsSource = null;
                bitEditor.dg.ItemsSource = job.DrillingBits;
                casingEditor.dg.ItemsSource = job.Casings;

                if(guiItem.Name == "Log")
                {
                    string msg = Server.str_gen_mod_job + "\n";
                    if (job.ID < 0)
                        msg = msg + Server.str_gen_job_req_new + "\n" + JsonConvert.SerializeObject(job);
                    else
                        msg = msg + Server.str_gen_job_req_update + "\n" + JsonConvert.SerializeObject(job);
                    guiItem.Client.SendRequest(msg);
                }
                if (guiItem.Name == "Archive")
                {
                  //  string msg = "TxtObj\n" + ArGui.FullFileName + "\n";
                  //  msg = msg + JsonConvert.SerializeObject(job);
                  //  guiItem.Client.SendRequest(msg);
                }
*/
            }
        }
    }

 
}
