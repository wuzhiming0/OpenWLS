using System;
using System.IO;
using System.ComponentModel;

using System.Data;
using System.Collections;
using System.Collections.Specialized;

using OpenWLS.Server.Base;
using OpenWLS.Server.LogDataFile;
using OpenWLS.Server.LogDataFile.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OpenWLS.Server.LogDataFile.XTF
{
    /// <summary>
    /// It contains all information about the curve
    /// </summary>
    //	[Serializable()]
    public class XtfCurve : XtfItem
	{
        public  delegate object ReadData();
        ReadData ReadXtfData;

        public void GetDataReader(XtfDataType xdt, DataReader r)
        {
            switch (xdt)
            {
                case XtfDataType.Byte:
                    ReadXtfData = r.ReadByteObject;
                    break;
                case XtfDataType.Int4:
                    ReadXtfData = r.ReadInt32Object;
                    break;
                case XtfDataType.Int2:
                    ReadXtfData = r.ReadInt16Object;
                    break;
                case XtfDataType.Character:
                    ReadXtfData = r.ReadSByteObject;
                    break;
                case XtfDataType.UInt2:
                    ReadXtfData = r.ReadUInt16Object;
                    break;
                case XtfDataType.UInt4:
                    ReadXtfData = r.ReadUInt32Object;
                    break;
                case XtfDataType.Single:
                    ReadXtfData = r.ReadSingleObject;
                    break;
                case XtfDataType.Double:
                    ReadXtfData = r.ReadDoubleObject;
                    break;
            }

        }

        #region Fields
        //		string			name;
        /// <summary>
        /// curve Head Data entry offset from the file begining
        /// </summary>
        int				headOffset;			

		/// <summary>
		/// Curve Head
		/// </summary>
        internal BinaryDataBlock head;

		/// <summary>
		/// its Composite curves defination if it's 
		/// </summary>
		internal XtfCompositeCurveHead []	compositeHeads;

		/// <summary>
		/// its Struct curves defination if it's
		/// </summary>
		internal XtfStructCurveHead      structHead;


		/// <summary>
		/// Data byte order
		/// </summary>
		internal bool		normalOder;


		internal int sizePerSample;
        int samples;
        int dims;

		internal int dataBlocks;

		int offsetExtendHead;
		int offsetAnnotrcs;
		int offsetStrucrcs;
		int offsetcompHead;

		//		XtfCurve[] curves;

		#endregion

		#region Methods	
		/// <summary>
		/// Constuctor
		/// </summary>
		public XtfCurve()
		{
			itemType = XtfItemType.Curve;
			dataBlocks = -1;
		}

		/// <summary>
		/// Read curve head from the xtf file
		/// </summary>
		/// <param name="strName"></param>curve name
		/// <param name="fStream"></param>xtf file
		/// <param name="iOffset"></param>offset from the file begining
		/// <param name="bNormalOder"></param>byte order
		/// <returns></returns>
		public bool	LoadCurveHead( FileStream fStream, int iOffset,  DataRow def_row)
		{
			//set general information
            head = new BinaryDataBlock();
            head.SetDefination(def_row, (iOffset-1) * 4096);
			fs = fStream;

            head.LoadDataBlock(fs);

			if(ICTYPE == XtfCurveType.Structure)
			{
                return false;
			//	structHead = new XtfStructCurveHead();
			//	structHead.ReadAnnotation(fs, ((int[])head["ANNOTRCS"])[0], normalOder);
            //  structHead.ReadDef(fs, ((int[])head["STRUCRCS"])[0], normalOder);
			//	structHead.CalcCurveOffsets((int)head["NLEVLS"]);
			}
			if(ICTYPE == XtfCurveType.Composite)
			{
                return false;
			//	this.compositeHeads = new XtfCompositeCurveHead[(int)head["COELECNT"]];
			//	fs.Seek(((int)this.head["COMPSTRT"] - 1 ) * 4096, SeekOrigin.Begin);
			//	for(int i = 0; i < compositeHeads.Length; i++)
			//	{
			//		compositeHeads[i] = new XtfCompositeCurveHead();
			//		compositeHeads[i].ReadDefinition(fs, this.normalOder);
			//	}

			}
			begin = headOffset;
			name = ((string)head["CHCURV"]).Trim();
            ComputeDataSize();
	
			//Size = head["LASTREC"] - iOffset + 1;
			return true;
		}

		/// <summary>
		/// Read Extended Curve Header Tables
		/// </summary>
		/// <returns></returns>DataSet
		public DataSet GetCurveExtandTables()
		{
			if(HasExtendHeader)
			{
				XtfCurveHeadExtend extendHeader = new XtfCurveHeadExtend();
                int i0 = ((int[])head["EXHDINDX"])[0];
                int i1 = ((int[])head["EXHDINDX"])[1];
				DataSet tables = extendHeader.ReadExtendHead(fs, i0, i1, normalOder);
				tables.DataSetName = Name;
				return tables;
			}
			return null;
		}

        public void LoadData(bool littleEndian, MHead h, DataFile df)
        {
            XtfDataType xtfDType = (XtfDataType)(short)head["IDTYPE"];
			Measurement m = new Measurement(h, df);

			DataReader r = new DataReader(fs,  ComputeDataSize());
			byte[] bs = r.GetBuffer();
			m.CreateMVWriter(bs);
			if(!littleEndian)
				DataType.ChangeEndian(m.Head.GetElementBytes(), bs);
				

			h.VMin = (float)head["CURVMIN"];
            h.VMax = (float)head["CURVMAX"];
			h.VAvg = (float)head["CURVAVG"];
			m.MVWriter.StartIndex = DBTOPDEP;
			m.MVWriter.StopIndex = DBBOTDEP;
			m.MVWriter.Postion = bs.Length;

            m.AddMVBlock();
		//	return w;

        }

        public MHead GetChannelHead(int seq)
        {
            MHead h = new MHead();
            h.Id = seq;
            h.Name = Name;
            h.UOM = (string)head["CHUNIT"];
            if (h.UOM != null)
                h.UOM = h.UOM.Trim();
            h.DType = DataType.GetSparkPlugDataType(IDTYPE);
			int[] dims = new[] { 1 };

			if (IDIMS3 > 0)
				dims = new int[] { IDIMS3, IDIMS2, IDIMS1 };
			else
			{
				if (IDIMS2 > 0)
					dims = new int[] { IDIMS2, IDIMS1 };
				else
				{
					if (IDIMS1 > 1)
						dims = new int[] { IDIMS1 };
				}
			}
			h.DataAxes = Base.DataAxes.CreateDataAxes(dims).ToString();

            h.Spacing = DBRLEVEL;
            h.UOI = CHDEPUNT;
     //       h.Samples = samples;
			double[] ds = (double[])head["SMVALSDB"];	//Array ofsix(6) secondary missing datum values
            int[] ints = (int[])head["SMVALSI4"];		//Array ofsix(6) secondary missing datum values
            h.VEmpty = ds[0];
            return h;
        }

		/// <summary>
		/// Calculate the curve data, extend, struct/composite head size
		/// </summary>
		/// <returns></returns>the curve size in 4096, including head, data, extend head, struct/composite head. 
		public override int ComputeDataSize()
		{
			//
			int ds = DataType.GetDataSize((XtfDataType)(short)head["IDTYPE"]);
			dims = (short)head["IDIMS1"];
			if((short)head["IDIMS2"] > 0)
                dims = dims * (short)head["IDIMS2"];
			if((short)head["IDIMS3"] > 0)
                dims = dims * (short)head["IDIMS3"];
			sizePerSample = dims * ds;

            //
            samples = (int)Math.Abs(( DBBOTDEP - DBTOPDEP ) / DBRLEVEL) + 1;
            head["NLEVLS"] = samples;
            return sizePerSample * samples;

		}
	
        #endregion

		#region	1General Properties

		[Category("1General")]
		[Description("Name")]
		public string Name
		{
			get
			{ return name; }
		}

		[Category("1General")]
		[Description("Data Type")]
		public XtfDataType	IDTYPE
		{
			get
			{ 
					return (XtfDataType)(short)head["IDTYPE"];
			}
		}
		[Category("1General")]
		[Description("Curve Type")]
		public XtfCurveType	ICTYPE	
		{
			get
			{	return (XtfCurveType)(short)head["ICTYPE"];			}
		}
		[Category("1General")]
		[Description("Vertical index type")]
		public XtfVHSampleType	IVTYPE					
		{
			get
			{ 	return  (XtfVHSampleType)(short)head["IVTYPE"];		}
		}	
		[Category("1General")]
		[Description("Horizontal index type")]
		public XtfVHSampleType	IHTYPE
		{
			get
			{	return  (XtfVHSampleType)(short)head["IHTYPE"];			}
		}
		[Category("1General")]
		[Description("Datum initialization value for 8 byte double precision data")]
		public double	DBLINIT
		{
			get
			{
				if(	IDTYPE == XtfDataType.Int2)
					return (double)(short)head["I2INIT"];
				else
				{
					if(IDTYPE == XtfDataType.UInt4)
						return (double)(int)head["I4INIT"];
					else
					{
						if(IDTYPE == XtfDataType.Single)
							return (double)(float)head["R4INIT"];
						else
							return  (double)head["DBLINIT"];
					}
				}
			}
		}
		[Category("1General")]
		[Description("Long curve description")]
		public string	CURVDESC
		{
			get
			{
					return  (string)head["CURVDESC"];
			}
		}
		[Category("1General")]
		[Description("Long curve unit description")]
		public string	LONGUNIT
		{
			get
			{	return  (string)head["LONGUNIT"];			}
		}
		[Category("1General")]
		[Description("Long curve name")]
		public string	LONGNAME
		{
			get
			{	return  (string)head["LONGNAME"];			}
		}
		[Category("1General")]
		[Description("Extended Header")]
		public  bool  HasExtendHeader
		{
			get
			{	return  (int)head["EXHDSIG"] == 12548;			}
		}
		#endregion

		#region	Depth and level spacing Properties

        public int[] ANNOTRCS
        {
            get { return ((int[])head["ANNOTRCS"]);  }
        }

        public int[] STRUCRCS
        {
            get { return ((int[])head["STRUCRCS"]); }
        }

        public int[] EXHDINDX
        {
            get { return ((int[])head["EXHDINDX"]); }
        }
		[Category("2 Depth and Level Spacing")]
		[Description("Double precision top index value")]
		public double DBTOPDEP
		{
			get
			{  return  (double)head["DBTOPDEP"];}
			set
			{
				head["DEPTOP"] = (float)value;
				head["DBTOPDEP"] = value;
			}
		}
		[Category("2 Depth and Level Spacing")]
		[Description("Double precision bottom index value")]
		public double DBBOTDEP
		{
			get
			{  return  (double)head["DBBOTDEP"];}
			set
			{
				head["DEPBOT"] = (float)value;
				head["DBBOTDEP"] = value;
			}
		}
		[Category("2 Depth and Level Spacing")]
		[Description("Double precision level spacing")]
		public double DBRLEVEL
		{
			get
			{  return  (double)head["DBRLEVEL"];}
			set
			{
				head["DBRLEVEL"] = value;
				head["RLEVCV"] = (float)value;
			}
		}

		[Category("2 Depth and Level Spacing")]
		[Description("Depth units mnemonics for this curve")]
		public string CHDEPUNT
		{
			get
			{
				return (string)this.head["CHDEPUNT"];
			}
		}


		#endregion
		#region	3 Dimensions Properties

		[Category("3 Dimensions")]
		[Description("Number of dimensions")]
		public short NDIMS
		{
			get
			{ 
				return  (short)head["NDIMS"];
			}
		}
		[Category("3 Dimensions")]
		[Description("Number of elements in dimension 1")]
		public short IDIMS1
		{
			get
			{
				return  (short)head["IDIMS1"];
			}
		}
		[Category("3 Dimensions")]
		[Description("Number of elements in dimension 2")]
		public short IDIMS2
		{
			get
			{	return  (short)head["IDIMS2"]; }
		}
		[Category("3 Dimensions")]
		[Description("Number of elements in dimension 3")]
		public short IDIMS3
		{
			get
			{
				return  (short)head["IDIMS3"];
			}
		}
		#endregion 

        /*
		#region 4 Date Time
		[Category("4Date Time ")]
		[Description("Encode create date & time")]
		public DateTime IEC_DATE_TIME
		{
			get
			{
				return head.GetTimeDate();
			}
		}
		[Category("4Date Time ")]
		[Description("Last access create date & time")]
		public DateTime IEA_DATE_TIME
		{
			get
			{
				return head.GetTimeDate();
			}
		}
		#endregion
	*/
		#region 5
		public override int Begin
		{
			get 
			{
				return begin;
			}
			set
			{
				begin = value;
				headOffset = begin;
				if(dataBlocks > 0)
				{
					//curve data
					head["LASTREC"] = begin + dataBlocks;	//plus the curve head 1, so do not - 1
					//struct
                    ANNOTRCS[1] = offsetAnnotrcs + begin + ANNOTRCS[1] - ANNOTRCS[0];
                    ANNOTRCS[0] = offsetAnnotrcs + begin;
                    STRUCRCS[1] = offsetStrucrcs + begin + STRUCRCS[1] - STRUCRCS[0];
                    STRUCRCS[0] = offsetStrucrcs + begin;
					//composite head
					head["COMPEND"] = offsetcompHead + begin + (int)head["COMPEND"] - (int)head["COMPSTRT"];
					head["COMPSTRT"] = offsetcompHead + begin;
					//extend head
					if(HasExtendHeader)
					{
                        int offsetExt = EXHDINDX[0];
                        int e0 = offsetExtendHead + begin;
                        EXHDINDX[0] = e0;

                        offsetExt = e0 - offsetExt;
                        XtfCurveHeadExtend.OffsetExtendHeadDef(fs, e0, EXHDINDX[1], this.normalOder, offsetExt);
					}
				}

			}
		}

		[BrowsableAttribute(false)]
		public   XtfStructCurveHead      StructHead
		{
			get
			{
				return structHead;
			}
		}
		#endregion

	}

	/// <summary>
	/// for convert xtf struct/composite curve to Plf curve
	/// </summary>
	public class FrameCurveData
	{
//		public LogCurveData curve;
		public int offset;
		public int count;
		public byte[] buffer;
	}

    

}
