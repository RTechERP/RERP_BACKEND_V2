namespace RERPAPI.Model.DTO
{
    public class RequestExportRequestDTO
    {
        public List<ProjectPartListExportDTO> ListItem { get; set; }
        public string WarehouseCode { get; set; }
    }
}