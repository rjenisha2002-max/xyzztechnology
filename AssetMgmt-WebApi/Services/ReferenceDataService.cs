using AssetMgmt_WebApi.Models;
using AssetMgmt_WebApi.Repositories;

namespace AssetMgmt_WebApi.Services
{
    public interface IReferenceDataService
    {
        Task<IEnumerable<AssetType>> GetAssetTypesAsync();
        Task<IEnumerable<AssetDefinition>> GetAssetDefinitionsAsync();
        Task<IEnumerable<Vendor>> GetVendorsAsync();
        Task<IEnumerable<PurchaseOrder>> GetPurchaseOrdersAsync();
        Task<IEnumerable<PurchaseOrder>> GetEligiblePurchaseOrdersAsync();
    }

   
    public class ReferenceDataServiceImpl : IReferenceDataService
    {
        private readonly IReferenceDataRepository _repository;

        public ReferenceDataServiceImpl(IReferenceDataRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<AssetType>> GetAssetTypesAsync() => _repository.GetAssetTypesAsync();
        public Task<IEnumerable<AssetDefinition>> GetAssetDefinitionsAsync() => _repository.GetAssetDefinitionsAsync();
        public Task<IEnumerable<Vendor>> GetVendorsAsync() => _repository.GetVendorsAsync();
        public Task<IEnumerable<PurchaseOrder>> GetPurchaseOrdersAsync() => _repository.GetPurchaseOrdersAsync();
        public Task<IEnumerable<PurchaseOrder>> GetEligiblePurchaseOrdersAsync() => _repository.GetEligiblePurchaseOrdersAsync();
    }
}
