using Microsoft.AspNetCore.Mvc;
using OpenLS.Base.UOM;
using OpenWLS.Server.DBase;
using OpenWLS.Server.DBase.Models.LocalDb;
using OpenWLS.Server.DBase.Repositories;


namespace OpenWLS.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UomController : ControllerBase
    {
        private readonly ILogger<UomController> _logger;

        public UomController(  ILogger<UomController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        [Route("Get")]
        public async Task<string?> GetUom()
        {
            if (MeasurementUnit.MDT == null) return null; 
            return Newtonsoft.Json.JsonConvert.SerializeObject(MeasurementUnit.MDT);
        }

        [HttpPost]
        [Route("Update")]
        public async Task<string?> Update(string str )
        {
            int k = MeasurementUnit.Save(str);
            if (k == 0)  return str;
            else return null;
        }

       
    }
}