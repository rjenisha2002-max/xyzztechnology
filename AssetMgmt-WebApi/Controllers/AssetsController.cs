using AssetMgmt_WebApi.DTOs;
using AssetMgmt_WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetMgmt_WebApi.Controllers
{
    //asset creation
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class AssetsController : ControllerBase
    {
        private readonly IAssetMasterService _assetMasterService;
        private readonly ILogger<AssetsController> _logger;

        public AssetsController(IAssetMasterService assetMasterService, ILogger<AssetsController> logger)
        {
            _assetMasterService = assetMasterService;
            _logger = logger;
        }

        // GET api/assets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssetMasterViewDto>>> GetAll()
        {
            return Ok(await _assetMasterService.GetAllAsync());
        }

        // GET api/assets/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AssetMasterViewDto>> GetById(int id)
        {
            var asset = await _assetMasterService.GetByIdAsync(id);
            if (asset == null) return NotFound(new { message = "Asset not found." });
            return Ok(asset);
        }

        // GET api/assets/search?keyword=laptop
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<AssetMasterViewDto>>> Search([FromQuery] string keyword = "")
        {
            return Ok(await _assetMasterService.SearchAsync(keyword));
        }

        // POST api/assets
        [HttpPost]
        [Authorize(Roles = "Admin,Purchase Manager")]
        public async Task<ActionResult<AssetMasterViewDto>> Create([FromBody] AssetMasterDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (success, message, asset) = await _assetMasterService.CreateAsync(dto);
            if (!success) return BadRequest(new { message });

            return CreatedAtAction(nameof(GetById), new { id = asset!.AmId }, asset);
        }

        // PUT api/assets/5
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Purchase Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] AssetMasterDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (success, message) = await _assetMasterService.UpdateAsync(id, dto);
            if (!success) return BadRequest(new { message });

            return Ok(new { message });
        }

        // DELETE api/assets/5
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _assetMasterService.DeleteAsync(id);
            if (!deleted) return NotFound(new { message = "Asset not found." });

            return Ok(new { message = "Asset deleted successfully." });
        }
    }
}
