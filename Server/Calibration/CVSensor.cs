using System;
using System.Data;
using System.Collections.Generic;
using OpenWLS.Server.Base;

namespace OpenWLS.Server.LogInstance.Calibration
{
    public class CVSensors : List<CVSensor>
    {
        public CVSensor GetSensor(int id)
        {
            foreach (CVSensor s in this)
                if (s.ID == id)
                    return s;
            return null;
        }

    }
  
    public class CVSensorStatus
    {
        public string SerialNumber { get; set; }
        public string Asset { get; set; }
        public string Name { get; set; }
        public int CP { get; set; }
        public int VP { get; set; }
        public int VA { get; set; }
        public int VB { get; set; }
        public int ID { get; set; }

    }

	/// <summary>
	/// Summary description for CVType.
	/// </summary>
	public class CVSensor
	{
		public string Name{get; set;}
        public int ID { get; set; }
        public DataTable Phase { get; set; }

		CVPhase[] phases;
        CVInstrument cvInst;

        public CVSensorStatus GetPhasesStatus()
        {
            CVSensorStatus s = new CVSensorStatus();
            s.SerialNumber = cvInst.SerialNu;
            s.Name = Name;
            s.CP = GetPhaseStatus("CP");
            s.VP = GetPhaseStatus("VP");
            s.VB = GetPhaseStatus("VB");
            s.VA = GetPhaseStatus("VA");
            s.ID = ID;
            return s;
        }

        int GetPhaseStatus(string name)
        {
            CVPhase p = GetCVPhase(name);
            if (p == null)
                return (int)CVPhaseStatus.NotAvailable;
            return (int)p.Status;
        }

        public CVPhase GetCVPhase(string name)
        {
            foreach (CVPhase p in phases)
                if (p.Name == name)
                    return p;
            return null;
        }

        public CVPhase[] GetCVPhases()
        {
            return phases;
        }

        public CVInstrument GetCVInstrument()
        {
                return cvInst;
        }


        public int ProcGUIText(DataReader r)
        {
            return 0;
        }



        public int GetDataRecordLength()
        {
            int l = 0;
            foreach(CVPhase p in phases)
                l += p.GetDataRecordLength();
            return l;
        }

        public void Init( CVInstrument inst)
		{
            cvInst = inst;
            int l = Phase.Rows.Count;
			phases = new CVPhase[l];
	        for(int i = 0; i < l; i++)
                phases[i] = new CVPhase( inst.SerialNu, Phase.Rows[i]);
		}




        public CVPhase this[string strPhase]
		{
			get
			{
				foreach(CVPhase phase in this.phases)
					if(phase.Name == strPhase)
						return phase;
				return null;
			}
		}



        public void SetAsset(string asset)
        {
            foreach (CVPhase phase in this.phases)
                phase.Record.Asset = asset;
        }

	}
	
}
