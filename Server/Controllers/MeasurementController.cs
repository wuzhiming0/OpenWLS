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
    public class MeasurementController : ControllerBase
    {
        private readonly ILogger<MeasurementController> _logger;
        private readonly IMeasurementRepository _rep;
        public MeasurementController(IMeasurementRepository rep,  ILogger<MeasurementController> logger)
        {
            _logger = logger;
            _rep = rep;
        }

        [HttpGet]
        [Route("All")]
        public Task<List<MeasurementDb>> GetAll()
        {
            return _rep.GetAll();
        }
        [HttpGet]
        [Route("Name/{name}")]
        public Task<MeasurementDb?> Get(string name)
        {
            return _rep.Get(name);
        }    
        [HttpGet]
        [Route("Id/{id}")]
        public Task<MeasurementDb?> Get(int id)
        {
            return _rep.Get(id);
        }           
        [HttpPost]
        [Route("Add")]
        public Task<MeasurementDb> Add(MeasurementDb mg)
        {
            return _rep.Add(mg);
        }
        [HttpPost]
        [Route("Update")]
        public Task<MeasurementDb> Update(MeasurementDb mg)
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

