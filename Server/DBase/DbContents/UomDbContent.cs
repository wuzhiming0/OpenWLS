using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWLS.Server.DBase.Models.GlobalDb;

namespace OpenWLS.Server.DBase.DbContents;

public class UomDbContent : DbContext
{

    public UomDbContent(DbContextOptions<UomDbContent> options) : base(options)
    {
       // Database.EnsureCreated();
    }

    public DbSet<UnitOfMeasurement> UOMs { get; init; }
    public DbSet<TypeOfMeasurement> TOMs { get; init; }


}
