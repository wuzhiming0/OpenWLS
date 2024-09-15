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
    public class InstSubRequest
    {
        static string str_controller = "api/InstSub";
        public static async Task<List<InstSubDb>> GetAll()
        {
            HttpResponseMessage response = ClientGlobals.HttpClient.GetAsync($"{str_controller}/All").Result;
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<InstSubDb>>();
            }
            return null;
        }
        public static async Task<List<InstSubDb>> GetDownholeList()
        {
            HttpResponseMessage response = ClientGlobals.HttpClient.GetAsync($"{str_controller}/Downhole").Result;
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<InstSubDb>>();
            }
            return null;
        }
        public static async Task<List<InstSubDb>> GetDownholeAuxList()
        {
            HttpResponseMessage response = ClientGlobals.HttpClient.GetAsync($"{str_controller}/DhAux").Result;
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<InstSubDb>>();
            }
            return null;
        }
        public static async Task<List<InstSubDb>> GetSurfaceList()
        {
            HttpResponseMessage response = ClientGlobals.HttpClient.GetAsync($"{str_controller}/Sf").Result;
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<InstSubDb>>();
            }
            return null;
        }
        public static async Task<InstSubDb?> GetInstSubAsync(int id)
        {
            HttpResponseMessage response =  ClientGlobals.HttpClient.GetAsync($"{str_controller}/Get/{id}").Result;
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<InstSubDb>();
            }
            return null;
        }

        public static async Task<InstSubDb> CreateInstSubAsync(InstSubDb inst)
        {
            var jsonString = JsonSerializer.Serialize(inst);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await ClientGlobals.HttpClient.PutAsync($"{str_controller}/Add", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<InstSubDb>();
            }
            return null;
        }

        public static async Task<InstSubDb> UpdateInstSubAsync(InstSubDb inst)
        {
            var jsonString = JsonSerializer.Serialize(inst);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await ClientGlobals.HttpClient.PutAsync($"{str_controller}/Update", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<InstSubDb>();
            }
            return null;
        }

        public static async Task<InstSubDb> DeleteInstSubAsync(int id)
        {
            HttpResponseMessage response = await ClientGlobals.HttpClient.GetAsync($"{str_controller}/Delete/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<InstSubDb>();
            }
            return null;
        }


    }
}
