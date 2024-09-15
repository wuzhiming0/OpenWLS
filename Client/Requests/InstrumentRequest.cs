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
    public class InstrumentRequest
    {
        static string str_controller = "api/Instrument";
        public static async Task<List<InstrumentDb>> GetAll()
        {
            HttpResponseMessage response = ClientGlobals.HttpClient.GetAsync($"{str_controller}/All").Result;
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<InstrumentDb>>();
            }
            return null;
        }

        public static async Task<List<InstrumentDb>> GetDownholeList()
        {
            HttpResponseMessage response =  ClientGlobals.HttpClient.GetAsync($"{str_controller}/Downhole").Result;
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<InstrumentDb>>();
            }
            return null;
        }

        public static async Task<InstrumentDb> CreateInstrumentAsync(InstrumentDb inst)
        {
            var jsonString = JsonSerializer.Serialize(inst);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await ClientGlobals.HttpClient.PutAsync($"{str_controller}/Add", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<InstrumentDb>();
            }
            return null;
        }

        public static async Task<InstrumentDb> UpdateInstrumentAsync(InstrumentDb inst)
        {
            var jsonString = JsonSerializer.Serialize(inst);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await ClientGlobals.HttpClient.PutAsync($"{str_controller}/Update", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<InstrumentDb>();
            }
            return null;
        }

        public static async Task<InstrumentDb> DeleteInstrumentAsync(int id)
        {
            HttpResponseMessage response = await ClientGlobals.HttpClient.GetAsync($"Instrument/Delete/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<InstrumentDb>();
            }
            return null;
        }


    }
}
