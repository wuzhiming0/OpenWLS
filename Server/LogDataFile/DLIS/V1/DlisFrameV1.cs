using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWLS.Server.Base;



using System.IO;
using OpenWLS.Server.LogDataFile.Models;
using OpenWLS.Server.LogDataFile;

namespace OpenWLS.Server.LogDataFile.DLIS.V1
{
    public class DlisFrameV1 : DlisFrame
    {
        
        bool CreateMeasurements(ObjectComponent oc, AttributeComponents template, SetComponent channelSet)
        {
            Samples = oc.Samples;
            object[] channelNames;
            int channelsIndex = template.GetComponentIndex("CHANNELS");
            if (oc[channelsIndex].Value is object[])
                channelNames = (object[])oc[channelsIndex].Value;
            else
            {
                channelNames = new object[1];
                channelNames[0] = oc[channelsIndex].Value;
            }
            // ms = new Measurement[channelNames.Length];
            for (int i = 0; i < channelNames.Length; i++)
            {
                string strName = ((DLISObjectName)channelNames[i]).Indenifier;
                foreach (ObjectComponent channel in channelSet.Objects)
                {
                    if (channel.Name == strName)
                    {
                        if (channel.Tag != null)
                            continue;
                        DlisChannel newDlisChannel = DlisChannel.CreateDlisChannel(channel, this, channelSet.Template);
                        if (newDlisChannel == null)
                            return false;

                        channel.Tag = newDlisChannel;
                        ms.Add(newDlisChannel);
                        break;
                    }
                }
            }
            return true;
        }
        public override bool InitFrame(ObjectComponent oc, AttributeComponents template, SetComponent channelSet, SetComponent axisSet)
        {
            Name = oc.Name;
            IndexType = Index.ConvertToIndexType((string)oc[template.GetComponentIndex("INDEX-TYPE")].Value);
            if (IndexType == LogIndexType.Unknown)
                IndexType = LogIndexType.SAMPLE_NUMBER;
            else
            {
                AttributeComponent ac = oc[template.GetComponentIndex("SPACING")];
                if (ac != null)
                {
                    LevelSpacing = Convert.ToDouble(ac.Value);
                    UOI = ac.Units;
                    indexDecreasing = LevelSpacing < 0;
                }
                else
                {
                    ac = oc[template.GetComponentIndex("DIRECTION")];
                    if (ac != null) indexDecreasing = (string)ac.Value == "DECREASING";
                }
            }
           IndexMin = Convert.ToDouble(oc[template.GetComponentIndex("INDEX-MIN")].Value);
           IndexMax = Convert.ToDouble(oc[template.GetComponentIndex("INDEX-MAX")].Value);

           if(!CreateMeasurements(oc, template, channelSet))
                return false;
           if(LevelSpacing == null)
            {
                Measurements[0].Head.IndexM = true;
                indexes.Add( new IndexVsM(Measurements[0]) );
                foreach (Measurement m in Measurements)
                    m.Head.Frame = Name;
           }
           else
                indexes.Add( new IndexEsWithoutGap(Measurements[0]) );
           ch_nu = 0;
           Samples = oc.Samples;
           return true;
        }



    }
    
}