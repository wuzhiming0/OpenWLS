
using OpenWLS.Client.LogInstance;
using OpenWLS.Client.Requests;
using OpenWLS.Server.DBase.Models.LocalDb;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using OpenWLS.Client.Base;
using OpenLS.Base.UOM;
using OpenWLS.Server;

namespace OpenWLS.Client
{
    public  class ClientGlobals
    {
        public static string projectDir;
        public static HttpClient HttpClient { get; set; }
        public static string WsUri { get; set; }
        public static JobC ActiveJob { get; set; }
        public static List<string>? ServerUris { get; set; }
        public static int? PerferedEdge { get; set; }
        public static bool? Simulation { get; set; }
        public static SysLogCntl SysLog { get; set; }
        //       public static APIClient APIClient { get; set; }
        public static async void Init()
        {
            projectDir = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            int k = projectDir.IndexOf("Client");
            projectDir = projectDir.Substring(0, k);

            //       MeasurementUnit.Init($"{projectDir}data/server/db/uom.json");

            ClientSetting? cs = ClientSetting.GetClientSetting();
            string httpClientUri = "http://localhost:5212";
            int activeJobId = -1;
            if (cs != null)
            {
                if (cs.ActiveSever != null)
                    httpClientUri = cs.ActiveSever;
                activeJobId = cs.ActiveJobId;
                PerferedEdge = cs.PerferedEdge;
                Simulation = cs.Simulation;
            }
            HttpClient = new HttpClient()
            {
                BaseAddress = new Uri(httpClientUri)
            };
            WsUri = httpClientUri.EndsWith("/") ? $"ws:{httpClientUri.Remove(0, httpClientUri.IndexOf('/'))}bin" :
                $"ws:{httpClientUri.Remove(0, httpClientUri.IndexOf('/'))}/bin";
            JobCs? jobs = await JobRequest.GetAllJobsAsync();
            if (jobs != null)
            {
                if (activeJobId < 0 || activeJobId >= jobs.Count)
                    activeJobId = jobs[jobs.Count - 1].Id;
                jobs.SelectJob(activeJobId);
                ActiveJob = jobs.SelectedJob;
            }
            await UomRequest.GetUomAsync();

        }

        public static void Save()
        {
            ClientSetting cs = new ClientSetting
            {
                ActiveJobId = ActiveJob.Id,
                ServerUris = ServerUris,
                ActiveSever = HttpClient.BaseAddress.ToString(),
                PerferedEdge = PerferedEdge,
                Simulation = Simulation,
            };
            cs.Save();
        }
    }

    internal class ClientSetting
    {
        static string pathDB = $"{ClientGlobals.projectDir}data/client/client_settings.json";
        public List<string>? ServerUris { get; set; }
        public string? ActiveSever { get; set; }
        public int ActiveJobId { get; set; }        
        public int? PerferedEdge {  get; set; }  
        public bool? Simulation {  get; set; }
        
        public static ClientSetting? GetClientSetting()
        {
            ClientSetting cs = null;
            if (System.IO.File.Exists(pathDB))
            {
                using (FileStream fs = new FileStream(pathDB, FileMode.Open))
                using (StreamReader ss = new StreamReader(fs))
                {
                    try
                    {
                        string str = ss.ReadToEnd();
                        cs = Newtonsoft.Json.JsonConvert.DeserializeObject<ClientSetting>(str);
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
            return cs;
        }

        public void Save()
        {
            using (StreamWriter file = File.CreateText(pathDB))
            {
                string str = Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
                file.Write(str);
            }
        }

    }
}
