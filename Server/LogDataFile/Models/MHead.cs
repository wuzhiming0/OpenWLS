using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OpenWLS.Server.Base;
using OpenWLS.Server.DBase;
using OpenWLS.Server.DBase.Models.GlobalDb;
using OpenWLS.Server.LogInstance.OperationDocument;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OpenWLS.Server.LogDataFile.Models;
public enum LogIndexType { NotIndex = -1, Unknown = 0, SAMPLE_NUMBER, TIME, DATE_TIME, BOREHOLE_DEPTH, VERTICAL_DEPTH, ANGULAR_DRIFT, RADIAL_DRIFT, USER_DEFINED };
public enum IndexUnit { unknown = -1, meter = 0, ft = 1, second = 2, date_time = 3 };
public enum LDFBinObjType { MVBlock, UlPackage, DlPackage, Calibration };
//[Flags]
//public enum MFlags { IndexChannel = 1, FrameChannel = 2, ESpacing = 4 };

/// <summary>
/// Measurement Head
/// </summary>

public class MHead1 : MeasurementDb
{
    public int Id { get; set; }
    public LogIndexType? IType { get; set; }
    public bool? IndexM { get; set; }            // index measurement
    public string? UOI { get; set; }   //unit of index
    public double? Spacing { get; set; }
    public string? IOffsets { get; set; }

    public long CTime { get; set; }    // create time

    public string? Frame { get; set; }

    public void Restore(DataRow dr)
    {
        Id = Convert.ToInt32(dr["Id"]);
        Name = (string)dr["Name"];
        if (dr["Desc"] != DBNull.Value) Desc = (string)dr["Desc"];
        if (dr["DataAxes"] != DBNull.Value) DataAxes = (string)dr["DataAxes"];
        DType = (SparkPlugDataType)Convert.ToInt32(dr["DType"]);
        if (dr["IType"] != DBNull.Value) IType = (LogIndexType)Convert.ToInt32(dr["IType"]);
        if (dr["IType"] != DBNull.Value) IndexM = true;
        if (dr["UOM"] != DBNull.Value) UOM = ((string)dr["UOM"]).Trim();
        if (dr["UOI"] != DBNull.Value) UOI = ((string)dr["UOI"]).Trim();
        if (dr["Frame"] != DBNull.Value) Frame = (string)dr["Frame"];
        if (dr["DFormat"] != DBNull.Value) DFormat = (string)dr["DFormat"];
        if (dr["IOffsets"] != DBNull.Value) IOffsets = (string)dr["IOffsets"];
        if (dr["Spacing"] != DBNull.Value) Spacing = Convert.ToDouble(dr["Spacing"]);
        if (dr["Deleted"] != DBNull.Value) Deleted = true;

    }
  
    public void AddToDB(SqliteDataBase db)
    {
        string sql = $"INSERT INTO MHead1s (Id, Name, CTime, Desc, DataAxes, DType, IType, IndexM, UOM, UOI, Frame, IOffsets, DFormat, Spacing ) VALUES ({Id}, '{Name}', {CTime}";
        if (Desc == null) sql = sql + ", NULL";
        else sql = sql + $", '{Desc}'";

        if (DataAxes == null) sql = sql + ", NULL";
        else sql = sql + $", '{DataAxes}'";

        sql = sql + $", {(int)DType}";
        if (IType == null) sql = sql + ", NULL";
        else sql = sql + $", {(int)IType}";

        if (IndexM == null) sql = sql + ", NULL";
        else sql = sql + $", {IndexM}";

        if (UOM == null) sql = sql + ", NULL";
        else sql = sql + $", '{UOM.Trim()}'";
        if (UOI == null) sql = sql + ", NULL";
        else sql = sql + $", '{UOI}'";

        if (Frame == null) sql = sql + ", NULL";
        else sql = sql + $", '{Frame.Trim()}'";

        if (IOffsets == null) sql = sql + ", NULL";
        else sql = sql + $", '{IOffsets}'";

        if (DFormat == null) sql = sql + ", NULL";
        else sql = sql + $", '{DFormat}'";

        if (Spacing == null || double.IsNaN((double)Spacing)) sql = sql + ", NULL )";
        else sql = sql + $", {Spacing} )";

        db.ExecuteNonQuery(sql);
        //   Id = (int)db.GetMaxID("MHead1s");
    }

