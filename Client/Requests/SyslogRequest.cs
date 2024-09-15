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
    public class SyslogRequest
    {
        public static string str_controller = "api/Syslog";
        public static async Task<List<SyslogItem>> GetItemsFromIdAsync(int id)
        {
            HttpResponseMessage response = await ClientGlobals.HttpClient.GetAsync($"{str_controller}/GetItemsFromId/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<SyslogItem>>();
            }
            return null;
        }

        public static async Task CreateItemAsync(SyslogItem job)
        {
            var jsonString = JsonSerializer.Serialize(job);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await ClientGlobals.HttpClient.PutAsync($"{str_controller}/Add", httpContent);
            if (response.IsSuccessStatusCode)
            {
                //    return await response.Content.ReadFromJsonAsync<SyslogItem>();
            }
            //      return null;
        }


    }
}
