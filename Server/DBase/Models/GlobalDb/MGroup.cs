using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.Xml;
using System.Xml.Linq;

namespace OpenWLS.Server.DBase.Models.GlobalDb
{
    public class MGroup
    {
        [Key]
        public int DbId { get; set; }        
        public int Id { get; set; }
        public int IDbId { get; set; }            //
        public int DataSize { get; set; }
        public uint? IntervalTime { get; set; }  // milli second
        public double? IntervalDepth { get; set; }
        public string Ms { get; set; }  // mearsurements
                                        // MPoint=xx,SubDbId=xx,TOM=xx,NuDisp=1,Record=1
        public string? Name { get; set; }
        public string? Desc { get; set; }
        public bool? Enable { get; set; }
        public bool? LocalAE { get; set; }
        public bool? Deleted { get; set; }

        public void CopyFrom(MGroup g)
        {
            Id = g.Id;
            IDbId = g.IDbId;
            DataSize = g.DataSize;
            Ms = g.Ms;
            Name = g.Name;
            Desc = g.Desc;
    //        TimeInterval = g.TimeInterval;
    //        DepthInterval = g.DepthInterval;
            Enable = g.Enable;
            LocalAE = g.LocalAE;
        }
    }

    public class MeasurmentParas
    {
        public int? Id { get; set; }
        public int? DbId { get; set; }
        public string? Name { get; set; }
        public double? MPoint { get; set; }
        public int? SubId { get; set; }
        public string? SubModel { get; set; }
        public string? TOM { get; set; }
        public bool? NuDisp { get; set; }
        public bool? Record { get; set; }


        public MeasurmentParas()
        {

        }
        
        public MeasurmentParas(string str)
        {
            string[] ss = str.Split(',');
            foreach(string s in ss) SetPara(s);
        }
        public void SetPara(string str)
        {
            string[] ss1 = str.Split('=');
            if (ss1.Length == 2)
            {
                string n = ss1[0].Trim();
                string v = ss1[1].Trim();
                if (n == "Id") { Id = Convert.ToInt32(v); return; }
                if (n == "DbId") { DbId = Convert.ToInt32(v); return; }
                if (n == "SubId") { SubId = Convert.ToInt32(v); return; }
                if (n == "Name") { Name = v; return; }
                if (n == "MPoint") { MPoint = Convert.ToDouble(v); return; }
                if (n == "TOM") { TOM = v; return; }
                if (n == "NuDisp") { NuDisp = Convert.ToBoolean(v); return; }
                if (n == "Record") { Record = Convert.ToBoolean(v); return; }
            }
        }

    }
}
