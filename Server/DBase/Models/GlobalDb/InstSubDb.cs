using System.ComponentModel.DataAnnotations;

namespace OpenWLS.Server.DBase.Models.GlobalDb
{
    public class InstSubDb
    {
        [Key]
        public int DbId { get; set; }
        public string Model { get; set; }
        public string Category { get; set; }
        public string? Desc { get; set; }
        public double Length { get; set; }
        public double Weight { get; set; }
        public double? Diameter { get; set; }   //height if panel
        public bool? SEqu { get; set; }         //is surface equipment
        public bool? Aux { get; set; }          // not sub of instrument, like centralizer
        public bool? Main { get; set; }
        public bool? Deleted { get; set; }

        public void CopyFrom(InstSubDb s)
        {
            DbId = s.DbId;
            Model = s.Model;
            Category = s.Category;
            Desc = s.Desc;
            Length = s.Length;
            Weight = s.Weight;
            Diameter = s.Diameter;
            SEqu = s.SEqu; 
            Aux = s.Aux;
            Main = s.Main;
        }

    }
}
