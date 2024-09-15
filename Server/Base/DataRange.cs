using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.Base
{
    public class DataRange
    {
        public double Min { get; set; }
        public double Max { get; set; }

        public DataRange()
        {
            Min = double.NaN;
            Max = double.NaN;
        }

        public void AddData(double dat)
        {
            if (double.IsNaN(Min) || dat < Min) Min = dat;
            if (double.IsNaN(Max) || dat > Max) Max = dat;
        }

        public DataRange Overlap(DataRange dr)
        {
            return new DataRange()
            {
                Min = Math.Max(Min, dr.Min),
                Max = Math.Min(Max, dr.Max)
            };
        }


        public bool Valid
        {
            get { return Min <= Max; }
        }

        public bool Inside(double val)
        {
            return val >= Min && val <= Max;
        }



    }
}
