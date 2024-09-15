using OpenLS.Base.UOM;
using OpenWLS.Server.Base;
using OpenWLS.Server.DBase;
using OpenWLS.Server.GView.Models;
using OpenWLS.Server.LogDataFile.Models;
using OpenWLS.Server.LogInstance;

namespace OpenWLS.Server.GView.ViewDefinition
{
    public class VdDocumentRt : VdDocument
    {
        double indexStep;
        double indexMul;

        public int? Id { get; set; }
        public string Name { get; set; }

        public VdDocumentRt() {

        }
        protected override void OnNewGvDocument(GvDocument gvDoc) 
        {
            gvDoc.Id = Id;
        }
        protected override void DrawItems(GvDocument gvDoc, float yOffset, ISyslogRepository sysLog)
        {
           // items.DrawItemsRt(top, geDoc, yOffset);  //cause solid line curve
           // GeEOS gen = new GeEOS();
           // SendElement(geDoc, gen);
        }

        void SetIndexRange(Depth depth, Time time, bool indexIncrease)
        {
            indexStep = 5 / YScale;      // 5 inch on screen 
            bool indexIndepth = IndexUnit == LogDataFile.Models.IndexUnit.ft || IndexUnit == LogDataFile.Models.IndexUnit.meter;
            if(indexIndepth)
            {
                indexMul = MeasurementUnit.GetDepthConvertMul(IndexUnit.ToString(), depth.Unit);
                if (indexIncrease)
                {
                    top = depth.Value * indexMul;
                    bottom = top + indexStep;
                }
                else
                {
                    bottom = depth.Value * indexMul;
                    top = bottom - indexStep;
                }
            }
            else
            {
                indexMul = MeasurementUnit.GetTimeConvertMul(IndexUnit.ToString(), time.Unit);
                top = time.Value * indexMul;
                bottom = top + indexStep;
            }        
        }
        
        public void StartLog(Depth depth, Time time, bool indexIncrease)
        {
            SetIndexRange(depth, time, indexIncrease); 
        }

        public void StopLog()
        {

        }
        public void PLotNewSamples()
        {

        }

    }

    public class VdDocumentRts : List<VdDocumentRt>
    {
        public void StartLog(Depth depth, Time time, bool indexIncreasing)
        {
            foreach (VdDocumentRt vd in this)
                vd.StartLog( depth, time, indexIncreasing );
        }
    }
}
