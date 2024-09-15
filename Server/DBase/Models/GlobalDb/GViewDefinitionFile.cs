using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.DBase.Models.GlobalDb;



public class GViewDefinitionFileBase
{
    public int Id { get; set; }
 //   public int BId { get; set; }   //Body Id
    public string Name { get; set; }
    public string? Desc { get; set; }  //Description
    public string? Version { get; set; }
    public bool? Deleted { get; set; }

    public GViewDefinitionFileBase()
    {
        Id = -1;
    }

    public override string ToString()
    {
        return Name;
    }
}


public class GViewDefinitionFile : GViewDefinitionFileBase
{ 
    public string? Body { get; set; }
}