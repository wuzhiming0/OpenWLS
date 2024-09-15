using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace OpenWLS.Server.Base
{
    public interface IScaleContainer
    {
        Scale Scale { get; set; }
 //       Scale GetScale();
  //      void SetScale(Scale s);
    }

    public class Scale
    {
        double   ratioSpan;  
        double   posSpan;
        public int ID { get; set; }
        public double Log10From { get; set; }
        public string Name { get; set; }
        public double From { get; set; }
        public double To { get; set; }
        public double PosFrom { get; set; }
        public double PosTo { get; set; }
        public bool Linear { get; set; }
        public bool Show { get; set;}

    //
        public Scale()
        {
            From = 0; To = 100;
            Linear = true;
            Name = "S";
        }

        public Scale(string name, double from, double to, bool linear)
        {
            From = from; To = to;
            Linear = linear;
            Name = name;
            Show = true;
        }

        public void Restore(DataRow dr)
        {
            ID = Convert.ToInt32(dr["ID"]);
            Name = (string)dr["Name"];
            From = Convert.ToDouble(dr["From"]);
            To = Convert.ToDouble(dr["To"]);
            Show = Convert.ToInt32(dr["Show"]) != 0 ;
        //    PosFrom = Convert.ToDouble(dr["PosFrom"]);
        //    PosTo = Convert.ToDouble(dr["PosTo"]);
            Linear = Convert.ToInt32(dr["Linear"]) != 0;
        }

        public void Save(DataRow dr)
        {
            dr["ID"] = ID;
            dr["Name"] = Name;
            dr["From"] = From;
            dr["To"] = To;

          //  dr["PosFrom"] = PosFrom;
          //  dr["PosTo"] = PosTo;
            dr["Linear"] = Linear? 1 : 0;
            dr["Show"] = Show ? 1 : 0;
        }
      
        public double GetDataPosition(double data,  ref sbyte mode)
        {
            double p = Linear? PosFrom + (data - From) * ratioSpan : PosFrom + (Math.Log10(data) - Log10From) * ratioSpan;
            int m = 0;
            while(p > PosTo){
                m++;           p -= posSpan;
                if(m >5)    return double.NaN;
            }
            while(p < PosFrom){
                m--;     p += posSpan;
                if(m < -5)
                    return double.NaN;
            }
            mode = (sbyte)m;
            return p;
     }

        public void PrepairScale()
        {
            double scaleSpan = To - From;
            posSpan = PosTo - PosFrom;
            if (Linear)
                ratioSpan = posSpan / scaleSpan;
            else
            {
                Log10From = Math.Log10(From);
                double log10Span = Math.Log10(To) - Log10From;
                ratioSpan = posSpan / log10Span;
            }
        }
        
        
     public float   GetDataAtPosition(float pos){
         return 0;
     }
     
    }

    public class Scales : List<Scale>
    {
        public void Restore(DataTable dt)
        {
            Clear();
            if (dt == null)
                return;
            foreach (DataRow dr in dt.Rows)
            {
                Scale s = new Scale();
                s.Restore(dr);
                Add(s);
            }
        }

        public void Save(DataTable dt)
        {
            dt.Rows.Clear();
            foreach (Scale s in this)
            {
                DataRow dr = dt.NewRow();
                s.Save(dr);
                dt.Rows.Add(dr);
            }
        }

 /*       public Scale GetScale(string name)
        {
            foreach (Scale s in this)
            {
                if (s.Name == name)
                    return s;
            }
            return null;
        }
*/
        public Scale GetScale(int id)
        {
            foreach (Scale s in this)
            {
                if (s.ID == id)
                    return s;
            }
            return null;
        }

        int GetNxtID()
        {
            int k = 0;
            bool b = true;
            while (b)
            {
                b = false;
                foreach (Scale s in this)
                    if (s.ID == k)
                        b = true;
                k++;
            }
            return k-1;
        }

        public Scale AddNewScale()
        {
            Scale sc = new Scale();
            int k = GetNxtID();
            sc.Name = "S" + k.ToString();
            sc.ID = k;
            Insert(0,sc);
            return sc;
        }

        public Scale AddNewScale(string name, double left, double right, bool linear)
        {
            Scale sc = new Scale(name, left, right, linear);
            int k = GetNxtID();
            sc.ID = k;
            Add(sc);
            return sc;
        }

        public void AddDefaultScales()
        {
            AddNewScale("gr", 0, 150, true);
            AddNewScale("temperature", 0, 200, true);
            AddNewScale("pressure", 0, 10000, true);
            AddNewScale("caliper", 6, 16, true);
            AddNewScale("density", 1.98, 2.98, true);
            AddNewScale("cn", -15, 45, true);
            AddNewScale("dt", 40, 140, true);
            AddNewScale("sp", 0, 100, true);
            AddNewScale("res1", 0.1, 1000, false);
            AddNewScale("res2", 0.2, 2000, false);
            AddNewScale("angle", 0, 360, true);
            AddNewScale("NEW", 0, 100, true);
        }
    }
}
