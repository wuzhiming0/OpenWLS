using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.DBase.Models.LocalDb
{
    public class Run
    {
        public int Id { get; set; }
        public int OcfId { get; set; }
        public int JId { get; set; }  //Job Id
        public string? Name { get; set; }
        public bool? Deleted { get; set; }
    }
}

