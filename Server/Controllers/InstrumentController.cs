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
    public class InstrumentController : ControllerBase
    {
        private readonly ILogger<InstrumentController> _logger;
        private readonly IInstrumentRepository _rep;
        public InstrumentController(IInstrumentRepository rep,  ILogger<InstrumentController> logger)
        {
            _logger = logger;
            _rep = rep;
        }

        [HttpGet]
        [Route("All")]
        public Task<List<InstrumentDb>> GetAll()
        {
            return _rep.GetList();
        }
       [HttpGet]
        [Route("Downhole")]
        public Task<List<InstrumentDb>> GetDownholeList()
        {
            return _rep.GetDownholeList();
        }       
        [HttpPost]
        [Route("Add")]
        public Task<InstrumentDb> Add(InstrumentDb inst)
        {
            return _rep.Add(inst);
        }
        [HttpPost]
        [Route("Update")]
        public Task<InstrumentDb> Update(InstrumentDb inst)
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

