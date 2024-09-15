
using OpenWLS.Server.Base;
using OpenWLS.Server.LogDataFile;
using OpenWLS.Server.LogDataFile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.LogDataFile.LAS
{
    public class LasSectionRow
	{	
		public string MNEM { get; set; }
		public string UNITS { get; set; }
		public string DATA { get; set; }
		public string DESCRIPTION { get; set; }
		string format { get; set; }
		public string ASSOCIATION { get; set; }
		int  dim { get; set; }

		public object Tag { get; set; }
		public string FORMAT
		{
			get
			{
				if (dim > 1)
					return "A" + dim.ToString() + format;	
				else
					return format;
			}
			set
			{
				if (value != null)
				{
					format = value;
					if (format.StartsWith("A"))
					{
						format = format.Substring(1, format.Length - 1);
						int k = 0;
						while (char.IsNumber(format[k]) && k < format.Length)
							k++;
						if (k > 0)
						{
							dim = Convert.ToInt32(format.Substring(0, k));
							format = format.Substring(k, format.Length - k);
						}
					}
				}
			}
		}

		public LasSectionRow()
		{
			dim = 1;
		}

		public LasSectionRow(Measurement m)
		{
            MHead h = (MHead)m.Head;
			MNEM = m.Head.Name;
			UNITS = m.Head.UOM;
			DESCRIPTION = m.Head.Desc;
			format = m.Head.DFormat;

			if (string.IsNullOrEmpty(format))
			{
				if (m.Head.NumberType)
				{
					switch (m.Head.DType)
					{
						case SparkPlugDataType.Int8:
						case SparkPlugDataType.UInt8:
						case SparkPlugDataType.UInt16:
						case SparkPlugDataType.Int16:
						case SparkPlugDataType.Int32:
						case SparkPlugDataType.UInt32:
							format = "I";
							break;
						default:
							format = "F";
							break;
					}
				}
				else
					format = "S";
			}

			// ASSOCIATION;
			dim = m.Head.SampleElements;
			Tag = m;
		}

		public MHead GetMeasurementHead(LasVersion v)
		{
            MHead h = new LogDataFile.Models.MHead();
			h.Name = MNEM;
			h.UOM = UNITS;	
			h.DFormat = format;
			h.Desc = DESCRIPTION;
			LasDataType t = GetLasDataType(v);
			h.DType = LasDataFile.GetDataType(t);
			h.Dimensions = t == LasDataType.String ? new int[] { -1 } : new int[] { dim };
			return h;
		}

		public LasDataType GetLasDataType(LasVersion v)
		{
			if (format != null)
			{
				if (format.Contains("F"))
					return LasDataType.Floating;
				if (format.Contains("I"))
					return LasDataType.Integer;
				if (format.StartsWith("S"))
					return LasDataType.String;
				if (format.Contains("E"))
					return LasDataType.Exponential;
				if (format.ToUpper().StartsWith("DATE"))
					return LasDataType.Date;
				if (format.StartsWith("T"))
					return LasDataType.Time;
				if (format.Contains("D"))
					return LasDataType.DegreeMS;
			}
			if (v == LasVersion.V20)
			{
				if (MNEM.StartsWith("DATE") && UNITS == null)
					return LasDataType.Date;
				if (MNEM.StartsWith("TIME") && UNITS == null)
					return LasDataType.Time;
			}
			//if (FORMAT.StartsWith("F"))
			return LasDataType.Floating;
		}

		public string Init(string strLine, LasVersion v)
		{
			strLine = strLine.Trim();
			//MNEM
			int k = strLine.IndexOf('.');
			if (k <= 0)
				return "no dot";
			MNEM = strLine.Substring(0, k).Trim();
			strLine = strLine.Substring(k + 1, strLine.Length - k - 1);
			//Unit
			k = strLine.IndexOf(' ');
			if (k < 0)
				return "no space for unit";
			if (k > 0)
			{
				UNITS = strLine.Substring(0, k).Trim();
				strLine = strLine.Substring(k + 1, strLine.Length - k - 1);
			}
			strLine = strLine.Trim();
			//Data
			k = strLine.IndexOf(':');
			if (k < 0)
				return "no : for Data";
			if (k > 0)
			{
				DATA = strLine.Substring(0, k).Trim();
				strLine = strLine.Substring(k + 1, strLine.Length - k - 1);
			}
			else
			{
				if (k == 0)
					strLine = strLine.Remove(0, 1);
			}


			if (v == LasVersion.V20)
			{
				DESCRIPTION = strLine.Trim();
				return null;
			}
			DESCRIPTION = strLine.Trim();
			k = strLine.IndexOf('{');
			if (k > 0)
			{
				DESCRIPTION = strLine.Substring(0, k).Trim();

				int k1 = strLine.IndexOf('}');
				if (k1 <= 0)
					return "missing '}' ";

				FORMAT = strLine.Substring(k + 1, k1-k-1).Trim();
				strLine = strLine.Substring(k1 + 1, strLine.Length - k1 - 1).Trim();
			}
			k = strLine.IndexOf('|');
			if (k > 0)
			{
				DESCRIPTION = strLine.Substring(0, k).Trim();
				ASSOCIATION = strLine.Substring(k + 1, strLine.Length - k - 1).Trim();
				return null;
			}
			return null;
		}

		public string ToString1()
		{
			string str = MNEM;
			str = str.PadRight(5, ' ');
			str = str + "." + UNITS;
			int k = 33;
			if (UNITS != null)
				k -= UNITS.Length;
			string d = DATA == null? "" : DATA;
			d = d.PadLeft(k);
			str = str + d;
			str = str + ": " + DESCRIPTION;
			str = str.PadRight(72, ' ');
			if(format != null)
				str = str + "{" + FORMAT + "}";
			if(ASSOCIATION != null)
				str = str + "|" + ASSOCIATION;
			return str;
		}
	}

	public class LasSectionRows : List<LasSectionRow>
	{
		public LasSectionRow GetLasSectionRow(string MNEM)
		{
			return this.Where(r => r.MNEM == MNEM).FirstOrDefault();
		}
	}
}
