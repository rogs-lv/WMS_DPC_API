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
    [RoutePrefix("wms/quality")]
    public class QualityController : ApiController
    {
        private readonly IQualityService qualityService;
        private readonly IReadCodebars readService;
        public QualityController(IQualityService quality, IReadCodebars readCodebars)
        {
            qualityService = quality;
            readService = readCodebars;
        }
        
        [HttpGet]
        [Route("WarehouseQuality")]
        [ResponseType(typeof(Response<string>))]
        [ResponseType(typeof(Response<List<Warehouse>>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult WarehouseQuality([FromUri] string whsCode) {

            if (string.IsNullOrEmpty(whsCode))
                return Ok(new Response<string>("", -1, "El almacén es obligatorio"));

            var response = qualityService.WarehouseQuality(whsCode);
            return Ok(new Response<List<Warehouse>>(response, 0, ""));
        }
        
        [HttpGet]
        [Route("BatchNumber")]
        [ResponseType(typeof(Response<bool>))]
        [ResponseType(typeof(Response<List<Batch>>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult BatchNumer([FromUri] string codebars, [FromUri] string warehouse, [FromUri] int status) {
            if (string.IsNullOrEmpty(codebars))
                return Ok(new Response<bool>(false, -1, "El codigo de barras es obligatorio"));

            if (!qualityService.CheckSameStatus(codebars, status))
            {
                var response = readService.ReadCode(codebars, warehouse, status);
                if (response.Count > 0)
                    return Ok(new Response<List<Batch>>(response, 0, ""));
                else
                    return Ok(new Response<List<Batch>>(response, -1, $"No se encontro información del lote: {codebars}"));
            }
            else 
                return Ok(new Response<bool>(true, -1, "El lote ya se encuentra actualizado"));
        }

        [HttpGet]
        [Route("DefaultLocationWarehouse")]
        [ResponseType(typeof(Response<DefaultLocationWhs>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult DefaultLocationWarehouse([FromUri] string warehouse) {
            var response = qualityService.DefaulLocationWarehouse(warehouse);
            return Ok(new Response<DefaultLocationWhs>(response, 0, ""));
        }
    }
}
