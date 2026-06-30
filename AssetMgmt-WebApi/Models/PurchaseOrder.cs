using System;

namespace AssetMgmt_WebApi.Models
{
    
    public partial class PurchaseOrder
    {
        public int PdId { get; set; }

        public string PdOrderNo { get; set; } = null!;

       
        public int PdAdId { get; set; }

        
        public int PdTypeId { get; set; }

        public int PdQty { get; set; }

        
        public int PdVendorId { get; set; }

        public DateTime PdDate { get; set; }

        public DateTime PdDdate { get; set; }

        public string PdStatus { get; set; } = null!;

        public virtual AssetDefinition AssetDefinition { get; set; } = null!;

        public virtual AssetType AssetType { get; set; } = null!;

        public virtual Vendor Vendor { get; set; } = null!;
    }
}
