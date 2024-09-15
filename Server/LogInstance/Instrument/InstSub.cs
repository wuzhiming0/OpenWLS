using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using OpenWLS.Server.APInstance;
using OpenWLS.Server.APInstance.Instrument;
using System.Data.SQLite;
using Newtonsoft.Json.Linq;
using OpenWLS.Server.DBase;
using OpenWLS.Server.DBase.Repositories.InstrumentM;
using OpenWLS.Server.DBase.Models.GlobalDb;
using System.Collections;
using System.Text.Json.Serialization;

namespace OpenWLS.Server.APInstance.Instrument
{
    public class InstSub : OperationDocument.InstSub
    {
        public int Id { get; set; }               //operation document
        public string Asset { get; set; }


        public InstSub()
        {

        }
        public InstSub(OperationDocument.InstSub sub_od)
        {
            Id = sub_od.Id;
            DbId = sub_od.DbId;
            Asset = sub_od.Asset;
        }



        /*
        public void Init(DataTable mdt, string serial, string asset)
        {
            Serial = serial;
            Asset = asset;
            foreach (DataRow mdr in mdt.Rows)
            {
                if (Convert.ToString(mdr["Serial"]) == Serial)
                {
                    Length = mdr["Length"] == DBNull.Value? 0: Convert.ToDouble(mdr["Length"]);
                    Weight = mdr["Weight"] == DBNull.Value ? 0 : Convert.ToDouble(mdr["Weight"]);
                    Diameter = mdr["Diameter"] == DBNull.Value ? 0 : Convert.ToDouble(mdr["Diameter"]);
                    break;
                }
            }
        } 
        */
    }

    public class InstSubs : List<InstSub>
    {
        //   public static char[] seps = new char[] { '|' };
        public OperationDocument.InstSubs GetOperationDocumentSubs() 
        {
            OperationDocument.InstSubs subs_od = new OperationDocument.InstSubs();
            foreach (InstSub s in this) subs_od.Add(new OperationDocument.InstSub(s));
            return subs_od;
        }

        public void AddSub(InstSub sub)
        {
            sub.Id = Count == 0 ? 1 : this.Max(s => s.Id) + 1;
            Add(sub);
        }

        public string GetIdsString()
        {
            return string.Join(',', this.Select(x => x.Id).ToArray());
        }
        public static InstSubs CreateSubs(OperationDocument.InstSubs subs_od,  ISyslogRepository? syslog)
        {
            InstSubs subs = new InstSubs();
            foreach (OperationDocument.InstSub sub_od in subs_od)
            {
                InstSub sub = new InstSub();
                sub.CopyFrom(sub_od);
                subs.Add(sub);
            }
            double b = 0;
            for (int i = subs.Count - 1; i >= 0; i--)
            {
                subs[i].Bottom = b;
                b += subs[i].Length;
            }
            return subs;
        }
    }
}
