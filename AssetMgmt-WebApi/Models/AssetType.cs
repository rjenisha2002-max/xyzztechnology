using System.Collections.Generic;

namespace AssetMgmt_WebApi.Models
{
    //assest type
    public partial class AssetType
    {
        public int AtId { get; set; }

        public string AtName { get; set; } = null!;

        public virtual ICollection<AssetDefinition> AssetDefinitions { get; set; } = new List<AssetDefinition>();

        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();

        public virtual ICollection<AssetMaster> AssetMasters { get; set; } = new List<AssetMaster>();
    }
}
