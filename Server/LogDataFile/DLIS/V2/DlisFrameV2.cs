using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWLS.Server.Base;



using System.IO;
using OpenWLS.Server.LogDataFile.Models;
using OpenWLS.Server.LogDataFile;

namespace OpenWLS.Server.LogDataFile.DLIS.V2
{
    public class DlisFrameV2 : DlisFrame
    {


        public override bool InitFrame(ObjectComponent oc, AttributeComponents template, SetComponent channelSet, SetComponent axisSet)
        {
            return false;
        }


    }
    
}