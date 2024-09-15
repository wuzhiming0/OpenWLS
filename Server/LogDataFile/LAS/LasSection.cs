using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json.Serialization;
using OpenWLS.Server.Base;
using OpenWLS.Server.LogDataFile.Models;
using System.Text.Json;
using OpenWLS.Server.LogDataFile.Models.NMRecord;

namespace OpenWLS.Server.LogDataFile.LAS
{
    public enum LasDataType { Floating = 1, Integer, String, Exponential, Date, Time, DegreeMS };

	public class LasSection
	{
		public static readonly string ver_section_name = "VERSION INFORMATION";
		public static readonly string well_section_name = "WELL INFORMATION";
		public static readonly string curve_section_name = "CURVE INFORMATION";
		public static readonly string ascii_section_name = "ASCII LOG DATA";
		public static readonly string para_section_name = "PARAMETER INFORMATION";
		public static readonly string other_section_name = "OTHER";
		
//# Lat & Long can also be presented as:
//# LATI .       45.37° 12' 58"                   : X LOCATION
//# LONG .       13.22° 30' 09"                   : Y LOCATION


			//8.7-35:
		public string ColumnDefinition { get; set; }
		public string Name	{ get; set; }

		public  LasSectionRows Rows	{get; set;}
		
		public int DataLines { get; set; }

		public bool IsDefinationSection { get; set; }

		public bool IsDataSection {
			get
			{
				return ColumnDefinition != null || Name == ascii_section_name;
			}
		}

		public LasFrame Frame { get; set; }

		public LasSection()
		{
			Rows = new LasSectionRows();
		}
		public LasSection(string name)
		{
			Name = name;
			Rows = new LasSectionRows();
		}
		public void Restore(string str, LasVersion ver)
		{
			string[] ss = str.Split('\n');
			foreach(string s in ss)
			{
				LasSectionRow srow = new LasSectionRow();
				string e1 = null;
					e1 = srow.Init(s, ver);
				if (e1 == null)
					Rows.Add(srow);
			}
		}

		public LasFrame CreateFrame( LasVersion v, LasSection colDef)
		{
			LasFrame af = new LasFrame();
			af.Name = Name;
			af.Samples = DataLines;

			foreach (LasSectionRow r in colDef.Rows)
			{
				MHead h = r.GetMeasurementHead(v);
				h.Frame = Name;
				//		h.Samples = DataLines;
				Measurement m = new Measurement()
				{
					Head = h
				};
				m.CreateMVWriter(DataLines);
				m.MVWriter.Tag = r.GetLasDataType(v);
				af.Measurements.Add(m);
			}
			return af;
		}

		public bool Init(string strName, LasVersion v)
		{
			strName = strName.Trim().Remove(0,1);
			int k = strName.IndexOf('|');
			if(k > 0)
			{
				ColumnDefinition = strName.Substring(k + 1, strName.Length - k - 1).Trim();
				strName = strName.Substring(0, k).Trim();
			}
			if (v >= LasVersion.V30)
				Name = strName;
			else
			{
				if (strName[0] == 'V')
					Name = ver_section_name;
				else
				{
					if (strName[0] == 'A')
						Name = ascii_section_name;
					else
					{
						if (strName[0] == 'W')
							Name = well_section_name;
						else
						{
							if (strName[0] == 'P')
								Name = para_section_name;
							else
							{
								if (strName[0] == 'C')
									Name = curve_section_name;
								else
								{
									if (strName[0] == 'O')
										Name = other_section_name;
									else
										return false;
								}
							}
						}
					}
				}
			}			
			Rows = new LasSectionRows();
			return true;
		}

		public string ToString1()
		{
			string str = "~" + Name + "\r\n";
			foreach (LasSectionRow r in Rows)
				str = str + r.ToString1() + "\r\n";
			return str;
		}
/*
		public void LoadValues(Parameters tps, FileVersion h)
		{
			foreach(LasSectionRow r in Rows)
			{
                Parameter? p = tps.Where(p1 => p1.MNEM == r.MNEM).FirstOrDefault();
				if(p!= null)
				{
					r.DATA = p.Val[0] ;
					r.UNITS = p.Units;
				}	
			}
			LasSectionRow r1 = Rows.GetLasSectionRow("STRT");
			r1.DATA = h.StartDepth.ToString();
			r1 = Rows.GetLasSectionRow("STOP");
			r1.DATA = h.StartDepth.ToString();
			r1.UNITS = h.MetricDepth? "M":"F";
		}
*/
	}

	public class LasSections : List<LasSection>
	{
		public LasSection GetLasSection(string name)
		{
			foreach (LasSection s in this)
			{
				if (name.ToUpper() == s.Name.ToUpper())
					return s;
			}
			return null;
		}

		public void AddNCObjects(DataFile df)
		{
			foreach (LasSection s in this)
			{
				if (s.IsDefinationSection || s.IsDataSection)
					continue;
				NMRecord nmr = new LogDataFile.Models.NMRecord.NMRecord()
				{
					RType = (int)NMRecordType.LasSection,
					Name = s.Name.ToUpper(),
					Val = JsonSerializer.Serialize(s.Rows)
				};

				nmr.AddNMRecordToDb(df);
			}
		}

		public LasSection GetDataLasSection(string defName)
		{
			foreach (LasSection s in this)
			{
				if (s.ColumnDefinition == defName)
					return s;
			}
			return null;
		}
		public void AddFrames(DataFile ar, LasVersion v)
		{
			foreach (LasSection s in this)
			{
				if (s.IsDataSection)
				{
					LasSection def = GetLasSection(s.ColumnDefinition);
					def.IsDefinationSection = true;		
					LasFrame af = s.CreateFrame(v, def);
					s.Frame = af;
					af.Samples = s.DataLines;
					af.Tag = s;
					ar.Frames.Add(af);
				}
			}
		}
	}

}
