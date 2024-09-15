using Microsoft.EntityFrameworkCore;
using OpenWLS.Server.DBase.DbContents;
using OpenWLS.Server.DBase.Models.LocalDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.DBase.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly LocalDbContent dbContext;

        public JobRepository(LocalDbContent dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Job>> GetJobList()
        {
            return await dbContext.Jobs.Where(e => e.Deleted == null).ToListAsync();
        }

        public async Task<Job?> GetJob(int id)
        {
            return await dbContext.Jobs
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Job> AddJob(Job job)
        {
            job.Id = dbContext.Jobs.Max(e => e.Id) + 1;
            var result = await dbContext.Jobs.AddAsync(job);
            await dbContext.SaveChangesAsync();
            string dir_name = Job.GetJobDirectory(job.Name);
            if( !Directory.Exists(dir_name) )
                Directory.CreateDirectory(dir_name);
            return job;
        }

        public async Task<Job> UpdateJob(Job job)
        {
            var result = await dbContext.Jobs
                .FirstOrDefaultAsync(e => e.Id == job.Id);

            if (result != null)
            {
  //              result.Name = job.Name;  //because of it ties to directory
                result.Company = job.Company;
                result.WellId = job.WellId;

                result.WellName = job.WellName;
                result.Unit = job.Unit;
                result.Engineer = job.Engineer;
                result.TimeStart = job.TimeStart;
                result.TimeStop = job.TimeStop;
                result.Note = job.Note;
                await dbContext.SaveChangesAsync();
                return result;
            }
            return null;
        }

        public async Task<Job> DeleteJob(int id)
        {
            var result = await dbContext.Jobs
                .FirstOrDefaultAsync(e => e.Id == id);
            if (result != null)
            {
                //    dbContext.Jobs.Remove(result);
                result.Deleted = true;
                await dbContext.SaveChangesAsync();
            }
            return result;
        }
        public async Task Clean()
        {
            var result = await dbContext.Jobs.Where(e => e.Deleted != null).ToListAsync();
            foreach (var job in result)
            {
                string dir_name = Job.GetJobDirectory(job.Name);
                if (!Directory.Exists(dir_name))
                    dbContext.Jobs.Remove(job);
            }
            await dbContext.SaveChangesAsync();
        }

    }

}
