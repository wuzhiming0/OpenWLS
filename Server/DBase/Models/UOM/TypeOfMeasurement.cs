using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.DBase.Models.UOM
{
    public class TypeOfMeasurement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string InternalU { get; set; }
        public string SelectedU { get; set; }
        public string DefaultU { get; set; }
        public int? Flags { get; set; }
    }

    public class UnitOfMeasurement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public double Add { get; set; }
        public double Mul { get; set; }
        public int? Flags { get; set; }
    }
}
