using Microsoft.AspNetCore.Mvc.TagHelpers;
using OpenWLS.Server.DBase.Models.GlobalDb;
using OpenWLS.Server.DBase.Repositories.InstrumentM;
using System.Data.Entity.Infrastructure;
using System.Text.Json.Serialization;

namespace OpenWLS.Server.LogInstance.OperationDocument
{
    public class MeasurementOd: MeasurementDb
    {
        public int Id { get; set; }
        [JsonIgnore]
        public int? IId { get; set; }
        public int? SubId { get; set; }
        public int? AcqId { get; set; }
        public string? SubModel { get; set; }
        public double MPointS { get; set; }          // relative, or to the bottom of the sub 
        public double MPoint { get; set; }          // absolute, or to bottom of the tool string 
        public bool? NuDisp { get; set; }            // numeric display
        public bool? Record { get; set; }

   
        public int GetTotalElements()
        {
            return Server.Base.DataAxes.GetTotalElements(DataAxes);
        }

    }

    public class MeasurementOds : List<MeasurementOd>
    {
        public int GetNxtId()
        {
            if (Count == 0) return 1;
            else
                return this.Max(a => a.Id) + 1;
        }

        public void RemoveMeasurementOfInst(int iid)
        {
            List<MeasurementOd> ms = this.Where(a=>a.IId == iid).ToList();
            foreach(MeasurementOd m in ms)  Remove(m); 
        }
        public void SetMPoint(InstSubs subs)
        {
            MeasurementOds dels = new MeasurementOds();
            InstSub emptySub = new InstSub();
            foreach (MeasurementOd m in this)
            {
                InstSub? s = m.SubId == null && m.SubModel == null? emptySub : subs.FirstOrDefault(a => a.Id == m.SubId);
                if (s == null)
                {
                    s = subs.FirstOrDefault(a => a.Model == m.SubModel);
                    if (s == null )
                        dels.Add(m);
                }
                if(s != null)
                    m.MPoint = m.MPointS + s.Bottom;
            }
            foreach (MeasurementOd m in dels)
                Remove(m);
        }
    }
}