    public void UpdateDB(SqliteDataBase db)
    {
        string sql = $"UPDATE MHead1s SET Name = '{Name}'";
        if (Desc == null) sql = sql + ", Desc = NULL";
        else sql = sql + $",Desc = '{Desc}'";

        if (DataAxes == null) sql = sql + ", Dims = NULL";
        else sql = sql + $", DataAxes = '{DataAxes}'";

        sql = sql + $",DType = {(int)DType}";
        if (IType == null) sql = sql + ", IType =  NULL";
        else sql = sql + $", IType = {(int)IType}";

        if (IndexM == null) sql = sql + ", IndexM =  NULL";
        else sql = sql + $", IndexM = {1}";

        if (UOM == null) sql = sql + ", UOM = NULL";
        else sql = sql + $", UOM = '{UOM.Trim()}'";
        if (UOI == null) sql = sql + ", UOI = NULL";
        else sql = sql + $", UOI = '{UOI.Trim()}'";

        if (Frame == null) sql = sql + ", Frame = NULL";
        else sql = sql + $", Frame = '{Frame.Trim()}'";

        if (IOffsets == null) sql = sql + ", IOffsets = NULL";
        else sql = sql + $", IOffsets = '{IOffsets}'";


        if (DFormat != null) sql = sql + ", DFormat = NULL";
        else sql = sql + $", DFormat = '{DFormat}'";

        if (Spacing == null || double.IsNaN((double)Spacing)) sql = sql + $", Spacing = NULL WHERE Id = {Id}";
        else sql = sql + $", Spacing = {Spacing} WHERE Id = {Id}";
        db.ExecuteNonQuery(sql);

    }
    public void DeleteFromDB(SqliteDataBase db)
    {
        string sql = $"UPDATE MHead1s SET Deleted = 1 WHERE Id = {Id}";
        db.ExecuteNonQuery(sql);
    }

    public void UnDeleteFromDB(SqliteDataBase db)
    {
        string sql = $"UPDATE MHead1s SET Deleted = NULL WHERE Id = {Id}";
        db.ExecuteNonQuery(sql);
    }

}

public class MHead2
{
    public int Id { get; set; }

  //  public double? StartIndex { get; set; }
  //  public double? StopIndex { get; set; }

    public double? VMin { get; set; }       //Value Min
    public double? VMax { get; set; }
    public double? VAvg { get; set; }
    public double? VFirst { get; set; }
    public double? VLast { get; set; }
    public double? VEmpty { get; set; }

    public double? IndexShift { get; set; } 
  //  public int Samples { get; set; }            //Last Block Id

    //   public string MVBlocks { get; set; }

    public void Restore(DataRow dr)
    {
        Id = Convert.ToInt32(dr["Id"]);
//        if (dr["StartIndex"] != DBNull.Value) StartIndex = Convert.ToDouble(dr["StartIndex"]);
//        if (dr["StopIndex"] != DBNull.Value) StopIndex = Convert.ToDouble(dr["StopIndex"]);

        if (dr["VMin"] != DBNull.Value) VMin = Convert.ToDouble(dr["VMin"]);
        if (dr["VMax"] != DBNull.Value) VMax = Convert.ToDouble(dr["VMax"]);
        if (dr["VAvg"] != DBNull.Value) VAvg = Convert.ToDouble(dr["VAvg"]);

        if (dr["VFirst"] != DBNull.Value) VFirst = Convert.ToDouble(dr["VFirst"]);
        if (dr["VLast"] != DBNull.Value) VLast = Convert.ToDouble(dr["VLast"]);
        if (dr["VEmpty"] != DBNull.Value) VEmpty = Convert.ToDouble(dr["VEmpty"]);


        if (dr["IndexShift"] != DBNull.Value) IndexShift = Convert.ToDouble(dr["IndexShift"]);
    //    Samples = Convert.ToInt32(dr["Samples"]);
        //    if (dr["Deleted"] != DBNull.Value) Deleted = true;

    }

