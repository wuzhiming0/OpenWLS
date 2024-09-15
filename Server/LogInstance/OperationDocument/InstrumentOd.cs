using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenLS.Base.UOM;
using OpenWLS.Server.LogInstance.Instrument;
using OpenWLS.Server.DBase.Models.GlobalDb;
using OpenWLS.Server.DBase.Repositories.InstrumentM;
using System.Diagnostics.Metrics;
//using MGroup = OpenWLS.Server.MGroup;

namespace OpenWLS.Server.LogInstance.OperationDocument
{
    public class InstrumentOd : InstrumentDb
    {
        public int Id { get; set; }           //operation document
        public int? Address { get; set; }
        //     public List<int> SubIds { get; set; }
        public int? Seq { get; set; } // for instrument with same name 

        [JsonIgnore]
        public InstSubs Subs { get; set; }

        [JsonIgnore]
        public string FullName
        {
            get
            {
                if (Seq == null) return Name;
                else return $"{Name}({Seq})";
            }
        }
        public InstrumentOd()
        {
   //         SubIds = new List<int>();
        }
    }

    public class InstrumentOds : List<InstrumentOd>
    {
        public List<InstrumentDb> GetDbInstruments( DBase.DbContents.GlobalDbContent globalDb )
        {
            List<InstrumentDb> insts_db = new List<InstrumentDb> ();
            foreach(InstrumentOd inst in this)
            {
                InstrumentDb? inst_db = globalDb.Insts.Where(a=>a.DbId == inst.DbId).FirstOrDefault();
                if(inst_db != null) insts_db.Add(inst_db);
            }
            return insts_db;
        }
        public List<MGroup> GetDbMGroups( DBase.DbContents.GlobalDbContent globalDb )
        {
            List<MGroup> mgs_db = new List<MGroup> ();
            foreach(InstrumentOd inst in this)
            {
                MGroup? mg_db = globalDb.MGroups.Where(a=>a.IDbId == inst.DbId).FirstOrDefault();
                if(mg_db != null) mgs_db.Add(mg_db);
            }
            return mgs_db;
        }
        public static List<MeasurementDb> GetDbMeasurements( DBase.DbContents.GlobalDbContent globalDb, List<MGroup> mgs_db)
        {
            List<MeasurementDb> ms_db = new List<MeasurementDb> ();
            foreach(MGroup mg in mgs_db)
            {
                string[] ss = mg.Ms.Split('\n');
                foreach (string s in ss)
                {
                    MeasurmentParas mps = new MeasurmentParas(s);
                    if (mps.DbId != null)
                    {
                        MeasurementDb? m_db = globalDb.Measurements.Where(a => a.DbId == mps.DbId).FirstOrDefault();
                        if (ms_db != null) ms_db.Add(m_db);
                    }
                    else
                    {
                        if (mps.Name != null)
                        {
                            MeasurementDb? m_db = globalDb.Measurements.Where(a => a.Name == mps.Name).FirstOrDefault();
                            if (ms_db != null) ms_db.Add(m_db);
                        }
                    }
                }
            }
            return ms_db;
        }


        public void Update( Server.LogInstance.Instrument.Instruments insts)
        {
            Clear();
            foreach(Server.LogInstance.Instrument.Instrument inst in insts)
            {
                InstrumentOd inst_od = new InstrumentOd()
                {
                    DbId = inst.DbId,
                    Id = inst.Id,
                    Address = inst.Address,
                    //  Assets = inst.
                };
                /*
                foreach(Server.LogInstance.Instrument.MGroup g in inst.MGroups)
                {
                    foreach (Server.LogInstance.Instrument.Measurement m in g.Measurements)
                    {

                    }
                }*/
            }
        }       

    }

    public class InstSub : InstSubDb
    {
        public int Id { get; set; }               //operation document
        public int? IId { get; set; }             //Instrument  
    //    public int DbId { get; set; }
        public string? Asset { get; set; }
        [JsonIgnore]
        public InstrumentOd? Instrument { get; set; }
        [JsonIgnore]
        public string? InstName
        {
            get
            {
                if (Instrument == null) return null;
                return Instrument.FullName;
            }
        }
        [JsonIgnore]
        public int? Address
        {
            get
            {
                if (Instrument == null) return null;
                return Instrument.Address;
            }
        }
        public double Bottom { get; set; }
        public InstSub()
        {

        }
  
    }

  
    public class InstSubs : List<InstSub>
    {
        public InstSubs() { }
        public InstSubs(List<InstSub> ss) {
            foreach (InstSub s in ss) Add(s);
        }
        public void CalcMPoints()
        {
            double mp = 0;
            foreach (InstSub sub in this)
            {
                sub.Bottom = mp;
                mp += sub.Length;
            }
        }
        public void RemoveSubsOfInst(int iid)
        {
            List<InstSub> ss = this.Where(a=>a.IId == iid).ToList();
            foreach (InstSub s in ss)
                Remove(s);
        }
    }

    public class AcqItem 
    {
        public int Id { get; set; }
        public int IId { get; set; }            // Tool Id
        public int MgId { get; set; }             // Measurement Group
        public int DataSize { get; set; }       

//        public int? AddressAE { get; set; }      //AE can be in any instrument 
//        public double? SamplesPS { get; set; }        // Per Second 
//        public double? SamplesPM { get; set; }         // Per Meter
        public uint? IntervalTime { get; set; }       // in milli-second
        public double? IntervalDepth { get; set; }      //
        public bool? Enable { get; set; }
        public int? CommSpeed { get; set; }
        public int? SRORatio { get; set; }

        public string? Desc { get; set; }
        public int[] MIds { get; set; }         // 
        [JsonIgnore]
        public int? Address { get; set; }         // 
        [JsonIgnore]
        public string InstName { get; set; }
        [JsonIgnore]
        public MeasurementOds Measurements { get; set; }
        [JsonIgnore]
        public string MNames { get { return string.Join(',', Measurements.Select(a => a.Name).ToArray());  } }

        public AcqItem()
        {

        }

        public AcqItem(MGroup g)
        {
            MgId = g.Id;
            IntervalTime = g.IntervalTime;
            IntervalDepth = g.IntervalDepth;
            DataSize = g.DataSize;
            Enable = g.Enable != null;
            Desc = g.Desc;            
        }
        public void CloneFrom(AcqItem a)
        {
            MgId = a.MgId;
            IntervalTime = a.IntervalTime;
            IntervalDepth = a.IntervalDepth;
            DataSize = a.DataSize;
            Enable = a.Enable != null;
            Desc = a.Desc;   
            Address = a.Address;
            IId = a.IId;
        }
    }

    public class AcqItems : List<AcqItem>
    {
        public void RemoveAcqItemsOfInst(int iid)
        {
            List<AcqItem> ais = this.Where(a=>a.IId == iid).ToList();
            foreach (AcqItem ai in ais)
                Remove(ai);
        }
    }

    public class ParameterValue
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class Attachment
    {
        public string Name { get; set; }
        public string Position { get; set; }
    }



    public class VdDocumentOd
    {
        public int Id { get; set; }
        public string Name { get; set; }
    //    public string FileName { get; set; }
        public string Content { get; set; }
    }

    public class VdDocumentOds : List<VdDocumentOd>
    {
        public VdDocumentOds()
        {

        }

        public VdDocumentOds(List<VdDocumentOd> vds)
        {
            int k = 1;
            foreach (VdDocumentOd vd in vds)
            {
                vd.Id = k++;
                Add(vd);
            }
                
        }
    }


}

