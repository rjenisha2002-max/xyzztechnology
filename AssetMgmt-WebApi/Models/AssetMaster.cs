using System;

namespace AssetMgmt_WebApi.Models
{
    //assert master
    public partial class AssetMaster
    {
        public int AmId { get; set; }

        
        public int AmAtypeId { get; set; }

        
        public int AmMakeId { get; set; }

       
        public int AmAdId { get; set; }

        public string AmModel { get; set; } = null!;

        public string AmSnumber { get; set; } = null!;

       
        public string AmMyyear { get; set; } = null!;

        
        public DateTime AmPdate { get; set; }

        
        public string AmWarranty { get; set; } = null!;

        public DateTime AmFrom { get; set; }

        public DateTime AmTo { get; set; }

       
        public int? AmPurchaseOrderId { get; set; }

        public virtual AssetType AssetType { get; set; } = null!;

        public virtual Vendor Vendor { get; set; } = null!;

        public virtual AssetDefinition AssetDefinition { get; set; } = null!;

        public virtual PurchaseOrder? PurchaseOrder { get; set; }
    }
}
