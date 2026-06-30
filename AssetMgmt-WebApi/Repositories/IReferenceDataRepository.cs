using AssetMgmt_WebApi.Models;

namespace AssetMgmt_WebApi.Repositories
{
   
    public interface IReferenceDataRepository
    {
        Task<IEnumerable<AssetType>> GetAssetTypesAsync();
        Task<IEnumerable<AssetDefinition>> GetAssetDefinitionsAsync();
        Task<IEnumerable<Vendor>> GetVendorsAsync();
        Task<IEnumerable<PurchaseOrder>> GetPurchaseOrdersAsync();
        Task<IEnumerable<PurchaseOrder>> GetEligiblePurchaseOrdersAsync();
    }
}
