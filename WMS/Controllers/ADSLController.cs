using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using WMS.DAO.IService;
using WMS.Models;

namespace WMS.Controllers
{
    [Authorize]
    [RoutePrefix("wms/ADSL")]
    public class ADSLController : ApiController
    {
        private readonly IADSLService adslService;
        public ADSLController(IADSLService _adslService)
        {
            this.adslService = _adslService;
        }

        [HttpGet]
        [Route("GetBussinesPartner")]
        [ResponseType(typeof(Response<List<PartnerADSL>>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetBussinesPartner()
        {
            var response = adslService.GetListPartner();
            return Ok(response);
        }
        [HttpGet]
        [Route("GetBatch")]
        [ResponseType(typeof(Response<bool>))]
        [ResponseType(typeof(Response<List<BatchADSL>>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetBatch([FromUri] string codebars, [FromUri] string warehouse, [FromUri] int priceList)
        {
            if (string.IsNullOrEmpty(codebars))
                return Ok(new Response<bool>(false, -1, "El codigo de barras es obligatorio"));

            var response = adslService.ReadCode(codebars, warehouse, priceList);
            if (response.Count > 0)
                return Ok(new Response<List<BatchADSL>>(response, 0, ""));
            else
                return Ok(new Response<List<BatchADSL>>(response, -1, $"No se encontro información del lote: {codebars}"));            
        }
        [HttpPost]
        [Route("ProcessDelivery")]
        [ResponseType(typeof(Response<List<BatchADSL>>))]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult ProcessDelivery([FromBody]List<BatchADSL> data, [FromUri]string cardCode, [FromUri] string usuario) {
            var response = adslService.ProcessDelivery(data, cardCode, usuario);
            return Ok(response);
        }
    }
}
