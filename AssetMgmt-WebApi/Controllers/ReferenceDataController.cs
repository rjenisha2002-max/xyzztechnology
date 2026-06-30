using AssetMgmt_WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetMgmt_WebApi.Controllers
{
   //reference data controller
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiExplorerSettings(IgnoreApi = true)]//to ignore the api------->reference data
    public class ReferenceDataController : ControllerBase
    {
        private readonly IReferenceDataService _referenceDataService;

        public ReferenceDataController(IReferenceDataService referenceDataService)
        {
            _referenceDataService = referenceDataService;
        }

        // GET api/referencedata/asset-types
        [HttpGet("asset-types")]
        public async Task<IActionResult> GetAssetTypes() =>
            Ok(await _referenceDataService.GetAssetTypesAsync());

        // GET api/referencedata/asset-definitions
        [HttpGet("asset-definitions")]
        public async Task<IActionResult> GetAssetDefinitions() =>
            Ok(await _referenceDataService.GetAssetDefinitionsAsync());

        // GET api/referencedata/vendors
        [HttpGet("vendors")]
        public async Task<IActionResult> GetVendors() =>
            Ok(await _referenceDataService.GetVendorsAsync());

        // GET api/referencedata/purchase-orders
        [HttpGet("purchase-orders")]
        public async Task<IActionResult> GetPurchaseOrders() =>
            Ok(await _referenceDataService.GetPurchaseOrdersAsync());

        // GET api/referencedata/purchase-orders/eligible
        
        [HttpGet("purchase-orders/eligible")]
        public async Task<IActionResult> GetEligiblePurchaseOrders() =>
            Ok(await _referenceDataService.GetEligiblePurchaseOrdersAsync());
    }
}
