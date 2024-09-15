using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json.Serialization;

using OpenWLS.Server.Base;
using OpenWLS.Server.LogDataFile;
using System.Xml.Linq;
using OpenWLS.Server.LogDataFile.Models;
using OpenWLS.Server.LogInstance.OperationDocument;

namespace OpenWLS.Server.GView.ViewDefinition
{
    public enum PdMType { One = 0, N = 1};

    public class VdMeasurement
    {
        public string Name { get; set; }
        public int? FileID { get; set; }
        public string? Frame { get; set; }
        public string? UOM { get; set; }
        public string? Dims { get; set; }
 //       public string FileName { get; set; }
        public int Element { get; set; }


        VdDFile? df;
        public bool NameDup { get; set; }
        [JsonIgnore]
        public VdDFile? DFile
        {
            get { return df; }
            set
            {
                df = value;
                if (df != null)
                    FileID = df.Id;
            }
        }

        [JsonIgnore]
        public PdMType MType { 
            get { 
                if (Dims == null) return PdMType.One;
                return PdMType.N;
            } 
        }

        public VdMeasurement()
        {
            Name = "";
        }

        public VdMeasurement(Measurement m, VdDFile d_file)
        {
            DFile = d_file;
            Name = m.Head.Name; Frame = m.Head.Frame;
            UOM = m.Head.UOM;
            if(m.Head.DataAxes != null)
                Dims = string.Join(',',  DataAxes.CreateDataAxes(m.Head.DataAxes).GetDimensions());
        }
        public VdMeasurement(MeasurementOd m_od, VdDFile d_file)
        {
            DFile = d_file;
            Name = m_od.Name; 
            Frame = $"A({m_od.AcqId})";
            UOM = m_od.UOM;
            if (m_od.DataAxes != null)
                Dims = string.Join(',', DataAxes.CreateDataAxes(m_od.DataAxes).GetDimensions());
        }

        public void Restore(DataRow dr)
        {
            Name = dr["M_Name"] == DBNull.Value ? "" : (string)dr["M_Name"];
            if (dr.Table.Columns.Contains("M_FileID") && dr["M_FileID"] != DBNull.Value)
                FileID = Convert.ToInt32(dr["M_FileID"]);
            if( dr.Table.Columns.Contains("M_Frame") && dr["M_Frame"] != DBNull.Value ) 
                Frame = (string)dr["M_Frame"];
        }

        public void Save(DataRow dr)
        {
            dr["M_FileID"] = df != null ? df.Id : FileID == null ? DBNull.Value : FileID;
            dr["M_Name"] = Name;
            if(Frame != null)
                dr["M_Frame"] = Frame;
        }

        public override string ToString()
        {
            if (Frame == null || (!NameDup)) return Name;
            return $"{Name}({Frame})";
        }
    }
    public class VdMeasurements : List<VdMeasurement>
    {
        public VdMeasurements()
        {

        }
        public VdMeasurements(IEnumerable<Measurement> ms, VdDFile d_file)
        {
            foreach (Measurement m in ms)
                Add( new VdMeasurement(m, d_file));
            CheckNames();
        }
        public VdMeasurements(MeasurementOds ms, VdDFile d_file)
        {
            foreach (MeasurementOd m in ms)
                Add( new VdMeasurement(m, d_file) );
            CheckNames();
        }

        void CheckNames()
        {
            foreach(VdMeasurement m in this)
            {
                List<VdMeasurement> ms = this.Where(a=> a.Name == m.Name).ToList();
                m.NameDup = ms.Count > 1;
            }
        }

  /*      int GetNextID()
        {
            int k = 0;
            foreach(PdMeasurement c in this)
            {
                if (c.ID >= k)
                    k = c.ID;
            }
            k++;
            return k;
        }
        public void SetIDs()
        {
            foreach (PdMeasurement c in this)
            {
                if (c.ID < 0)
                    c.ID = GetNextID();
            }
        }*/
    }

}
