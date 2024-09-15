using OpenWLS.Client.GView.Models;
using OpenWLS.Server.GView.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace OpenWLS.Client.LogInstance.Calibration
{
    public class CVReportView : AGvView
    {
      //  List<GEVParameter> paras;

        public string References
        {
            set
            {
                ClearParameters();
                /*
                string str = value;
                string[] strs = str.Split(new char[] { ',' });
                int k = doc.EIDCur;
                foreach (string s in strs)
                {
                    GvNumber val = items.GetValueItem(s);
                    if (val != null)
                    {
                        GEVParameter p = GEVParameter.FromGEVNumberValue(val, items);
                        p.Infor.EID = k;
                        p.DocGe = doc;
                        k++;
                        items.Add(p);
                    }
                }*/
                UpdateParas();
            }
        }


    }
}
