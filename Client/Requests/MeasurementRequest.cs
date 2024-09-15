using OpenWLS.Server.DBase;
using OpenWLS.Server.DBase.Models.GlobalDb;
using OpenWLS.Server.DBase.Models.LocalDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenWLS.Client.Requests
{
    public class MeasurementRequest
    {
        static string str_controller = "api/Measurement";
        public static async Task<List<MeasurementDb>> GetAll()
        {
            HttpResponseMessage response = await ClientGlobals.HttpClient.GetAsync($"{str_controller}/All");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<MeasurementDb>>();
            }
            return null;
        }
        public static async Task<MeasurementDb?> GetByName(string name)
        {
            HttpResponseMessage response =  ClientGlobals.HttpClient.GetAsync($"{str_controller}/Name/{name}").Result;
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<MeasurementDb>();
            }
            return null;
        }

        public static async Task<MeasurementDb?> GetById(int id)
        {
            HttpResponseMessage response =  ClientGlobals.HttpClient.GetAsync($"{str_controller}/Id/{id}").Result;
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<MeasurementDb>();
            }
            return null;
        }

        public static async Task<MeasurementDb> CreateMeasurementAsync(MeasurementDb m)
        {
            var jsonString = JsonSerializer.Serialize(m);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response =  ClientGlobals.HttpClient.PutAsync($"{str_controller}/Add", httpContent).Result;
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<MeasurementDb>();
            }
            return null;
        }

        public static async Task<MeasurementDb> UpdateMeasurementAsync(MeasurementDb m)
        {
            var jsonString = JsonSerializer.Serialize(m);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await ClientGlobals.HttpClient.PutAsync($"{str_controller}/Update", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<MeasurementDb>();
            }
            return null;
        }

        public static async Task<MeasurementDb> DeleteMeasurementAsync(int id)
        {
            HttpResponseMessage response = await ClientGlobals.HttpClient.GetAsync($"{str_controller}/Delete/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<MeasurementDb>();
            }
            return null;
        }


    }
}
