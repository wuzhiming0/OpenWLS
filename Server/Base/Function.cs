using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.Base
{
    public interface IFunction
    {
        void F(double[] inputs, double[] output);
        double F(double input);
        void SetCoeffs(object coeffs);
        void SetCoeff(string name, object val);
    }

    public class LinearFunction : IFunction
    {
        double a;
        double m;
        public LinearFunction()
        {
            a = 0;
            m = 1;
        }

        public LinearFunction(double mul, double add){
            m = mul;
            a = add;
        } 

        public  void F(double[] inputs, double[] outputs){
            for(int i = 0 ; i < inputs.Length; i++)
                outputs[i] = inputs[i] * m + a;
        }

        public double F(double input)
        {
            return input * m + a;
        }

        public void SetCoeffs(object coeffs)
        {
            double[] ds = (double[])coeffs;
            m = ds[0];
            a = ds[1];
        }

        public void SetCoeff(string name, object val)
        {
            if (name == "M")
                m = Convert.ToDouble(val);
            if (name == "A")
                a = Convert.ToDouble(val);
        }
    }

}
