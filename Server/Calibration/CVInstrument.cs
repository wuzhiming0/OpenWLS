using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using System.Reflection;

using Newtonsoft.Json;
using OpenWLS.Server.DBase;

namespace OpenWLS.Server.LogInstance.Calibration
{
    /*
    public class CVInstruments : List<CVInstrument>
    {
        public CVSensors GetSensors()
        {
            CVSensors ss = new CVSensors();
            foreach (CVInstrument inst in this)
            {
                foreach()
            }
            
        } 

        public byte[] SaveDataRecords( bool littleEndian)
        {
            int l = 0;
            foreach (CVInstrument inst in this)
                l += inst.GetDataRecordLength();
            DataWriter w = new DataWriter(littleEndian, l);
            foreach (CVInstrument inst in this)
                inst.SaveCVInstrument(w);
            return w.GetBuffer();
        }

        public void LoadDataRecord(DataReader r)
        {

        }

    }
    */
	/// <summary>
	/// Summary description for CVInstrument.
	/// </summary>
	public class CVInstrument
	{
        Instrument.Instrument inst;

 //       [JsonIgnore]
        public string SerialNu { get; set; }
        public Instrument.Instrument Inst
        {
            get
            {
                return inst;

            }
            set
            {
                inst = value;
            }
        }

		List<CVSensor> sensors;
        public static CVInstrument CreateCVInstrument(int tmid, SqliteDataBase mdb)
        {
            string sql = "SELECT * FROM calibration WHERE TMID = " + tmid.ToString();
            DataTable dt = mdb.GetDataTable(sql);
            if (dt == null || dt.Rows.Count == 0)
                return null;
            CVInstrument inst = new CVInstrument();
          //  inst.SerialNu = 
            string r = (string)dt.Rows[0]["report"];
            string vals = (string)dt.Rows[0]["vals"];
            inst.sensors = JsonConvert.DeserializeObject<List<CVSensor>>(vals);
            foreach (CVSensor c in inst.sensors)
                c.Init(inst);
            CVReports reports = JsonConvert.DeserializeObject<CVReports>(r);
            inst.Init(reports);

            return inst;
        }

        /*
                public static CVInstrument CreateCVInstrument( string assy, string serialName)
                {
                    Assembly a = Assembly.LoadFrom(assy + ".dll");
                    if (a == null)
                        return null;
                    string strFn = "OpenLS." + assy + "." + serialName + ".CD" + serialName + ".json";
                    try
                    {
                        CVInstrument inst = new CVInstrument();
                        using (StreamReader sr = new StreamReader(a.GetManifestResourceStream(strFn)))
                        {
                            string str = sr.ReadToEnd();
                            inst.SerialNu = serialName;
                            inst.sensors = JsonConvert.DeserializeObject<List<CVSensor>>(str);
                            foreach (CVSensor c in inst.sensors)
                                c.Init(inst);
                        }
                        strFn = "OpenLS." + assy + "." + serialName + ".CR" + serialName + ".json";
                        using (StreamReader sr = new StreamReader(a.GetManifestResourceStream(strFn)))
                        {
                            string str = sr.ReadToEnd();
                            CVReports reports = JsonConvert.DeserializeObject<CVReports>(str);
                            inst.Init(reports);

                        }
                        return inst;
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                }
        */

        public CVInstrument()
        {

        }

        public void Init(CVReports reports)
        {
            foreach (CVSensor s in this.sensors)
            {
                CVPhase[] ps = s.GetCVPhases();
                foreach (CVPhase p in ps)
                {
                    p.CVInstrument = this;
                    CVReport r = reports.GetReport(s.Name, p.Name);
                    if(r != null)
                        p.Report.Init(r.Tables);
                }
            }
        }

        public CVSensor GetSensor(string strSensor)
        {
            foreach (CVSensor s in this.sensors)
                if (s.Name == strSensor)
                    return s;
            return null;
        }


        public int GetDataRecordLength()
        {
            int l = 0;
            foreach (CVSensor t in sensors)
                l += t.GetDataRecordLength();
            return l;
        }


 //       [JsonIgnore]
		public List<CVSensor> Sensors
		{
			get
			{
				return sensors;
			}
		}




    }

}
