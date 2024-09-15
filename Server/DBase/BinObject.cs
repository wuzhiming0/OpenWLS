using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO.Compression;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace OpenWLS.Server.DBase;

public class BinObject
{
    public int Id { get; set; }
    public bool? Zip { get; set; }
    public byte[] Val { get; set; }

    public void AddToDB(SqliteDataBase db, string tbl_name)
    {
        string sql = $"INSERT INTO {tbl_name} ( Id, Zip, Val ) VALUES ( {Id}, ";
        if(Zip == null) sql = sql + "NULL, @val )";
        else sql = sql + "1, @val )";
        SQLiteParameter para = new SQLiteParameter("@val", System.Data.DbType.Binary);
        para.Value = Zip == null? Val : ZipByteArray(Val) ;
        db.ExecuteNonQuery(sql, para );
        //Id = (int)db.GetMaxID("BinObjs");
    }

    public void UpdateDB(SqliteDataBase db, string tbl_name)
    {
        string sql = $"UPDATE {tbl_name} SET Val = @val";
        if(Zip == null ) sql = sql + $", Zip = NULL WHERE Id = {Id}";
        else sql = sql + $", Zip = 1 WHERE Id = {Id}";
        SQLiteParameter para = new SQLiteParameter("@val", System.Data.DbType.Binary);
        para.Value = Zip == null ? Val : ZipByteArray(Val);
        db.ExecuteNonQuery(sql);
    }
    public static MemoryStream GetStream(SQLiteDataReader reader)
    {
        const int CHUNK_SIZE = 2 * 1024;
        byte[] buffer = new byte[CHUNK_SIZE];
        long bytesRead;
        long fieldOffset = 0;
        MemoryStream stream = new MemoryStream();
        try
        {
            while ((bytesRead = reader.GetBytes(0, fieldOffset, buffer, 0, buffer.Length)) > 0)
            {
                stream.Write(buffer, 0, (int)bytesRead);
                fieldOffset += bytesRead;
            }
            //   return stream.ToArray();
            return stream;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public static byte[] ZipByteArray(byte[] bs )
    {
        using (var compressedFileStream = new MemoryStream())
        {
            //Create an archive and store the stream in memory.
            using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, false))
            {
               //Create a zip entry for each attachment
               var zipEntry = zipArchive.CreateEntry("");
                //Get the stream of the attachment
                using (var originalFileStream = new MemoryStream(bs))
                {
                    using (var zipEntryStream = zipEntry.Open())
                    {
                        //Copy the attachment stream to the zip entry stream
                        originalFileStream.CopyTo(zipEntryStream);
                    }
                }
            }
            return compressedFileStream.ToArray();
        }
    }

    public static MemoryStream Unzip(Stream fs)
    {
        using (var archive = new ZipArchive(fs))
        {
            var entry = archive.Entries.FirstOrDefault();

            if (entry != null)
            {
                using (var unzippedEntryStream = entry.Open())
                {
                    var ms = new MemoryStream();
                    unzippedEntryStream.CopyTo(ms);
                    var unzippedArray = ms.ToArray();
                    return ms;
                }
            }
            return null;
        }
    }
    public static byte[] GetVal(SqliteDataBase db, int id, string tbl_name)
    {
        string sql = $"SELECT Zip, Val FROM {tbl_name} WHERE Id = {id}";
        SQLiteDataReader r = db.GetSQLiteDataReader(sql);
        while (r.Read())
        {
            var v = r["Val"];
            if (v != DBNull.Value)
            {
                if (r["Zip"] == DBNull.Value)
                    return (byte[])v;
                else
                    return Unzip(new MemoryStream((byte[])v)).ToArray();
            }
        }
        return null;
    }
  /*  public static byte[] GetVal(SqliteDataBase db, int id, bool zip)
    {
     
        using (SQLiteDataReader reader = db.GetSQLiteDataReader(sql))
        {
            if (reader != null)
            {
                if (reader.Read())
                {
                    using (MemoryStream ms = GetStream(reader))
                    {
                        if (ms != null)
                        {
                            ms.Seek(0, SeekOrigin.Begin);
                            using (MemoryStream mss = Unzip(ms))
                            {
                                mss.Seek(0, SeekOrigin.Begin);
                                if (zip)
                                    return Unzip(ms).ToArray();
                                return mss.GetBuffer();
                            }
                        }
                    }
                }
            }          
        } 
        return null;
    }*/
}
