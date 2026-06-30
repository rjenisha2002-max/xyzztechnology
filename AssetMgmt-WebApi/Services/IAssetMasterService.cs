using AssetMgmt_WebApi.DTOs;

namespace AssetMgmt_WebApi.Services
{
    public interface IAssetMasterService
    {
        Task<IEnumerable<AssetMasterViewDto>> GetAllAsync();
        Task<AssetMasterViewDto?> GetByIdAsync(int id);
        Task<IEnumerable<AssetMasterViewDto>> SearchAsync(string keyword);
        Task<(bool Success, string Message, AssetMasterViewDto? Asset)> CreateAsync(AssetMasterDto dto);
        Task<(bool Success, string Message)> UpdateAsync(int id, AssetMasterDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
