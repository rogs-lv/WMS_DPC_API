using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using WMS.DAO.IService;
using WMS.Entities;
using WMS.Models;

namespace WMS.Controllers
{
    [Authorize]
    [RoutePrefix("wms/profile")]
    public class ProfileController : ApiController
    {
        private readonly IProfileService profileService;
        public ProfileController(IProfileService profile)
        {
            profileService = profile;
        }

        [HttpGet]
        [Route("GetProfiles")]
        [ResponseType(typeof(Response<List<Profile>>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetProfiles()
        {
            List<Profile> response = profileService.GetProfiles();
            return Ok(new Response<List<Profile>>(response, 0, ""));
        }

        [HttpGet]
        [Route("GetWarehouses")]
        [ResponseType(typeof(Response<List<Warehouse>>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetWarehouses() {
            var response = profileService.GetWarehouse();
            return Ok(new Response<List<Warehouse>>(response, 0, ""));
        }

        [HttpPost]
        [Route("CreateProfile")]
        [ResponseType(typeof(Response<string>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult CreateProfile([FromBody] ProfileRequest profile) { 
            if(string.IsNullOrEmpty(profile.UserProfile.Password) || string.IsNullOrEmpty(profile.UserProfile.NameUser) || string.IsNullOrEmpty(profile.UserProfile.IdUser))
                return Content(HttpStatusCode.OK, new Response<string>("Faltan campos obligatorios", -1, $"No ingreso los campos marcados como obligatorios"));

            var response = profileService.CreateNewProfile<string>(profile.UserProfile, profile.UserModules, profile.UserAdditionalSettings);
            return Content(HttpStatusCode.OK, response);
        }

        [HttpPatch]
        [Route("UpdateProfile")]
        [ResponseType(typeof(Response<string>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult UpdateProfile([FromBody] ProfileRequest profile) {
            if (string.IsNullOrEmpty(profile.UserProfile.WhsCode) || string.IsNullOrEmpty(profile.UserProfile.NameUser) || string.IsNullOrEmpty(profile.UserProfile.IdUser))
                return Content(HttpStatusCode.OK, new Response<string>("Faltan campos obligatorios", -1, $"No ingreso los campos marcados como obligatorios"));
            var response = profileService.UpdateProfile<string>(profile.UserProfile, profile.UserModules, profile.UserAdditionalSettings);
            return Ok(response);
        }

        [HttpGet]
        [Route("GetAdminModules")]
        [ResponseType(typeof(Response<string>))]
        [ResponseType(typeof(Response<List<ModuleUser>>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetAdminModules([FromUri] string IdUser)
        {
            if (string.IsNullOrEmpty(IdUser))
                return Ok(new Response<string>("", -1, "El usuario es requerido"));

            List<ModuleUser> response = profileService.GetModulesUser(IdUser);
            return Ok(new Response<List<ModuleUser>>(response, 0, ""));
        }
    }
}
