using System;
using System.IO;
using System.Data;
using System.Collections;
using OpenWLS.Server.Base;
using OpenWLS.Server.DBase.Models.CalibrationDb;

//using System.Drawing;
//using System.Windows.Forms;
//using System.Windows.Forms.PropertyGridInternal;


namespace OpenWLS.Server.Calibration
{
    public enum CVDataRecordType { Unknown, CMX, CVD };
    public class CVRecordValue
    {
        CV1DValue[] cv1DValues;
        double[] cvMatrixes;

        #region Properties

        public bool Passed
        {
            get
            {
                bool pass = true;
                foreach (CV1DValue c1d in cv1DValues)
                {
                    if (c1d.OutOfLimit)
                        pass = false;
                }
                return pass;
            }
        }

        public int Count1D
        {
            get
            {
                return cv1DValues.Length;
            }
        }

        public CV1DValue this[int index]
        {
            get
            {
                return cv1DValues[index];
            }
        }

        public CV1DValue this[string name]
        {
            get
            {
                foreach (CV1DValue cvv in cv1DValues)
                    if (cvv.Name == name)
                        return cvv;
                return CV1DValue.Empty;
            }
        }

        public int Length
        {
            get
            {
                int l = 52 * cv1DValues.Length + 12; // 3 lengthes
                if (cvMatrixes != null)
                    l += cvMatrixes.Length * 4;
                return l;
            }
        }

        public double[] Matrixes
        {
            get
            {
                return cvMatrixes;
            }
            set
            {
                cvMatrixes = value;
            }
        }
        #endregion

        public static CVRecordValue FromCMX(DataReader r, int cv1Ds, int matrixes)
        {
            CVRecordValue newDataRecord = new CVRecordValue();

            newDataRecord.cv1DValues = new CV1DValue[cv1Ds];
            newDataRecord.cvMatrixes = new double[matrixes];
            for (int i = 0; i < cv1Ds; i++)
            {
                newDataRecord.cv1DValues[i] = new CV1DValue();
                newDataRecord.cv1DValues[i].ReadCVValueFromCMX(r);
            }
            for (int i = 0; i < matrixes; i++)
                newDataRecord.cvMatrixes[i] = r.ReadSingle();
            for (int i = 0; i < cv1Ds; i++)
                newDataRecord.cv1DValues[i].UpdateCMXRecord(newDataRecord.cvMatrixes);

            return newDataRecord;
            //
            // TODO: Add constructor logic here
            //
        }

        public static CVRecordValue FromCVD(FileStream fs, long location, int length)
        {
            if (fs != null && fs.Length >= location + length)
            {
                fs.Seek(location, SeekOrigin.Begin);
                DataReader r = new DataReader(fs, length);
                CVRecordValue newDataRecord = new CVRecordValue();
                if (newDataRecord.UpdateDataRecord(r, length))
                    return newDataRecord;
            }
            return null;
        }

        public static CVRecordValue FromCVD(DataReader r, int length)
        {
            CVRecordValue newDataRecord = new CVRecordValue();
            if (newDataRecord.UpdateDataRecord(r, length))
                return newDataRecord;
            return null;
        }

        public bool UpdateDataRecord(DataReader r, int length)
        {
            int l2 = r.ReadInt32();
            if (l2 == length || 0 == length)
            {
                //   r.Seek(CVRecord.RecordSize, SeekOrigin.Current);
                int cv1ds = r.ReadInt32();
                int cvxds = r.ReadInt32();
                if (52 * cv1ds + 4 * cvxds + 12 == length || 0 == length)
                {

                    cv1DValues = new CV1DValue[cv1ds];
                    cvMatrixes = new double[cvxds];
                    for (int i = 0; i < cv1ds; i++)
                    {
                        cv1DValues[i] = new CV1DValue();
                        cv1DValues[i].ReadCVValueFromCVD(r);
                    }
                    for (int i = 0; i < cvxds; i++)
                        cvMatrixes[i] = r.ReadSingle();
                    return true;
                }
            }
            return false;
        }

