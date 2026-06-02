using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class POKHFilesRepo : GenericRepo<POKHFile>
    {
        public POKHFilesRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}