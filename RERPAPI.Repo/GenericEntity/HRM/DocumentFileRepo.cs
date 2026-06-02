using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.DocumentManager
{
    public class DocumentFileRepo : GenericRepo<DocumentFile>
    {
        public DocumentFileRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}