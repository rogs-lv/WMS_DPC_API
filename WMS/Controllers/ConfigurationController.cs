using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using WMS.DAO.IService;
using WMS.Models;

namespace WMS.Controllers
{
    [Authorize]
    [RoutePrefix("wms/configuration")]
    public class ConfigurationController : ApiController
    {
        private readonly IConfigurationService configurationService;

        public ConfigurationController(IConfigurationService config)
        {
            configurationService = config;
        }

        [HttpGet]
        [Route("GetModules")]
        [ResponseType(typeof(Response<string>))]
        [ResponseType(typeof(Response<List<ModuleResponse>>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetModules([FromUri]string IdUser) {
            if (string.IsNullOrEmpty(IdUser))
                return Ok(new Response<string>("", -1, "El usuario es requerido"));

            List<ModuleResponse> response = configurationService.GetModulesUser(IdUser);
            return Ok(new Response<List<ModuleResponse>>(response, 0, ""));
        }
        [HttpGet]
        [Route("GetAdditionalSettings")]
        [ResponseType(typeof(Response<string>))]
        [ResponseType(typeof(Response<List<AdditionalSettings>>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetAdditionalSettings([FromUri] string IdUser)
        {
            if (string.IsNullOrEmpty(IdUser))
                return Ok(new Response<string>("", -1, "El usuario es requerido"));

            List<AdditionalSettings> response = configurationService.GetAdditionalSettings(IdUser);
            return Ok(new Response<List<AdditionalSettings>>(response, 0, ""));
        }
    }
}
