using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.PLT1.SgrA
{
    public class G2dData
    {

        double[] dat;
        double[] acc_dat;
        int acc_cnt;

    //    bool size_changed;
        double max;

        public int AccuCntMax { get; set; }

        public double[] Data{get{return dat;}}
        public double[] AccuDat{get{return acc_dat;}}

        public bool Updated { get; set; }

        public double MaxAccuData
        {
            get
            {
               return max;
            }
        }


        public G2dData(){
            dat = new double[256];
            acc_dat = new double[256];
        }

        public int AccuCnt
        {
            get { return acc_cnt; }
            set
            {
                acc_cnt = value;
                if (acc_cnt == 0)
                {
                    for (int i = 0; i < acc_dat.Length; i++)
                        this.acc_dat[i] = 0;
                }

            }
        }

        public void SetData(double[] ds)
        {
            if (ds.Length != dat.Length)
            {
                dat = new double[ds.Length];
                acc_dat = new double[ds.Length];
                AccuCnt = 0;
            }
            Buffer.BlockCopy(ds, 0, dat, 0, dat.Length);
            max = 0;
            for (int i = 0; i < acc_dat.Length; i++)
            {
                acc_dat[i] += ds[i];
                if (acc_dat[i] > max)
                    max = acc_dat[i];
            }
            Updated = true;
      
        }



    }
}
