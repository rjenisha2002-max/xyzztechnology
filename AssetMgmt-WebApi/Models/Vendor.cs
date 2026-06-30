using System;

namespace AssetMgmt_WebApi.Models
{
    
    public partial class Vendor
    {
        public int VdId { get; set; }

        public string VdName { get; set; } = null!;

        public string VdType { get; set; } = null!;

      
        public int VdAtypeId { get; set; }

        public DateTime VdFrom { get; set; }

        public DateTime VdTo { get; set; }

        public string VdAddr { get; set; } = null!;

        public virtual AssetType AssetType { get; set; } = null!;
    }
}