        public CVRecordValue()
        {

        }
     /*
        public void AppendDataRecord(string strFN, CVRecord cvIR)
        {
            FileStream fs = new FileStream(strFN, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            AppendDataRecord(fs, cvIR);
            fs.Close();
        }
   
        public void AppendDataRecord(FileStream fs, CVRecord cvIR)
        {
            DataReader r = new DataReader(40);
            fs.Seek(0, SeekOrigin.Begin);
            fs.Read(r.GetBuffer(), 0, 40);
            if (r.ReadString(40).StartsWith(CVRecordSs.cvDataFileDescription))
            {
                fs.Seek(-3, SeekOrigin.End);
                fs.Read(r.GetBuffer(), 0, 3);
                r.Seek(0, SeekOrigin.Begin);
                if (r.ReadString(3) == "EOF")
                {
                    fs.Seek(-3, SeekOrigin.End);
                    int l = Length + CVRecordS.RecordSize;
                    cvIR.SetLengthLocation(fs.Length - 3 + CVRecordS.RecordSize, Length);

                    DataWriter w = new DataWriter(l);

                    WriteDataRecord(w, cvIR);

                    fs.Write(w.GetBuffer(), 0, l);

                    fs.Write(r.GetBuffer(), 0, 3);
                }
            }
        }
       

*/

       
     
        public byte[] GetDataArray()
        {
            DataWriter w = new DataWriter(Length);
            int cvMatrixesLength = cvMatrixes == null ? 0 : cvMatrixes.Length;
            w.WriteData(Length);
            w.WriteData(cv1DValues.Length);
            w.WriteData(cvMatrixesLength);
            foreach (CV1DValue cvv in cv1DValues)
                cvv.SaveCVValueToCVD(w);
            if (cvMatrixes != null)
            {
                for (int i = 0; i < cvMatrixes.Length; i++)
                    w.WriteData(cvMatrixes[i]);
            }
            return w.GetBuffer();
        }
        public int GetCV1DValueIndex(string strName)
        {
            for (int i = 0; i < cv1DValues.Length; i++)
                if (cv1DValues[i].Name == strName)
                    return i;
            return -1;
        }

        public double GetCV1DValue(string strName)
        {
            for (int i = 0; i < cv1DValues.Length; i++)
                if (cv1DValues[i].Name == strName)
                    return cv1DValues[i].Value;
            return double.NaN;
        }

        public void SetItemValue(string strItem, double data)
        {
            foreach (CV1DValue cv1Dv in cv1DValues)
            {
                if (cv1Dv.Name == strItem)
                    cv1Dv.SetValue(data);
            }
        }


    }


    public class CV1DValue
    {
        string name;
        string units;
        double data;
        //      double dataTemp;
        bool limitHighAvailable;
        bool limitLowAvailable;
        double limitHigh;
        double limitLow;
        int[] unkownInts;
        int[] indexes;
        string units1;
        byte[] unkownBytes;


        public static CV1DValue Empty
        {
            get
            {
                return new CV1DValue();
            }
        }

        #region Properties

        public bool OutOfLimit
        {
            get
            {
                if (limitHighAvailable && data > limitHigh)
                    return true;
                if (limitLowAvailable && data < limitLow)
                    return true;
                return false;
            }
        }
        public string Name
        {
            get
            {
                return name;
            }
        }
        public string Units
        {
            get
            {
                return units;
            }
        }
        public double Value
        {
            get
            {
                return data;
            }
        }
        public double LimitHigh
        {
            get
            {
                return limitHigh;
            }
        }

        public double LimitLow
        {
            get
            {
                return limitLow;
            }
        }

        public bool LimitHighAvailable
        {
            get
            {
                return limitHighAvailable;
            }
        }

