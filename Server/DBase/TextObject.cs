using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.DBase;
//public enum TxtObjType { OCF = 1, VDF = 2 };
public class TextObject
{
    public int Id { get; set; }
    public string? Val { get; set; }

 /*   public void AddToDB(SqliteDataBase db)
    {
        string sql = "INSERT INTO TxtObjs  Val ) VALUES ( '{Val}' )";
        db.ExecuteNonQuery(sql);
        Id = (int)db.GetMaxID("TxtObjs");
    }

    public void AddToDB(DbContext db)
    {
        SqliteDataBase sql = db.Database.Connection;
        string sql = $"INSERT INTO Id, TxtObjs  Val ) VALUES ( {Id}, '{Val}' )";
        db.ExecuteNonQuery(sql);
    }
 */

    public static MemoryStream ZipFile(string fn)
    {
        if (!File.Exists(fn))
            return null;
        using (FileStream csvStream = File.Open(fn, FileMode.Open, FileAccess.Read))
        {
            MemoryStream zipToCreate = new MemoryStream();
            using (ZipArchive archive = new ZipArchive(zipToCreate, ZipArchiveMode.Create, true))
            {
                ZipArchiveEntry fileEntry = archive.CreateEntry(Path.GetFileName(fn));
                using (var entryStream = fileEntry.Open())
                {
                    csvStream.CopyTo(entryStream);
                }
            }
            return zipToCreate;
            // return zipToCreate.ToArray();
        }
    }

}
    public class TextObjectVdf : TextObject
    {

    }
    public class TextObjectOcf : TextObject
    {

    }