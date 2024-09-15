using System.Data.Common;
using System.Data.SQLite;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OpenWLS.Server.LogDataFile.Models;
using OpenWLS.Server.DBase;
using OpenWLS.Server.LogDataFile.Models.NMRecord;

namespace OpenWLS.Server.LogDataFile;

public class LogDataFileDbContent : DbContext
{
    public LogDataFileDbContent(DbContextOptions<LogDataFileDbContent> options) : base(options)
    {

    }
    /*    public LogDataFileDbContent(string connectionString)
        : base(new SQLiteConnection() { ConnectionString = connectionString }, true)
        {
        }
    */
    public DbSet<MHead1> MHead1s { get; init; }
    public DbSet<MHead2> MHead2s { get; init; }
    public DbSet<MVBlock> MVBlocks { get; init; }
    public DbSet<CalibrationRecord> Cals { get; init; }  //Calibrations 
    public DbSet<NMRecord> NMRecords { get; init; }
    public DbSet<BinObject> MvObjs { get; set; }
    public DbSet<BinObject> NmObjs { get; set; }
}
