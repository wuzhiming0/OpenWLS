using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenWLS.Server.LogDataFile;
using OpenWLS.Server.Base;
using OpenWLS.Server.DBase.Models.LocalDb;
//using OpenWLS.Server.LogInstance.Instrument;



namespace OpenWLS.Server.LogInstance.RtDataFile
{
    public class DataFileRt : DataFile
    {
        //Measurements rtChs;
        LogInstanceS li;
        double indexStart;
//        bool record;
        public DataFileRt(LogInstanceS li)
        {
            this.li = li;
        }

        /*  
          public void SetItemID(ArItem ai)
          {
              ai.ID = NxtItemID;
              NxtItemID++;
              ai.Archive = this;
          }*/
        public void StartLog(LogIndexMode lm,  Job job)
        {
          /*  record = _record;
             li.UpdateOcf();
             if (record)
                      CreateNew(fn);
                  indexStart = api.Index.Value;
                  SetItemID(api.OCF);

                  if (record)
                      api.OCF.SaveItem();


                  head.StartTime = DateTime.Now.Ticks;
                  head.StartDepth = api.Depth.Value;

                  SetItemID(locs);
           */
        }

        public void StopLog()
        {
            //if (record)
            {
                /*   foreach (ArChannel dc in chs)
                   {
                       if (dc.SampleBuffer.GetBodySize(head.Version) > 0)
                           dc.SampleBuffer.SaveBuffer();
                       dc.Head.ValueMin = dc.SampleBuffer.ValueMin;
                       dc.Head.ValueMax = dc.SampleBuffer.ValueMax;
                   }
                   head.StopTime = DateTime.Now.Ticks;
                   head.StopDepth = api.Depth.Value;
                   head.SaveItem();
                   chTbl.SaveItem();

                   Locations.SaveItem();
    */
                Close();
            }
        }


        public void GetIndexRange(out double indexMin, out double indexMax)
        {
            //     if(frames.Count == 0)
            {
                indexMin = double.NaN;
                indexMax = double.NaN;
            }
            /*     else
               {
                 indexMin = ((ArFrameRt)frames[0]).IndexMin;
                   indexMax = ((ArFrameRt)frames[0]).IndexMax;
                   for(int i = 0; i < frames.Count; i++)
                   {
                       double min = ((ArFrameRt)frames[i]).IndexMin;
                       double max = ((ArFrameRt)frames[i]).IndexMax;
                       if (max > indexMax)
                           indexMax = max;
                       if (min < indexMin)
                           indexMin = min;
                   }

               }
            */
        }

    }
}
