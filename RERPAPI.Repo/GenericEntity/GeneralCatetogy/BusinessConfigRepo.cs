using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.GeneralCatetogy
{
    public class BusinessConfigRepo : GenericRepo<BusinessConfig>
    {
        public BusinessConfigRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public List<int> GetDepartmentIDsByConfigType(int configType)
        {
            return GetAll(c => c.ConfigType == configType)
                .Select(c => c.DepartmentID)
                .Distinct()
                .ToList();
        }
    }
}
