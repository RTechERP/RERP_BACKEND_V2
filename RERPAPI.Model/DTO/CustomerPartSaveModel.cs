using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class CustomerPartSaveModel
    {
        public List<CustomerPart> AddedParts { get; set; } = new List<CustomerPart>();
        public List<CustomerPart> UpdatedParts { get; set; } = new List<CustomerPart>();
        public List<int> DeletedPartIds { get; set; } = new List<int>();
    }
}