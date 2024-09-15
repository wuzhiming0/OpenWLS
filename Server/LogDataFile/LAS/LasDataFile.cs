using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using OpenWLS.Server.Base;
using System.Text.Json.Serialization;
using OpenWLS.Server.LogDataFile;
using OpenWLS.Server.LogDataFile.Models;
using OpenWLS.Server.DBase.Models.LocalDb;
using OpenWLS.Server.DBase;
using System.Text.Json;
using OpenWLS.Server.LogDataFile.Models.NMRecord;
using static OpenWLS.Server.LogDataFile.Models.NMRecord.FileVersion;

namespace OpenWLS.Server.LogDataFile.LAS
{
    public enum LasVersion { Unknown = -1, V12 = 1, V20 = 2, V30 = 3 }
    public enum LasLineType { UnKnown = -1, Empty = 0, Comment = 1, SectionName = 2, SectionRow = 3, LogData = 4 }
    public class LasDataFile
    {
        public static char flag_comment = '#';
        public static char flag_section_name = '~';
        static void SaveChannels( DataFile df, ISyslogRepository sysLog)
        {
            foreach (Frame f in df.Frames)
            {
                _ = sysLog.AppendMessage($"Import Frame {f.Name}");
                string chNames = "";
                int mid = 0;
                foreach (Measurement m in f.Measurements)
                {
                    chNames = chNames + "," + m.Head.Name;
                    m.Id = mid++;

                    if(m.Head.IndexM != null)
                    {
                      ///  if (df.Head.MetricDepth ^ df.Head.MetricDepth)
                      //      ProcShift.IndexUnitFrame(m, df);
                    }
                    ((MHead)m.Head).AddHeadToDb(df);
                    m.DataFile = df;
                    //          m.AddMVBlock(m.MVWriter.VBuffer, 0, m.MVWriter.VBuffer.Length, m.MVWriter.TotalSamples);
                    m.AddMVBlock();
                }
                _ = sysLog.AppendMessage("Channels: " + chNames.Remove(0,1));
            }
        }

        static void SaveFileHead( DataFile df, ISyslogRepository sysLog)
        {
            _ = sysLog.AppendMessage("Save File head.");
            df.AddHeadToDb();
        }

        public static DataFileInfor? Import(string fn, ISyslogRepository sysLog)
        {
            using (FileStream fs = new FileStream(fn, FileMode.Open, FileAccess.Read))
            {  
                string outFn = fn + DataFile.file_ext;
                DataFile dstDf = DataFile.CreateDataFile(outFn, VersionOption.V1, sysLog);   

                LasDeSerializer des = new LasDeSerializer();
                _ = sysLog.AddMessage("Open " + fn);
                des.Read(fs,  dstDf, sysLog);
                fs.Close();

                SaveFileHead(dstDf, sysLog );
                SaveChannels(dstDf, sysLog);
             //   SaveNC(dstDf);
                _ = sysLog.AddMessage("Imported sucessful to " + outFn);
                dstDf.Close();
                return dstDf.GetFileInfor();
            }
        }

        public static UInt32 ConvertToDate(string s)
        {
            UInt32 d = 0;
            string[] ss = s.Split('\\');
            if (ss.Length == 3)
            {
                d = Convert.ToUInt32(ss[0]);
                d += Convert.ToUInt32(ss[1]) << 8;
                d += Convert.ToUInt32(ss[2]) << 16;
            }
            s = s.Replace("-", "").Trim();
            if (s.Length == 8)
            {
                d = Convert.ToUInt32(s.Substring(6, 2));
                d += Convert.ToUInt32(s.Substring(4, 2)) << 8;
                d += Convert.ToUInt32(s.Substring(0, 4)) << 16;
            }
            return d;
        }

