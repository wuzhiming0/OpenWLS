using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json.Serialization;
using System.Security.Principal;
using OpenWLS.Server.LogDataFile.Models;
using OpenWLS.Server.LogDataFile.DLIS;
using OpenWLS.Server.LogDataFile;
using OpenWLS.Server.DBase;
using System.Drawing;

namespace OpenWLS.Server.LogDataFile.LAS
{
    public class LasDeSerializer
    {       
        LasSection section;
        LasSections sections;
        LasVersion ver;
        bool wrap;
        string strLine;
        Stream lasStream;
        int logDataStart;
        char dlm;
/*        Frames fs;
        public Frames Frames
        {
            get { return fs; }
        }*/
        public LasSections Sections
        {
            get { return sections; }
        }

        public LasDeSerializer()
        {
            sections = new LasSections();
            ver = LasVersion.Unknown;
        }

        void SetFileHead(DataFile df)
        {
            double start = double.NaN;
            double stop = double.NaN;
            double levelSpacing = double.NaN;
            IndexUnit u = IndexUnit.unknown;
            LasSection ws = sections.GetLasSection(LasSection.well_section_name);
            if (ws == null)
                return;
            LasSectionRow r = ws.Rows.GetLasSectionRow("STRT");
            if (r != null)
                start = Convert.ToDouble(r.DATA);
            r = ws.Rows.GetLasSectionRow("STOP");
            if (r != null)
                stop = Convert.ToDouble(r.DATA);
            r = ws.Rows.GetLasSectionRow("STEP");
            if (r != null)
            {
                levelSpacing = Convert.ToDouble(r.DATA);
                u = Index.ConvertToIndexUnit(r.UNITS);
            }
         //   df.Head.SetDepthRange(u, start, stop);

        }

        public string Read(Stream stream, DataFile df, ISyslogRepository syslog)
        {
            lasStream = stream;
            lasStream.Seek(0, SeekOrigin.Begin);
            string err = null;
            using (StreamReader sr = new StreamReader(lasStream))
            {
                err = ParsePass1( sr, syslog );
                if (err == null)
                {
                    _ = syslog.AppendMessage("Pass 1 done");
                    lasStream.Seek(0, SeekOrigin.Begin);
                    if (ver <= LasVersion.V20)
                        err = ParsePass2V2(sr, df);
                    else
                        err = ParsePass2V3(sr, df);
                }
                else
                    _ = syslog.AddMessage($"Pass 1 error: {err}, {Color.Red.ToArgb()}");
            }
            SetFileHead(df);

            return err;
        }

        LasLineType GetLineType()
        {
            if (strLine == null || strLine.Length == 0)
                return LasLineType.Empty;
            strLine = strLine.Trim();
            if (strLine[0] == LasDataFile.flag_comment)
                return LasLineType.Comment;
            if (strLine[0] == LasDataFile.flag_section_name)
                return LasLineType.SectionName;
            if(section != null)
            {
                if (section.Name == LasSection.ascii_section_name || section.ColumnDefinition != null)
                    return LasLineType.LogData;
                return LasLineType.SectionRow;
            }
            return LasLineType.UnKnown;
        }

        void OnAfterSection(ISyslogRepository syslog)
        {
            if (section.Name == LasSection.ver_section_name)
            {
                LasSectionRow r = section.Rows.GetLasSectionRow("VERS");
                if (r.DATA.StartsWith("1.2"))
                    ver = LasVersion.V12;
                else
                {
                    if (r.DATA.StartsWith("2.0"))
                        ver = LasVersion.V20;
                    else
                    {
                        if (r.DATA.StartsWith("3.0"))
                            ver = LasVersion.V30;
                        else
                        {
                            ver = LasVersion.Unknown;
                        }
                            
                    }
                }
                syslog.AppendMessage( $"Version: {r.DATA}");
                r = section.Rows.GetLasSectionRow("WRAP");
                wrap = r.DATA.ToUpper()[0] == 'Y';
                r = section.Rows.GetLasSectionRow("DLM");
                if (r != null)
                {
                    if (r.DATA.Trim() == "SPACE")
                        dlm = ' ';
                    if (r.DATA.Trim() == "COMMA")
                        dlm = ',';
                    if (r.DATA.Trim() == "TAB")
                        dlm = '\t';
                }    
            }
            
        }

