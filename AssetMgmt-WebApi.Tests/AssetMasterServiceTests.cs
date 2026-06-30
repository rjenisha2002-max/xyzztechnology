using AssetMgmt_WebApi.DTOs;
using AssetMgmt_WebApi.Models;
using AssetMgmt_WebApi.Repositories;
using AssetMgmt_WebApi.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace AssetMgmt_WebApi.Tests
{
    
    public class AssetMasterServiceTests
    {
        private static AssetMasterDto SampleDto(int? poId = null, string serial = "SN-0001") => new()
        {
            AmAtypeId = 1,
            AmMakeId = 1,
            AmAdId = 1,
            AmModel = "Galaxy A14",
            AmSnumber = serial,
            AmMyyear = "2025",
            AmPdate = DateTime.Today,
            AmWarranty = "Y",
            AmFrom = DateTime.Today,
            AmTo = DateTime.Today.AddYears(1),
            AmPurchaseOrderId = poId
        };

        [Fact]
        public async Task CreateAsync_RejectsWhenPurchaseOrderNotEligible()
        {
            var repo = new Mock<IAssetMasterRepository>();
            repo.Setup(r => r.GetEligiblePurchaseOrderAsync(99)).ReturnsAsync((PurchaseOrder?)null);

            var service = new AssetMasterServiceImpl(repo.Object, NullLogger<AssetMasterServiceImpl>.Instance);

            var (success, message, asset) = await service.CreateAsync(SampleDto(poId: 99));

            Assert.False(success);
            Assert.Contains("not in 'Asset Details registered internally' status", message);
            Assert.Null(asset);
            repo.Verify(r => r.AddAsync(It.IsAny<AssetMaster>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_RejectsDuplicateSerialNumber()
        {
            var repo = new Mock<IAssetMasterRepository>();
            repo.Setup(r => r.SerialNumberExistsAsync("SN-0001", 0)).ReturnsAsync(true);

            var service = new AssetMasterServiceImpl(repo.Object, NullLogger<AssetMasterServiceImpl>.Instance);

            var (success, message, asset) = await service.CreateAsync(SampleDto());

            Assert.False(success);
            Assert.Contains("serial number already exists", message);
        }

        [Fact]
        public async Task CreateAsync_SucceedsWhenPurchaseOrderIsEligible()
        {
            var eligiblePo = new PurchaseOrder { PdId = 1, PdStatus = "Asset Details registered internally" };
            var createdAsset = new AssetMaster
            {
                AmId = 10,
                AssetType = new AssetType { AtName = "Mobile" },
                Vendor = new Vendor { VdName = "Samsung" },
                AssetDefinition = new AssetDefinition { AdName = "Mobile Phone" }
            };

            var repo = new Mock<IAssetMasterRepository>();
            repo.Setup(r => r.GetEligiblePurchaseOrderAsync(1)).ReturnsAsync(eligiblePo);
            repo.Setup(r => r.SerialNumberExistsAsync("SN-0001", 0)).ReturnsAsync(false);
            repo.Setup(r => r.AddAsync(It.IsAny<AssetMaster>())).ReturnsAsync(createdAsset);
            repo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(createdAsset);

            var service = new AssetMasterServiceImpl(repo.Object, NullLogger<AssetMasterServiceImpl>.Instance);

            var (success, message, asset) = await service.CreateAsync(SampleDto(poId: 1));

            Assert.True(success);
            Assert.NotNull(asset);
            Assert.Equal("Mobile", asset!.AssetTypeName);
            repo.Verify(r => r.AddAsync(It.IsAny<AssetMaster>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_SucceedsWithoutPurchaseOrderLink()
        {
            
            var createdAsset = new AssetMaster
            {
                AmId = 11,
                AssetType = new AssetType { AtName = "Laptop" },
                Vendor = new Vendor { VdName = "Mobio" },
                AssetDefinition = new AssetDefinition { AdName = "Laptop" }
            };

            var repo = new Mock<IAssetMasterRepository>();
            repo.Setup(r => r.SerialNumberExistsAsync("SN-0002", 0)).ReturnsAsync(false);
            repo.Setup(r => r.AddAsync(It.IsAny<AssetMaster>())).ReturnsAsync(createdAsset);
            repo.Setup(r => r.GetByIdAsync(11)).ReturnsAsync(createdAsset);

            var service = new AssetMasterServiceImpl(repo.Object, NullLogger<AssetMasterServiceImpl>.Instance);

            var (success, _, asset) = await service.CreateAsync(SampleDto(poId: null, serial: "SN-0002"));

            Assert.True(success);
            Assert.NotNull(asset);
            repo.Verify(r => r.GetEligiblePurchaseOrderAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenAssetNotFound()
        {
            var repo = new Mock<IAssetMasterRepository>();
            repo.Setup(r => r.DeleteAsync(123)).ReturnsAsync(false);

            var service = new AssetMasterServiceImpl(repo.Object, NullLogger<AssetMasterServiceImpl>.Instance);

            var result = await service.DeleteAsync(123);

            Assert.False(result);
        }

        [Fact]
        public async Task SearchAsync_ReturnsMatchingAssets()
        {
            var assets = new List<AssetMaster>
            {
                new()
                {
                    AmId = 1, AmSnumber = "SN-LAPTOP-01",
                    AssetType = new AssetType { AtName = "Laptop" },
                    Vendor = new Vendor { VdName = "Mobio" },
                    AssetDefinition = new AssetDefinition { AdName = "Laptop" }
                }
            };

            var repo = new Mock<IAssetMasterRepository>();
            repo.Setup(r => r.SearchAsync("laptop")).ReturnsAsync(assets);

            var service = new AssetMasterServiceImpl(repo.Object, NullLogger<AssetMasterServiceImpl>.Instance);

            var results = (await service.SearchAsync("laptop")).ToList();

            Assert.Single(results);
            Assert.Equal("SN-LAPTOP-01", results[0].AmSnumber);
        }
    }
}
