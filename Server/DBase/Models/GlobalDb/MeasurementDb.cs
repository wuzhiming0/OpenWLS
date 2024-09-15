using OpenWLS.Server.Base;
using OpenWLS.Server.LogDataFile.Models;
using System.ComponentModel.DataAnnotations;

namespace OpenWLS.Server.DBase.Models.GlobalDb
{
    /// <summary>
    /// Can't use schemaId "$Measurement" for type "$OpenWLS.Server.DBase.Models.GlobalDb.Measurement". 
    /// The same schemaId is already used for type "$OpenWLS.Server.LogDataFile.Models.Measurement"
    /// </summary>
    public class MeasurementDb  
    {
        [Key]
        public int DbId { get; set; }
   //     public long CTime { get; set; }    // create time
        public string Name { get; set; }
        public string? Desc { get; set; }           //Description

        // [JsonIgnore]
        // public DataAxes? DataAxes { get; set; } 
        public string? DataAxes { get; set; }            //Dimensions
 
        public string? UOM { get; set; }
        public string? TOM { get; set; }

        //      public string? Frame { get; set; }
        public string? DFormat { get; set; }
       
        public SparkPlugDataType? DType { get; set; }     //DataType
        public bool? Deleted { get; set; }

        public void CopyFrom( MeasurementDb m)
        {
            DbId = m.DbId;
            Name = m.Name;
            Desc = m.Desc;
            DataAxes = m.DataAxes;
            DType = m.DType;
            UOM = m.UOM;
            TOM = m.TOM;
            DFormat = m.DFormat;
            //Spacing = m.Spacing; 
            //IType = m.IType;
            //IndexM = m.IndexM;          
            //UOI = m.UOI;  
        }
    }
}