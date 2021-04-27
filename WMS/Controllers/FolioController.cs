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

namespace WMS.Controllers
{
    [Authorize]
    [RoutePrefix("wms/folio")]
    public class FolioController : ApiController
    {
        private readonly IFolioService folioService;
        public FolioController(IFolioService folio)
        {
            folioService = folio;
        }

        [HttpGet]
        [Route("CheckFolioFile")]
        [ResponseType(typeof(Response<bool>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult CheckFolioFile([FromUri]string user, [FromUri]string file) {
            if (folioService.CheckFolioFile(user, file))
                return Ok(new Response<bool>(true, 0, ""));
            else
                return Ok(new Response<bool>(false, -1, ""));
        }

        [HttpPost]
        [Route("WriteFileFolio")]
        [ResponseType(typeof(Response<bool>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult WriteFileFolio([FromUri] string user, [FromUri] string fileName, [FromBody] string[] batchs) {
            if (batchs == null)
                return Content(HttpStatusCode.BadRequest, new Response<bool>(false, -1, "El campo de lotes no puede ir vacio"));

            var response = folioService.WriteFolioFile(user, fileName, batchs);
            return Ok(response);
        }
    }
}
