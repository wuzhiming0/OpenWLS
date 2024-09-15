using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWLS.Server.DBase.Models.GlobalDb;
using OpenWLS.Server.DBase.Models.CalibrationDb;

namespace OpenWLS.Server.DBase.DbContents;

public class CalibrationDbContent : DbContext
{

    public CalibrationDbContent(DbContextOptions<CalibrationDbContent> options) : base(options)
    {
       // Database.EnsureCreated();
    }

    public DbSet<CVRecord> CVRecords { get; init; }
    public DbSet<BinObject> BinObjs { get; init; }

}
