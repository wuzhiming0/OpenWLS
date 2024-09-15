using Newtonsoft.Json;
using OpenWLS.Server.Base;
using OpenWLS.Server.DBase;
using OpenWLS.Server.DBase.DbContents;
using System.Security.Cryptography;
using System.Text;

namespace OpenWLS.Server.LogInstance.OperationDocument
{
    public enum OCFileSource { OCFile = 0, DataFile = 1 };
    public class InstrumentGroup
    {
        public InstSubs Subs { get; set; }
        public InstrumentOds Insts { get; set; }


    //    public InstrumentDb InstrumentDb { get; set; }
        public List<ParameterValue> ParaVals { get; set; }

        //        public DataTable Subs { get; set; }
        public List<Attachment> Attachments { get; set; }
        public int MaxInstId
        {
            get { 
                if (Insts.Count == 0) return 0;
                else return Insts.Max(a => a.Id);
            }
        }
       

        public int MaxSubId
        {
            get { 
                if (Subs.Count == 0) return 0;
                else return Subs.Max(a => a.Id);
            }
        }
       
        public InstrumentGroup()
        {
            Insts = new InstrumentOds();
            Subs = new InstSubs();
            ParaVals = new();
            Attachments = new();

        }


        public string GetJsonString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }
        public void DeleteInst(int id )
        {
            InstrumentOd? inst = Insts.Where(a => a.Id == id).FirstOrDefault();
            if (inst != null)
                DeleteInst(inst);
        }
        public void DeleteInst(InstrumentOd inst)
        {
            Insts.Remove(inst);
            Subs.RemoveSubsOfInst(inst.Id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>deleted instrument</returns>
        public InstrumentOd? DeleteInstSub(int id)
        {            
            InstSub? s = Subs.FirstOrDefault(a => a.Id == id);
            if (s != null)
            {
                Subs.Remove(s);
                InstrumentOd? inst = Insts.Where(a => a.Id == s.IId).FirstOrDefault();
                if (inst != null)
                {
                    List<InstSub> ss = Subs.Where(a=>a.IId == s.IId).ToList();
                    if (ss.Count == 0)
                        DeleteInst(inst);
                    else
                        inst = null;                    
                }
                return inst;
            }
            return null;
            
        }

            /*
            void CreateInstrumentDb(GlobalDbContent globalDb)
            {
                InstrumentDb = new InstrumentDb();

                InstrumentDb.Instruments = Tools.GetDbInstruments(globalDb);
                InstrumentDb.Subs = Subs.GetDbInstruments(globalDb);
                InstrumentDb.MGroups = Tools.GetDbMGroups(globalDb);
                InstrumentDb.Measurements = Instruments.GetDbMeasurements(globalDb, InstrumentDb.MGroups);
            }
            */





        }
    public  class  OperationDocument {
        public int? EdId { get; set; }                      //edge id in local db
        public InstrumentGroup SfEquipment { get; set; }
        public InstrumentGroup DhTools { get; set; }
        public AcqItems ACT { get; set; }
        //       public MGroups MGroups { get; set; }
        public MeasurementOds Measurements { get; set; }
        public VdDocumentOds? VdDocs { get; set; }

        public OperationDocument() {
            SfEquipment = new InstrumentGroup();
            DhTools = new InstrumentGroup();
            ACT = new AcqItems();
            Measurements = new MeasurementOds();
        }

        public int GetNxtInstId()
        {
            return Math.Max(SfEquipment.MaxInstId, DhTools.MaxInstId) + 1;
        }
        public int GetNxtSubId()
        {
            return Math.Max(SfEquipment.MaxSubId, DhTools.MaxSubId) + 1;
        }
        /*
        public LogInstanceS CreateLogInstance(GlobalDbContent globalDb, ISyslogRepository syslog)
        {
            LogInstanceS li = new LogInstanceS();

            
            //api.Subs = Subs;
            //api.Tools = Server.LogInstance.Instrument.Instruments.CreateInstruments(Insts, syslog);
            //api.CreateMGroups(ACT, syslog);
            //api.CreateDisplays(VdDocs);
            return li;
        }*/

        public string GetJsonString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }
        public string GetInstsOnlyJsonString()
        {
            OperationDocument op_new = new OperationDocument()
            {
                DhTools = DhTools,
                SfEquipment = SfEquipment
            };
            return JsonConvert.SerializeObject(op_new, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }

    }

}