    public void AddToDB(SqliteDataBase db)
    {
        
        string sql = $"INSERT INTO MHead2s ( Id, VMin, VMax,VAvg, VFirst, VLast, VEmpty, IndexShift ) VALUES ( {Id}";
    //    if (StartIndex == null || double.IsNaN((double)StartIndex)) sql = sql + ", NULL";
    //    else sql = sql + $", {StartIndex}";

     //   if (StopIndex == null || double.IsNaN((double)StopIndex)) sql = sql + ", NULL";
     //   else sql = sql + $", {StopIndex}";

        if (VMin == null || double.IsNaN((double)VMin)) sql = sql + ", NULL";
        else sql = sql + $", {VMin}";

        if (VMax == null || double.IsNaN((double)VMax)) sql = sql + ", NULL";
        else sql = sql + $", {VMax}";

        if (VAvg == null || double.IsNaN((double)VAvg)) sql = sql + ", NULL";
        else sql = sql + $", {VAvg}";

        if (VFirst == null || double.IsNaN((double)VFirst)) sql = sql + ", NULL";
        else sql = sql + $", {VFirst}";

        if (VLast == null || double.IsNaN((double)VLast)) sql = sql + ", NULL";
        else sql = sql + $", {VLast}";


        if (VEmpty == null || double.IsNaN((double)VEmpty)) sql = sql + ", NULL";
        else sql = sql + $", {VEmpty}";

        if (IndexShift == null || double.IsNaN((double)IndexShift)) sql = sql + ", NULL )";
        else sql = sql + $", {IndexShift} )";

        //    if (IndexShift == null) sql = sql + ", NULL";
        //    else sql = sql + $", {IndexShift}";

    //    sql = sql + $", {Samples} )";
        db.ExecuteNonQuery(sql);

    }

    public void UpdateDB(SqliteDataBase db)
    {
        string sql = "UPDATE MHead2s SET";
    //    if (StartIndex == null || double.IsNaN((double)StartIndex)) sql = sql + ", StartIndex = NULL";
    //    else sql = sql + $", StartIndex = {StartIndex}";

     //   if (StopIndex == null || double.IsNaN((double)StopIndex)) sql = sql + ", StopIndex = NULL";
     //   else sql = sql + $", StopIndex = {StopIndex}";

        if (VMin == null || double.IsNaN((double)VMin)) sql = sql + " VMin =  NULL";
        else sql = sql + $" VMin = {VMin}";

        if (VMax == null || double.IsNaN((double)VMax)) sql = sql + ", VMax =  NULL";
        else sql = sql + $", VMax = {VMax}";

        if (VAvg == null || double.IsNaN((double)VAvg)) sql = sql + ", VAvg =  NULL";
        else sql = sql + $", VMax = {VAvg}";

        if (VFirst == null || double.IsNaN((double)VFirst)) sql = sql + ", VFirst = NULL";
        else sql = sql + $", VFirst = {VFirst}";

        if (VLast == null || double.IsNaN((double)VLast)) sql = sql + ", VLast = NULL";
        else sql = sql + $", VLast = {VLast}";


        if (VEmpty == null || double.IsNaN((double)VEmpty)) sql = sql + ", VEmpty = NULL";
        else sql = sql + $", VEmpty = {VEmpty}";

        if (IndexShift == null || double.IsNaN((double)IndexShift)) sql = sql + ", IndexShift = NULL";
        else sql = sql + $", IndexShift = {IndexShift}";

        //      sql = sql + $", Samples = {Samples} WHERE Id = {Id}";
        sql = sql + $" WHERE Id = {Id}";

        db.ExecuteNonQuery(sql);
    }

}

public class MHead
{
    protected MHead1 h1;
    protected MHead2 h2;

