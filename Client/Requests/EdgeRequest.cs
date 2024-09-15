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

namespace OpenWLS.Client.Requests
{
    public class EdgeC : Edge
    {
        [JsonIgnore]
        public string SEType
        {
            get
            {
                if (EType == null) return EdgeType.OpenWLS_Std.ToString();
                else return EType.ToString();
            }
        }
        [JsonIgnore]
        public int? IEType
        {
            get
            {
                if (EType == null) return 0;
                else return (int)EType;
            }
            set
            {
                EType = value == null? null : (EdgeType)value;
            }
        }
    }

    public class EdgeCs : List<EdgeC>
    {

    }
    public class EdgeRequest
    {
        public static string str_controller = "api/Edge";
        public static async Task<EdgeCs?> GetAllEdgesAsync()
        {
            HttpResponseMessage response =  ClientGlobals.HttpClient.GetAsync($"{str_controller}/All").Result;
            if (response.IsSuccessStatusCode)
            {
                EdgeCs? es = await response.Content.ReadFromJsonAsync<EdgeCs>();
            //    if (es != null && ClientGlobals.ActiveEdge != null)
            //        es.SelectEdge(ClientGlobals.ActiveEdge.Id);
                return es;
            }
            return null;
        }

        public static async Task<EdgeC> CreateEdgeAsync(EdgeC edge)
        {
            var jsonString = JsonSerializer.Serialize(edge);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await ClientGlobals.HttpClient.PostAsync($"{str_controller}/Add/", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<EdgeC>();
            }
            return null;
        }

        public static async Task<EdgeC> UpdateEdgesAsync(EdgeC edge)
        {
            var jsonString = JsonSerializer.Serialize(edge);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await ClientGlobals.HttpClient.PostAsync($"{str_controller}/Update", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<EdgeC>();
            }
            return null;
        }

        public static async Task<EdgeC> DeleteEdgesAsync(int edge_id)
        {
            HttpResponseMessage response = await ClientGlobals.HttpClient.GetAsync($"{str_controller}/Delete/{edge_id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<EdgeC>();
            }
            return null;
        }


    }
}
