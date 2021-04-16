using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using WMS.DAO.IService;
using WMS.Models;
using WMS.Token;

namespace WMS.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("wms/authentication")]
    public class AuthenticationController : ApiController
    {
        private readonly IAuthenticateService authService;
        public AuthenticationController(IAuthenticateService auth_Service) {
            authService = auth_Service;
        }
        [HttpPost]
        [Route("login")]
        [ResponseType(typeof(Response<string>))]
        [ResponseType(typeof(Response<AuthenticateResponse>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult Login(AuthenticateRequest credential)
        {
            if (credential == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            AuthenticateResponse response = authService.Authenticate(credential);
            
            if (response == null)
                return Content(HttpStatusCode.OK, new Response<string>("", -1, "El usuario o contraseña son incorrectos"));

            if (!string.IsNullOrEmpty(response.Token))
                return Ok(new Response<AuthenticateResponse>(response, 0, ""));
            else
                return Content(HttpStatusCode.OK, new Response<string>("", -1, "El usuario o contraseña son incorrectos"));
        }
    }
}
