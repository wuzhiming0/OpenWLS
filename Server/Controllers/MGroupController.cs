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
    public class MGroupController : ControllerBase
    {
        private readonly ILogger<MGroupController> _logger;
        private readonly IMGroupRepository _rep;
        public MGroupController(IMGroupRepository rep,  ILogger<MGroupController> logger)
        {
            _logger = logger;
            _rep = rep;
        }

        [HttpGet]
        [Route("All")]
        public Task<List<MGroup>> GetAll()
        {
            return _rep.GetAll();
        }
        [HttpGet]
        [Route("Inst/{iid}")]
        public Task<List<MGroup>> GetMGroupsOfInst(int iid)
        {
            return _rep.GetListOfInst(iid);
        }

       
        [HttpPost]
        [Route("Add")]
        public Task<MGroup> Add(MGroup mg)
        {
            return _rep.Add(mg);
        }
        [HttpPost]
        [Route("Update")]
        public Task<MGroup> Update(MGroup mg)
        {
            return _rep.Update(mg);
        }

        [HttpGet]
        [Route("Delete/{id}")]
        public Task Delete(int id)
        {
            return _rep.Delete(id);
        }


    }
}

