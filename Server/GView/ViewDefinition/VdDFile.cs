using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using OpenWLS.Server.LogDataFile;
using OpenWLS.Server.DBase;
using System.Collections;
using System.Diagnostics.Metrics;
using OpenWLS.Server.LogDataFile.Models.NMRecord;
using OpenWLS.Server.LogDataFile.Models;
using OpenLS.Base.UOM;

namespace OpenWLS.Server.GView.ViewDefinition
{
    public class VdDFile
    {
 //       public event EventHandler<EventArgs> FileInforChanged;
        public int Id { get; set; }
        public string Name { get; set; }
        public string Job { get; set; }

        public VdMeasurements Measurements { get; set; }
    //    public VdMeasurements MeasurementXds { get; set; }
        NMRecords nmrs;
        public NMRecords NMRecords
        {
            get { return nmrs;  }
            set {
                nmrs = value;
                NMRecord? rh = nmrs.Where(r => r.RType == (int)NMRecordType.FileVersion).FirstOrDefault();
                if(rh != null)
                {
                    FileVersion h = new FileVersion();
                    h.CopyFrom(rh);
                }

            }
        }

        public double StartTime { get; set; }   // in s
        public double StopTime { get; set; }        
        public double TopDepth { get; set; }   // in m
        public double BottomDepth { get; set; }

        DataFile? df;

        public VdDFile()
        {

        }

        public VdDFile(int id, string job, string name)
        {
            Id = id;
            Name = name;
            Job = job;
        }
        public override string ToString()
        {
            return Name;
        }

        public void Restore(DataRow dr)
        {
            Id = Convert.ToInt32(dr["Id"]);
            if (dr["Name"] != DBNull.Value)
                Name = (string)(dr["Name"]);
            if (dr["Job"] != DBNull.Value)
                Job = (string)(dr["Job"]);
     /*       
            if (dr.Table.Columns.Contains("StartTime"))
            {
                StartTime = dr["StartTime"] == DBNull.Value ? double.NaN : Convert.ToDouble(dr["StartTime"]);
                StopTime = dr["StopTime"] == DBNull.Value ? double.NaN : Convert.ToDouble(dr["StopTime"]);
            }
            if (dr.Table.Columns.Contains("TopDepth"))
            {
                TopDepth = dr["TopDepth"] == DBNull.Value ? double.NaN : Convert.ToDouble(dr["TopDepth"]);
                BottomDepth = dr["BottomDepth"] == DBNull.Value ? double.NaN : Convert.ToDouble(dr["BottomDepth"]);
            }
     */
        }
     

        public void Save(DataRow dr)
        {
            dr["Id"] = Id;
            dr["Name"] = Name;
            dr["Job"] = Job;
/*
            dr["StartTime"] = StartTime;
            dr["TopDepth"] = TopDepth;
            dr["StopTime"] = StopTime;
            dr["BottomDepth"] = BottomDepth;
 */      
        }

        /*
        public void SetIndexes(string str)
        {
            string[] strs = str.Split(new char[] { ',' });
            if (strs.Length == 5)
            {
                StartTime = Convert.ToDouble(strs[1]);
                StopTime = Convert.ToDouble(strs[2]);
                TopDepth = Convert.ToDouble(strs[3]);
                BottomDepth = Convert.ToDouble(strs[4]);
          //      if (FileInforChanged != null)
          //          FileInforChanged(this, new EventArgs());
            }
        }
        */

        public void LoadDataFile(ISyslogRepository sysLog)
        {
            df = DataFile.OpenDataFile($"{OpenWLS.Server.DBase.Models.LocalDb.Job.GetJobDirectory(Job)}\\{Name}.ldf", sysLog);
            if (df != null)
            {
                df.LoadFileInfor();
                TopDepth = double.MaxValue; BottomDepth = double.MinValue;
                StartTime = StopTime = 0;
                foreach (Frame f in df.Frames)
                {
                    foreach(LogDataFile.Index index in f.Indexes)
                    {
                        if(index.Type == LogIndexType.BOREHOLE_DEPTH)
                        {
                            double d = MeasurementUnit.GetDepthConvertMul("m", index.UOM);
                            double start = d * index.Start;
                            double stop = d * index.Stop;
                            double top = Math.Min(start, stop);
                            double bot = Math.Max(start, stop);
                            if(top < TopDepth) TopDepth = top;
                            if(bot > BottomDepth) BottomDepth = bot;
                        }
                        else
                        {
                            if (index.Type == LogIndexType.TIME)
                            {
                                double d = MeasurementUnit.GetTimeConvertMul("s", index.UOM);
                                double start = d * index.Start;
                                double stop = d * index.Stop;
                                if (start < StartTime) StartTime = start;
                                if (stop > StopTime) StopTime = stop;
                            }
                        }
                    }
                }
            }
        }

        
        public Measurement? GetMeasurement(string name)
        {
            if (df == null)
                return null;
            return GetMeasurement(null, name);
        }
     
