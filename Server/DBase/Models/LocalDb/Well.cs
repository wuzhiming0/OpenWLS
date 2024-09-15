using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using OpenWLS.Server.Base;

namespace OpenWLS.Server.DBase.Models.LocalDb
{

    public class Well
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string GInfor { get; set; }  // general information
                                            //       public string WellInfor { get; set; }
        public string DrillingBits { get; set; }
        public string Casings { get; set; }

        public static Parameters NewGInfor()
        {
            string str = "COMP||COMPANY||CN:CN1\r\nWELL||WELL||WN\r\nFLD||FIELD||FN\r\nLOC||LOCATION||FL:FL1:FL2\r\nCTRY||COUNTRY||NATI\r\nPROV||PROVINCE||nSTAT\r\nSRVC||SERVICE COMPANY\r\nUWI||UNIQUE WELL ID||UWID\r\nLIC||LICENSE NUMBER\r\nAPI||API NUMBER||APIN\r\nLATI||Latitude|Deg|LATI\r\nLONG||Longitude|Deg|LONG\r\nGDAT||Geodetic Datum\r\nUTM||UTM LOCATION";
            string[] ss1 = str.Split('\n');
            Parameters ps = new Parameters();
            foreach (string s1 in ss1)
            {
                string[] ss2 = str.Split('|');
                Parameter p = new Parameter()
                {
                    MNEM = ss2[0],
                    Val = new string[] { ss2[1] },
                    Name = ss2[2].Trim()
                };
                if (ss2.Length == 4)
                    p.Units = ss2[3];
                ps.Add(p);
            }
            return ps;
        }

        //size|top|bot
        public static Parameters GetSizes(string str)
        {
            Parameters ps = new Parameters();
            string[] ss1 = str.Split('\n');
            foreach (string s1 in ss1)
            {
                string[] ss2 = s1.Split('|');
                Parameter p = new Parameter()
                {
                    Val = new string[] { ss2[0] },
                    Zone = "{ss2[1]}|{ss2[2]}"
                };
                ps.Add(p);
            }
            return ps;
        }

        public Parameters GetDringBits()
        {
            return GetSizes(DrillingBits);
        }
        public Parameters GetCasing()
        {
            return GetSizes(Casings);
        }
        public Parameters GetGeneralInformation()
        {
            return JsonSerializer.Deserialize<Parameters>(GInfor);
        }
        public void SetGeneralInformation(Parameters paras)
        {
            GInfor = JsonSerializer.Serialize(paras, new JsonSerializerOptions
            {
            //    WriteIndented = true,
                IgnoreNullValues = true,
             //   PropertyNameCaseInsensitive = true,
            });
        }


    }



    public class MudResisivity
    {
        public string Value { get; set; }
        public string Temperature { get; set; }
        public string Depth { get; set; }
        public string Source { get; set; }
    }
    public class MudResisivitys : List<MudResisivity>
    {

    }
}
