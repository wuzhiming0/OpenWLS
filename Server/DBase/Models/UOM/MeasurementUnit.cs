using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using Newtonsoft.Json;
using OpenWLS.Server.Base;
using System.IO;
using OpenWLS.Server;

namespace OpenLS.Base.UOM
{
    public class MeasurementUnit
    {
        static string db_path;
        static DataTable mdt;
        LinearFunction f;

        public static DataTable MDT
        {
            set { mdt = value; }
            get { return mdt; }
        }
        public static void Init(string dbPath)
        {
            db_path = dbPath;
            // string str = StringConverter.GetEmbeddedString("OpenLS.Base.UOM.uom.json");
            // mdt = (DataTable)JsonConvert.DeserializeObject<DataTable>(str);
            using (FileStream fs = new FileStream(db_path, FileMode.Open))
            using (StreamReader ss = new StreamReader(fs))
            {
                try
                {
                    string str = ss.ReadToEnd();
                    mdt = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(str);
                }
                catch (Exception e)
                {
                }
            }
        }

        public static int Save(string str)
        {
            try
            {
                using (FileStream fs = new FileStream(db_path, FileMode.Open))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(str);
                }
                return 0;
            }
                catch (Exception e)
                {
                }
            return -1;
        }


        protected string unitOrignal;
        protected string unitCur;

        public string Name { get { return unitCur;  } }
        public string Type {get; set;}
        public LinearFunction Func { get { return f; } }

        public static void SetUnitSelection(DataTable dt)
        {
 //           DataTable mdt = MDT;
            foreach (DataRow dr in dt.Rows)
            {
                DataRow tr = GetUnitTypeRow((string)dr["Type"]);
                if (tr != null)
                    tr["Selected"] = dr["Unit"];
            }
        }

        public static void SaveUnitSelectionTable(DataTable dt)
        {
            DataTable mdt = MDT;
            dt.Rows.Clear();
            foreach (DataRow dr in mdt.Rows)
            {
                DataRow tr = dt.NewRow();
                tr["Type"] = dr["Name"];
                tr["Unit"] = dr["Selected"];
                dt.Rows.Add(tr);
            }
        }

        public static DataRow GetUnitTypeRow(string name)
        {
            foreach (DataRow dr in MDT.Rows)
            {
                if ((string)dr["Name"] == name)
                    return dr;
            }
            return null;
        }

        public static DataRow GetUnitRow(string uom, string tom)
        {
            DataRow tu = GetUnitTypeRow(tom);
            if(tu == null)
                return null;
            return GetUnitRow(uom, tu);
        }

        public static string GetSelectedUnit(string tom)
        {
            DataRow tu = GetUnitTypeRow(tom);
            if (tu == null)
                return null;
            return (string)tu["Selected"];
        }
        public static DataRow? GetUnitRow(string uom, DataRow typeRow)
        {
            if (typeRow == null)
                return null;
            DataTable dt = (DataTable)typeRow["Units"];
            string s1 = uom.ToLower();
            foreach (DataRow dr in dt.Rows)
                if ( ((string)dr["Text"]).ToLower() == s1 )
                    return dr;
            return null;
        }
        /*
        static public MeasurementUnit CreateMU(string uom, string tom)
        {
            if (uom == null || t)
                return null;

            return new MeasurementUnit(uom, tom);
        }
        */
        public MeasurementUnit()
        {

        }

        public MeasurementUnit(string uom, string tom)
        {
            unitOrignal = uom;
            unitCur = uom;
            Type = tom;
        }

        static public double GeUnitConvertMul( string unitTo, string unitFrom, string strType)
        {
            DataRow yr = GetUnitTypeRow(strType);
            DataRow? tr = GetUnitRow(unitTo, yr);
            DataRow? fr = GetUnitRow(unitFrom, yr);
            if (tr == null || fr == null)
                return double.NaN;
            return Convert.ToDouble(tr["Mul"]) / Convert.ToDouble(fr["Mul"]);
        }
        static public string GetStdDepthUOM(string str)
        {
            if (str == "meter") return "m";
            if (str == "feet") return "ft";
            return str;
        }
        static public double GetDepthConvertMul(string unitTo, string unitFrom)
        {
            unitTo = GetStdDepthUOM(unitTo);
            unitFrom = GetStdDepthUOM(unitFrom);
            return GeUnitConvertMul(unitTo, unitFrom, "Depth");
        }

        static public double GetDepthConvertMul(string unitFrom)
        {
            string unitTo = GetSelectedUnit("Depth");
            return GeUnitConvertMul(unitTo, unitFrom, "Depth");
        }

        static public double GetTimeConvertMul( string unitTo, string unitFrom)
        {
            return 1;
            //return  GeUnitConvertMul(unitTo, unitFrom, "Time");
        }
            /*
             * y1 = a1 * x1 + b1
             * y2 = a2 * x2 + b2
             * x1 = (y1 - b1) / a1
             * y2 = a2 * (y1 - b1) / a1 + b2
             */
            public void ChangeUnit(string uom, string tom){
            if (tom != Type || tom == null)
                return;
            DataRow tRow = GetUnitTypeRow(tom);
            if (tRow == null)
                return;
            DataRow fromRow = GetUnitRow(unitOrignal, tRow);
            DataRow toRow = GetUnitRow(uom, tRow);
            if (fromRow == null || toRow == null)
                return; 
            double a1 = Convert.ToDouble(fromRow["Mul"]);
            double b1 = Convert.ToDouble(fromRow["Add"]);
            double a2 = Convert.ToDouble(toRow["Mul"]);
            double b2 = Convert.ToDouble(toRow["Add"]);

            double m = a2 / a1;
            double a = b2 - m * b1;
            f = new LinearFunction(m, a);

            unitCur = uom;
        }

        public void ChanegUnitToSelected()
        {
       //     if (tom != Type)
        //        return;
            string uom = GetSelectedUnit(Type);
            ChangeUnit(uom, Type);   
        }       

    }
}
