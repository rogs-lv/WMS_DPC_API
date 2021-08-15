﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using WMS.DAO.IService;
using WMS.Models;

namespace WMS.Controllers
{
    //[Authorize]
    [RoutePrefix("wms/loadUser")]
    public class LoadUserController : ApiController
    {
        private readonly ILoadUserService loadUserService;
        public LoadUserController(ILoadUserService load)
        {
            loadUserService = load;
        }
        [HttpPost]
        [Route("SaveUsers")]
        [ResponseType(typeof(Response<int>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        public IHttpActionResult SaveUsers([FromBody] List<LoadUser> data) {
            var response = loadUserService.SaveUsers(data);
            return Ok(response);
        }
        
        [HttpPost]
        [Route("SaveModules")]
        [ResponseType(typeof(Response<int>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        public IHttpActionResult SaveModules([FromBody] List<LoadModule> data, [FromUri]int option)
        {
            var response = loadUserService.SaveModule(data, option);
            return Ok(response);
        }
        
        [HttpPost]
        [Route("SaveAdditionalConfiguration")]
        [ResponseType(typeof(Response<int>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        public IHttpActionResult SaveAdditionalConfiguration([FromBody] List<LoadModule> data, [FromUri] int option)
        {
            var response = loadUserService.SaveOtherConfiguration(data, option);
            return Ok(response);
        }
    }
}
