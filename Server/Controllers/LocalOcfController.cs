using Microsoft.AspNetCore.Mvc;
using OpenWLS.Server.DBase;
using OpenWLS.Server.DBase.Models.GlobalDb;
using OpenWLS.Server.DBase.Models.LocalDb;
using OpenWLS.Server.DBase.Repositories;

namespace OpenWLS.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocalOcfController : ControllerBase
    {
        private readonly ILogger<LocalOcfController> _logger;
        private readonly ILocalOCFRepository _rep;
        public LocalOcfController(ILocalOCFRepository rep,  ILogger<LocalOcfController> logger)
        {
            _logger = logger;
            _rep = rep;
        }

        [HttpGet]
        [Route("All")]
        public Task<List<OperationControlFileBase>> GetOCFList()
        {
            return _rep.GetList();
        }
        [HttpGet]
        [Route("Body/{id}")]
        public Task<TextObject> GetOCF(int id)
        {
            return _rep.GetBody(id);
        }

        [HttpPost]
        [Route("Add")]
        public Task<OperationControlFile> AddOCF(OperationControlFile ocf)
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
        public Task DeleteOCF(int id)
        {
            return _rep.Delete(id);
        }



    }
}

