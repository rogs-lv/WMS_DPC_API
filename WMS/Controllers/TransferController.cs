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
        public TransferController(ITransferService transfer)
        {
            transferService = transfer;
        }
        #region location movement
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
        #endregion
        #region transfer request
        [HttpGet]
        [Route("OpenTransferRequest")]
        [ResponseType(typeof(Response<List<OpenTransferRequest>>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult OpenTransferRequest([FromUri] string warehouseUser) {
            var response = transferService.OpenTransfersRequests(warehouseUser);
            return Ok(new Response<List<OpenTransferRequest>>(response, 0, ""));
        }

        [HttpGet]
        [Route("GetDocumentRequest")]
        [ResponseType(typeof(Response<DocumentTransfer>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetDocumentRequest([FromUri] string warehouseUser, string numberDocument)
        {
            var response = transferService.GetDocumentRequest(warehouseUser, numberDocument);
            return Ok(new Response<DocumentTransfer>(response, 0, ""));
        }
        #endregion
        #region common
        [HttpGet]
        [Route("BatchNumber")]
        [ResponseType(typeof(Response<bool>))]
        [ResponseType(typeof(Response<List<Batch>>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult BatchNumber([FromUri] string codebars, [FromUri] string warehouse) {
            if (string.IsNullOrEmpty(codebars))
                return Ok(new Response<bool>(false, -1, "El codigo de barras es obligatorio"));

            var response = transferService.ReadCode(codebars, warehouse);
            if (response.Count > 0)
                return Ok(new Response<List<Batch>>(response, 0, ""));
            else
                return Ok(new Response<List<Batch>>(response, -1, $"No se encontro información del lote: {codebars}"));
        }
        [HttpGet]
        [Route("LocationWarehouse")]
        [ResponseType(typeof(Response<DefaultLocationWhs>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult LocationWarehouse([FromUri] string warehouse) {
            var response = transferService.LocationWarehouse(warehouse);
            return Ok(new Response<DefaultLocationWhs>(response, 0, ""));
        }
        [HttpPost]
        [Route("ProcessMovement")]
        [ResponseType(typeof(Response<Transfer>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult ProcessMovement([FromBody] DataMovement data) {
            var response = transferService.ProcessMovement(data.batchs, data.request);
            return Ok(response);
        }
        #endregion
    }
}
