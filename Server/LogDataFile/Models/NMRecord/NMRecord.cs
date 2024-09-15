using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Text.Json.Serialization;
using OpenWLS.Server.Base;
using OpenWLS.Server.DBase;

namespace OpenWLS.Server.LogDataFile.Models.NMRecord
{
    public enum NMRecordType
    {
        FileVersion = 1, WellInfor = 2, Syslog = 3, Para = 4, Calibration = 5, OpDoc = 6,
        DlPack = 0x10, UlPack = 0x11, CVRecord = 0x12,
        LasSection = 0x1000,
        XtfCurveHead = 0x2000
    };

    /// <summary>
    /// None Measurement Record
    /// </summary>
    public class NMRecord
    {
        public static string val_tbl_name = "NmObjs";
        public int Id { get; set; }
        public int RType { get; set; }      //Record Type
        public string? Name { get; set; }
        //       public int DataType { get; set; }
        public string? Ext { get; set; }  // extension
        public bool? Deleted { get; set; }
        [JsonIgnore]
        public object Val { get; set; }
        [JsonIgnore]
        public NMRecordType RecordType { get { return (NMRecordType)RType;  } }

        public static NMRecord CreateNMRecord(NMRecord nmr)
        {
            NMRecord nmr_new = null;
            switch ((NMRecordType)nmr.RType)
            {
                case NMRecordType.FileVersion:
                    nmr_new = new FileVersion();
                    break;
                case NMRecordType.CVRecord:
                    nmr_new = new CalibrationRecord();
                    break;
                case NMRecordType.Syslog:
                    nmr_new = new SyslogItem();
                    break;
                /*     case NMRecordType.WellInfor:
                         nmr = new ();
                         break;
                */
                default:
                    nmr_new = new NMRecord();
                    break;
            }
            if (nmr_new != null)
            {
                nmr_new.CopyFrom(nmr);
                nmr_new.RestoreExt();
                return nmr_new;
            }
            else
                return nmr;
        }

        public NMRecord()
        {

        }
        public NMRecord(DataRow dr)
        {
            Restore(dr);
        }

        public void CopyFrom(NMRecord nmr)
        {
            Id = nmr.Id;
            RType = nmr.RType;
            //         DataType = nmr.DataType;
            Name = nmr.Name;
            Ext = nmr.Ext;
            RestoreExt();
        }
        public void Restore(DataRow dr)
        {
            Id = Convert.ToInt32(dr["Id"]);
            RType = Convert.ToInt32(dr["RType"]);
            //         DataType = Convert.ToInt32(dr["DataType"]);
            if (dr["Name"] != DBNull.Value)
                Name = (string)dr["Name"];
            if (dr["Ext"] != DBNull.Value)
                Ext = ((string)dr["Ext"]);
             //   Ext = ((string)dr["Ext"]).Replace("''", "'");


        }
        public virtual void SaveExtension()
        {

        }
        public virtual void RestoreExt() { }

        byte[] GetValBytes()
        {
            if (Val is byte[])
            {
                //  DataType = (int)SparkPlugDataType.Bytes;
                return (byte[])Val;
            }
            if (Val is string)
            {
                //   DataType = (int)SparkPlugDataType.String;
                return Encoding.UTF8.GetBytes((string)Val);
            }
            //  return BitConverter.GetBytes(Val);
            return null;
        }

        public void AddNMRecordToDb(DataFile df)
        {
            AddNMRecordToDb(df, null);
        }

        public void AddNMRecordToDb(DataFile df, bool? zip)
        {
            SaveExtension();
            Id = (int)df.GetMaxID("NMRecords") + 1;
            string sql = $"INSERT INTO NMRecords ( Id, RType, Name, Ext ) VALUES ({Id}, {RType}";
            if (Name == null)
                sql = sql + ", NULL";
            else
                sql = sql + $", '{Name}'";

            if (Ext == null)
                sql = sql + ", NULL)";
            else
                sql = sql + $", '{Ext.Replace("'", "''")}')";

            df.ExecuteNonQuery(sql);
            byte[] bs = GetValBytes();
            if (bs != null)
            {
                BinObject bo = new BinObject()
                {
                    Id = Id,
                    Val = bs,
                    Zip = zip
                };
                bo.AddToDB(df, val_tbl_name);
            }


        }

        public void UpdateVal(DataFile df, object val)
        {
            UpdateVal(df, val, null);
        }
        public void UpdateVal(DataFile df, object val, bool? zip)
        {
            Val = val;
            byte[] bs = GetValBytes();
            BinObject bo = new BinObject()
            {
                Id = Id,
                Val = bs,
                Zip = zip
            };
            bo.UpdateDB(df, "");
        }
    }

    public class NMRecords : List<NMRecord>
    {
        public static NMRecords GetNMRecords(SqliteDataBase db)
        {
            NMRecords rs = new NMRecords();
            string sql = $"SELECT * FROM NMRecords";
            DataTable dt = db.GetDataTable(sql);
            foreach (DataRow dr in dt.Rows)
            {
                rs.Add(new NMRecord(dr));
            }
            return rs;
        }

    }
}