using System;
using System.IO;
using System.Data;
using OpenWLS.Server.Base;

namespace OpenWLS.Server.LogDataFile.XTF
{
	/// <summary>
	/// 
	/// </summary>
	public class XtfCurveHeadExtend
	{
		public XtfCurveHeadExtend()
		{
			// 
			// TODO: Add constructor logic here
			//
		}
		#region Defination
		string[]		tablename;
		int[]			tablecode;
		int[]			tablelen;
		int[]			tableloc;
		int[]			offset;
		int[]			flags;
		int				tableLink;
//		int			unused;
	//	XtfCurveExtendTable[] tbl;
	//	XtfCurveExtendTable[] tbl;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fs"></param>
		/// <param name="iOffset"></param>
		/// <param name="count"></param>
		/// <param name="bNormalOrder"></param>
		/// <param name="offsetExt"></param>
		public static void OffsetExtendHeadDef(FileStream fs, int iOffset, int count, bool bNormalOrder, int offsetExt)
		{
			long seek = (iOffset-1) * 4096;
			fs.Seek( seek, SeekOrigin.Begin);
			DataReader r = new DataReader(fs,  3924);
			r.SetByteOrder(bNormalOrder);
			DataWriter w = new DataWriter( r.GetBuffer());
			w.SetByteOrder(bNormalOrder);
			int i = 0;
			while( i < count)
			{
				//256 + 4 (table code) + 4 (tablelen) = 264
				r.Seek(264,SeekOrigin.Current);
				w.Seek(264, SeekOrigin.Current);
				int j = r.ReadInt32();
				//table loc
				w.WriteData((int)(j + offsetExt));
				r.Seek(12, SeekOrigin.Current);
				w.Seek(12, SeekOrigin.Current);
				i++;
				if(i % 14 == 0)
				{
					w.WriteToStreamAll(fs, seek);
					seek = (iOffset-1 + r.ReadInt32()) * 4096;
					fs.Seek(seek, SeekOrigin.Begin);
					r = new DataReader(fs,  3924);
					w = new DataWriter(r.GetBuffer());
					r.SetByteOrder(bNormalOrder);
					w.SetByteOrder(bNormalOrder);
				}
			}
			w.WriteToStreamAll(fs, seek);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="fs"></param>
		/// <param name="iOffset"></param>
		/// <param name="count"></param>
		/// <param name="bNormalOrder"></param>
		/// <returns></returns>
		public int ReadExtendHeadDef(FileStream fs, int iOffset, int count, bool bNormalOrder)
		{
			tablename = new string[count];
			tablecode = new int[count];
			tablelen = new int[count];
			tableloc = new int[count];
			offset = new int[count];
			flags = new int[count];
			fs.Seek((iOffset-1) * 4096, SeekOrigin.Begin);
			DataReader r = new DataReader(fs,  3924);
			r.SetByteOrder(bNormalOrder);
			int i = 0;
			while( i < count)
			{
				tablename[i] = r.ReadString(256);
				tablecode[i] = r.ReadInt32();
				tablelen[i] = r.ReadInt32();
				tableloc[i] = r.ReadInt32();
				offset[i] = r.ReadInt32();
				flags[i] = r.ReadInt32();
				r.Seek(4,SeekOrigin.Current);
				i++;
				if(i % 14 == 0)
				{
					tableLink =  r.ReadInt32();
					fs.Seek((iOffset-1 + tableLink) * 4096, SeekOrigin.Begin);
					r = new DataReader(fs,  3924);
					r.SetByteOrder(bNormalOrder);
				}
			}
			i = tablelen[count - 1] / 4096;
			if(( tablelen[count - 1] - i * 4096) > 0)
				i++;
			i += tableloc[count - 1] - tableloc[0];
			i++;
			return i;
		}


		public DataSet ReadExtendHead(FileStream fs, int iOffset,int count, bool bNormalOrder)
		{
			DataSet tables = new DataSet("CurveExtendHead");
			ReadExtendHeadDef(fs, iOffset, count, bNormalOrder);
			XtfCurveExtendTable tbl = new XtfCurveExtendTable();
			for( int i = 0; i < count; i++)
			{
			//	tbl = new XtfCurveExtendTable();
				DataTable table = tbl.GetDataTable(fs, (tableloc[i] - 1 ) * 4096, bNormalOrder );
				table.TableName = tablename[i].Trim();
				tables.Tables.Add(table);
			}
			return tables;
		}
/*
		public void AddToParameters(ParameterCollection parameters, FileStream fs, int iOffset,int count, bool bNormalOrder)
		{
			if(parameters != null)
			{
				ReadExtendHeadDef(fs, iOffset, count, bNormalOrder);
				XtfCurveExtendTable tbl = new XtfCurveExtendTable() ;
				for( int i = 0; i < count; i++)
					parameters.Add(new Parameter(tablename[i].Trim(), "XTFCurveExtendHeadTable", tbl.GetReadExtendHeadData(fs, (tableloc[i] - 1 ) * 4096, bNormalOrder ) ) );
			}
		}
        */
		#endregion

		public class XtfCurveExtendTable
		{
			public XtfCurveExtendTable()
			{
			}
			#region Dictionary
			short	version;
			short	totalRec;
			int		recSize;
			short	numFld;
			short	firstRec;
			string	tblTitle;
			int		rowSize;

			public DataTable GetDataTable(FileStream fs, int iOffset, bool bNormalOrder)
			{ 
				ReadExtendDic( fs,  iOffset,  bNormalOrder );
				DataReader r = new DataReader(fs,  16 * numFld + recSize * totalRec);
				r.SetByteOrder(bNormalOrder);
				return ReadExtendEntry(r);
			}

			public DataTable GetDataTable(byte[] buffer, bool bNormalOrder)
			{ 
				DataReader r = new DataReader(  buffer);
				r.SetByteOrder(bNormalOrder);
				ReadExtendDic( r );
				return ReadExtendEntry(r);
			}

			void ReadExtendDic(FileStream fs, int iOffset, bool bNormalOrder)
			{
				fs.Seek(iOffset, SeekOrigin.Begin);
				DataReader r = new DataReader(fs,  92);
				r.SetByteOrder(bNormalOrder);
				ReadExtendDic(r);
			}

			void ReadExtendDic(DataReader r)
			{
				version = r.ReadInt16();
				totalRec = r.ReadInt16();
				recSize = r.ReadInt16();
				numFld = r.ReadInt16();
				firstRec = r.ReadInt16();
				tblTitle = r.ReadString(81);
				r.ReadSByte();
			}



			internal byte[] GetReadExtendHeadData(FileStream fs, int iOffset, bool bNormalOrder)
			{
				ReadExtendDic( fs,  iOffset,  bNormalOrder );
				int l = 16 * numFld + recSize * totalRec + 92;
				byte[] buffer = new byte[l];
				fs.Seek(-92, SeekOrigin.Current);
	//			fs.Seek(iOffset, SeekOrigin.Begin);
				fs.Read(buffer, 0, l);
				return buffer;
			}
			#endregion	
			#region Entry
			string[]	dataName;
			XtfDataType[]		dataType;
			short[]		dataLen;
			short[]		dataUnit;

			public DataTable ReadExtendEntry(DataReader r)
			{
				rowSize = 0;
				dataName = new string[numFld];
				dataType = new XtfDataType[numFld];
				dataLen = new short[numFld];
				dataUnit = new short[numFld];
				int i, j;
				DataTable table  = new DataTable();
		//		MyDataReader r = new MyDataReader(fs, bNormalOrder, 16*numFld);
				for(i = 0; i < numFld; i++)
				{
					dataName[i] = r.ReadStringAndTrim(9);
					r.Seek(1, SeekOrigin.Current);
					dataType[i] = (XtfDataType)r.ReadInt16();
					dataLen[i] = r.ReadInt16();
					dataUnit[i] = r.ReadInt16();
					DataColumn newDataColumn = new DataColumn(dataName[i]);
					newDataColumn.DataType = DataType.GetSystemDataType(dataType[i]);
					table.Columns.Add(newDataColumn);
					rowSize = rowSize + (int)dataLen[i];
				}
				for(i = 0; i < totalRec; i++)
				{
		//			r = new MyDataReader(fs, bNormalOrder, rowSize);
					DataRow newRow = table.NewRow();
					for(j = 0; j < numFld; j++)
						newRow[dataName[j]] = r.ReadDataObject(dataType[j], (int)dataLen[j]);
					table.Rows.Add(newRow);
				}
				return table;
			}
			#endregion		
		}
	}


