
using OpenWLS.Server.DBase;
using OpenWLS.Server.LogDataFile.Models;
using System.Data;
using System.Diagnostics.Metrics;
using System.Text.Json.Serialization;

namespace OpenWLS.Server.LogDataFile.Models
{
    public partial class  Measurement
    {
        public void AddMVBlock()
        {
            if (dataFile != null && mVWriter != null && mVWriter.Postion > 0)
            {
                MVBlock mvb = new MVBlock()
                {                    
                    StartIndex = mVWriter.StartIndex,
                    StopIndex = mVWriter.StopIndex,
                    Samples = mVWriter.Postion / mVWriter.SampleBytes
                };
                if (mVBlocks.Count == 0 )
                    StartIndex = (double)mVWriter.StartIndex;
                StopIndex = (double)mVWriter.StopIndex;

                mvb.AddToDB(dataFile, mVWriter, mVBlocks.MId, mVWriter.Postion);
                mVBlocks.Add(mvb);

              //  Head.Samples += mvb.Samples;
            }

        }


    }

    public partial class Measurements 
    {
        
        public static Measurements ReadMeasurementHeads(DataFile df)
        {
            string sql = $"SELECT * FROM MHead1s WHERE Deleted is NULL";
            DataTable dt = df.GetDataTable(sql);
            List<MHead> hs = new();
            foreach (DataRow dr in dt.Rows)
            {
                MHead h = new MHead();
                h.RestoreH1(dr);
                hs.Add(h);
            }
            sql = "SELECT * FROM MHead2s";
            dt = df.GetDataTable(sql);
            foreach (DataRow dr in dt.Rows)
            {
                int id = Convert.ToInt32(dr["Id"]);
                MHead? h = hs.Where(h => h.Id == id).FirstOrDefault();
                if (h != null) h.RestoreH2(dr);
            }
            Measurements ms = new Measurements();
            foreach (MHead h in hs)
                ms.Add(new Measurement(h, df));
            return ms;
        }

        /*
        public Measurement GetMeasurement(string full_name)
        {
            int k = full_name.IndexOf('.');
            string fn = k <= 0 ? null : full_name.Substring(0, k);
            string mn = k > 0 ? full_name.Substring(k + 1, full_name.Length - k - 1) : full_name;
            return GetMeasurement(fn, mn);
        }
        */

    

    }
}
