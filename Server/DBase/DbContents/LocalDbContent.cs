using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWLS.Server.DBase.Models.LocalDb;
using OpenWLS.Server.DBase.Models.GlobalDb;

namespace OpenWLS.Server.DBase.DbContents;

public class LocalDbContent : DbContext
{


    public LocalDbContent(DbContextOptions<LocalDbContent> options) : base(options)
    {
        //Database.EnsureCreated();
    }
    public DbSet<OperationControlFileBase> OCFs { get; init; }
    public DbSet<GViewDefinitionFileBase> VDFs { get; init; }
    public DbSet<Job> Jobs { get; init; }
    public DbSet<Run> Runs { get; init; }
    public DbSet<Pass> Passes { get; init; }
    public DbSet<Edge> Edges { get; init; }
    public DbSet<TextObjectVdf> VdfObjs { get; init; }  // two DbSets with same TEntry is not allowed in one DbContext 
    public DbSet<TextObjectOcf> OcfObjs { get; init; }

    public DbSet<BinObject> BinObjs { get; init; }


}

