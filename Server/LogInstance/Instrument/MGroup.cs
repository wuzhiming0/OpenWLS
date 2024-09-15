using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using System.Xml;
using Newtonsoft.Json.Linq;
//using Newtonsoft.Json;

using OpenWLS.Server.APInstance;
using System.Data.SQLite;

using OpenWLS.Server.APInstance.RtDataFile;
using OpenWLS.Server.LogDataFile.Models;
using OpenWLS.Server.DBase.Repositories.InstrumentM;
using System.Net;

namespace OpenWLS.Server.APInstance.Instrument
{
    public class MGroup : OperationDocument.MGroup
    {

        protected double group_index_offset;                  //mearsurement point                   
        protected Instrument inst;
        protected Measurements ms;
        protected int bufSamples;
        protected APInstance api;

        public double IndexOffset { get { return group_index_offset;  } }

        public Instrument Inst { 
            get { return inst; }
            set { inst = value; }
        }
        public Measurements Measurements { get { return ms; } }
        public TimeMeasurement TimeM { get; set; }
        public DepthMeasurement DepthM { get; set; }

        public MGroup()
        {
            bufSamples = 1024;
        }


        /*
        public static MGroup CreateMGroup(APInstance api, AcqItem actItem, IMGroupRepository repos_mg, IMeasurementRepository repos_m)
        {
            int inst_id = actItem.IId;
            int mg_id = actItem.MgId;
            Instrument? inst = api.Tools.Where(e=> e.Id == actItem.IId).FirstOrDefault();
            if(inst != null)
            {
                MGroup? mg = inst.AddMGroup(mg_id, repos_mg);
                mg.UpdateFromAcqItem( actItem);
       //         mg.CreateMeasurments(repos_m);
                mg.api = api;
                return mg;
            }
            return null;
        }
        */
    

        public int  UpdateFromAcqItem( AcqItem actItem )
        {
         //  double sr_time = dr["SamplesPerSecond"] == DBNull.Value? double.NaN : Convert.ToDouble(dr["SamplesPerSecond"]);
         //       double sr_depth = dr["SamplesPerMeter"] == DBNull.Value ? double.NaN : Convert.ToDouble(dr["SamplesPerMeter"]);

        IntervalTime = actItem.IntervalTime;
        IntervalDepth = actItem.IntervalDepth;
            Enable = actItem.Enable;

            //FrameName = null;
            //if (double.IsNaN(sample_interval_depth) && double.IsNaN(sample_interval_time))
            //    FrameName = Name + "T" + inst.Address.ToString("x");

            return 0;    
        }

        /*
        public int CreateMeasurments()
        {
            ms = new Measurements();
            foreach(string s in Ms.Split('\n'))
            {
                Measurement m = new Measurement();
                m.Restore(s, repos);
                ms.Add(m);
            }
            ms.ChanegUnitToSelected();
            return 0;
        }*/
        public Frame StartLog(bool depthAsIndex, bool indexIncreasing, DataFileRt df)
        {
            Frame f = new Frame($"I{inst.Id}MG{Id}", bufSamples);
            f.DataFile = df;
            double sampleSpacing;
            if (depthAsIndex)
            {
                TimeM = null;
                sampleSpacing = (double)IntervalDepth;
                DepthM = new DepthMeasurement(bufSamples, df, api, f, (double)IntervalDepth, indexIncreasing);
                f.AddMeasurement(DepthM.MeasurementDf);
            }
            else
            {
                DepthM = null;
                sampleSpacing = (double)IntervalTime;
                TimeM = new TimeMeasurement(bufSamples, df, api, f, (double)IntervalTime);
                f.AddMeasurement(TimeM.MeasurementDf);
            }

            double mpMin = double.MaxValue;
            double mpMax = double.MinValue;
            foreach (Measurement m in Measurements)
            {
                //m.CreateMeasurement(depthAsIndex, bufSamples, df, f);
                if (m.MeasurementDf != null)
                {
                    if (mpMax > m.MPoint)  // mearsure point is ref to bottom of the tool
                        mpMax = m.MPoint;
                    if (mpMin < m.MPoint)
                        mpMin = m.MPoint;
                    df.Measurements.Add(m.MeasurementDf);
                }
            }

            df.Frames.Add(f);
            return f;
        }
    }

    public class MGroups : List<MGroup>
    {
        public static MGroups GetAllMGroups(int iid_db, IMGroupRepository repos_mg)
        {
            MGroups mgs = new MGroups();
            return mgs; 
        }
        public MGroup GetMGroup(int id)
        {
            foreach (MGroup dg in this)
                if (dg.Id == id)
                    return dg;
            return null;
        }

        public void StartLog(bool depthAsIndex, bool indexIncreasing, DataFileRt ar)
        {
     //       foreach (MGroup dg in this)
     //           dg.StartLog(depthAsIndex, indexIncreasing, dcs, ar);
        }

    }
}
