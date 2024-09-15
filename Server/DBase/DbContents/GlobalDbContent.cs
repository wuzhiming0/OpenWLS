using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWLS.Server.DBase.Models.GlobalDb;
//using OpenWLS.Server.LogDataFile.Models;

namespace OpenWLS.Server.DBase.DbContents;

public class GlobalDbContent : DbContext
{

    public GlobalDbContent(DbContextOptions<GlobalDbContent> options) : base(options)
    {
       // Database.EnsureCreated();
    }
    public DbSet<InstrumentDb> Insts { get; init; }
    public DbSet<MeasurementDb> Measurements { get; init; }
    public DbSet<MGroup> MGroups { get; init; }
    public DbSet<InstSubDb> Subs { get; init; }
    public DbSet<BinObject> SubImages { get; init; }    
    public DbSet<OperationControlFileBase> OCFs { get; init; }
    public DbSet<GViewDefinitionFileBase> VDFs { get; init; }
    public DbSet<TextObjectVdf> VdfObjs { get; init; }
    public DbSet<TextObjectOcf> OcfObjs { get; init; }


}
