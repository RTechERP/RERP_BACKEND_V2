namespace RERPAPI.Model.DTO
{
    public class DownloadProjectPartlistPriceRequestDTO
    {
        public int ProjectId { get; set; }
        public int PartListId { get; set; }
        public string ProductCode { get; set; }
    }
}