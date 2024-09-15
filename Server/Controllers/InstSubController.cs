using Microsoft.AspNetCore.Mvc;
using OpenWLS.Server.DBase;
using OpenWLS.Server.DBase.Models.GlobalDb;
using OpenWLS.Server.DBase.Models.LocalDb;
using OpenWLS.Server.DBase.Repositories;
using OpenWLS.Server.DBase.Repositories.InstrumentM;

namespace OpenWLS.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InstSubController : ControllerBase
    {
        private readonly ILogger<InstrumentController> _logger;
        private readonly IInstSubRepository _rep;
        public InstSubController(IInstSubRepository rep,  ILogger<InstrumentController> logger)
        {
            _logger = logger;
            _rep = rep;
        }

        [HttpGet]
        [Route("All")]
        public Task<List<InstSubDb>> GetAll()
        {
            return _rep.GetList();
        }
        [HttpGet]
        [Route("Downhole")]
        public Task<List<InstSubDb>> GetDownholeList()
        {
            return _rep.GetDownholeList();
        }
        [HttpGet]
        [Route("DhAux")]
        public Task<List<InstSubDb>> GetDownholeAuxList()
        {
            return _rep.GetDownholeAuxList();
        }
        [HttpGet]
        [Route("Sf")]
        public Task<List<InstSubDb>> GetSurfaceList()
        {
            return _rep.GetSurfaceList();
        }

        [HttpGet]
        [Route("Get/{id}")]
        public Task<InstSubDb> Get(int id)
        {
            return _rep.Get(id);
        }

        [HttpPost]
        [Route("Add")]
        public Task<InstSubDb> Add(InstSubDb inst)
        {
            return _rep.Add(inst);
        }
        [HttpPost]
        [Route("Update")]
        public Task<InstSubDb> Update(InstSubDb inst)
        {
            return _rep.Update(inst);
        }

        [HttpGet]
        [Route("Delete/{id}")]
        public Task Delete(int id)
        {
            return _rep.Delete(id);
        }


    }
}

