using OpenWLS.Client.LogInstance;
using OpenWLS.Client.LogInstance.Instrument;
using OpenWLS.Server.Base;
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


namespace OpenWLS.PLT1.MemA
{
    /// <summary>
    /// Interaction logic for Mem1Cntl.xaml
    /// </summary>
    public partial class MemACntl : InstCntl
    {
    //    bool updated;

        public enum EFlashStatus{ Off = 0, Scan = 1, Ready = 2, Busy = 3};
        EFlashStatus ef_status;
        bool imageFlip;
        System.Windows.Threading.DispatcherTimer timer;
        delegate void EFlashStatusChangedDelegate(); 

        public MemACntl()
        {
            InitializeComponent();
            //   updated = false;
            ResetStatus();
          //  flashBtn.Content = FindResource("EFlashOff");
       //     Mem1EFArch arch = new Mem1EFArch();
        //    efInforView.Text = arch.ToString();
         //   efInforView.ItemsSource = new Mem1EFArch();
        }

        public void ResetStatus()
        {
            ef_status =  EFlashStatus.Off;
            UpdateEflashStatus();
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
      //      if ((bool)e.NewValue && (!updated))
     //           inst.SendRequest("GetInfor");
        }

        private void flashBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ef_status== EFlashStatus.Off && timer == null)
            {
               // Inst.SendRequest("Scan");
                timer = new System.Windows.Threading.DispatcherTimer();
                timer.Interval = new TimeSpan(10000000);
                timer.Tick += timer_Tick;
                imageFlip = false;
                ef_status = EFlashStatus.Scan;     
                timer.Start();
            }

        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (ef_status == EFlashStatus.Scan)
                flashBtn.Content = imageFlip ? FindResource("EFlashOff") : FindResource("EFlashScan");
            else
                flashBtn.Content = imageFlip ? FindResource("EFlashOn") : FindResource("EFlashBusy");
            imageFlip = !imageFlip;            
        }

        public void StopScan(bool success)
        {
            timer.Stop();
            timer.Tick -= timer_Tick;
            timer = null;
            ef_status = success ? EFlashStatus.Ready : EFlashStatus.Off;
            UpdateEflashStatus();
        }

        public void StartEFlashTask()
        {
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(10000000);
            timer.Tick += timer_Tick;
            imageFlip = false;
            ef_status = EFlashStatus.Busy;
            timer.Start();
        }

        public void EndEFlashTask()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Tick -= timer_Tick;
                timer = null;
                ef_status = EFlashStatus.Ready;
                UpdateEflashStatus();
            }
        }
        public void EndEFlashScanWithError()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Tick -= timer_Tick;
                timer = null;

            }
            ef_status = EFlashStatus.Off;
            UpdateEflashStatus();
        }

        public void UpdateEflashStatus()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new EFlashStatusChangedDelegate(UpdateEflashStatus), null);
            }
            else
            {
                switch (ef_status)
                {
                    case EFlashStatus.Ready:
                        flashBtn.Content = FindResource("EFlashOn");
                        break;
                    default:
                        flashBtn.Content = FindResource("EFlashOff");
                        break;
                }
            }
        }

        public void SetEFlashUseage(string str)
        {
             int[] ds = StringConverter.ToIntArray(str, ',');
             if (ds.Length == 3)
             {
                 efInforView.SetSpaces(ds);
                 if (ef_status != EFlashStatus.Ready)
                     EndEFlashTask();
             }
        }

        private void UploadBtn_Click(object sender, RoutedEventArgs e)
        {
           // Inst.SendRequest("Upload\n" + dfsLisView.SelectedIndex.ToString());
        }

        private void NewBtn_Click(object sender, RoutedEventArgs e)
        {
           // Inst.SendRequest("NewFile");
        }

        private void EraseBtn_Click(object sender, RoutedEventArgs e)
        {
           // Inst.SendRequest("Erase\n" + dfsLisView.SelectedIndex.ToString());
        }

    }

}
