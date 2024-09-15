
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenWLS.Server.Base;
using OpenWLS.Server.LogDataFile;
using OpenWLS.Server.DBase;
using OpenWLS.Server.LogDataFile.Models;
using OpenWLS.Server.LogDataFile.Models.NMRecord;

namespace OpenWLS.Server.LogDataFile
{
    public class ProcShift
    {
        public static string str_shift_file = "File";
        public static string str_shift_frame = "Frame";
        public static string str_shift_nframe = "NFrame";
        public static string str_shift_channel = "Measurement";

        public static void IndexUnitNoFrame(Measurement m)
        {
            double d = m.Head.IndexMetric ? 1 / 0.3048 : 0.3048;
            m.Head.UOI = m.Head.IndexMetric ? "ft" : "m";
            m.StartIndex *= d;
            m.StopIndex *= d;
        }

        public static void IndexUnitFrame(Measurement m, DataFile df)
        {
            double d = m.Head.IndexMetric ? 1 / 0.3048 : 0.3048;
            m.Head.UOM = m.Head.IndexMetric ? "ft" : "m";
            MVReader r = new MVReader(m);
            double[] ds = r.ReadAllDoubles(); 
            for (int i = 0; i < ds.Length; i++)
                ds[i] *= d;
            m.Head.VMin *= d;
            m.Head.VMax *= d;
            m.Head.VFirst *= d;
            m.Head.VLast *= d;
            MVWriter w = new MVWriter(m.Head, m.Samples);
            w.WriteBuffer(ds);
            m.UpdateMVBlock(w.Bytes);
        }

        public static void IndexShiftFrame(Measurement m, double d, DataFile df)
        {
            MVReader r = new MVReader(m);
            double[] ds = r.ReadAllDoubles();
            for (int i = 0; i < ds.Length; i++)
                ds[i] += d;
            m.Head.VMin += d;
            m.Head.VMax += d;
            m.Head.VFirst += d;
            m.Head.VLast += d;
            MVWriter w = new MVWriter(m.Head);
            w.WriteBuffer(ds);
            m.UpdateMVBlock(w.Bytes);
        }
        public static string IndexUnit(DataReader r, ISyslogRepository syslog)
        {
            string fn = r.ReadLine();
            string err; 
            DataFile df   = DataFile.OpenDataFile(fn, syslog);
            if (df == null)
                return "";
            int du = Convert.ToInt32(r.ReadLine());
            bool indexMetric = du == 0;
    //        if(indexMetric ^ df.Head.MetricDepth)
            {
                foreach (Frame f in df.Frames)
                {
                    Index ai = f.Indexes.GetChannelIndex(LogIndexType.BOREHOLE_DEPTH);
                    if (ai == null)
                        continue;
                    if (string.IsNullOrEmpty(f.Name))
                    {
                        foreach (Measurement m in f.Measurements)
                            IndexUnitNoFrame(m);
                    }
                    else
                        IndexUnitFrame((Measurement)ai.Measurement, df);
                }


        /*        double d = df.Head.MetricDepth ? 1 / 0.3048 : 0.3048;
                df.Head.DepthUnit = du;
                df.Head.StartDepth *= d;
                df.Head.StopDepth *= d;
                df.UpdateHeadToDb();
        */
            }
       //     proc.SendDataFileInfor(df);
            string u = indexMetric ? "meter" : "feet";
            syslog.AddMessage($"Changed depth unit of {df.FileName} to {u}");
            df.Close();
            return null;
        }



        public static string ShiftIndex(DataReader r, ISyslogRepository syslog)
        {
            string fn = r.ReadLine();
            string err;
            DataFile df = DataFile.OpenDataFile(fn, syslog);
            if (df == null)
                return "";


            string s1 = r.ReadLine();
            double d = Convert.ToDouble(r.ReadLine());
            string ms = "file " + df.FileName;
            if (s1 == str_shift_file)
            {
                IndexShift(df, LogIndexType.BOREHOLE_DEPTH, d);
           //     df.Head.StartDepth += d;
           //     df.Head.StopDepth += d;
                df.UpdateHeadToDb();          
            }
            else
            {
                string s2 = r.ReadLine();
                if (s1 == str_shift_frame)
                {
                    Frame f = df.Frames.GetFrame(s2);
                    if (f != null)
                    {
                        IndexShift(df, f, LogIndexType.BOREHOLE_DEPTH, d);
                        ms = "frame " + f.Name + " in " + ms;
                    }
                    else
                        ms = "none";
                }
                else
                {
                    if (s1 == str_shift_nframe)
                    {
                        Measurement m = df.Measurements.Where(m1 => m1.Id == (Convert.ToInt32(s2))).FirstOrDefault();
                        if (m != null)
                        {
                            Frame f = df.Frames.GetFrameNF(m);
                            if (f != null)
                            {
                                IndexShift(df, LogIndexType.BOREHOLE_DEPTH, d);
                                ms = "frame with channel " + m.Head.Name + " in " + ms;
                            }
                            else
                                ms = "none";
                        }
                    }
                    else
                    {
                        if (s1 == str_shift_channel)
                        {
                            Measurement m = df.Measurements.Where(m1 => m1.Id == (Convert.ToInt32(s2))).FirstOrDefault();
                            if (m != null)
                            {
                                IndexShift(df, LogIndexType.BOREHOLE_DEPTH, d);
                                ms = "channel " + m.Head.Name + " in " + ms;
                            }
                        }
                    }
                }
            }

    //        proc.SendDataFileInfor(df);
            syslog.AddMessage($"Shifted depth {d} of {ms}");

            df.Close();
            return null;
        }

        public static void IndexShift(DataFile df, LogIndexType itype,  double d)
        {
            foreach (Frame f in df.Frames)
                IndexShift(df, f, itype, d);
        }

        public static void IndexShiftNoFrame(Measurement m, double d)
        {
            m.StartIndex += d;
            m.StopIndex += d;
        }

        public static void IndexShift(DataFile df,  Frame f, LogIndexType itype, double d)
        {
            Index ai = f.Indexes.GetChannelIndex(itype);
            if (ai == null)
                return;

            if (string.IsNullOrEmpty(f.Name))
            {
                foreach (Measurement m in f.Measurements)
                    IndexShiftNoFrame(m, d);
            }
            else
                IndexShiftFrame((Measurement)ai.Measurement, d, df);
            
        }

        public static void IndexShift(Measurement m, LogIndexType itype, double d)
        {
            Frame f = m.Frame;
            if (string.IsNullOrEmpty(m.Head.Frame))
            {
                Index ai = f.Indexes.GetChannelIndex(itype);
                if (ai == null)
                    return;
                if (string.IsNullOrEmpty(f.Name))
                        IndexShiftNoFrame(m, d);
                else
                {
                    if (itype == LogIndexType.BOREHOLE_DEPTH)
                    {
                        m.Head.IndexShift += d;
                    }
                }
            }
        }
        /*
        public static string GetLasSectionVdf(DataFile df, string sn, string vn)
        {
            return null;
        }

        public static string SetLasSectionVar(DataFile df, string sn, string vn)
        {
            return null;
        }*/


    }
}
