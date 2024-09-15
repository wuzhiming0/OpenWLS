using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Windows;

//using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;

using System.Collections.Generic;

using System.Text.Json.Serialization;

using OpenWLS.Server.Base;
using OpenWLS.Server.LogDataFile;
using OpenWLS.Server.LogDataFile.Models;
using OpenWLS.Server.DBase;
using System.Drawing;
using System.Text.Json;
using static OpenWLS.Server.LogDataFile.Models.NMRecord.FileVersion;

namespace OpenWLS.Server.LogDataFile.XTF
{
    public enum XtfSystemCode { PC = 1, Perkin_Elmer, Vax, IBM_mainframe, Unix }
    public enum XtfCurveType { Conventional = 1, Waveform, Matrix, Structure, Composite }
    public enum XtfVHSampleType { Regularly = 1, Irregularly }
    /// <summary>
    /// class inherited from LogFile to access data in Xtf file
    /// </summary>
    public class XtfFile
    {
        DataFile dataFile;

        #region fields
        BinaryDataBlock fhRecord1;
        BinaryDataBlock fhRecord8;
        //H3 Curve Names
        public string[] chcurv = new string[512];
        //H4 Curve Data Addresses
        public int[] i4first = new int[512];
        public int[] nlevs = new int[512];
        //H5 Curve Data Dimensions
        public short[] ndims = new short[512];
        public short[] idims1 = new short[512];
        public short[] idims2 = new short[512];
        public short[] idims3 = new short[512];
        //H6 Curve Top and Bottom Indices
        public float[] topdepcv = new float[512];
        public float[] botdepcv = new float[512];
        //H7 Curve Level Spacings and Types
        public float[] rlevcv = new float[512];
        public XtfCurveType[] ictype = new XtfCurveType[512];
        public XtfDataType[] idtype = new XtfDataType[512];
        public XtfVHSampleType[] ihtype = new XtfVHSampleType[512];
        public XtfVHSampleType[] ivtype = new XtfVHSampleType[512];

        bool validFile = true;
        double top, bottom, levelSpacing;

        /// <summary>
        /// the xtf file stream
        /// </summary>
        FileStream fileStream;
        /// <summary>
        /// curves
        /// </summary>
        List<XtfCurve> curves;
        
        		int			curveNumber;
        //int additionalDTNumber;
     
        /// <summary>
        /// additional data types
        /// </summary>
        //		List<XtfAdditionalDataType>	additionalDataTypes;

        /// <summary>
        /// file head
        /// </summary>
 //       public XtfFileHead fileHead;
        #endregion

        /// <summary>
        /// Set the file type to xtf type
        /// </summary>
        public XtfFile()
        {
            curves = new List<XtfCurve>();
            //			additionalDataTypes = new  List<XtfAdditionalDataType>();
        }

        public FileStream GetFileStream()
        {
            return this.fileStream;
        }
     
        /// <summary>
        /// Load curve heads 
        /// </summary>
        void ImportCurves(DataTable def_dt, bool littleEndian)
        {
            DataRow defRow = BinaryDataBlock.GetDefRow(def_dt, "XTFCurveHeader");
            curveNumber = Convert.ToInt32(fhRecord1["ISNUMCV"]);
  //          curves.Clear();
            //create curve objects and load their heads
            for (int i = 0; i < curveNumber; i++)
            {
                XtfCurve curve = new XtfCurve();
                curve.LoadCurveHead(fileStream, i4first[i], defRow);

                MHead h = curve.GetChannelHead(i);
                curve.LoadData(littleEndian, h, dataFile);
                h.AddHeadToDb(dataFile);

            }
        }


