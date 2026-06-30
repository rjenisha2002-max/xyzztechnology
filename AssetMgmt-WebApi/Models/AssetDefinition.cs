namespace AssetMgmt_WebApi.Models
{
    //assest definition
    public partial class AssetDefinition
    {
        public int AdId { get; set; }

        public string AdName { get; set; } = null!;

        
        public int AdTypeId { get; set; }

      
        public string AdClass { get; set; } = null!;

        public virtual AssetType AssetType { get; set; } = null!;
    }
}