	public class XtfStructCurveHead
	{
	//	internal	int[]	offset;
		internal	int[]	offsetBytes;
	//	internal	XtfCurveData[]	data;
		public XtfStructCurveHead()
		{

		}
		public void CalcCurveOffsets(int iSamples)
		{
		//	offset[0] = 0;
			int i;
			for( i = 1; i <= ngroups; i++)
			{
	//			offset[i] = offset[i-1] + MiscFunctions.GetDataSize((XtfDataType)i2types[i-1]) * i2counts[i -1] * iSamples;
				offsetBytes[i] = offsetBytes[i-1] + DataType.GetDataSize((XtfDataType)i2types[i-1]) * i2counts[i -1];
			}
		}

	
/*
		public FrameCurveData[] CreateNewPlfCurves(CurveInfor curveInfor)
		{
			string strNamePrefix = curveInfor.name;
			FrameCurveData[] frameCurves = new FrameCurveData[this.ngroups];
			curveInfor.dimensions = new int[1];
			for(int i = 0; i < this.ngroups; i++)
			{
				frameCurves[i] = new FrameCurveData();  
				if(this.i2counts[i] > 1)
					frameCurves[i].curve = new PlfXDCurve();
				else
					frameCurves[i].curve = new Plf1DCurve();
				curveInfor.name = strNamePrefix + this.ibannot[i];
				curveInfor.dataType = (XtfDataType)this.i2types[i];
				curveInfor.dimensions[0] = this.i2counts[i];
		//		curveInfor.littleEndian = ( this.ISTO == XtfSystemCode.PC);
				frameCurves[i].curve.CopyCurveInforFrom(curveInfor);
				frameCurves[i].offset = this.offsetBytes[i];
				frameCurves[i].count = this.i2counts[i] * DataType.GetDataSize(curveInfor.dataType);
			}
			return frameCurves;
		}
        */

