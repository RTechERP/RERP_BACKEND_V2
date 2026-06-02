namespace RERPAPI.Model.DTO
{
    public class CreateTreeRequestDTO
    {
        public int ProjectId { get; set; }
        public List<int> SelectedProjectTypeIds { get; set; }
    }
}