        public bool LimitLowAvailable
        {
            get
            {
                return limitLowAvailable;
            }
        }

        #endregion

        public CV1DValue()
        {

        }

        internal void SetValue(double newValue)
        {
            data = newValue;
        }




        public void SaveDataRow(DataRow dr)
        {
            dr["Name"] = name;
            if (units != null)
                dr["Unit"] = units;
            if (limitLowAvailable)
                dr["Min"] = limitLow;
            if (limitHighAvailable)
                dr["Max"] = limitHigh;

            dr["Value"] = data;
        }

        public void LoadData(DataRow dr)
        {
            name = (string)dr["name"];
            units = dr.Table.Columns.Contains("Unit") && dr["Unit"] != DBNull.Value ? (string)dr["Unit"] : "";
            if (dr.Table.Columns.Contains("Min") && dr["Min"] != DBNull.Value)
            {
                limitLowAvailable = true;
                limitLow = Convert.ToDouble(dr["Min"]);
            }
            else
                limitLowAvailable = false;

            if (dr.Table.Columns.Contains("Max") && dr["Max"] != DBNull.Value)
            {
                limitHighAvailable = true;
                limitHigh = Convert.ToDouble(dr["Max"]);
            }
            else
                limitHighAvailable = false;

            data = Convert.ToDouble(dr["Value"]);
        }

        string GetShortString(string str)
        {
            int l = str.Length;
            if (l > 6)
            {
                int i = str.IndexOf(".");
                return str.Substring(0, 6);
            }
            return str;
        }

        /*

                public void DrawItem(Graphics g, RectangleF rect, Font dataFont, Font toleranceFont, string dataFormat, StringFormat sf)
                {
                    if(!double.IsNaN(data))
                    {
                        string str = dataFormat == null? data.ToString() : data.ToString(dataFormat);
                        if(recording)
                            str = dataFormat == null ? dataTemp.ToString() : dataTemp.ToString(dataFormat);
                        SizeF size = g.MeasureString(str,dataFont);
                        while(size.Width >= rect.Width)
                        {
                            str = str.Remove(str.Length-1, 1);
                            size = g.MeasureString(str,dataFont);
                        }
                        rect.Height = dataFont.Height;
                        if(highLight)
                            g.FillRectangle(Brushes.Yellow, rect);
                        if(OutOfLimit)
                            g.DrawString(str, dataFont, Brushes.Red, rect,sf);
                        else
                            g.DrawString(str, dataFont, Brushes.DarkGreen, rect,sf);
                    }
                    if(!this.highLight)
                    {
                        string strLimit = "(";
                        if(limitLowAvailable)
                            strLimit = strLimit + GetShortString(limitLow.ToString());
                        if(limitHighAvailable)
                            strLimit = strLimit + " , " + GetShortString(limitHigh.ToString()) + ")";
                        if(strLimit.Length > 1)
                        {
                            rect.Y += dataFont.Height;
                            rect.Height = toleranceFont.Height;
                            g.DrawString(strLimit, toleranceFont, Brushes.Black, rect,sf);
                        }
                    }
                    this.highLight = false;
                }

        /*
                public void DrawItem(C1.C1Pdf.C1PdfDocument pdfDoc, RectangleF rect, Font dataFont, Font toleranceFont, string dataFormat, StringFormat sf)
                {
                    if(!double.IsNaN(data))
                    {
                        string str = dataFormat == null? data.ToString() : data.ToString(dataFormat);
                        if(recording)
                            str = dataFormat == null ? dataTemp.ToString() : dataTemp.ToString(dataFormat);
                        SizeF size = pdfDoc.MeasureString(str, dataFont);
                        while(size.Width >= rect.Width)
                        {
                            str = str.Remove(str.Length-1, 1);
                            size = pdfDoc.MeasureString(str, dataFont);
                        }
                        rect.Height = dataFont.Height;
                        if(highLight)
                            pdfDoc.FillRectangle(Brushes.Yellow, rect);
                        if(OutOfLimit)
                            pdfDoc.DrawString(str, dataFont, Brushes.Red, rect, sf);
                        else
                            pdfDoc.DrawString(str, dataFont, Brushes.DarkGreen, rect, sf);
                    }
                    if(!this.highLight)
                    {
                        string strLimit = "(";
                        if(limitLowAvailable)
                            strLimit = strLimit + GetShortString(limitLow.ToString());
                        if(limitHighAvailable)
                            strLimit = strLimit + " , " + GetShortString(limitHigh.ToString()) + ")";
                        if(strLimit.Length > 1)
                        {
                            rect.Y += dataFont.Height;
                            rect.Height = toleranceFont.Height;
                            pdfDoc.DrawString(strLimit, toleranceFont, Brushes.Black, rect, sf);
                        }
                    }
                    this.highLight = false;
                }
        */

