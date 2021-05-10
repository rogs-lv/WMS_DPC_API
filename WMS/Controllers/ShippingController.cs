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
    [RoutePrefix("wms/shipping")]
    public class ShippingController : ApiController
    {
        private readonly IShippingService shippingService;
        public ShippingController(IShippingService shipping)
        {
            shippingService = shipping;
        }

        [HttpGet]
        [Route("GetPartners")]
        [ResponseType(typeof(Response<List<Partner>>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetPartners() {
            var response = shippingService.GetListPartner();
            return Ok(response);
        }

        [HttpGet]
        [Route("BatchNumber")]
        [ResponseType(typeof(Response<bool>))]
        [ResponseType(typeof(Response<List<Batch>>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult BatchNumber([FromUri] string codebars, [FromUri] string warehouse)
        {
            if (string.IsNullOrEmpty(codebars))
                return Ok(new Response<bool>(false, -1, "El codigo de barras es obligatorio"));

            var response = shippingService.ReadCode(codebars, warehouse);
            if (response.Count > 0)
                return Ok(new Response<List<Batch>>(response, 0, ""));
            else
                return Ok(new Response<List<Batch>>(response, -1, $"No se encontro información del lote: {codebars}"));
        }
    }
}
