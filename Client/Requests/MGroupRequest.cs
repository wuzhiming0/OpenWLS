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
    public class MGroupRequest
    {
        static string str_controller = "api/MGroup";
        public static async Task<List<MGroup>> GetAll()
        {
            HttpResponseMessage response = await ClientGlobals.HttpClient.GetAsync($"{str_controller}/All");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<MGroup>>();
            }
            return null;
        }

        public static async Task<List<MGroup>> GetMGroupsOfInst(int iid)
        {
            HttpResponseMessage response =  ClientGlobals.HttpClient.GetAsync($"{str_controller}/Inst/{iid}").Result;
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<MGroup>>();
            }
            return null;
        }

        public static async Task<MGroup> CreateMGroupAsync(MGroup inst)
        {
            var jsonString = JsonSerializer.Serialize(inst);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await ClientGlobals.HttpClient.PutAsync($"{str_controller}/Add", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<MGroup>();
            }
            return null;
        }

        public static async Task<MGroup> UpdateMGroupAsync(MGroup inst)
        {
            var jsonString = JsonSerializer.Serialize(inst);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await ClientGlobals.HttpClient.PutAsync($"{str_controller}/Update", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<MGroup>();
            }
            return null;
        }

        public static async Task<MGroup> DeleteMGroupAsync(int id)
        {
            HttpResponseMessage response = await ClientGlobals.HttpClient.GetAsync($"{str_controller}/Delete/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<MGroup>();
            }
            return null;
        }


    }
}
