using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.DBase.Models.LocalDb;

public class Job
{
 
    public int Id { get; set; }
//    [PropertyOrder(1)]
    public string? Name { get; set; }

    public string? Company { get; set; }
    public string? WellId { get; set; }
    public string? WellName { get; set; }

    public string? Unit { get; set; }         //skid/truck/crew 
    public string? Engineer { get; set; }

    public long TimeStart { get; set; }

    public long TimeStop { get; set; }
    public string? Note { get; set; }

    public bool? Deleted { get; set; }

    public static string GetJobDirectory(string job_name)
    {
        return $"{ServerGlobals.projectDir}data\\server\\jobs\\{job_name}";
    }


}

public interface IJobRepository
{
    Task<List<Job>> GetJobList();
    Task<Job?> GetJob(int id);
    Task<Job> AddJob(Job job);
    Task<Job> UpdateJob(Job job);
    Task<Job> DeleteJob(int id);
    Task Clean();
}

