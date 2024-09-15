using Microsoft.AspNetCore.Mvc;
using OpenWLS.Server.DBase;
using OpenWLS.Server.DBase.Models.LocalDb;

namespace OpenWLS.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SyslogController : ControllerBase
    {
        private readonly ILogger<SyslogController> _logger;
        private readonly ISyslogRepository _rep;
        public SyslogController(ISyslogRepository rep,  ILogger<SyslogController> logger)
        {
            _logger = logger;
            _rep = rep;
        }

        [HttpGet]
        [Route("GetItemsFromId/{id}")]
        public List<SyslogItem> GetItemsFromId(int id)
        {
            return _rep.GetItemsFromId(id).Result;
        }

        [HttpPost]
        [Route("Add")]
        public Task Add(SyslogItem item)
        {
            return _rep.AddItem(item);
        }
    }
}