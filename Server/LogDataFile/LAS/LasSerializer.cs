using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using OpenWLS.Server.Base;
using OpenWLS.Server.LogDataFile.Models;
using OpenWLS.Server.LogDataFile;

namespace OpenWLS.Server.LogDataFile.LAS
{
    public class LasSerializer
    {
        static string strVI_V20 = @" VERS.                       2.0    :   CWLS LOG ASCII STANDARD -VERSION 2.0
 WRAP.                       NO     :   ONE LINE PER TIME STEP";

static string strWI_V20 = 
@"STRT    .S      0.0000                           :START TIME 
STOP    .S      39.9000                          :STOP TIME
STEP    .S      0.3000                           :STEP
NULL    .       -999.25                          :NULL VALUE
COMP    .       ANY OIL COMPANY INC.             :COMPANY
WELL    .       ANY ET 12-34-12-34               :WELL
FLD     .       WILDCAT                          :FIELD
LOC     .       12-34-12-34W5                    :LOCATION
PROV    .       ALBERTA                          :PROVINCE 
SRVC    .       ANY LOGGING COMPANY INC.         :SERVICE COMPANY
DATE    .       13-DEC-86                        :LOG DATE
UWI     .       100123401234W500                 :UNIQUE WELL ID";

        static string strVI_V30 = @"~VERSION INFORMATION
 VERS.                          3.0 : CWLS LOG ASCII STANDARD -VERSION 3.0
 WRAP.                           NO : ONE LINE PER DEPTH STEP
 DLM .                        COMMA : DELIMITING CHARACTER BETWEEN DATA COLUMNS";

        static string strWI_V30 = 
@"STRT .M              1670.0000                : First Index Value
 STOP .M              1669.7500                : Last Index Value 
 STEP .M              -0.1250                  : STEP 
 NULL .               -999.25                  : NULL VALUE
 COMP .       ANY OIL COMPANY INC.             : COMPANY
 WELL .       ANY ET AL 12-34-12-34            : WELL
 FLD  .       WILDCAT                          : FIELD
 LOC  .       12-34-12-34W5M                   : LOCATION
 CTRY .       CA                               : COUNTRY
 PROV .       ALBERTA                          : PROVINCE 
 SRVC .       ANY LOGGING COMPANY INC.         : SERVICE COMPANY
 DATE .       13/12/1986                       : LOG DATE  {DD/MM/YYYY}
 UWI  .       100123401234W500                 : UNIQUE WELL ID
 LIC  .       0123456                          : LICENSE NUMBER
 API  .       12345678                         : API NUMBER
 LATI.DEG                             34.56789 : Latitude  {F}
 LONG.DEG                           -102.34567 : Longitude  {F}
 GDAT .       NAD83                            : Geodetic Datum
 UTM.                                        : UTM LOCATION";

        LasSection viSection;   //VERSION INFORMATION
        LasSection wiSection;   //WELL INFORMATION

        public LasSection ViSection { get{ return viSection;  } }   //VERSION INFORMATION
        public LasSection WiSection { get { return wiSection; } }   //WELL INFORMATION

        LasVersion version;
        public LasSerializer(LasVersion ver)
        {
            version = ver;
            viSection = new LasSection("VERSION INFORMATION");
            wiSection = new LasSection("WELL INFORMATION");
            if (version > LasVersion.V20)
            {
                viSection.Restore(strVI_V30, version);
                wiSection.Restore(strWI_V30, version);
            }
            else
            {
                viSection.Restore(strVI_V20, version);
                wiSection.Restore(strWI_V20, version);
            }                
        }

        public LasSection GetCurveInformationSection(Frame frame, LogIndexType lit,  int seq, string[] ch_names)
        {
            LasSection s = new LasSection();
            bool b = false;
            if (version > LasVersion.V20)
            {
                if (string.IsNullOrEmpty(frame.Name))
                    s.Name = "LOG DATA" + seq.ToString() + " DEFINATION";
                else
                    s.Name = frame.Name + " DEFINATION";
            }
            else
                s.Name = "CURVE INFORMATION";
            foreach(Measurement m in frame.Measurements)
            {
                foreach(string cn in ch_names)
                {
                    if(m.Head.LongName == cn)
                    {
                        LasSectionRow r = new LasSectionRow(m);
                        s.Rows.Add(r);
                        b = true;
                        break;
                    }
                }
            }
            if(b)
                return s;
            return null;
        }

        public bool WriteLogDataBlock(StreamWriter sw, DataFile df, LasSection s)
        {
            string sn = version > LasVersion.V20? "~" + s.Name.Substring(0, s.Name.Length-11) : "~Log Data";
            sw.WriteLine(sn);
            List<string[]> ss1 = new List<string[]>();
            foreach(LasSectionRow r in s.Rows)
            {
                Measurement m = (Measurement)r.Tag;
                MVReader cr = new MVReader(m);
                string[] ss = null;
                if (m.Head.NumberType)
                {
                    if (m.Head.SampleElements == 1)
                    {
                        double[] ds = cr.ReadAllDoubles();
                        ss = StringConverter.GetStringArray(ds, m.Head.GetStdNumericFormat());
                    }
                    else
                    {

                        double[][] ds = cr.ReadAllXdDoubles();
                        ss = StringConverter.GetStringArray(ds, m.Head.GetStdNumericFormat());
                    }
                }
                else
                    ss = cr.LoadStringAll();
                ss1.Add(ss);
            }
            if(ss1.Count > 0)
            {
                for(int i = 0; i < ss1[0].Length; i++)
                {
                    string s1 = ss1[0][i];
                    for (int j = 1; j < ss1.Count; j++)
                        s1 = s1 + "," + ss1[j][i];
                    sw.WriteLine(s1);
                }
            }
            return false;
        }
    }
}
