using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWLS.Server.LogDataFile.Models;
using OpenWLS.Server.LogDataFile.DLIS;
using OpenWLS.Server.LogDataFile;

namespace OpenWLS.Server.LogDataFile.LAS
{
    public class LasFrame : Frame
    {
        public object Tag { get; set; }
        public void UpdateChannelHeads()
        {
            foreach (Measurement m in Measurements)
            {
                m.Head.Name = m.Head.Name.Replace("[", "");
                m.Head.Name = m.Head.Name.Replace("]", "");

                m.Head.VFirst = m.MVWriter.FirstVal;
                m.Head.VLast = m.MVWriter.LastVal;        
           //     m.Head.Samples = m.MVWriter.TotalSamples;
               
                m.Head.Spacing = null;
            //    m.Head.VMin = m.SampleBuffer.ValueMin;
            //    m.Head.VMax = c1.SampleBuffer.ValueMax;
            }
        }
        public void SetChannelHeadIndex(int samples)
        {
            foreach(Measurement m in Measurements)
            {               
                if (m.Head.UOM == null)
                    continue;
                string uom = m.Head.UOM.ToUpper();
                if( m.Head.Name == "DEPT" )
                { 
                    if (uom == "M" || uom == "F" || uom  == "FT")
                    {
                        m.Head.IType = LogIndexType.BOREHOLE_DEPTH;
                        m.Head.IndexM = true; 
                        return;
                    }
                }
                if( m.Head.Name == "TIME" )
                {
                    if (uom == "S" || uom == "M")
                    {
                        m.Head.IType = LogIndexType.TIME;
                        m.Head.IndexM = true; 
                        return;
                    }
                }
            }
            Samples = samples;
        }

    }


}