    public int Id { get { return h1.Id; } set { h1.Id = value; } }
    public string Name { get { return h1.Name; } set { h1.Name = value; } }
    public string? Desc { get { return h1.Desc; } set { h1.Desc = value; } }           //Description
    public string? DataAxes { get { return h1.DataAxes; } set { h1.DataAxes = value; } }          
//    [JsonIgnore]
//    public DataAxes? DataAxes { get { return h1.DataAxes; } set { h1.DataAxes = value; } }            //Dimensions
    public SparkPlugDataType DType { get { return h1.DType == null? SparkPlugDataType.Float : (SparkPlugDataType)h1.DType; } set { h1.DType = value; } } //DataType

    public LogIndexType? IType { get { return h1.IType; } set { h1.IType = value; } }
    public bool? IndexM { get { return h1.IndexM; } set { h1.IndexM = value; } }            // index measurement
    public string? UOM { get { return h1.UOM; } set { h1.UOM = value; } }
    public string? UOI { get { return h1.UOI; } set { h1.UOI = value; } }   //unit of index
    public string? Frame { get { return h1.Frame; } set { h1.Frame = value; } }
    public string? IOffsets { get { return h1.IOffsets; } set { h1.IOffsets = value; } }
    public string? DFormat { get { return h1.DFormat; } set { h1.DFormat = value; } }
    public double? Spacing { get { return h1.Spacing; } set { h1.Spacing = value; } }
    public bool? Deleted { get { return h1.Deleted; } set { h1.Deleted = value; } }

    //public double? StartIndex { get { return h2.StartIndex; } set { h2.StartIndex = value; } }
    //public double? StopIndex { get { return h2.StopIndex; } set { h2.StopIndex = value; } }

    public double? VMin { get { return h2.VMin; } set { h2.VMin = value; } }       //Value Min
    public double? VMax { get { return h2.VMax; } set { h2.VMax = value; } }
    public double? VAvg { get { return h2.VAvg; } set { h2.VAvg = value; } }
    public double? VFirst { get { return h2.VFirst; } set { h2.VFirst = value; } }
    public double? VLast { get { return h2.VLast; } set { h2.VLast = value; } }
    public double? VEmpty { get { return h2.VEmpty; } set { h2.VEmpty = value; } }
    public double? IndexShift { get { return h2.IndexShift; } set { h2.IndexShift = value; } }
  
 //   public int Samples { get { return h2.Samples; } set { h2.Samples = value; } }

    [JsonIgnore]
    public int[] Dimensions
    {
        get
        {
            return Base.DataAxes.CreateDataAxes(DataAxes).GetDimensions();
        }
        set
        {

             DataAxes = Base.DataAxes.CreateDataAxes(value).ToString();
        }
    }
    [JsonIgnore]
    public int SampleElements
    {
        get
        {
            return Base.DataAxes.GetTotalElements(DataAxes);
        }
    }

    [JsonIgnore]
    public string LongName
    {
        get
        {
            if (string.IsNullOrEmpty(Frame))
                return Name;
            return Frame + "." + Name;
        }
    }
    /*
    [JsonIgnore]
    public double? Top
    {
        get
        {
            if (StartIndex == null || StopIndex == null) return null;
            return Math.Min((double)StartIndex, (double)StopIndex);
        }
    }

    [JsonIgnore]
    public double? Bot
    {
        get
        {
            if (StartIndex == null || StopIndex == null) return null;
            return Math.Max((double)StartIndex, (double)StopIndex);
        }
    }
    */
    [JsonIgnore]
    public bool NumberType
    {
        get { return DType <= SparkPlugDataType.Double; }
    }

    [JsonIgnore]
    public bool IndexMetric
    {
        get
        {
            string s = UOI == null ? UOM : UOI;
            if (s == null)
                return false;
            s = s.ToLower();
            return s == "m" || s == "meter";
        }
    }
    /*
    [JsonIgnore]
    public bool? IndexDecrease
    {
        get
        {
            if (Spacing == null)
            {
                if (IndexM == null) return null;
                return VFirst > VLast;
            }
            else
                return Spacing < 0;

        }
    }
    */

