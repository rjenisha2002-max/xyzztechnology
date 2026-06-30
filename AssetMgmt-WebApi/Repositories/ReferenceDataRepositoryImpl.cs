using AssetMgmt_WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AssetMgmt_WebApi.Repositories
{
    public class ReferenceDataRepositoryImpl : IReferenceDataRepository
    {
        private readonly AssetMgmtContext _context;

        public ReferenceDataRepositoryImpl(AssetMgmtContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AssetType>> GetAssetTypesAsync() =>
            await _context.AssetTypes.OrderBy(t => t.AtName).ToListAsync();

        public async Task<IEnumerable<AssetDefinition>> GetAssetDefinitionsAsync() =>
            await _context.AssetDefinitions.Include(d => d.AssetType)
                .OrderBy(d => d.AdName).ToListAsync();

        public async Task<IEnumerable<Vendor>> GetVendorsAsync() =>
            await _context.Vendors.Include(v => v.AssetType)
                .OrderBy(v => v.VdName).ToListAsync();

        public async Task<IEnumerable<PurchaseOrder>> GetPurchaseOrdersAsync() =>
            await _context.PurchaseOrders
                .Include(p => p.AssetDefinition)
                .Include(p => p.AssetType)
                .Include(p => p.Vendor)
                .OrderByDescending(p => p.PdId)
                .ToListAsync();

        public async Task<IEnumerable<PurchaseOrder>> GetEligiblePurchaseOrdersAsync() =>
            await _context.PurchaseOrders
                .Include(p => p.AssetDefinition)
                .Include(p => p.AssetType)
                .Include(p => p.Vendor)
                .Where(p => p.PdStatus == "Asset Details registered internally")
                .OrderByDescending(p => p.PdId)
                .ToListAsync();
    }
}
