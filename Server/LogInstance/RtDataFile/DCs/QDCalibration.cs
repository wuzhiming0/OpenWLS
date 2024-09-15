using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenWLS.Server.Base;

namespace OpenWLS.Server.LogInstance.RtDataFile.DCs
{
    public class QdCalibrationHexFile
    {
        public ushort FileType { get; set; }
        public ushort Version { get; set; }
        public ushort SerNo { get; set; }
        public string PartNo { get; set; } //8
        public uint CalDate { get; set; }
        [JsonIgnore]
        public DateTime CalDate1 { get { return new DateTime((int)(CalDate >> 16), (int)(CalDate >> 8) & 0xff, (int)CalDate & 0xff); } }
        public sbyte Pmin { get; set; } //Min Pressure (kpsi)
        public sbyte Pmax { get; set; }
        public sbyte Tmin { get; set; } //Min Temperature (x 5°C)
        public sbyte Tmax { get; set; }
        public QtCalOutput[] QtCalOutputs { get; set; }


        public QdCalibrationHexFile()
        {
            FileType = 0x0D01;
            Version = 0x0123;
            DateTime dt = DateTime.Now;
            CalDate = (uint)(dt.Year << 16) + (uint)(dt.Month << 8) + (uint)dt.Day;
            QtCalOutputs = new QtCalOutput[2];
            QtCalOutputs[0] = new QtCalOutput(1);
            QtCalOutputs[1] = new QtCalOutput(2);
        }

        public void ReadHexFile(DataReader r)
        {
            bool b = r.GetByteOrder();
            r.SetByteOrder(false);
            FileType = r.ReadUInt16();
            Version = r.ReadUInt16();
            SerNo = r.ReadUInt16();
            PartNo = r.ReadString(8);
            CalDate = r.ReadUInt32();
            Pmin = r.ReadSByte();
            Pmax = r.ReadSByte();
            Tmin = r.ReadSByte();
            Tmax = r.ReadSByte();
            QtCalOutputs[0].ReadTbl(r);
            QtCalOutputs[1].ReadTbl(r);

            r.SetByteOrder(b);
        }


        public byte[] GetByteArray()
        {
            DataWriter w = new DataWriter(256);
            w.SetByteOrder(false);
            w.WriteData(FileType);
            w.WriteData(Version);
            w.WriteData(SerNo);
            w.WriteString(PartNo, 8);
            w.WriteData(CalDate);
            w.WriteData(Pmin);
            w.WriteData(Pmax);
            w.WriteData(Tmin);
            w.WriteData(Tmax);
            QtCalOutputs[0].Save(w);
            QtCalOutputs[1].Save(w);
            byte[] bs = w.GetBuffer();
            bs[252] = 255;
            bs[253] = 0;
            bs[254] = 0;

            byte cs = 0;
            for (int i = 0; i < 255; i++)
                cs += bs[i];
            bs[255] = cs;
            return bs;
        }
    }



    public class QtCalOutput
    {
        public byte CalType { get; set; } //1=Pressure psi, bar
                                          //2=Temperature °C, °F
        public byte Prescale { get; set; }

        public sbyte N1 { get; set; } // Fit Order in X1
        public sbyte N2 { get; set; }  // Fit Order in X2
        public float S1 { get; set; }  //Scale Factor Standard Units
        public float S2 { get; set; }  // Scale Factor Alternate Units
        [JsonIgnore]
        public string S1S
        {
            get
            {
                return BitConverter.ToString(BitConverter.GetBytes(S1));
            }
        }

        public string S2S
        {
            get
            {
                return BitConverter.ToString(BitConverter.GetBytes(S2));
            }
        }
        public int OFS2 { get; set; }  //  Offset for Alternate Units 0x00000000 =  0 psi / 4096

        public int[] Cs { get; set; }  //Coefficients (Max 25)

        [JsonIgnore]
        public string CoeffS
        {
            get
            {
                string s = "";
                int k = 0;
                for (int i = 0; i <= N1; i++)
                {
                    string s1 = "";
                    for (int j = 0; j <= N2; j++)
                        s1 = s1 + Cs[k++].ToString("x") + ", ";
                    s = s + s1 + "\n";
                }
                return s.Substring(0, s.Length - 1);
            }
        }
        public QtCalOutput(byte t)
        {
            CalType = t;
            if (t == 1)
            {
                Prescale = 0;
                N1 = 3;
                N2 = 3;
                S1 = (float)(1.0 / 4096);
                S2 = (float)(0.0689476 / 4096);
                OFS2 = 0;
            }
            else
            {
                Prescale = 3;
                N1 = 0;
                N2 = 3;
                S1 = (float)(1.0 / 4096);
                S2 = (float)(1.8 / 4096);
                OFS2 = 0x00011C72;
            }
            int s = (N1 + 1) * (N2 + 1);
            Cs = new int[s];
        }

        public void Save(DataWriter w)
        {
            w.WriteData(CalType);
            w.WriteData(Prescale);
            w.WriteData(N1);
            w.WriteData(N2);

            w.WriteData(S1);
            w.WriteData(S2);
            w.WriteData(OFS2);

            for (int i = 0; i < Cs.Length; i++)
                w.WriteData(Cs[i]);

            for (int i = Cs.Length; i < 25; i++)
                w.WriteData(0);

        }


        public void ReadTbl(DataReader r)
        {
            CalType = r.ReadByte();
            Prescale = r.ReadByte();
            N1 = r.ReadSByte();
            N2 = r.ReadSByte();

            S1 = r.ReadSingle();
            S2 = r.ReadSingle();
            OFS2 = r.ReadInt32();

            int s = (N1 + 1) * (N2 + 1);
            Cs = new int[s];
            for (int i = 0; i < s; i++)
                Cs[i] = r.ReadInt32();

        }

        public double qdHexCalc(uint xp, uint xt, bool altUnits)
        {
            double qdHexCalc;
            long Z = 0;
            long Temp;
            int n = (N1 + 1) * (N2 + 1);
            for (int i = 0; i < N1; i++)        // outer loop for np, xp polynomial
            {
                Z = Z * xp >> 24;             // multiply by xp (0 first time through loop) 
                Temp = 0;
                for (int j = 0; j < N2; j++)    // inner loop for nt, xt polynomial 
                {
                    Temp = xt * Temp >> 24;   // process factored using coefficients from last to first Temp = Temp + Cs( n)
                    n--;
                    Temp += Cs[n];
                }
                Z = Z + Temp;                   //add temperature compensated pressure coefficient 
            }
            if (altUnits)
            {
                Z = Z + OFS2;                   // OFS2 placed just before coefficients
                qdHexCalc = Z * S2;             // alternate scale factor S2 else 
            }
            else
                qdHexCalc = Z * S1;             // primary units scale factor S1

            return qdHexCalc;
        }
    }
}