        public Measurement? GetMeasurement(string? frame, string name)
        {
            if (df == null)
                return null;
            return df.Measurements.GetMeasurement(frame, name);
        }
        /*
        public Measurement GetMeasurement(int id)
        {
            if (df == null)
                return null;
            return df.Measurements.GetMeasurement(id);
        }*/


        public void CreateMeasurements(IEnumerable<Measurement> ms)
        {

            Measurements = new VdMeasurements();
            //MeasurementXds = new VdMeasurements();
            foreach (var m in ms)
            {
                VdMeasurement m1 = new VdMeasurement(m, this);
                Measurements.Add(m1);
            //    if (m1.Dims != null)
            //        MeasurementXds.Add(m1);
            }
        }
    }

    public class VdDFiles : List<VdDFile>
    {
        public event EventHandler<EventArgs> FilesInforChanged;

        public double StartTime { 
            get 
            {
                if (Count == 0) return double.NaN;
                return this.Min(f => f.StartTime);
            }
        }
        public double StopTime
        {
            get
            {
                if (Count == 0) return double.NaN;
                return this.Max(f => f.StopTime);
            }
        }
        public double TopDepth {
            get
            {
                if (Count == 0) return double.NaN;
                return this.Min(f => f.TopDepth);
            }
        }
        public double BottomDepth
        {
            get
            {
                if (Count == 0) return double.NaN;
                return this.Max(f => f.BottomDepth);
            }
        }
        public VdDFiles()
        {

        }

        public int GetNxtId()
        {
            if (Count == 0) return 1;
            else
                return this.Max(e => e.Id) + 1;
        }

        public bool Contains(string job, string dfn)
        {
   //         string str = dfn.ToPathString();
            foreach (VdDFile s in this)
            {
                if (s.Name == dfn && s.Job == job)
                    return true;
            }
            return false;
        }

        public void LoadDataFiles(ISyslogRepository sysLog)
        {
            foreach (VdDFile s in this)
                s.LoadDataFile(sysLog);
        }

        public void Restore(DataTable dt)
        {
            Clear();
            if (dt == null)
                return;
            foreach (DataRow dr in dt.Rows)
            {
                VdDFile s = new VdDFile();
                s.Restore(dr);
                if(s.Name != null)
                {
               //     s.FileInforChanged += df_FileInforChanged;
                    Add(s);
                }

            }
        }

        public void Save(DataTable dt)
        {
            dt.Rows.Clear();
            foreach (VdDFile s in this)
            {
                if (s.Name != null)
                {
                    DataRow dr = dt.NewRow();
                    s.Save(dr);
                    dt.Rows.Add(dr);
                }
            }
        }

        public VdDFile GetDfile(int? id)
        {
            if (id == null) return null;
            foreach (VdDFile s in this)
                if (s.Id == id)
                    return s;
            return null;
        }
        
        public VdDFile GetRtDfile()
        {
            foreach (VdDFile s in this)
                if (s.Name == "rt")
                    return s;
            return null;
        }
        public Measurement GetMeasurement(int? fid, string? frame, string name)
        {
            if (fid != null)
            {
                VdDFile f = GetDfile((int)fid);
                if(f != null)return f.GetMeasurement(frame, name);
            }
            return null;
        }

        public VdMeasurement GetPdMeasurement(int? fid, string? frame, string name)
        {
            if (fid != null)
            {
                VdDFile f = GetDfile((int)fid);
                if (f != null)
                {
                    Measurement? m = f.GetMeasurement(frame, name);
                    if (m != null)
                    {
                        return new VdMeasurement(m, f);
                    }
                }
            }
            return null;
        }
        /*   public Measurement GetMeasurement(int fid,  string name)
           {
               VdDFile f = GetDfile(fid);
               if (f != null)
                   return f.GetMeasurement(name);
               return null;
           }
   */



        /*
                void df_FileInforChanged(object sender, EventArgs e)
                {
                    SetIndexes();
                    if (FilesInforChanged != null)
                        FilesInforChanged(this, new EventArgs());           
                }
       

        public void SetMeasurements(string ar, IEnumerable<Measurement> ms)
        {
            foreach (VdDFile s in this)
            {
                if (s.Name == ar)
                    s.CreateMeasurements(ms);
            }            
        }

        public void SetIndexes(string str,   string indexes)
        {
            foreach (VdDFile s in this)
            {
                if (s.Name == str)
                    s.SetIndexes(indexes);
            }            
        }
 */
    }

}
