namespace RERPAPI.Model.DTO.Asset
{
    public class AssetAllocationExportFullDto
    {
        public AssetAllocationExportDto? Master { get; set; }
        public List<AssetAllocationDetailExportDto>? Details { get; set; }
    }
}