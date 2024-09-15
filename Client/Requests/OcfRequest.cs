//using Newtonsoft.Json;
using OpenWLS.Server.DBase;
using OpenWLS.Server.DBase.Models.GlobalDb;
using OpenWLS.Server.DBase.Models.LocalDb;
using OpenWLS.Server.LogInstance.OperationDocument;
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
    public class OcfRequest
    {
        public static string str_controller_g = "api/GlobalOcf";
        public static string str_controller_l = "api/LocalOcf";

        public static string GetControllerString(bool global) => global ? str_controller_g : str_controller_l;
        public static async Task<List<OperationControlFile>> GetAll(bool global)
        {
            HttpResponseMessage response =  ClientGlobals.HttpClient.GetAsync($"{GetControllerString(global)}/All").Result;
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<OperationControlFile>>();
            }
            return null;
        }

        public static async Task<string?> GetBody(int id, bool global)
        {
            HttpResponseMessage response = ClientGlobals.HttpClient.GetAsync($"{GetControllerString(global)}/Body/{id}").Result;

            TextObject? t = response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<TextObject>() : null;
            if (t == null)
                ClientGlobals.SysLog.AddMessage($"Get Operation Document {id} from Db failed", System.Windows.Media.Colors.Red);
            else
            {
                if (t != null)
                    ClientGlobals.SysLog.AddMessage($"Get Operation Document {id} from Db.");
                else
                    ClientGlobals.SysLog.AddMessage($"Operation Document {id} in Db is Empty", System.Windows.Media.Colors.Red);
            }
            return t.Val;
        }

        public static async Task<OperationControlFile?> CreateOcfAsync(OperationControlFile ocf, bool global)
        {
            var jsonString = JsonSerializer.Serialize(ocf);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = ClientGlobals.HttpClient.PostAsync($"{GetControllerString(global)}/Add", httpContent).Result;
            OperationControlFile? res = response.IsSuccessStatusCode? await response.Content.ReadFromJsonAsync<OperationControlFile>() : null;
            if (res == null)
                ClientGlobals.SysLog.AddMessage($"Create Operation Document {ocf.Name} failed", System.Windows.Media.Colors.Red);
            else
                ClientGlobals.SysLog.AddMessage($"Create Operation Document {res.Id} : {res.Name}");
            return res;
        }

        public static async Task<OperationControlFile?> UpdateOcfAsync(OperationControlFile ocf, bool global)
        {
            var jsonString = JsonSerializer.Serialize(ocf);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response =  ClientGlobals.HttpClient.PostAsync($"{GetControllerString(global)}/Update", httpContent).Result;
            OperationControlFile? res = response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<OperationControlFile>() : null;
            if (res == null)
                ClientGlobals.SysLog.AddMessage($"Update Operation Document {ocf.Name} failed", System.Windows.Media.Colors.Red);
            else
                ClientGlobals.SysLog.AddMessage($"Update Operation Document {res.Id} : {res.Name}");
            return res;
        }

        public static async Task<OperationControlFile?> DeleteOcAsync(int ocf_id, bool global)
        {
            HttpResponseMessage response =  ClientGlobals.HttpClient.GetAsync($"{GetControllerString(global)}/Delete/{ocf_id}").Result;
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<OperationControlFile>();
            }
            return null;
        }


    }
}
