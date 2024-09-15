using Microsoft.AspNetCore.Mvc;
using OpenWLS.Server.DBase;
using OpenWLS.Server.DBase.Models.LocalDb;


namespace OpenWLS.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PassController : ControllerBase
    {
        private readonly ILogger<PassController> _logger;
        private readonly IPassRepository _rep;
        public PassController(IPassRepository rep,  ILogger<PassController> logger)
        {
            _logger = logger;
            _rep = rep;
        }

        [HttpGet]
        [Route("All")]
        public Task<List<Pass>> GetAll()
        {
             return  _rep.GetPassList();
        }

        [HttpPost]
        [Route("Add")]
        public Task<Pass> Add( Pass Pass )
        {
            return _rep.AddPass(Pass);
        }

        [HttpPost]
        [Route("Update")]
        public Task<Pass> Update(Pass Pass)
        {
            return _rep.UpdatePass(Pass);
        }

        [HttpPost]
        [Route("Delete/{id}")]
        public Task<Pass> Delete(int id)
        {
            return _rep.DeletePass(id);
        }
    }
}