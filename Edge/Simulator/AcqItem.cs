using OpenLS.Base.UOM;
using OpenWLS.Server.LogInstance.Instrument;
using OpenWLS.Server.LogInstance.OperationDocument;
using OpenWLS.Server.LogInstance.RtDataFile;
using OpenWLS.Server.Base;
using OpenWLS.Server.LogDataFile.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Net;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace OpenWLS.Edge.Simulator
{
    public enum AcqItemState { Idle = 0, Wait = 1, Acquiring = 2, Done = 3 };
    public class AcqItem : Server.LogInstance.OperationDocument.AcqItem
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected InstrumentSim inst;
        protected MeasurementSims ms;
        protected int bufSamples;
 //       protected LogInstance api;
        public bool Active { get; set; }

        public uint NxtSampleTime { get; set; }
        public   AcqItemState State { get; set; }

        public MeasurementSims Measurements { get { return ms; } }
 //       public TimeMeasurement TimeM { get; set; }
 //       public DepthMeasurement DepthM { get; set; }

        //     public string RecordMs { get; set; }
        //     public string NoRecordMs { get; set; }
        /*
             bool enable;
             public bool Enable {
                 get { return enable; }
                 set
                 {
                     enable = value;
                     if (PropertyChanged != null)
                         PropertyChanged(this, new PropertyChangedEventArgs("Enable"));
                 }
             }
        */
        //     public bool CV { get; set; }


        public AcqItem()
        {
            Active = true;
          //  AddressAE = -1;
        }
        public bool Same(AcqItem acq)
        {
            return Id == acq.Id;
        }

         
 
        public void Init()
        {
            
            double f = MeasurementUnit.GetDepthConvertMul("m");
       //     SamplesPerMeter = IntervalDepth / f;
       //     SamplesPS = IntervalTime == null || IntervalTime == 0? null : 1 / (double)IntervalTime;
       //     SamplesPM = IntervalDepth == null || IntervalDepth == 0? null : f / (double)IntervalDepth  ;

        }
        public double GetSamplesPerSecond()
        {
            if (IntervalTime == null) return 1;
            else return 1000 / (uint)IntervalTime;
        }
       

    }


    public class AcqItems : List<AcqItem>
    {
        public static AcqItems GetAcqItemsOfInst(Server.LogInstance.OperationDocument.AcqItems acqItems, int iid )
        {
            List<Server.LogInstance.OperationDocument.AcqItem> acqs = acqItems.Where(a=>a.IId == iid).ToList();
            AcqItems items = new AcqItems();
            foreach(Server.LogInstance.OperationDocument.AcqItem a in acqs)
            {
                AcqItem item = new AcqItem();
                item.CloneFrom(a);
                items.Add(item);
            }
            return items;
        }
        public double GetSamplingLCM()
        {
            if (Count == 0)
                return double.NaN;
            int[] ds = new int[Count];
            for (int i = 0; i < Count; i++)
                ds[i] = (int)this[i].GetSamplesPerSecond();
            return DataType.lcm_of_array_elements(ds);
        }
   /*     public OperationDocument.AcqItem GetActItem(int inst_id, int mg_id)
        {

        }
        public OperationDocument.AcqItems GetOperationDocumentAcqItems()
        {
            OperationDocument.AcqItems act = new OperationDocument.AcqItems();
            foreach(AcqItem a in this)
            {
                OperationDocument.AcqItem a_od = new OperationDocument.AcqItem();
              //  a_od.MUpdates = a.Mg

            }
            return act;
        }
*/


        public void Init(InstrumentSim inst)
        {

        }



    }
}