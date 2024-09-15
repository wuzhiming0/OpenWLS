using Microsoft.AspNetCore.Mvc;
using OpenWLS.Server.DBase;
using OpenWLS.Server.DBase.Models.LocalDb;
using OpenWLS.Server.DBase.Repositories;
using OpenWLS.Server.LogInstance;
using OpenWLS.Server.LogInstance.OperationDocument;

namespace OpenWLS.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogInstanceController : ControllerBase
    {
     //   private readonly ILogger<LogInstanceController> _logger;
        private readonly IJobRepository _job_rep;
        private readonly ILocalOCFRepository _ocf_rep;
        private readonly IEdgeRepository _edge_rep;
        private readonly ISyslogRepository _syslog_rep;
        public LogInstanceController(IJobRepository job_rep, ILocalOCFRepository ocf_rep, IEdgeRepository edge_rep, ISyslogRepository syslog_rep)
        {
            _syslog_rep = syslog_rep;
            _job_rep = job_rep;
            _ocf_rep = ocf_rep;
            _edge_rep = edge_rep;
        }

        [HttpGet]
        [Route("All")]
        public LogInstances GetAll()
        {
            LogInstances lis = new LogInstances();
            foreach(LogInstance.LogInstance li in ServerGlobals.logInstances)
                lis.Add(li);
             return lis;
        }

        [HttpGet]
        [Route("Create/{job_id}/{ocf_id}/{edge_id}/{sim}")]
        public Server.LogInstance.LogInstance? Create(int job_id, int ocf_id, int edge_id, int sim )
        {
            Job? job = _job_rep.GetJob(job_id).Result;
            if (job == null) return null;
            TextObject? t = _ocf_rep.GetBody(ocf_id).Result;
            if (t == null) return null;
            OperationDocument? doc = Newtonsoft.Json.JsonConvert.DeserializeObject<OperationDocument>(t.Val);
            if (doc == null) return null;
            DBase.Models.LocalDb.Edge? edge_db = edge_id > 0? _edge_rep.Get(edge_id).Result : null;
            LogInstanceS li = LogInstanceS.Create(ocf_id, job, doc, edge_db, sim );
            LogInstance.LogInstance li_res = new LogInstance.LogInstance();
                li_res.CloneFrom(li);
            return li_res;           
        }

        [HttpPost]
        [Route("Playback")]
        public Server.LogInstance.LogInstance Playback(string fn )
        {
            LogInstanceS li = LogInstanceS.Playback(fn);
            LogInstance.LogInstance li_res = new LogInstance.LogInstance();
            li_res.CloneFrom(li);
            return li_res;
        }
        /*
        [HttpGet]
        [Route("Connect/{li_id}")]
        public Task<Server.LogInstance.LogInstance> Connect(int li_id)
        {
            return UpdateJob(job);
        }

        [HttpGet]
        [Route("Disconnect/{li_id}")]
        public Task<Job> Delete(int li_id)
        {
            return _rep.DeleteJob(id);
        }
        */
        [HttpGet]
        [Route("Stop/{li_id}")]
        public LogInstance.LogInstance Stop(int li_id)
        {
            return LogInstanceS.Stop(li_id);
        }

    }
}