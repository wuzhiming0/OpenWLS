using OpenWLS.Client.LogInstance;
using OpenWLS.Client.Base;
using OpenWLS.Server.DBase.Models.LocalDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Data;
using OpenLS.Base.UOM;

namespace OpenWLS.Client.Requests
{
 
    public class UomRequest
    {
        public static string str_controller = "api/Uom";
        public static async Task GetUomAsync()
        {
            HttpResponseMessage response =  ClientGlobals.HttpClient.GetAsync($"{str_controller}/Get").Result;
            if (response.IsSuccessStatusCode)
            {
                string? str  = await response.Content.ReadAsStringAsync();
                if (str != null)
                {
                    DataTable? dt = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(str);
                    if (dt != null)
                        MeasurementUnit.MDT = dt;
                }
            }

        }

      

        public static async Task UpdateUom()
        {
            var jsonString = JsonSerializer.Serialize(MeasurementUnit.MDT);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await ClientGlobals.HttpClient.PostAsync($"{str_controller}/Update", httpContent);
            if (response.IsSuccessStatusCode)
            {
                string? str = await response.Content.ReadAsStringAsync();
                if (str != null)
                {
                    DataTable? dt = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(str);
                    if (dt != null)
                        MeasurementUnit.MDT = dt;
                }
            }

        }



    }
}