        public void ReadCVValueFromCMX(DataReader r)
        {
            name = r.ReadString1(20);
            unkownInts = new int[3];
            unkownInts[0] = r.ReadInt32();
            unkownInts[1] = r.ReadInt32();
            data = r.ReadSingle();
            limitLowAvailable = r.ReadInt32() == 0 ? false : true;
            limitLow = r.ReadSingle();
            limitHighAvailable = r.ReadInt32() == 0 ? false : true;
            limitHigh = r.ReadSingle();
            unkownInts[2] = r.ReadInt32();
            units = r.ReadString1(21);
            units1 = r.ReadString1(21);
            if (unkownInts[2] != 0x20001)
            {
                unkownBytes = r.ReadByteArray(74, false);
                indexes = new int[3];
                indexes[0] = r.ReadInt32() / 4;
                indexes[1] = r.ReadInt32() / 4;
                indexes[2] = r.ReadInt32() / 4;
                r.Seek(16, SeekOrigin.Current);
            }
            else
                unkownBytes = r.ReadByteArray(102, false);

            //			r.Seek(102, SeekOrigin.Current);
        }

        public void ReadCVValueFromCVD(DataReader r)
        {
            name = r.ReadString1(20);
            units = r.ReadString1(20);
            data = r.ReadSingle();
            limitLow = r.ReadSingle();
            limitHigh = r.ReadSingle();
            limitHighAvailable = !double.IsNaN(limitHigh);
            limitLowAvailable = !double.IsNaN(limitLow);

        }

        public void SaveCVValueToCVD(DataWriter w)
        {
            w.WriteString(name, 20, 0);         //20
            w.WriteString(units, 20, 0);        //40
            w.WriteData((float)data);                  //44
            if (limitLowAvailable)
                w.WriteData((float)limitLow);           //48
            else
                w.WriteData(float.NaN);
            if (limitHighAvailable)
                w.WriteData((float)limitHigh);          //52
            else
                w.WriteData(float.NaN);
        }

        /*
		public void InitCellItem(ListViewItem lvi)
		{
			lvi.Text = name;
			if(units != null)
				lvi.SubItems.Add(units);
			else
				lvi.SubItems.Add("");

			lvi.SubItems.Add(data.ToString());
			if(limitHighAvailable)
			{
				lvi.SubItems.Add(limitLow.ToString());
				lvi.SubItems.Add(limitHigh.ToString());
			}
			else
			{
				lvi.SubItems.Add("");
				lvi.SubItems.Add("");
			}

		}
        */
        public void UpdateCMXRecord(double[] matrix)
        {
            if (matrix == null || matrix.Length == 0 || indexes == null)
                return;
            if (indexes[0] <= matrix.Length)
            {
                data = matrix[indexes[0]];

                if (unkownInts[2] == 0x20010)
                {
                    limitLow = matrix[indexes[1]];
                    limitHigh = matrix[indexes[2]];
                }
            }
        }

    }

}
