using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
