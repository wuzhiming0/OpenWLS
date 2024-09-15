using OpenWLS.Server.Base;
using OpenWLS.Server.DBase.Models.GlobalDb;
using OpenWLS.Server.LogDataFile.Models;
using OpenWLS.Server.LogInstance.OperationDocument;

namespace OpenWLS.Server.LogInstance.RtDataFile
{
    public class FrameRt
    {
        public string Name { get; set; }
        public Instrument.Measurements Measurements { get; set; }

        public FrameRt(AcqItem acqItem, Instrument.Measurements ms)
        {
            Measurements = new Instrument.Measurements();
            Name = acqItem.Id.ToString();
            foreach(int mid in acqItem.MIds)
            {
                Instrument.Measurement? m = ms.Find(a=>a.Id == mid);
                if (m != null)
                {
                    m.FrameRt = this;
                    m.MeasurementDf.Head.Frame = Name;
                    Measurements.Add(m);
                }
            }
        }


    }
    public class FrameRts : List<FrameRt>
    {
        public FrameRts(AcqItems acqItems, Instrument.Measurements ms)
        {
            foreach (AcqItem a in acqItems)
                Add(new FrameRt(a, ms));
        }

        public void StartLog(bool depthIndex)
        {
            foreach (FrameRt frameRt in this)
            {
                foreach (Instrument.Measurement m in frameRt.Measurements)
                    m.StartLog(depthIndex);
            }
        }
    }

}
