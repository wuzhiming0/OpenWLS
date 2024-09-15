using Microsoft.AspNetCore.Mvc;
using OpenWLS.Server.DBase;
using OpenWLS.Server.DBase.Models.GlobalDb;
using OpenWLS.Server.DBase.Models.LocalDb;
using OpenWLS.Server.DBase.Repositories;

namespace OpenWLS.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GlobalOcfController : ControllerBase
    {
        private readonly ILogger<GlobalOcfController> _logger;
        private readonly IGlobalOCFRepository _rep;
        public GlobalOcfController(IGlobalOCFRepository rep,  ILogger<GlobalOcfController> logger)
        {
            _logger = logger;
            _rep = rep;
        }

        [HttpGet]
        [Route("All")]
        public Task<List<OperationControlFileBase>> GetList()
        {
            return _rep.GetList();
        }
        [HttpGet]
        [Route("Body/{id}")]
        public Task<TextObject> GetBody(int id)
        {
            return _rep.GetBody(id);
        }
        [HttpPost]
        [Route("Add")]
        public Task<OperationControlFile> Add(OperationControlFile ocf)
        {
            return _rep.Add(ocf);
        }
        [HttpPost]
        [Route("Update")]
        public Task<OperationControlFile> Update(OperationControlFile ocf)
        {
            return _rep.Update(ocf);
        }

        [HttpGet]
        [Route("Delete/{id}")]
        public Task Delete(int id)
        {
            return _rep.Delete(id);
        }

        [HttpGet]
        [Route("Open/{id}")]
        public Task Open(int id)
        {
            TextObject? t_ob = _rep.GetBody(id).Result;
            if (t_ob != null)
            {
                
            }
            return Task.CompletedTask;

        }

    }
}

