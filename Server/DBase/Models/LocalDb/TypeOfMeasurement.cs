using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.DBase.Models.LocalDb
{
    public class TypeOfMeasurement
    {
        public string Name { get; set; }
        public string Internal { get; set; }
        public string Selected { get; set; }
        public string Default { get; set; }
        public string Units { get; set; }
    }

    public class UnitOfMeasurement
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public double Add { get; set; }
        public double Mul { get; set; }
    }
}
