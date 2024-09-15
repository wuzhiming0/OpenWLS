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

public class SyslogDbContent : DbContext
{
    public SyslogDbContent(DbContextOptions<SyslogDbContent> options) : base(options)
    {

    }
    public DbSet<SyslogItem> Items { get; init; }

}

