using Microsoft.AspNetCore.Mvc;
using OpenWLS.Server.DBase;
using OpenWLS.Server.DBase.Models.LocalDb;


namespace OpenWLS.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobController : ControllerBase
    {
        private readonly ILogger<JobController> _logger;
        private readonly IJobRepository _rep;
        public JobController(IJobRepository rep,  ILogger<JobController> logger)
        {
            _logger = logger;
            _rep = rep;
        }

        [HttpGet]
        [Route("All")]
        public Task<List<Job>> GetAll()
        {
             return  _rep.GetJobList();
        }

        [HttpPost]
        [Route("Add")]
        public Task<Job> Add( Job job )
        {
            return _rep.AddJob(job);
        }
        [HttpPost]
        [Route("Update")]
        public Task<Job> Update(Job job)
        {
            return _rep.UpdateJob(job);
        }

        [HttpGet]
        [Route("Delete/{id}")]
        public Task<Job> Delete(int id)
        {
            return _rep.DeleteJob(id);
        }

        [HttpGet]
        [Route("Clean")]
        public Task Clean(int id)
        {
            return _rep.Clean();
        }

    }
}