		#region Defination
		short[]	i2counts = new short[1023];
		short[]	i2types = new short[1023];
		short ngroups;
		sbyte isto_def;
		public void ReadDef(FileStream fs, int iOffset, bool bNormalOrder)
		{
			fs.Seek((iOffset-1) * 4096, SeekOrigin.Begin);
			DataReader r = new DataReader(fs,4096);
			r.SetByteOrder(bNormalOrder);
			int i;
			for( i = 0; i < 1023; i++)
				i2counts[i] = r.ReadInt16();
			for( i = 0; i < 1023; i++)
				i2types[i] = r.ReadInt16();
			ngroups = r.ReadInt16();	
			isto_def = r.ReadSByte();	
			isto_def = r.ReadSByte();
		//	offset = new int[ngroups+1];
			offsetBytes = new int[ngroups+1];
		//	data = new XtfCurveData[ngroups];
		}
		public int[] I2COUNTS
		{
			get
			{
				int [] IA = new int[NGROUPS];
				for(int i = 0; i < NGROUPS; i++ )
					IA[i] = i2counts[i];
				return IA;
			}
		}
		public int[] I2TYPES
		{
			get
			{
				int [] IA = new int[NGROUPS];
				for(int i = 0; i < NGROUPS; i++ )
					IA[i] = i2types[i];
				return IA;
			}
		}

		public int NGROUPS
		{
			get
			{
				return (int)ngroups;
			}
		}

		public XtfSystemCode ISTO
		{
			get
			{
				return (XtfSystemCode)isto_def;
			}
		}
		#endregion
		#region Annotation
		string[] ibannot;
		short	 annotln;
		sbyte	 isto_ann;
		public void ReadAnnotation(FileStream fs, int iOffset, bool bNormalOrder)
		{
			fs.Seek((iOffset-1) * 4096, SeekOrigin.Begin);
			DataReader r = new DataReader(fs,4096);
			r.SetByteOrder(bNormalOrder);

			r.Seek(4092, SeekOrigin.Begin);
			annotln = r.ReadInt16();	
			isto_ann = r.ReadSByte();	
			isto_ann = r.ReadSByte();	
			r.Seek(0, SeekOrigin.Begin);		
			ibannot = r.ReadStrings((char)0, annotln);
		}
		public string [] IBANNOT
		{
			get
			{
				return ibannot;
			}
		}	
		#endregion
	}

}