        void SetFileHeadDef(DataTable dt)
        {
            fhRecord1 = new BinaryDataBlock();
            fhRecord8 = new BinaryDataBlock();
            fhRecord1.SetDefination(dt, "XTFFHRecord1", -1);
            fhRecord8.SetDefination(dt, "XTFFHRecord8", -1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iIndex"></param>
        /// <returns></returns>
        int GetCurveIndex(int iIndex)
        {
            int i;
            if ((iIndex & 1) == 1)
                i = 256 + iIndex / 2;
            else
                i = iIndex / 2;
            return i;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fs"></param>
        public void ReadFileHead(FileStream fStream)
        {

            fhRecord1.LoadDataBlock(fStream);
            fhRecord8.LoadDataBlock(fStream);

            fStream.Seek(2 * 4096, SeekOrigin.Begin);

            DataReader r = new DataReader(fStream,  5 * 4096);
            r.SetByteOrder(false);
            int i;	
			//H3 Curve Names
			for( i = 0; i < chcurv.Length; i++)
				chcurv[i] = r.ReadString(8);
			//H4 Curve Data Addresses
			for( i = 0; i < i4first.Length; i++)
				i4first[GetCurveIndex(i)] = r.ReadInt32();
			for( i = 0; i < nlevs.Length; i++)
				nlevs[GetCurveIndex(i)] = r.ReadInt32();
			//HCurve Data Dimensions
			for( i = 0; i < ndims.Length; i++)
			{
				ndims[GetCurveIndex(i)] = r.ReadInt16();
				idims1[GetCurveIndex(i)] = r.ReadInt16();
			}
			for( i = 0; i <idims2.Length; i++)
			{
				idims2[GetCurveIndex(i)] = r.ReadInt16();
				idims3[GetCurveIndex(i)] = r.ReadInt16();
			}
			//HCurve Top and Bottom Indices
			for( i = 0; i <topdepcv.Length; i++)
				topdepcv[GetCurveIndex(i)] = r.ReadSingle();
			for( i = 0; i <topdepcv.Length; i++)
				botdepcv[GetCurveIndex(i)] = r.ReadSingle();
			//H7 Curve Level Spacings and Types
			for( i = 0; i < rlevcv.Length; i++)
				rlevcv[GetCurveIndex(i)] = r.ReadSingle();
			for( i = 0; i < ictype.Length; i++)
			{
				ictype[GetCurveIndex(i)] = ( XtfCurveType)r.ReadByte(); 	
				idtype[GetCurveIndex(i)] = (XtfDataType)r.ReadByte(); 
				ihtype[GetCurveIndex(i)] = (XtfVHSampleType)r.ReadByte(); 
				ivtype[GetCurveIndex(i)] = (XtfVHSampleType)r.ReadByte(); 		
			}
        }
        public static string GetEmbeddedString(string res)
        {
            Assembly _assembly = Assembly.GetExecutingAssembly();
            using (StreamReader sr = new StreamReader(_assembly.GetManifestResourceStream(res)))
            {
                return sr.ReadToEnd();
            }
        }
        public DataFileInfor? ImportXtf(string fileName, ISyslogRepository syslog)
        {
            try
            {
                fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
            catch (Exception e2)
            {
                validFile = false;
                return null;
            }

            try
            {
                string str = GetEmbeddedString("OpenWLS.Server.LogDataFile.XTF.XtfFormat.json");

                DataTable? dt = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(str);
                if(dt == null)
                {
                    syslog.AddMessage($"not able to get data table from {str}", (uint)Color.Red.ToArgb());
                    return null;
                }
                //create and load file head

                SetFileHeadDef(dt);
                ReadFileHead(fileStream);
                //set empty curve and additional datatypes if not a valid xtf file
                string? t1 = Convert.ToString(fhRecord1["CHTYPE"]);
                string? t2 = Convert.ToString(fhRecord8["CH8WSIG"]);
                if ( t1 == null || t2 == null || t1 != ".xtf" ||  t2.Trim()!= "WSI")
                {
                    syslog.AddMessage($"not xtf file format - {t1}", (uint)Color.Red.ToArgb());
                    curves.Clear();
                    validFile = false;
                    return null;
                }
                //load all data types in the file
                bool littleEndian = (byte)fhRecord1["NUMSYS"] == (byte)XtfSystemCode.PC;

                dataFile = DataFile.CreateDataFile($"{fileName}{DataFile.file_ext}", VersionOption.V1, syslog);
               
              //  dataFile.CreateNew( + DataFile.file_ext);
                ImportCurves(dt, littleEndian);

                validFile = true;
                top = Convert.ToDouble(fhRecord1["SURVTOP"]);
                bottom = Convert.ToDouble(fhRecord1["SURVBOT"]);
                levelSpacing = Convert.ToDouble(fhRecord1["SURVRLEV"]);
 
                
                IndexUnit u = Index.ConvertToIndexUnit( (string)fhRecord1["CHUNIT"]);
             //   dataFile.Head.SetDepthRange(u, top, bottom);



                dataFile.Close();
                fileStream.Close();
                return dataFile.GetFileInfor();
            }
            catch (Exception e3)
            {
                if(syslog != null)
                    syslog.AddMessage(e3.Message, (uint)Color.Red.ToArgb());
                validFile = false;
            }
            return null;
        }
        
    }


	


}