        string ParsePass1(StreamReader sr, ISyslogRepository syslog)
        {
            string err = null;
            int line = 0;
            logDataStart = 0;
            try
            {
                while (!sr.EndOfStream)
                {
                    strLine = sr.ReadLine().Trim();
                    line++;
                    if (strLine.Length == 0)
                        continue;
                    LasLineType t = GetLineType();
                    switch (t)
                    {
                        case LasLineType.SectionName:
                            if (section != null)
                            {
                                OnAfterSection(syslog);
                                sections.Add(section);
                            }
                            section = new LasSection();
                            if (!section.Init(strLine, ver))
                                err = err + "\nLine " + line.ToString() + " - Invalid Section: " + strLine;
                            break;
                        case LasLineType.SectionRow:
                            LasSectionRow srow = new LasSectionRow();
                            string e1 = null;
                            if (section.Name == LasSection.other_section_name)
                                srow.DATA = strLine;
                            else
                                e1 = srow.Init(strLine, ver);
                            if (e1 == null)
                                section.Rows.Add(srow);
                            else
                                err = err + "\nLine " + line.ToString() + " - " + e1 + ": " + strLine;
                            break;
                        case LasLineType.LogData:
                            if (logDataStart == 0)
                                logDataStart = line;
                            section.DataLines++;
                            break;
                    }
                }
            }
            catch(Exception e2)
            { 
                err = err + "\nLine " + line.ToString() + " - " + e2.Message + ": " + strLine;
                return err;
            }
 
            sections.Add(section);
            return err;
        }

        LasFrame GetFrame(DataFile df, LasFrame af, string strLine)
        {
            if (af != null)
                af.SetChannelHeadIndex(section.DataLines);
            section = new LasSection();
            section.Init(strLine, ver);

            foreach(LasFrame f in df.Frames)
            {
                if (f.Name == section.Name)
                    return f;
            }

            return null;
        }

        void LoadV3Data(StreamReader sr, DataFile df)
        {
            LasFrame af = null;
          //  char[] sep = new char[] { ' ', ',' };
            try
            {
               while (!sr.EndOfStream)
                {
                    strLine = sr.ReadLine().Trim();
                    if(strLine.Length == 0)
                        continue;
                    LasLineType t = GetLineType();
                    switch (t)
                    {
                        case LasLineType.SectionName:
                            af = GetFrame(df, af, strLine);
                            break;
                        case LasLineType.LogData:
                            string[] vs = strLine.Split(dlm);
                            int k = 0;
                            foreach (string val in vs)
                            {
                                if (val.Length > 0)
                                    LasDataFile.ReadLasLogDataVal(((DlisChannel)af.Measurements[k++]).MVWriter, val);
                            }
                            break;
                    }
                }
            }
            catch(Exception e1)
            {

            }
 
        }

        string ParsePass2V3(StreamReader sr, DataFile df)
        {
            if (wrap)
                return "wrap is not supported";
            string err = null;

            sections.AddFrames(df, ver);
            sections.AddNCObjects(df);

            LoadV3Data(sr, df);

            foreach (LasFrame f in df.Frames)
            {
                f.UpdateChannelHeads();
                f.SetChannelHeadIndex(f.Samples);
            }

            return err;
        }

        string ParsePass2V2(StreamReader sr, DataFile df)
        {
            if (wrap)
                return "wrap is not supported";
            string err = null;
            LasSection defSect = sections.GetLasSection(LasSection.curve_section_name);
            LasSection datSect = sections.GetLasSection(LasSection.ascii_section_name);
            defSect.IsDefinationSection = true;
            LasFrame  af = datSect.CreateFrame( ver, defSect);

            sections.AddNCObjects(df);

            for (int i = 1; i < logDataStart; i++)
                strLine = sr.ReadLine();
            int dl = 0;
            while (!sr.EndOfStream &&  dl < datSect.DataLines)
            {
                strLine = sr.ReadLine().Trim();
                if (strLine[0] == LasDataFile.flag_comment)
                    continue;
                string[] vs = strLine.Split(' ');
                int k = 0;
                foreach (string val in vs)
                {
                    if (val.Length > 0)
                        LasDataFile.ReadLasLogDataVal(((Measurement)af.Measurements[k++]).MVWriter, val);
                }
                dl++;
            }

            if(err == null)
            {
                af.UpdateChannelHeads();
                af.SetChannelHeadIndex(datSect.DataLines);
                df.Frames.Add(af);
            }

            return err;
        }



    }
}
