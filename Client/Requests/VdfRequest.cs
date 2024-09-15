using OpenWLS.Server.Base;
using OpenWLS.Server.DBase;
using OpenWLS.Server.DBase.Models.GlobalDb;
using OpenWLS.Server.DBase.Models.LocalDb;
using OpenWLS.Server.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenWLS.Client.Requests
{
    public class VdfRequest
    {
        public static string str_controller_g = "api/GlobalVdf";
        public static string str_controller_l = "api/LocalVdf";

        public static string GetControllerString(bool global) => global ? str_controller_g : str_controller_l;
        public static async Task<List<GViewDefinitionFile>> GetAll(bool global)
        {
            HttpResponseMessage response =  ClientGlobals.HttpClient.GetAsync($"{GetControllerString(global)}/All").Result;
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<GViewDefinitionFile>>();
            }
            return null;
        }
        public static async Task<TextObject> GetBody(int id, bool global)
        {
            HttpResponseMessage response = ClientGlobals.HttpClient.GetAsync($"{GetControllerString(global)}/Body/{id}").Result;
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TextObject>();
            }
            return null;
        }

        public static async Task<GViewDefinitionFile> CreateVdfAsync(GViewDefinitionFile vdf, bool global)
        {
            var jsonString = JsonSerializer.Serialize(vdf);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await ClientGlobals.HttpClient.PostAsync($"{GetControllerString(global)}/Add", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<GViewDefinitionFile>();
            }
            return null;
        }

        public static async Task<GViewDefinitionFile> UpdateVdfAsync(GViewDefinitionFile vdf, bool global)
        {
            var jsonString = JsonSerializer.Serialize(vdf);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await ClientGlobals.HttpClient.PostAsync($"{GetControllerString(global)}/Update", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<GViewDefinitionFile>();
            }
            return null;
        }

        public static async Task<GViewDefinitionFile> DeleteVdfAsync(int vdf_id, bool global)
        {
            HttpResponseMessage response = await ClientGlobals.HttpClient.GetAsync($"{GetControllerString(global)}/Delete/{vdf_id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<GViewDefinitionFile>();
            }
            return null;
        }

        public static async Task GenerateGvDocFromVdf(string vdf_json, bool rt, IWsMsgProc msg_proc)
        {
            DataWriter w = new DataWriter(WsService.ws_buffer_size);
            w.WriteData(WsService.request_gvd_ldf);
            if (rt) w.WriteData((byte)1);
            else w.WriteData((byte)0);
            w.Seek(1, SeekOrigin.Current);
            w.WriteStringWithSizeInt32(vdf_json);

            WsRequest.Request(w.GetUsedBuffer(), msg_proc);
        }
    }
}
