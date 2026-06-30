using AssetMgmt_WebApi.Models;

namespace AssetMgmt_WebApi.Repositories
{
    public interface IAssetMasterRepository
    {
        Task<IEnumerable<AssetMaster>> GetAllAsync();
        Task<AssetMaster?> GetByIdAsync(int id);
        Task<IEnumerable<AssetMaster>> SearchAsync(string keyword);
        Task<AssetMaster> AddAsync(AssetMaster asset);
        Task<bool> UpdateAsync(AssetMaster asset);
        Task<bool> DeleteAsync(int id);
        Task<PurchaseOrder?> GetEligiblePurchaseOrderAsync(int purchaseOrderId);
        Task<bool> SerialNumberExistsAsync(string serialNumber, int excludeId = 0);
    }
}