        public static double ConvertToTime(string s)
        {
            s = s.Replace(":", "").Trim();
            switch (s.Length)
            {
                case 2:
                    return Convert.ToDouble(s) * 3600;
                case 4:
                    return (Convert.ToDouble(s.Substring(0, 2)) * 3600)
                        + (Convert.ToDouble(s.Substring(2, 2)) * 60);
                case 6:
                    return (Convert.ToDouble(s.Substring(0, 2)) * 3600)
                        + (Convert.ToDouble(s.Substring(2, 2)) * 60)
                        + (Convert.ToDouble(s.Substring(4, 2)));
                case 9:
                    return (Convert.ToDouble(s.Substring(0, 2)) * 3600)
                        + (Convert.ToDouble(s.Substring(2, 2)) * 60)
                        + (Convert.ToDouble(s.Substring(4, 5)));
            }
            return double.NaN;
        }

        public static void ReadLasLogDataVal(MVWriter w, string val)
        {
            LasDataType t = (LasDataType) w.Tag;
            switch (t)
            {
                case LasDataType.Floating:
                case LasDataType.Integer:
                    w.WriteSample(Convert.ToDouble(val));
                    break;
                case LasDataType.Date:
                    w.WriteSample(ConvertToDate(val));
                    break;
                case LasDataType.Time:
                    w.WriteSample(ConvertToTime(val));
                    break;
                case LasDataType.String:
                    w.WriteSample( Encoding.UTF8.GetBytes( val) );
                    break;
                case LasDataType.Exponential:
                    w.WriteSample(Convert.ToDouble(val));
                    break;
            }
        }
        public static SparkPlugDataType GetDataType(LasDataType t)
        {
            switch (t)
            {
                case LasDataType.Floating:
                    return SparkPlugDataType.Float;
                case LasDataType.Integer:
                    return SparkPlugDataType.Int32;
                case LasDataType.Time:
                    return SparkPlugDataType.Double;
                case LasDataType.Date:
                    return SparkPlugDataType.DateTime;
                case LasDataType.String:
                    return SparkPlugDataType.String;
                case LasDataType.Exponential:
                case LasDataType.DegreeMS:
                    return SparkPlugDataType.Double;
                default:
                    return SparkPlugDataType.Unknown;
            }
        }

        public static int Export(string fn, LasVersion version, ISyslogRepository syslog, string[] ch_names)
        {
            DataFile df = DataFile.OpenDataFile(fn, syslog);
            if (df == null)
                return -1;

            string outFn = df.FileName + ".las";
            LasSerializer ser = new LasSerializer(version);


            Parameters wellInfor = Well.NewGInfor();
   //         ser.WiSection.LoadValues(wellInfor, df.Head);
            Well well = new Well();
            well.SetGeneralInformation(wellInfor);
            NMRecord nMRecord = new NMRecord()
            {
                RType = (int)NMRecordType.WellInfor,
                Val = JsonSerializer.Serialize(well, new JsonSerializerOptions
                {
//                    WriteIndented = true,
                    IgnoreNullValues = true,
 //                   PropertyNameCaseInsensitive = true,
                })
            };
            nMRecord.AddNMRecordToDb(df);

            using (StreamWriter sw = new StreamWriter(outFn))
            {
                sw.WriteLine(ser.ViSection.ToString1());
                sw.WriteLine(ser.WiSection.ToString1());
                List<LasSection> ss = new List<LasSection>();
                int k = 1;
                foreach (Frame f in df.Frames)
                {
                    LasSection s = ser.GetCurveInformationSection(f, LogIndexType.BOREHOLE_DEPTH, k, ch_names);
                    if (s != null)
                    {
                        sw.WriteLine(s.ToString1());
                        ss.Add(s);
                        if (version <= LasVersion.V20)
                            break;
                        k++;
                    }
                }
                foreach (LasSection s in ss)
                    ser.WriteLogDataBlock(sw, df, s);
            }
            df.Close();
            syslog.AddMessage("exported sucessful to " + fn);
            return 0;
        }
    }


}
