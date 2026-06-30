using AssetMgmt_WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AssetMgmt_WebApi.Repositories
{
   //inherited with iassetmasterrepository
    public class AssetMasterRepositoryImpl : IAssetMasterRepository
    {
        private readonly AssetMgmtContext _context;

        public AssetMasterRepositoryImpl(AssetMgmtContext context)
        {
            _context = context;
        }

        private IQueryable<AssetMaster> BaseQuery() =>
            _context.AssetMasters
                .Include(a => a.AssetType)
                .Include(a => a.Vendor)
                .Include(a => a.AssetDefinition)
                .Include(a => a.PurchaseOrder);

        public async Task<IEnumerable<AssetMaster>> GetAllAsync()
        {
            return await BaseQuery().OrderByDescending(a => a.AmId).ToListAsync();
        }

        public async Task<AssetMaster?> GetByIdAsync(int id)
        {
            return await BaseQuery().FirstOrDefaultAsync(a => a.AmId == id);
        }

        public async Task<IEnumerable<AssetMaster>> SearchAsync(string keyword)
        {
            keyword = keyword?.Trim().ToLower() ?? string.Empty;

            return await BaseQuery()
                .Where(a =>
                    a.AmSnumber.ToLower().Contains(keyword) ||
                    a.AmModel.ToLower().Contains(keyword) ||
                    a.AssetType.AtName.ToLower().Contains(keyword) ||
                    a.Vendor.VdName.ToLower().Contains(keyword) ||
                    a.AssetDefinition.AdName.ToLower().Contains(keyword))
                .OrderByDescending(a => a.AmId)
                .ToListAsync();
        }

        public async Task<AssetMaster> AddAsync(AssetMaster asset)
        {
            _context.AssetMasters.Add(asset);
            await _context.SaveChangesAsync();
            return asset;
        }

        public async Task<bool> UpdateAsync(AssetMaster asset)
        {
            var existing = await _context.AssetMasters.FindAsync(asset.AmId);
            if (existing == null) return false;

            existing.AmAtypeId = asset.AmAtypeId;
            existing.AmMakeId = asset.AmMakeId;
            existing.AmAdId = asset.AmAdId;
            existing.AmModel = asset.AmModel;
            existing.AmSnumber = asset.AmSnumber;
            existing.AmMyyear = asset.AmMyyear;
            existing.AmPdate = asset.AmPdate;
            existing.AmWarranty = asset.AmWarranty;
            existing.AmFrom = asset.AmFrom;
            existing.AmTo = asset.AmTo;
            existing.AmPurchaseOrderId = asset.AmPurchaseOrderId;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.AssetMasters.FindAsync(id);
            if (existing == null) return false;

            _context.AssetMasters.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PurchaseOrder?> GetEligiblePurchaseOrderAsync(int purchaseOrderId)
        {
            return await _context.PurchaseOrders
                .FirstOrDefaultAsync(p => p.PdId == purchaseOrderId &&
                                          p.PdStatus == "Asset Details registered internally");
        }

        public async Task<bool> SerialNumberExistsAsync(string serialNumber, int excludeId = 0)
        {
            return await _context.AssetMasters
                .AnyAsync(a => a.AmSnumber == serialNumber && a.AmId != excludeId);
        }
    }
}
