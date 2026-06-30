using AssetMgmt_WebApi.DTOs;
using AssetMgmt_WebApi.Models;
using AssetMgmt_WebApi.Repositories;

namespace AssetMgmt_WebApi.Services
{
    
    public class AssetMasterServiceImpl : IAssetMasterService
    {
        private readonly IAssetMasterRepository _assetMasterRepository;
        private readonly ILogger<AssetMasterServiceImpl> _logger;

        public AssetMasterServiceImpl(IAssetMasterRepository assetMasterRepository,
            ILogger<AssetMasterServiceImpl> logger)
        {
            _assetMasterRepository = assetMasterRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<AssetMasterViewDto>> GetAllAsync()
        {
            var assets = await _assetMasterRepository.GetAllAsync();
            return assets.Select(ToViewDto);
        }

        public async Task<AssetMasterViewDto?> GetByIdAsync(int id)
        {
            var asset = await _assetMasterRepository.GetByIdAsync(id);
            return asset == null ? null : ToViewDto(asset);
        }

        public async Task<IEnumerable<AssetMasterViewDto>> SearchAsync(string keyword)
        {
            var assets = await _assetMasterRepository.SearchAsync(keyword);
            return assets.Select(ToViewDto);
        }

        public async Task<(bool Success, string Message, AssetMasterViewDto? Asset)> CreateAsync(AssetMasterDto dto)
        {
            
            if (dto.AmPurchaseOrderId.HasValue)
            {
                var eligiblePo = await _assetMasterRepository.GetEligiblePurchaseOrderAsync(dto.AmPurchaseOrderId.Value);
                if (eligiblePo == null)
                {
                    _logger.LogWarning("Asset creation rejected - PO {PoId} is not eligible (status must be 'Asset Details registered internally')", dto.AmPurchaseOrderId);
                    return (false, "Asset cannot be created: the selected Purchase Order is not in 'Asset Details registered internally' status.", null);
                }
            }

            if (await _assetMasterRepository.SerialNumberExistsAsync(dto.AmSnumber))
            {
                return (false, "An asset with this serial number already exists.", null);
            }

            var entity = new AssetMaster
            {
                AmAtypeId = dto.AmAtypeId,
                AmMakeId = dto.AmMakeId,
                AmAdId = dto.AmAdId,
                AmModel = dto.AmModel,
                AmSnumber = dto.AmSnumber,
                AmMyyear = dto.AmMyyear,
                AmPdate = dto.AmPdate,
                AmWarranty = dto.AmWarranty,
                AmFrom = dto.AmFrom,
                AmTo = dto.AmTo,
                AmPurchaseOrderId = dto.AmPurchaseOrderId
            };

            var created = await _assetMasterRepository.AddAsync(entity);
            var full = await _assetMasterRepository.GetByIdAsync(created.AmId);

            _logger.LogInformation("Asset {Serial} created with id {Id}", dto.AmSnumber, created.AmId);
            return (true, "Asset created successfully.", full == null ? null : ToViewDto(full));
        }

        public async Task<(bool Success, string Message)> UpdateAsync(int id, AssetMasterDto dto)
        {
            if (await _assetMasterRepository.SerialNumberExistsAsync(dto.AmSnumber, id))
            {
                return (false, "Another asset already uses this serial number.");
            }

            dto.AmId = id;
            var entity = new AssetMaster
            {
                AmId = id,
                AmAtypeId = dto.AmAtypeId,
                AmMakeId = dto.AmMakeId,
                AmAdId = dto.AmAdId,
                AmModel = dto.AmModel,
                AmSnumber = dto.AmSnumber,
                AmMyyear = dto.AmMyyear,
                AmPdate = dto.AmPdate,
                AmWarranty = dto.AmWarranty,
                AmFrom = dto.AmFrom,
                AmTo = dto.AmTo,
                AmPurchaseOrderId = dto.AmPurchaseOrderId
            };

            var updated = await _assetMasterRepository.UpdateAsync(entity);
            if (!updated) return (false, "Asset not found.");

            _logger.LogInformation("Asset {Id} updated", id);
            return (true, "Asset updated successfully.");
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _assetMasterRepository.DeleteAsync(id);
            if (result) _logger.LogInformation("Asset {Id} deleted", id);
            return result;
        }

        private static AssetMasterViewDto ToViewDto(AssetMaster a) => new()
        {
            AmId = a.AmId,
            AmAtypeId = a.AmAtypeId,
            AssetTypeName = a.AssetType?.AtName ?? string.Empty,
            AmMakeId = a.AmMakeId,
            VendorName = a.Vendor?.VdName ?? string.Empty,
            AmAdId = a.AmAdId,
            AssetDefinitionName = a.AssetDefinition?.AdName ?? string.Empty,
            AmModel = a.AmModel,
            AmSnumber = a.AmSnumber,
            AmMyyear = a.AmMyyear,
            AmPdate = a.AmPdate,
            AmWarranty = a.AmWarranty,
            AmFrom = a.AmFrom,
            AmTo = a.AmTo,
            AmPurchaseOrderId = a.AmPurchaseOrderId,
            PurchaseOrderNo = a.PurchaseOrder?.PdOrderNo
        };
    }
}
