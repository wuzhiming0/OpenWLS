using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

//using System.Management;


namespace OpenWLS.Edge.USB
{
    public class USBPort
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vid">vendor</param>
        /// <param name="pid">product</param>
        /// <returns></returns>
        public static List<string> GetComPortNames(string vid, string pid)
        {
            //  string VID = "04D8";
            //  String PID = "000A";
            string[] strs = SerialPort.GetPortNames();
            string pattern = string.Format("^VID_{0}.PID_{1}", vid, pid);
            Regex _rx = new Regex(pattern, RegexOptions.IgnoreCase);
            List<string> comports = new List<string>();
            RegistryKey rk1 = Registry.LocalMachine;
            RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");
            foreach (string s3 in rk2.GetSubKeyNames())
            {
                RegistryKey rk3 = rk2.OpenSubKey(s3);
                foreach (string s in rk3.GetSubKeyNames())
                {
                    if (_rx.Match(s).Success)
                    {
                        RegistryKey rk4 = rk3.OpenSubKey(s);
                        foreach (string s2 in rk4.GetSubKeyNames())
                        {
                            RegistryKey rk5 = rk4.OpenSubKey(s2);
                            RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
                            string str1 = (string)rk6.GetValue("PortName");
                            foreach (string str2 in strs)
                            {
                                if (str1 == str2) comports.Add(str1);
                            }
                        }
                    }
                }
            }
            return comports;
        }

    

        public static List<string> GetUsbPortSerialNumbers(string desp)
        {
            List<string> sns = new List<string>();
            FTD2XX_NET.FTDI ftdi = new FTD2XX_NET.FTDI();
            uint numDevices = 0;
            ftdi.GetNumberOfDevices(ref numDevices);
            FTD2XX_NET.FTDI.FT_DEVICE_INFO_NODE[] devicelist = new FTD2XX_NET.FTDI.FT_DEVICE_INFO_NODE[numDevices];
            FTD2XX_NET.FTDI.FT_STATUS ftStatus = ftdi.GetDeviceList(devicelist);

            foreach (FTD2XX_NET.FTDI.FT_DEVICE_INFO_NODE n in devicelist)
            {
                if (n.Description == desp)
                    sns.Add(n.SerialNumber);
            }
            return sns;
        }

        public static FTD2XX_NET.FTDI.FT_DEVICE_INFO_NODE[]? GetFtdiDeviceList()
        {
            FTD2XX_NET.FTDI ftdi = new FTD2XX_NET.FTDI();
            uint numDevices = 0;
            ftdi.GetNumberOfDevices(ref numDevices);
            FTD2XX_NET.FTDI.FT_DEVICE_INFO_NODE[] devicelist = new FTD2XX_NET.FTDI.FT_DEVICE_INFO_NODE[numDevices];
            FTD2XX_NET.FTDI.FT_STATUS ftStatus = ftdi.GetDeviceList(devicelist);
            if (ftStatus == FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                return devicelist;
            else return null;
        }
    }
}
