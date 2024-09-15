using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using System.Text.Json.Serialization;
using OpenWLS.Server.LogDataFile;
using OpenWLS.Server.DBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWLS.Server.WebSocket;
using OpenWLS.Server.Base;
using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace OpenWLS.Server.LogDataFile.Models;

/// <summary>
/// Measurement Value Block
/// </summary>
public class MVBlock
{
    public static string val_tble_name = "MvObjs";
    public int Id { get; set; }
 //   public int MId { get; set; }

    public int Samples { get; set; }
    public int? Grp { get; set; }     //  blocks with same group number are continous 
    public double StartIndex { get; set; }  // frame measurement : sequence id;   none frame measurement : true index
    public double StopIndex { get; set; }

//    [JsonIgnore]
//    public int SamplesEnd { get; set; }
    [JsonIgnore]
    public byte[]? VBuffer { get; set; }

    public MVBlock()
    {
    }
    public MVBlock(DataRow dr)
    {
        Restore(dr);
    }
    public void Restore(DataRow dr)
    {
        Id = Convert.ToInt32(dr["Id"]);
        //       MId = Convert.ToInt32(dr["MId"]);
        Samples = Convert.ToInt32(dr["Samples"]);
        if (dr["Grp"] != DBNull.Value) Grp = Convert.ToInt32(dr["Grp"]);
        if (dr["StartIndex"] != DBNull.Value) StartIndex = Convert.ToDouble(dr["StartIndex"]);
        if (dr["StopIndex"] != DBNull.Value) StopIndex = Convert.ToDouble(dr["StopIndex"]);
    }

    public void AddToDB(SqliteDataBase db, SampleBuffer sb, int mid, int total_bytes)
    {
        byte[] bs = new byte[total_bytes];
        Buffer.BlockCopy(sb.Bytes, 0, bs, 0, total_bytes);
        Samples = total_bytes / sb.SampleBytes;
        AddToDB(db, bs, mid);
    }
    public void AddToDB(SqliteDataBase db, byte[] bs, int mid)
    {
        Id = (int)db.GetMaxID(val_tble_name) + 1;
        string sql = $"INSERT INTO MVBlocks ( Id, Mid, Samples, Grp, StartIndex, StopIndex ) VALUES ( {Id}, {mid}, {Samples}";
        if (Grp == null || double.IsNaN((double)Grp)) sql = sql + ", NULL";
        else sql = sql + $", {Grp}";

        if (StartIndex == null || double.IsNaN((double)StartIndex)) sql = sql + ", NULL";
        else sql = sql + $", {StartIndex}";
        if (StopIndex == null || double.IsNaN((double)StopIndex)) sql = sql + ", NULL)";
        else sql = sql + $", {StopIndex})";
        db.ExecuteNonQuery(sql);
        
        BinObject bo = new BinObject()
        {
            Val = bs,
            Id = Id,
        };
        bo.AddToDB(db, val_tble_name);
    }

    public void LoadValueBuffer(DataFile df)
    {
        if(VBuffer == null && df != null)
            VBuffer = BinObject.GetVal(df, Id, MVBlock.val_tble_name);
    }

    public void UpdateDB(SqliteDataBase db)
    {
        string sql = $"UPDATE MVBlocks SET Id = {Id}, Samples = {Samples}";

        if (Grp == null) sql = sql + ", Grp = NULL";
        else sql = sql + $", Grp = {Grp}";
        sql = sql + $", StartIndex = {StartIndex}, StopIndex = {StopIndex} WHERE Id = {Id}";
        db.ExecuteNonQuery(sql);
    }


}

public class MVBlocks : List<MVBlock>
{
    //    SqliteDataBase dataFile;   
 //   [JsonProperty]
    public int MId { get; set; }

    //   public int SampleBytes { get; set; }
 //   public byte[]? val_buffer;
 //   public byte[] ValBytes { get { return val_buffer; } set { val_buffer = value; } }

    public MVBlocks()
    {

    }
    public MVBlocks(SqliteDataBase db, int mid)
    {
        ReadMVBlocks(db, mid);
    }
    public MVBlocks (Measurement m)
    {
        ReadMVBlocks(m.DataFile, m.Id);
    }
 
    public void ReadMVBlocks(SqliteDataBase db, int mid)
    {
        MId = mid;
        if (db == null) return;
        string sql = $"SELECT * FROM MVBlocks WHERE MId = {mid}";
        DataTable dt = db.GetDataTable(sql);
        foreach (DataRow dr in dt.Rows)
            Add(new MVBlock(dr));
     //   this.OrderBy(mvb => mvb.StartIndex);
    }

    public  byte[] LoadAllValue(DataFile df)
    {
        int k = 0;
        foreach (MVBlock b in this)
        {
            b.LoadValueBuffer(df);
            k += b.VBuffer.Length;
        }
        byte[] bs = new byte[k];
        k = 0;
        foreach (MVBlock b in this)
        {
            Buffer.BlockCopy(b.VBuffer, 0, bs, k, b.VBuffer.Length);
            k += b.VBuffer.Length;
        }
        return bs;
    }


    public static void DeleteMVBlocks(SqliteDataBase db, int mid)
    {
        DataTable dt = db.GetDataTable($"SELECT Id FROM MVBlocks WHERE MId = {mid}");
        foreach (DataRow dr in dt.Rows)
        {
            int id = Convert.ToInt32(dr["Id"]);
            //delete all bin value rows
            db.ExecuteNonQuery($"DELETE FROM BinObjs WHERE Id = {id}");
        }
        //delete mvblock
        db.ExecuteNonQuery($"DELETE FROM MVBlocks WHERE MId = {mid}");
    }
    /*
    public void ReverseSamples(int sample_bytes)
    {
        Reverse();
        int samples = 0;

        if (val_buffer != null)
        {
            byte[] bs = new byte[sample_bytes];
            int total_bytes = val_buffer.Length;
            int top = 0;
            int bot = total_bytes - sample_bytes;
            while (top < bot)
            {
                Buffer.BlockCopy(val_buffer, top, bs, 0, sample_bytes);            //top -> bs
                Buffer.BlockCopy(val_buffer, bot, val_buffer, bot, sample_bytes);     // bot -> top    
                Buffer.BlockCopy(bs, 0, val_buffer, bot, sample_bytes);            // bs -> bot 
                top += sample_bytes;
                bot -= sample_bytes;
            }
        }
    }*/

    public void AddToDB(DataFile dataFile, MVBlock mVBlock)
    {

    }

    public void UpdateMValue(SqliteDataBase db, byte[] val)
    {

    }

    public int GetTotalSamples()
    {
        int s = 0;
        foreach (MVBlock b in this) s += b.Samples;
        return s;
    }

    public MVBlock? GetBlock(int id)
    {
        return this.Where( b => b.Id == id ).FirstOrDefault();
    }

}





