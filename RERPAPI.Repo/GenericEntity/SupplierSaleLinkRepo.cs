using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RERPAPI.Model.DTO;

namespace RERPAPI.Repo.GenericEntity
{
    public class SupplierSaleLinkRepo : GenericRepo<SupplierSaleLink>
    {
        public SupplierSaleLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
