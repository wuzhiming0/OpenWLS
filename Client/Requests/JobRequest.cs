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

namespace OpenWLS.Client.Requests
{
    public class JobRequest
    {
        public static string str_controller = "api/Job";
        public static async Task<JobCs?> GetAllJobsAsync()
        {
            HttpResponseMessage response = await ClientGlobals.HttpClient.GetAsync($"{str_controller}/All");
            if (response.IsSuccessStatusCode)
            {
                JobCs? jobs = await response.Content.ReadFromJsonAsync<JobCs>();
                if (jobs != null && ClientGlobals.ActiveJob != null)
                    jobs.SelectJob(ClientGlobals.ActiveJob.Id);
                return jobs;
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jid"></param>
        /// <returns>names of ocf</returns>
        public static async Task<List<Run>?> GetRunsOfJobAsync(int jid)
        {
            HttpResponseMessage response = await ClientGlobals.HttpClient.GetAsync($"{str_controller}/Runs/{jid}");
            if (response.IsSuccessStatusCode)
            {
                List<Run>? rs = await response.Content.ReadFromJsonAsync<List<Run>>();
                return rs;
            }
            return null;
        }
        public static async Task<JobC> CreateJobAsync(JobC job)
        {
            var jsonString = JsonSerializer.Serialize(job);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await ClientGlobals.HttpClient.PostAsync($"{str_controller}/Add/", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<JobC>();
            }
            return null;
        }

        public static async Task<JobC> UpdateJobsAsync(JobC job)
        {
            var jsonString = JsonSerializer.Serialize(job);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await ClientGlobals.HttpClient.PostAsync($"{str_controller}/Update", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<JobC>();
            }
            return null;
        }

        public static async Task<JobC> DeleteJobsAsync(int job_id)
        {
            HttpResponseMessage response = await ClientGlobals.HttpClient.GetAsync($"{str_controller}/Delete/{job_id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<JobC>();
            }
            return null;
        }

        /// <summary>
        /// earse the deleted jobs without work directory
        /// </summary>
        /// <param name="job_id"></param>
        /// <returns></returns>
        public static async Task CleanJobsAsync(int job_id)
        {
            HttpResponseMessage response = await ClientGlobals.HttpClient.GetAsync($"{str_controller}/Clean");
            if (response.IsSuccessStatusCode)
            {
       //         return await response.Content.ReadFromJsonAsync<JobC>();
            }
     //       return null;
        }
    }
}
