using Microsoft.EntityFrameworkCore;
using OpenWLS.Server.LogInstance.OperationDocument;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.DBase.Models.GlobalDb;
public class OperationControlFileBase
{
    public int Id { get; set; }
//    public int BId { get; set; }        //
    public string Name { get; set; }        // concise
    public string? Subs { get; set; }
    public string? Desc { get; set; }  // Description
    public string? Version { get; set; }
    public bool? Deleted { get; set; }

}
public class OperationControlFile : OperationControlFileBase
{  

    public string? Body { get; set; }

    public OperationControlFile()
    {
        Id = -1;
    }
    public override string ToString()
    {

        return Name;
    }
}