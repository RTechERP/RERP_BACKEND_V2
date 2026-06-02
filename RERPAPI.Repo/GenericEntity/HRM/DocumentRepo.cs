using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.DocumentManager
{
    public class DocumentRepo : GenericRepo<Document>
    {
        public DocumentRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}