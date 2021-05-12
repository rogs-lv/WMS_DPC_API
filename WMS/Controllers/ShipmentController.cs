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
    [RoutePrefix("wms/shipment")]
    public class ShipmentController : ApiController
    {
        private readonly IShipmentService shipmentService;
        private readonly IShippingService shippingService;
        public ShipmentController(IShipmentService shipment, IShippingService shipping)
        {
            shipmentService = shipment;
            shippingService = shipping;
        }

        [HttpGet]
        [Route("GetClients")]
        [ResponseType(typeof(Response<List<Partner>>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetClients()
        {            
            var response = shippingService.GetListPartner();
            return Ok(response);
        }
        [HttpGet]
        [Route("GetNumberShipment")]
        [ResponseType(typeof(Response<List<Shipment>>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetNumberShipment([FromUri] int docnum) {
            var response = shipmentService.GetNumberShipment(docnum);
            return Ok(new Response<List<Shipment>>(response, 0, ""));
        }

        [HttpGet]
        [Route("ApplyGR")]
        [ResponseType(typeof(Response<bool>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult ApplyGR([FromUri] string batch)
        {
            var response = shipmentService.ApplyGR(batch);
            return Ok(new Response<bool>(response, 0, ""));
        }

        [HttpGet]
        [Route("ValidateEFEEM")]
        [ResponseType(typeof(Response<ValidateEFEEM>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult ValidateEFEEM([FromUri] string batch, [FromUri] int type)
        {
            var response = shipmentService.isValidEFEEM(batch, type);
            return Ok(new Response<ValidateEFEEM>(response, 0, ""));
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

        [HttpPut]
        [Route("UpdateBatchs")]
        [ResponseType(typeof(Response<int>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult UpdateBatchs([FromBody] string[] batchs, [FromUri] string status, [FromUri] int docnum, [FromUri] int docentry) {
            var response = shipmentService.UpdateBatchs(batchs, status, docnum, docentry);
            return Ok(new Response<int>(response, 0, ""));
        }
    }
}
