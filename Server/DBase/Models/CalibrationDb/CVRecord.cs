using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Drawing;


using Newtonsoft.Json;


using System.Globalization;
using System.Data.SQLite;
using OpenWLS.Server.Base;
using OpenWLS.Server.DBase;
using OpenWLS.Server.Calibration;
using System.Collections.Immutable;

namespace OpenWLS.Server.DBase.Models.CalibrationDb
{

    /// <summary>
    /// Summary description for CVRecord.
    /// </summary>
    public  class CVRecord
    {
        public static int RecordSize = 174;
        public static List<CVRecord> Filter(List<CVRecord> rs, string cond )
        {
            string[] ss = cond.Split('=');
            if(ss.Length == 2)
            {
                if (ss[0] == "Id")
                    return rs.Where(a=> a.Id == Convert.ToInt32(ss[1])).ToList();
                if (ss[0] == "IId")
                    return rs.Where(a => a.IId == Convert.ToInt32(ss[1])).ToList();
                if (ss[0] == "DTime")
                    return rs.Where(a => a.DTime == Convert.ToInt64(ss[1])).ToList();
                if (ss[0] == "Serial")
                    return rs.Where(a => a.Serial == ss[1]).ToList();
                if (ss[0] == "Asset")
                    return rs.Where(a => a.Asset == ss[1]).ToList();
                if (ss[0] == "Phase")
                    return rs.Where(a => a.Phase == ss[1]).ToList();
                if (ss[0] == "Type")
                    return rs.Where(a => a.Type == ss[1]).ToList();
                if (ss[0] == "Unit")
                    return rs.Where(a => a.Unit == ss[1]).ToList();
                if (ss[0] == "Source")
                    return rs.Where(a => a.Source == ss[1]).ToList();
                if (ss[0] == "Calibrator")
                    return rs.Where(a => a.Calibrator == ss[1]).ToList();
                if (ss[0] == "Auxiliary")
                    return rs.Where(a => a.Auxiliary == ss[1]).ToList();             
            }
            return rs;
        }

        protected CVRecordValue recordValue;

        public int Id { get; set; }
        public int IId { get; set; }           //Instrument Id in Master Db
        public long DTime { get; set; }
        public string Serial { get; set; }
        public string Asset { get; set; }
        public string Phase { get; set; }
        public string Type { get; set; }
        public string? Unit { get; set; }
        public string? Source { get; set; }
        public string? Calibrator { get; set; }
        public string? Auxiliary { get; set; }
        public bool? Deleted { get; set; }

        public CVRecord()
        {

        }

        public void CopyFrom(CVRecord f)
        {
            Id = f.Id;
            IId = f.IId;
            DTime = f.DTime;
            Serial = f.Serial;
            Asset = f.Asset;
            Phase = f.Phase;
            Type = f.Type;
            Unit = f.Unit;
            Source = f.Source;
            Calibrator = f.Calibrator;
            Auxiliary = f.Auxiliary;
        }

        public string GetString()
        {
            List<string> ss = new List<string>()
            {
                $"Id={Id}", $"IId={IId}", $"DTime={DTime}", $"Serial={Serial}", $"Asset={Asset}", $"Phase={Phase}", $"Type={Type}"
            };
            if(Source != null) ss.Add($"Source={Source}");
            if(Calibrator != null) ss.Add($"Calibrator={Calibrator}");
            if(Auxiliary != null) ss.Add($"Auxiliary={Auxiliary}");
            return string.Join(',', ss.ToArray());
        }

        public bool SameAs(CVRecord r)
        {
            if (r.Asset == Asset &&
                //		(inforRecord.Serial == Serial) &&
                r.Type == Type &&
                r.Phase == Phase &&
                r.DTime == DTime)
                return true;
            else
                return false;
        }

        public bool SamePhaseAs(CVRecord r)
        {
            if (r.Asset == Asset &&
                //	(inforRecord.Serial == Serial) &&
                r.Type == Type &&
                r.Phase == Phase)
                return true;
            else
                return false;
        }

        public bool SameTypeAs(CVRecord r)
        {
            if (r.Asset == Asset &&
                //	(inforRecord.Serial == Serial) &&
                r.Type == Type)
                return true;
            else
                return false;
        }
        public string GetPhaseLongString()
        {
            if (Phase == "CP")
                return "Primary Calibration";
            if (Phase == "VP")
                return "Primary Verification";
            if (Phase == "VB")
                return "Before Verification";
            if (Phase == "VA")
                return "After Verification";
            return "";
        }

        public CVRecordValue GetRecordValue()
        {
            return recordValue;
        }
        public string GetDateTimeString()
        {
            return new DateTime(DTime).ToString("MM/dd/yyyy HH:mm:ss");
        }
    }


    



}
