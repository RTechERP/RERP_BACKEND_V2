namespace RERPAPI.Model.DTO.Asset
{
    public class TranferExportFullDto
    {
        public TranferAssetExportDto Master { get; set; }
        public List<TranferAssetDetailExportDto> Details { get; set; }
    }
}