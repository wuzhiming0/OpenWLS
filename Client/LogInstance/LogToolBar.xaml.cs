using OpenWLS.Server.LogInstance;
using OpenWLS.Server.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Client;
using OpenLS.Base.UOM;

namespace OpenWLS.Client.LogInstance
{
    /// <summary>
    /// Interaction logic for LogToolBar.xaml
    /// </summary>
    public partial class LogToolBar : DockPanel
    {
        public event EventHandler StateChanged;

        LiState state;
        public LiState State
        {
            get {  return state;  }
            set
            {
                switch (value)
                {
                    case LiState.Blank:
                    case LiState.Edit:
                    case LiState.CV:
                    case LiState.Log_NoEdge:
                    case LiState.Log_EdgeConnected:
                        Running = false;
                        EnableAcqBtns(false);
                        break;

                    case LiState.Log:
                        Running = true;
                        EnableAcqBtns(true);
                        break;

                    case LiState.Log_Standby:
                    case LiState.Playback_Standby:
                    case LiState.Playback:
                        EnableAcqBtns(true);
                        Running = false;
                        break;
                }
                state = value;
                indexBtn.IsEnabled = state == LiState.Log_Standby;
            }
        }
        double time;
        public double Time
        {
            get { return time; }
            set
            {
                time = value;
                TimeSpan timespan = TimeSpan.FromSeconds(time);
                timeTb.Text = timespan.ToString("hh\\:mm\\:ss");
            }
        }
        double depth;
        public double Depth
        {
            get  { return depth; }
            set
            {
                depth = value;
                depthTb.Text = depth.ToString("f1");
            }

        }

        bool record;
        public bool Record
        {
            get { return record; }
            set
            {
                record = value;
                string str = record ? "RecOn" : "RecOff";
                UpdateButton(recBtn, str);
            }
        }

        bool running;
        public bool Running
        {
            get { return running; }
            set
            {
                running = value;
                string str = running ? "LogStop" : "LogReady";
                UpdateButton(acqContiousBtn, str);
            }
        }



        LogIndexMode logMode;
        public LogIndexMode LogMode
        {
            get { return logMode; }
            set
            {
                logMode = value;
                string str;
                switch (logMode)
                {
                    case LogIndexMode.Time:
                        str = "IndexTime";
                        break;
                    case LogIndexMode.Up:
                        str = "IndexUp";
                        break;
                    default:
                        str = "IndexDown";
                        break;
                }
                UpdateButton(indexBtn, str);
            }
        }

        public LogToolBar()
        {
            InitializeComponent();
            state = LiState.Blank;
            Running = false;
            Record = false;
            LogMode = LogIndexMode.Time;

            indexBtn.IsEnabled = false;
            Depth = 0;
            Time = 0;
            depthUnitLabel.Content = MeasurementUnit.GetSelectedUnit("Depth");
        }
        void EnableAcqBtns(bool en)
        {
            Dispatcher.Invoke(() =>
            {
                acqContiousBtn.IsEnabled = en;
                recBtn.IsEnabled = en;
                indexBtn.IsEnabled = en;
            });
        }

        public LogIndexMode GetNextMode()
        {
            int k = (int)logMode;
            k++;
            if (k > 3)
                k = 0;
            return (LogIndexMode)k;
        }


        public void UpdateButton(Button btn, string str)
        {
            Dispatcher.Invoke(() =>
            {
                if (btn != null)
                    btn.Content = FindResource(str);
            });
        }


        void RequestState()
        {
            DataWriter w = new DataWriter(8);

            w.WriteData((ushort)LiWsMsg.LiState);

            if( running ) w.WriteData((ushort)0);   //to stop
            else w.WriteData((ushort)1);            //to start

            if (record) w.WriteData((ushort)1);     //record
            else w.WriteData((ushort)0);            //no record

            w.WriteData((ushort)logMode);           //index

            ((MainWindow)App.Current.MainWindow).LiClientMain.SendWsPackage(w.GetBuffer());
        }

        private void acqContiousBtn_Click(object sender, RoutedEventArgs e)
        {
            RequestState();
        }

        private void recBtn_Click(object sender, RoutedEventArgs e)
        {
            record = !record;
        }

        private void indexBtn_Click(object sender, RoutedEventArgs e)
        {
             logMode = GetNextMode();
        }



    }
}
