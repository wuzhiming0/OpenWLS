using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.DBase.Models.LocalDb;

public class Pass
{
    public int Id { get; set; }
    public int JId { get; set; }  //Job Id
    public int OcfId { get; set; }

    //   public int RId { get; set; }        //Run Id
    public long TimeStart { get; set; }
    public long TimeStop { get; set; }
    public double DepthStart { get; set; }
    public double DepthStop { get; set; }
    public string? Note { get; set; }
    public bool? Deleted { get; set; }
}

public interface IPassRepository
{
    Task<List<Pass>> GetPassList();
    Task<Pass?> GetPass(int id);
    Task<Pass> UpdatePass(Pass r);
    Task<Pass> AddPass(Pass r);
    Task<Pass> DeletePass(int id);
}

