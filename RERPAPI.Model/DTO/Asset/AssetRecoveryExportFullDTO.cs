namespace RERPAPI.Model.DTO.Asset
{
    public class AssetRecoveryExportFullDto
    {
        public AssetRecoveryExportDto Master { get; set; }
        public List<AssetRecoveryDetailExportDto> Details { get; set; }
    }
}