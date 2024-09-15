using System.ComponentModel.DataAnnotations;

namespace OpenWLS.Server.DBase.Models.GlobalDb
{
    public class InstrumentDb
    {
        [Key]
        public int DbId { get; set; }                // Id in Master Instrument DB
        public int? Address { get; set; }         // 
        public string Name { get; set; }
        public string Category { get; set; }
        public string SubDbIds { get; set; }    
        public string? Desc { get; set; }
        public string? GuiDll { get; set; }
        public string? ApDll { get; set; }    
        public bool? SurfaceEqu { get; set; }
      //  public bool? EdgeDevice { get; set; }
        public bool? Deleted{ get; set; }


        public void CopyFrom(InstrumentDb inst)
        {
            DbId = inst.DbId;
            Address = inst.Address;
            Name = inst.Name;
            Category = inst.Category;
            Desc = inst.Desc;
            SurfaceEqu = inst.SurfaceEqu;
     //       EdgeDevice = inst.EdgeDevice;
            GuiDll = inst.GuiDll;
            ApDll = inst.ApDll;
            SubDbIds = inst.SubDbIds;
        }

        public override string ToString()
        {
            return Name;
        }
    }

}
