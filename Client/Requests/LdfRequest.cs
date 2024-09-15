using Microsoft.AspNetCore.Mvc;
using OpenWLS.Server.DBase;
using OpenWLS.Server.DBase.Models.GlobalDb;
using OpenWLS.Server.DBase.Models.LocalDb;
using OpenWLS.Server.LogDataFile.Models;
using OpenWLS.Server.LogDataFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using OpenWLS.Server.Base;
//
using OpenWLS.Client.LogInstance;
using System.Windows.Shapes;
using OpenWLS.Client.LogDataFile;
using System.Net;
using OpenWLS.Server.WebSocket;
using OpenWLS.Client.Base;
//using System.ComponentModel;

namespace OpenWLS.Client.Requests
{
    public class LdfRequest
    {
        public static string str_controller = "api/LogDataFile";
        public static async Task<List<string>> GetDfileNamesOfFolder(string path)
        {
            var httpContent = new StringContent(JsonSerializer.Serialize(path), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await ClientGlobals.HttpClient.PostAsync($"{str_controller}/FolderFileNames/", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<string>>();
            }
            return null;
        }
        public static async Task<List<string>> GetDfileNamesOfJob(string job_name)
        {
   //         HttpResponseMessage response = await Globals.HttpClient.GetAsync($"{str_controller}/JobFileNames/{job_name}");
            HttpResponseMessage response =  ClientGlobals.HttpClient.GetAsync($"{str_controller}/JobFileNames/{job_name}").Result;
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<string>>();
            }
            return null;
        }

        public static async Task<DataFileInfor> Open(string path)
        {
            var httpContent = new StringContent(JsonSerializer.Serialize(path), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await ClientGlobals.HttpClient.PostAsync($"{str_controller}/Open", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<DataFileInfor>();
            }
            return null;
        }
        public static async Task<DataFileInfor> Open(string job, string fn)
        {
            string str = job == string.Empty? fn : $"{JobC.GetJobDirectory(job)}/{fn}";
            var httpContent = new StringContent(JsonSerializer.Serialize(str), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await ClientGlobals.HttpClient.PostAsync($"{str_controller}/Open", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<DataFileInfor>();
            }
            return null;
        }

        public static async Task Get1DNumberData(string fn, int[] mids, IWsMsgProc msg_proc)
        {
            DataWriter w = new DataWriter(WsService.ws_buffer_size);
            w.WriteData(WsService.request_ldf_1d);
            w.WriteData((ushort)mids.Length);
            foreach(int id in mids)
                w.WriteData(id);
            w.WriteStringWithSizeUint16(fn);

            WsRequest.Request(w.GetUsedBuffer(), msg_proc);
        }
        public static async Task GetXdNumberData(string file_name, int mid)
        {

        }



            /*
                    public static async Task<byte[]> GetxDNumberData(string file_name, int mid)
                    {
                        var httpContent = new StringContent(JsonSerializer.Serialize(file_name), Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await Globals.HttpClient.PostAsync($"{str_controller}/xDData/{mid}", httpContent);
                        if (response.IsSuccessStatusCode)
                        {
                            return await response.Content.ReadFromJsonAsync<byte[]>();
                        }
                        return null;
                    }

                    public static async Task<byte[]> GetNMData(string file_name, int id)
                    {
                        var httpContent = new StringContent(JsonSerializer.Serialize(file_name), Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await Globals.HttpClient.PostAsync($"{str_controller}/NMData/{id}", httpContent);
                        if (response.IsSuccessStatusCode)
                        {
                            return await response.Content.ReadFromJsonAsync<byte[]>();
                        }
                        return null;
                    }
            */
            public static async Task<DataFileInfor> Import(string file_name)
        {
            var httpContent = new StringContent(JsonSerializer.Serialize(file_name), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await ClientGlobals.HttpClient.PostAsync($"{str_controller}/Import", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<DataFileInfor>();
            }
            return null;
        }
    }
}

