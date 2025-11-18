using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.DocumentManager
{
    public class DocumentFileRepo : GenericRepo<DocumentFile>
    {
        public DocumentFileRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
