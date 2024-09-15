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
using OpenWLS.Server.LogInstance.OperationDocument;

namespace OpenWLS.Client.Requests
{
    public class LogInstanceRequest
    {
        public static string str_controller = "api/LogInstance";

        /// <summary>
        /// Get basic information of all Log Instances
        /// </summary>
        /// <returns></returns>
        public static async Task<Server.LogInstance.LogInstances> GetAllInstancesAsync()
        {
            HttpResponseMessage response =  ClientGlobals.HttpClient.GetAsync($"{str_controller}/All").Result;
            if (response.IsSuccessStatusCode)
            {
                Server.LogInstance.LogInstances? lis = await response.Content.ReadFromJsonAsync<Server.LogInstance.LogInstances>();
                return lis;
            }
            return null;
        }

        /// <summary>
        /// Create a new LogInstance
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static async Task<Server.LogInstance.LogInstance?> CreateAsync(int job_id, int ocf_id, int sim, int edge_id)
        {
            HttpResponseMessage response =  ClientGlobals.HttpClient.GetAsync($"{str_controller}/Create/{job_id}/{ocf_id}/{edge_id}/{sim}").Result;
            Server.LogInstance.LogInstance? li = response.IsSuccessStatusCode? await response.Content.ReadFromJsonAsync<Server.LogInstance.LogInstance>() : null;
          
            if (li == null)
                ClientGlobals.SysLog.AddMessage($"Create LogInstance {ocf_id} failed", System.Windows.Media.Colors.Red);
            else
                ClientGlobals.SysLog.AddMessage($"Create LogInstance {li.Id} : {li.Name}\n {li.State}");
            return li;
        }

        /// <summary>
        /// Start a new playback LogInstance
        /// </summary>
        /// <param name="fn: file name"></param>
        /// <returns></returns>
        public static async Task<Server.LogInstance.LogInstance> PlaybackAsync(string fn)
        {
          //  var jsonString = JsonSerializer.Serialize(doc);
            var httpContent = new StringContent(fn, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await ClientGlobals.HttpClient.PostAsync($"{str_controller}/Playback/", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Server.LogInstance.LogInstance>();
            }
            return null;
        }
        /*
        /// <summary>
        /// Connect to a running LI
        /// </summary>
        /// <param name="li_id"></param>
        /// <returns></returns>
        public static async Task<Server.LogInstance.LogInstance> ConnectAsync(int li_id)
        {
            HttpResponseMessage response = await ClientGlobals.HttpClient.GetAsync($"{str_controller}/Connect/{li_id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Server.LogInstance.LogInstance>();
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="li_id"></param>
        /// <returns></returns>
        public static async Task<Server.LogInstance.LogInstance> DisconnectAsync(int li_id)
        {
            HttpResponseMessage response = await ClientGlobals.HttpClient.GetAsync($"{str_controller}/Disconnect/{li_id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Server.LogInstance.LogInstance>();
            }
            return null;
        }
        */
        /// <summary>
        /// Stop a running LI
        /// </summary>
        /// <param name="job_id"></param>
        /// <returns></returns>
        public static async Task StopAsync(int li_id)
        {
            HttpResponseMessage response = await ClientGlobals.HttpClient.GetAsync($"{str_controller}/Stop/{li_id}");
            if (response.IsSuccessStatusCode)
            {
       //         return await response.Content.ReadFromJsonAsync<JobC>();
            }
     //       return null;
        }
    }
}
