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
    [RoutePrefix("wms/transfer")]
    public class TransferController : ApiController
    {
        private readonly ITransferService transferService;
        private readonly IReadCodebars readService;
        public TransferController(ITransferService transfer, IReadCodebars read)
        {
            transferService = transfer;
            readService = read;
        }

        [HttpGet]
        [Route("ViewLocation")]
        [ResponseType(typeof(Response<List<BatchsInLocation>>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult ViewLocation([FromUri] string location, [FromUri] string warehouseTransfer) {
            var response = transferService.ViewLocation(location, warehouseTransfer);
            return Ok(new Response<List<BatchsInLocation>>(response, 0, ""));
        }

        [HttpGet]
        [Route("SuggestedLocation")]
        [ResponseType(typeof(Response<List<AvailableLocation>>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult SuggestedLocation([FromUri] string warehouseUser)
        {
            var response = transferService.AvailableLocations(warehouseUser);
            return Ok(new Response<List<AvailableLocation>>(response, 0, ""));
        }

        [HttpGet]
        [Route("GetLocation")]
        [ResponseType(typeof(Response<Location>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetLocation([FromUri] string warehouse, [FromUri] string bincode)
        {
            var response = transferService.AbsEntryFromBinCode(warehouse, bincode);
            return Ok(new Response<Location>(response, 0, ""));
        }

        [HttpGet]
        [Route("BatchNumber")]
        [ResponseType(typeof(Response<bool>))]
        [ResponseType(typeof(Response<List<Batch>>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult BatchNumber([FromUri] string codebars, [FromUri] string warehouse) {
            if (string.IsNullOrEmpty(codebars))
                return Ok(new Response<bool>(false, -1, "El codigo de barras es obligatorio"));

            var response = readService.ReadCode(codebars, warehouse);
            if (response.Count > 0)
                return Ok(new Response<List<Batch>>(response, 0, ""));
            else
                return Ok(new Response<List<Batch>>(response, -1, $"No se encontro información del lote: {codebars}"));
        }
    }
}
