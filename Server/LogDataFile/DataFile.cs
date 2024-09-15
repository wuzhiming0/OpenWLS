using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.IO;
using System.Reflection;
using System.ComponentModel;
using System.Text.Json.Serialization;

using OpenWLS.Server.Base;
using System.Drawing;
using OpenWLS.Server.DBase;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Runtime.CompilerServices;
using System.Collections;
using OpenWLS.Server.LogDataFile.Models.NMRecord;
using static OpenWLS.Server.LogDataFile.Models.NMRecord.FileVersion;
using OpenWLS.Server.LogDataFile.Models;

namespace OpenWLS.Server.LogDataFile
{
    public class DataFile :  SqliteDataBase , IDisposable
    {
 //       public static string real_time_ldf_name = "RealTime";
        public static readonly string file_ext = ".ldf";
        public static readonly string file_filter = "*.ldf";

        protected FileVersion version;
        protected Measurements ms;
        protected NMRecords nms;
        protected Frames frames;

        public FileVersion Version { get { return version; } }
        public Measurements Measurements { get { return ms; } }
        public NMRecords NMRecords { get { return nms; } }
        public Frames Frames { get { return frames; } }
        public string FileName { get { return pathDB;  } }  // from SqliteDataBase
        public static  DataFile CreateDataFile(string fn, VersionOption v, ISyslogRepository sysLog)
        {
            if (File.Exists(fn))
                fn = Base.StringConverter.GetNxtFileName(fn, ".ldf");
               // Globals.sysLog.AddMessage("{fn} exists, creat failed.");

            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream("OpenWLS.Server.LogDataFile.ldf.db"))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                byte[] bs = new byte[stream.Length];
                reader.Read(bs, 0, bs.Length);
                using (FileStream fs = new FileStream(fn, FileMode.Create))
                using (BinaryWriter outputFile = new BinaryWriter(fs))
                {
                    outputFile.Write(bs);
                }
            }
            DataFile df =  OpenDataFile(fn, sysLog);
            FileVersion fv = new FileVersion(v);
            fv.AddNMRecordToDb(df);
            return df;
        }

        public static DataFile? OpenDataFile(string fn, ISyslogRepository? sysLog)
        {
            if (File.Exists(fn))
            {
                DataFile df = new DataFile();
                df.OpenDb(fn);
                return df;
            }
            else
            {
                if(sysLog != null)
                    sysLog.AddMessage($"{fn} does not exist, open failed.");
                return null;
            }

        }


        static public string GetNxtDataFileName(string jobFolder,  string ocfFn)
        {
            int k = 0;
            bool b = true;
            string str = "";
         //   string s1 = Path.GetDirectoryName(ocfFn);
            string s2 = Path.GetFileName(ocfFn);
            if (s2.ToLower().EndsWith(".ocf"))
                s2 = s2.Substring(0, s2.Length - 4);
            while (b)
            {
                b = false;
                str = s2 + "_" + k.ToString() + file_ext;
                var fs = Directory.EnumerateFiles(jobFolder + @"/das");
                foreach(string fn in fs)
                {
                    if(Path.GetFileName(fn) == str)
                    {
                        b = true; break;
                    }
                }
                k++;
            }
           
            return str;
        }

        public DataFile() 
        {
            frames = new Frames();
            version = new FileVersion();
            nms = new NMRecords();
        }

        public void Dispose()
        {
            Close();
        }

        public void LoadFileInfor()
        {
            ms = Measurements.ReadMeasurementHeads(this);
            foreach (Measurement m in ms)
            {
                m.MVBlocks.ReadMVBlocks(this, m.Id);
                m.ProcessIndex();
            }

            nms = NMRecords.GetNMRecords(this);
            frames.Init(ms);
        }

        public DataFileInfor GetFileInfor()
        {
            LoadFileInfor();
            return new DataFileInfor()
            {
                FileName = pathDB,
                Measurements = ms,
                NMRecords = nms,
            };
        }
        public byte[] GetNmObj(int id)
        {
            return BinObject.GetVal(this, id, "NmObjs");
        }

        virtual public string GetMeasurementNames()
        {
            return Measurements.GetMeasurementNames();
        }
        
        public string GetNcTxtObjectText(int id)
        {       
            return null;
        }

        public void CleanUp()
        {
                

        }

        public void AddHeadToDb()
        {

        }
        public void UpdateHeadToDb()
        {

        }
        
    }

    public class DataFileInfor
    {
        public string FileName { get; set; }
        public Measurements Measurements { get; set; }
        public NMRecords NMRecords { get; set; }

    }


}
