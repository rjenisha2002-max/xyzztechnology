namespace AssetMgmt_WebApi.DTOs
{
    //create/update an Asset Master record 
    public class AssetMasterDto
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
    }

    // Asset Master record
    public class AssetMasterViewDto
    {
        public int AmId { get; set; }
        public int AmAtypeId { get; set; }
        public string AssetTypeName { get; set; } = null!;
        public int AmMakeId { get; set; }
        public string VendorName { get; set; } = null!;
        public int AmAdId { get; set; }
        public string AssetDefinitionName { get; set; } = null!;
        public string AmModel { get; set; } = null!;
        public string AmSnumber { get; set; } = null!;
        public string AmMyyear { get; set; } = null!;
        public DateTime AmPdate { get; set; }
        public string AmWarranty { get; set; } = null!;
        public DateTime AmFrom { get; set; }
        public DateTime AmTo { get; set; }
        public int? AmPurchaseOrderId { get; set; }
        public string? PurchaseOrderNo { get; set; }
    }
}
