using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.AddNewBillExport
{
    public class InventoryProjectExportRepo:GenericRepo<InventoryProjectExport>
    {
         
        public InventoryProjectExportRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
