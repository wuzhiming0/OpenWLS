using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using OpenWLS.Server.LogInstance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace OpenWLS.Edge.Simulator
{
    public class MGroupAcquired
    {
        public byte MgId { get; set; }
        public uint Time { get; set; }
        public byte[] Vals  { get; set; }
    }



    public class AcquisitionEngine
    {
        //DateTime dt_start;

        public AcqItems AcqItems { get; set; }
        protected AcqItems acqItems;
        protected uint samplingInterval;
        protected System.Timers.Timer acqTimer;
        protected uint acqTime;

        Queue<AcqItem> acqQueue;
        bool acq_busy;
        InstrumentSim inst;
        public AcquisitionEngine(InstrumentSim inst)
        {
            acqTimer = new System.Timers.Timer();
            acqTimer.Elapsed += AcqTimer_Elapsed;
            acqQueue = new Queue<AcqItem>();
            acq_busy = false;
            this.inst = inst;
        }

        private void AcqTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            acqTime += samplingInterval;
            TimeCheckAcqItems();
        }


        void TimeCheckAcqItems()
        {
            foreach (AcqItem a in acqItems)
            {
                if ((bool)a.Enable && a.NxtSampleTime <= acqTime)
                {
                    if (a.State == AcqItemState.Done || (a.State == AcqItemState.Idle))
                    {
                        a.State = AcqItemState.Wait;
                        acqQueue.Enqueue(a);
                        if (!acq_busy)
                            Task.Run(() => { AcquireLoop(); });
                    }
                }
            }

        }

        void AcquireLoop()
        {
            acq_busy = true;
            while (acqQueue.Count > 0)
            {
                AcqItem a;
                acqQueue.TryDequeue(out a);
                if (a != null)
                {
                    inst.AcquireMgroup(a.Id);
                    a.State = AcqItemState.Done;
                }
            }
            acq_busy = false;
        }


        public void StartAcq()
        {
          //  dt_start = DateTime.Now;
            acqTime = 0;
            double samplingLCM = acqItems.GetSamplingLCM(); //LCM Least Common Multiple
            if (double.IsNaN(samplingLCM))
            {
                //			api.SysLog.AddMessage("No acquistion item.");
                return;
            }
            if (samplingLCM == 0)
            {
                //	api.SysLog.AddMessage("zero time sampling rate.");
                return;
            }
                 else
                    samplingInterval = (uint)(1000/samplingLCM);


            foreach (AcqItem a in acqItems)
            {
                a.IntervalTime = a.IntervalTime;
                a.State = AcqItemState.Idle;
            }

            TimeCheckAcqItems();

            acqTimer.Interval = samplingInterval;
            acqTimer.Start();
        }

        public void StopAcq()
        {
            acqTimer.Stop();
       
        }

    }
}
