using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.DocumentManager
{
    public class DocumentTypeRepo : GenericRepo<DocumentType>
    {
        public DocumentTypeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}