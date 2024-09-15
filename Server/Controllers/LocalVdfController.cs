using Microsoft.AspNetCore.Mvc;
using OpenWLS.Server.DBase;
using OpenWLS.Server.DBase.Models.GlobalDb;
using OpenWLS.Server.DBase.Models.LocalDb;
using OpenWLS.Server.DBase.Repositories;

namespace OpenWLS.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocalVdfController : ControllerBase
    {
        private readonly ILogger<LocalVdfController> _logger;
        private readonly ILocalVDFRepository _rep;
        public LocalVdfController(ILocalVDFRepository rep,  ILogger<LocalVdfController> logger)
        {
            _logger = logger;
            _rep = rep;
        }

        [HttpGet]
        [Route("All")]
        public Task<List<GViewDefinitionFileBase>> GetVDFList()
        {
            return _rep.GetVDFList();
        }
        [HttpGet]
        [Route("Body/{id}")]
        public Task<TextObject> GetVDF(int id)
        {
            return _rep.GetVDF(id);
        }

        [HttpPost]
        [Route("Add")]
        public Task<GViewDefinitionFile> AddVDF(GViewDefinitionFile vdf)
        {
            return _rep.AddVDF(vdf);
        }
        [HttpPost]
        [Route("Update")]
        public Task<GViewDefinitionFile> Update(GViewDefinitionFile vdf)
        {
            return _rep.UpdateVDF(vdf);
        }

        [HttpGet]
        [Route("Delete/{id}")]
        public Task DeleteVDF(int id)
        {
            return _rep.DeleteVDF(id);
        }



    }
}

