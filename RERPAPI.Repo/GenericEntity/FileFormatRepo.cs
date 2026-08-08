using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class FileFormatRepo : GenericRepo<FileFormat>
    {
        public FileFormatRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