    public MHead()
    {
        h1 = new MHead1();
        h2 = new MHead2();
    }

    public MHead1 GetHead1()
    {
        return h1;
    }
    public int Get1DElements()
    {
        if (DataAxes == null) return 1;
        else
        {
            return Base.DataAxes.CreateDataAxes(DataAxes).GetDimensions()[0];            
        }
    }

    public int GetSampleBytes()
    {
        return  SampleElements * DataType.GetDataSize(DType);
    }
    public int GetElementBytes()
    {
        return DataType.GetDataSize(DType);
    }
    public string GetStdNumericFormat()
    {
        if (DFormat == null)
            return null;
        string s = DFormat.StartsWith("A") ? DFormat.Substring(1, DFormat.Length - 1) : DFormat;
        int k = s.IndexOf(';');
        if (k > 0)
            s = s.Substring(0, k);
        if (s.Length == 1)
            return null;
        if (s.StartsWith("I"))
        {
            s = s.Substring(1, s.Length - 1);
            string s1 = "0";
            int c = Convert.ToInt32(s1);
            for (int i = 1; i < c; i++)
                s1 = "#" + s1;
            return "{0," + s + ":" + s1 + "}";
        }
        if (s.StartsWith("F"))
        {
            s = s.Substring(1, s.Length - 1);
            k = s.IndexOf('.');
            int t = Convert.ToInt32(s.Substring(0, k));
            int d = Convert.ToInt32(s.Substring(k + 1, s.Length - k - 1));
            string s1 = "0";
            int c = Convert.ToInt32(s1);
            for (int i = 1; i < d; i++)
                s1 = s1 + "0";
            s1 = "0." + s1;
            for (int i = 1; i < t - d - 1; i++)
                s1 = "#" + s1;
            return "{0," + t.ToString() + ":" + s1 + "}";
        }
        if (s.StartsWith("E"))
        {
            return s.Substring(1, s.Length - 1);
        }

        return null;
    }



    public static void EraseDeletedMeasurements(SqliteDataBase db)
    {
        DataTable dt = db.GetDataTable("SELECT Id FROM MHead1s WHERE Deleted is not NULL");
        List<MHead> hs = new List<MHead>();
        foreach (DataRow dr in dt.Rows)
        {

            int id = Convert.ToInt32(dr["Id"]);
            //delete head2
            db.ExecuteNonQuery($"DELETE FROM MHead2s WHERE Id = {id}");
            MVBlocks.DeleteMVBlocks(db, id);
        }

        //delete all head1
        db.ExecuteNonQuery("DELETE FROM MHead1s WHERE Deleted is not NULL");
    }
    public void DeleteMeasurementFromDb(DataFile df) { ((MHead1)h1).DeleteFromDB(df); }
    public void AddHeadToDb(DataFile df) { ((MHead1)h1).AddToDB(df); h2.Id = h1.Id; ((MHead2)h2).AddToDB(df); }
    public void AddHead1ToDb(DataFile df) { ((MHead1)h1).AddToDB(df); }
    public void AddHead2ToDb(DataFile df) { ((MHead2)h2).AddToDB(df); }

    public void RestoreH1(MeasurementOd m)
    {
        h1 = new MHead1();
        h1.CopyFrom(m);
    }
    public void RestoreH1(DataRow dr)
    {
        h1 = new MHead1();
        h1.Restore(dr);
    }
    public void RestoreH2(DataRow dr)
    {
        h2 = new MHead2();
        h2.Restore(dr);
    }

    public MHead(int mid, SqliteDataBase db)
    {
        string sql = $"SELECT * FROM MHead1s WHERE Id = {mid}";
        DataTable dt = db.GetDataTable(sql);
        if (dt.Rows.Count == 1)
            RestoreH1(dt.Rows[0]);

        sql = "SELECT * FROM MHead2s WHERE Id = {mid}";
        dt = db.GetDataTable(sql);
        if (dt.Rows.Count == 1)
            RestoreH2(dt.Rows[0]);
    }

}

