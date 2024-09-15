using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Windows.Media;

using Newtonsoft.Json;

using OpenWLS.PLT1.Edge;

using OpenWLS.Server.Base;

namespace OpenWLS.PLT1.TesterA
{
    public class InstTesterA : PLT1Instrument
    {      
        public InstTesterA()
        {
            Address = default_addr = IBProtocol.S_MOD_ADDR;
        }

        void DownloadOCF()
        {

        }
        



        string ReadTime(DataReader r)
        {
            DateTime dt = DataType.GetDateTimeFromRTC(r);
            return dt.ToShortDateString() + dt.ToShortTimeString();
        }

        override public void OnConnected()
        {

        }





    }
}
