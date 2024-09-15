using Microsoft.AspNetCore.Mvc;
using OpenWLS.Server.DBase;
using OpenWLS.Server.DBase.Models.LocalDb;
using OpenWLS.Server.DBase.Repositories;

namespace OpenWLS.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EdgeController : ControllerBase
    {
        private readonly ILogger<EdgeController> _logger;
        private readonly IEdgeRepository _rep;
        public EdgeController(IEdgeRepository rep,  ILogger<EdgeController> logger)
        {
            _logger = logger;
            _rep = rep;
        }

        [HttpGet]
        [Route("All")]
        public Task<List<Edge>> GetAll()
        {
             return  _rep.GetAll();
        }

        [HttpPost]
        [Route("Add")]
        public Task<Edge> Add( Edge job )
        {
            return _rep.Add(job);
        }
        [HttpPost]
        [Route("Update")]
        public Task<Edge?> Update(Edge job)
        {
            return _rep.Update(job);
        }

        [HttpGet]
        [Route("Delete/{id}")]
        public Task<Edge?> Delete(int id)
        {
            return _rep.Delete(id);
        }



    }
}