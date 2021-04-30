using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using WMS.DAO.IService;
using WMS.Models;

namespace WMS.Controllers
{
    [Authorize]
    [RoutePrefix("wms/inventory")]
    public class InventoryController : ApiController
    {
        private readonly IInventoryService inventoryService;
        private readonly IReadCodebars readService;
        public InventoryController(IInventoryService inventory, IReadCodebars readCodebars)
        {
            inventoryService = inventory;
            readService = readCodebars;
        }
        
        [HttpGet]
        [Route("NumbersOfRecount")]
        [ResponseType(typeof(Response<int>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult NumbersOfRecount([FromUri] string warehouseUser) {
            var response = inventoryService.NumbersOfRecount(warehouseUser);
            return Ok(new Response<int>(response, 0, ""));
        }

        [HttpGet]
        [Route("LocationsWhsInventory")]
        [ResponseType(typeof(Response<WarehouseLocation>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult LocationsWhsInventory([FromUri] string warehouseCount) {
            var response = inventoryService.LocationsWhsInventory(warehouseCount);
            return Ok(new Response<WarehouseLocation>(response, 0, ""));
        }

        [HttpGet]
        [Route("SeeLocation")]
        [ResponseType(typeof(Response<List<BatchsInLocation>>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult SeeLocation([FromUri] string location, [FromUri] string warehouseInventory) {
            var response = inventoryService.SeeLocation(location, warehouseInventory);
            return Ok(new Response<List<BatchsInLocation>>(response, 0, ""));
        }

        [HttpGet]
        [Route("WarehouseInventory")]
        [ResponseType(typeof(Response<List<Warehouse>>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult WarehouseInventory([FromUri] string warehouse, [FromUri] string typeQuery) {
            var response = inventoryService.WarehouseInventory(warehouse, typeQuery);
            return Ok(new Response<List<Warehouse>>(response, 0, ""));
        }

        [HttpGet]
        [Route("AbsEntryFromBinCode")]
        [ResponseType(typeof(Response<Location>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult AbsEntryFromBinCode([FromUri] string warehouse, [FromUri] string binCode, [FromUri] string type) {
            var response = inventoryService.AbsEntryFromBinCode(warehouse, binCode, type);
            return Ok(new Response<Location>(response, 0, ""));
        }

        [HttpGet]
        [Route("AccessQuantitySAP")]
        [ResponseType(typeof(Response<bool>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult AccessQuantitySAP([FromUri] string userId) {
            var response = inventoryService.AccessQuantitySAP(userId);
            return Ok(new Response<bool>(response, 0, ""));
        }

        [HttpGet]
        [Route("BatchNumber")]
        [ResponseType(typeof(Response<bool>))]
        [ResponseType(typeof(Response<List<Batch>>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult BatchNumer([FromUri] string codebars, [FromUri] string warehouse, [FromUri] int status = 0)
        {
            var response = readService.ReadCode(codebars, warehouse);
            if (response.Count > 0)
                return Ok(new Response<List<Batch>>(response, 0, ""));
            else
                return Ok(new Response<List<Batch>>(response, -1, $"No se encontro información del lote: {codebars}"));
        }

        [HttpGet]
        [Route("CheckInventoryFile")]
        [ResponseType(typeof(Response<bool>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult CheckInventoryFile([FromUri]string warehouse, [FromUri] string user, [FromUri] string file)
        {
            if (inventoryService.CheckInventoryFile(warehouse, user, file))
                return Ok(new Response<bool>(true, 0, ""));
            else
                return Ok(new Response<bool>(false, -1, ""));
        }

        [HttpPost]
        [Route("WriteFileFolio")]
        [ResponseType(typeof(Response<bool>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult WriteFileFolio([FromUri]string warehouse, [FromUri] string user, [FromUri] string fileName, [FromBody] string[] stringValues)
        {
            if (stringValues == null)
                return Content(HttpStatusCode.BadRequest, new Response<bool>(false, -1, "No puede generar el archivo sin realizar lecturas"));

            var response = inventoryService.WriteInventoryFile(warehouse, user, fileName, stringValues);
            return Ok(response);
        }
    }
}
